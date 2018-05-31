using UnityEngine;
using System.Collections.Generic;
using System;
using System.Threading;
using System.Collections;
using LitJson;
using UnityEngine.Events;

public class MicManager : Manager
{
	public static MicManager Instance { get { return GetManager<MicManager> (); } }

	AudioClip recordClip;
	static string recordFilePath;

	public Dictionary<string, string> voiceDic = new Dictionary<string, string> ();
	public UnityEvent stopVoiceEvent = new UnityEvent ();

	static int fileIndex;
	public static bool IsRecordSucceed { get; private set; }
	public static string CurRecordFilePath { get { return recordFilePath + fileIndex + ".wav"; } }

	//const int MaxFileNum = 10;
	//int fileIndex;
	//public List<string> recordfilePaths = new List<string>();




	protected override void Init ()
	{
		recordFilePath = Application.persistentDataPath + "/AudioRec/voice";
		m_RecordTime = 0;
		athread = new Thread (new ThreadStart (GetSoundF));
		athread.IsBackground = false;//防止后台现成。相反需要后台线程就设为false
		athread.Start ();
		IsAthread = false;
		if (m_AudioSource == null)
		{
			m_AudioSource = gameObject.AddComponent<AudioSource> ();
		}
	}
	Thread athread;
	object lockd = new object ();
	bool IsAthread;
	private static int ConstRecordLength = 8;
	private static int RecordSampleRate = 44100;
	public static void OnRuntimeMethodLoad ()
	{
		m_RecordClip = Microphone.Start (null, true, ConstRecordLength, RecordSampleRate);
	}

	private AudioSource m_AudioSource;
	private static AudioClip m_RecordClip;

	private Queue<float> SoundFQueue = new Queue<float> ();

	private Action m_GetSample;
	private Action<float> m_SoundWave;
	private Action m_AlreadySilentCallback;
	public enum MicStatus { None, Recording, Playing }
	private MicStatus m_Status = MicStatus.None;
	private bool m_AlreadySilent;
	private bool m_GetWave;
	private float m_RecordTime;
	private float m_GetVoiceTime;
	private AudioClip m_PlayClip;
	private bool m_IsPlaying;
	public void StartRecord (Action flyStar = null, Action<float> soundWave = null, Action silentCallback = null)
	{
		//m_GetSample = getSample;
		m_GetSample = flyStar;
		m_SoundWave = soundWave;
		m_AlreadySilentCallback = silentCallback;

		//m_RecordClip = Microphone.Start(null, true, 6, 44100);
		//m_AudioSource.clip = m_RecordClip;
		//m_AudioSource.volume = 0.001f;
		//m_AudioSource.Play();
		m_Status = MicStatus.Recording;
		m_AlreadySilent = false;
		m_GetWave = false;
		m_SilentTime = 0;
		m_RecordTime = 0;
		m_GetVoiceTime = 0;
		m_RecordData = null;
		tempFloatBuffer = new float[3];
		tempFloatBufferListen = new float[3];
		//m_StartGetPosition = true;
	}
	private float[] m_RecordData;
	private int m_StopPosition;
	public void StopRecord ()
	{
		m_GetSample = null;
		if (m_SoundWave != null)
			m_SoundWave.Invoke (0);
		m_SoundWave = null;
		m_Status = MicStatus.None;
		m_StopPosition = Microphone.GetPosition (null);
	}

	void GetRecordData ()
	{
		if (m_RecordClip == null || Mathf.Approximately (m_RecordTime, 0))
			return;

		int length = (int)(RecordSampleRate * m_RecordTime);
		m_RecordData = new float[length];
		int startPos = m_StopPosition - length;

		int maxLength = m_RecordClip.samples * m_RecordClip.channels;
		float[] samples = new float[maxLength];
		m_RecordClip.GetData (samples, 0);

		int counter = 0;
		for (int i = startPos; i < m_StopPosition; i++)
		{
			if (i < 0)
			{
				m_RecordData[counter] = samples[i + maxLength];
			}
			else
			{
				m_RecordData[counter] = samples[i];
			}
			counter++;
		}
	}

	private const float VolumeValve = 0.05f;
	void ProcessRecordClip ()
	{
		int startIndex = 0, endIndex = m_RecordData.Length;
		int counter = 0;
		for (int i = 0; i < m_RecordData.Length; i++)
		{
			if (Mathf.Abs (m_RecordData[i]) > VolumeValve)
			{
				counter++;
				if (counter > 10)
				{
					startIndex = i;
					break;
				}
			}
		}
		counter = 0;
		for (int i = m_RecordData.Length - 1; i >= 0; i--)
		{
			if (Mathf.Abs (m_RecordData[i]) > VolumeValve)
			{
				counter++;
				if (counter > 10)
				{
					endIndex = i;
					break;
				}
			}
		}

		List<float> list = new List<float> ();
		for (int i = startIndex; i < endIndex; i++)
		{
			list.Add (m_RecordData[i]);
		}

		if (list.Count == 0)
		{
			m_PlayClip = null;
		}
		else
		{
			//LogManager.Log(string.Format("start index is :{0}, endInde is:{1}", startIndex, endIndex));
			AudioClip clip = AudioClip.Create ("temp", list.Count, 1, RecordSampleRate, false);
			clip.SetData (list.ToArray (), 0);
			m_PlayClip = clip;
		}
	}

	private Action m_PlayCompleteCallback;
	public void Play (bool needRecord = false, Action callback = null, string sentence = "")
	{
		m_IsPlaying = true;
		m_PlayCompleteCallback = callback;
		GetRecordData ();
		if (m_RecordData != null)
		{
			ProcessRecordClip ();
			m_AudioSource.pitch = 1;
			if (m_PlayClip != null)
			{
				m_AudioSource.clip = m_PlayClip;
				m_AudioSource.loop = false;
				m_AudioSource.volume = 1;
				m_AudioSource.Play ();

				if (needRecord)
				{
					LogManager.LogWarning (" 播放 录制音频 发送OSS ");
					//recordfilePaths.Add(filepath);
					recordClip = m_PlayClip;
					IsRecordSucceed = SavWav.Save (CurRecordFilePath, m_PlayClip);
					// 发送OSS
					SnapAppApi.UploadFileToAlyOSS (CurRecordFilePath, (string result) => {
						//LogManager.Log ("   OSS 回调 = " , result , " / " , result , "   " , Time.time);
						voiceDic.Add (result, sentence);
					});
					fileIndex += 1;
				}
			}
			else
			{
				if (m_PlayCompleteCallback != null)
					m_PlayCompleteCallback.Invoke ();
			}
		}
		else
		{
			if (m_PlayCompleteCallback != null)
				m_PlayCompleteCallback.Invoke ();
		}
	}

	public void StopPlay ()
	{
		m_AudioSource.Stop ();
	}

	public void Reset ()
	{
		StopAllCoroutines ();
		fileIndex = 0;
		voiceDic.Clear ();
		IsRecordSucceed = false;
	}



	float[] SoundData;
	int positionStart;
	int positionEnd;

	private void Update ()
	{
		switch (m_Status)
		{
			case MicStatus.Recording:
				if (m_RecordClip != null)
					Record ();
				break;
			case MicStatus.None:
				if (m_RecordClip != null && IsAthread == false)
				{
					m_RecordTime += Time.deltaTime;
					if (m_RecordTime > ConstRecordLength)
						m_RecordTime = ConstRecordLength;

					positionEnd = Microphone.GetPosition (null);
					positionStart = 0;
					if (positionEnd - GetDataNum > 0)
						positionStart = positionEnd - GetDataNum;
					SoundData = new float[m_RecordClip.samples];
					m_RecordClip.GetData (SoundData, positionStart);
				}

				break;
		}

		if (m_IsPlaying)
		{
			if (!m_AudioSource.isPlaying)
			{
				if (m_PlayCompleteCallback != null)
					m_PlayCompleteCallback.Invoke ();
				m_IsPlaying = false;
				//LogManager.Log("not playing ");
			}
		}
	}

	private void OnApplicationQuit ()
	{
		Microphone.End (null);
	}

	int m_TempIndex = 0;
	float[] tempFloatBuffer = new float[3];
	const int GetDataNum = 4096;
	float[] tempFloatBufferListen = new float[3];
	int F = 1000;
	float m_SilentTime = 0;
	Byte[] m_AudioBuffer = new Byte[GetDataNum * 2];
	Int16[] m_IntData = new Int16[GetDataNum];
	Byte[] m_AudioListenBuffer = new byte[GetDataNum * 2];
	Int16[] m_IniListenBuffer = new Int16[GetDataNum];
	const int RescaleFactor = 32767;
	float[] m_Samples = new float[GetDataNum];
	float[] m_SamplesListen = new float[4096];
	Byte[] m_ByteArr = new Byte[2];
	Byte[] m_ByteArrListen = new Byte[2];
	private void Record ()
	{
		/* 
		F = AudioManager.Instance.GetF ();
		LogManager.Log ("F = " , F);
        */
		m_RecordTime += Time.deltaTime;
		if (m_RecordTime > ConstRecordLength) m_RecordTime = ConstRecordLength;
		int position = Microphone.GetPosition (null);
		int startPosition = 0;
		if (position - GetDataNum > 0)
			startPosition = position - GetDataNum;
		m_RecordClip.GetData (m_Samples, startPosition);
		AudioListener.GetOutputData (m_SamplesListen, 0);
		int lengthData = Mathf.Min (position - startPosition, GetDataNum);


		float value = 0f;

		if (lengthData == 0)
		{
			return;
		}

		for (int i = 0; i < lengthData; i++)
		{
			value += Math.Abs (m_Samples[i]);
			m_IntData[i] = (short)(m_Samples[i] * RescaleFactor);
			m_ByteArr = BitConverter.GetBytes (m_IntData[i]);
			m_ByteArr.CopyTo (m_AudioBuffer, i * 2);
			m_IniListenBuffer[i] = (short)(m_SamplesListen[i] * RescaleFactor);
			m_ByteArrListen = BitConverter.GetBytes (m_IniListenBuffer[i]);
			m_ByteArrListen.CopyTo (m_AudioListenBuffer, i * 2);
		}

		float average = value / lengthData;
		if (m_GetWave && !m_AlreadySilent)
		{
			if (average > VolumeValve)
			{
				m_SilentTime = 0;
			}
			else
			{
				m_SilentTime += Time.deltaTime;
				if (m_SilentTime > 1)
				{//静默时间超过1秒
					if (m_AlreadySilentCallback != null)
					{
						m_AlreadySilentCallback.Invoke ();
					}
					m_AlreadySilent = true;
					return;
				}
			}
		}
		m_SoundWave.Invoke (average);

		float totalAbsValue = 0.0f;
		short sample = 0;
		float totalAbsValueListen = 0.0f;
		for (int i = 0; i < lengthData * 2; i += 2)
		{
			sample = (short)((m_AudioBuffer[i]) | m_AudioBuffer[i + 1] << 8);
			totalAbsValue += Mathf.Abs ((float)sample) / (lengthData);
			sample = (short)((m_AudioListenBuffer[i]) | m_AudioListenBuffer[i + 1] << 8);
			totalAbsValueListen += Mathf.Abs ((float)sample) / (lengthData);
		}
		tempFloatBuffer[m_TempIndex % 3] = totalAbsValue;
		tempFloatBufferListen[m_TempIndex % 3] = totalAbsValueListen;
		float temp = 0.0f;
		float tempListen = 0.0f;
		for (int i = 0; i < 3; i++)
		{
			temp += tempFloatBuffer[i];
			tempListen += tempFloatBufferListen[i];
		}

		/*
		if (tempListen < 1000)
			F = 1000;
        else
            F = (int) tempListen + 1; 
        */

		//if(tempListen > F)
		//F = (int) tempListen + 1; 

		if (m_GetWave == false)
		{
			if (temp > F)
			{
				m_GetVoiceTime += Time.deltaTime;
				if (m_GetVoiceTime > 0.2f && m_GetSample != null)
				{
					m_GetSample.Invoke ();
					m_GetWave = true;
				}
			}
		}

		m_TempIndex++;
	}

	public void ResetCord ()
	{
		m_Status = MicStatus.None;
		m_GetWave = false;
	}

	private void GetSoundF ()
	{
		while (true)
		{
			if (m_Status == MicStatus.None && m_RecordClip != null)
			{
				lock (lockd)
				{
					IsAthread = true;
					int lengthData = positionEnd - positionStart;
					Byte[] audioBuffer = new Byte[lengthData * 2];
					Int16[] intData = new Int16[lengthData];
					int rescaleFactor = 32767; //to convert float to Int16

					float value = 0f;

					if (lengthData == 0)
					{
						IsAthread = false;
						continue;
					}

					for (int i = 0; i < lengthData; i++)
					{
						value += Math.Abs (SoundData[i]);
						intData[i] = (short)(SoundData[i] * rescaleFactor);
						Byte[] byteArr = new Byte[2];
						byteArr = BitConverter.GetBytes (intData[i]);
						byteArr.CopyTo (audioBuffer, i * 2);
					}
					float totalAbsValue = 0.0f;
					short sample = 0;
					for (int i = 0; i < lengthData * 2; i += 2)
					{
						sample = (short)((audioBuffer[i]) | audioBuffer[i + 1] << 8);
						totalAbsValue += Mathf.Abs ((float)sample) / (lengthData);
					}
					tempFloatBuffer[m_TempIndex % 3] = totalAbsValue;
					float temp = 0.0f;
					for (int i = 0; i < 3; i++)
					{
						temp += tempFloatBuffer[i];
					}

					SoundFQueue.Enqueue (temp);
					if (SoundFQueue.Count > 3)
					{
						SoundFQueue.Dequeue ();
					}

					float sum = 0f;
					float[] soundFF = SoundFQueue.ToArray ();
					for (int i = 0; i < SoundFQueue.Count; i++)
					{
						sum += soundFF[i];
					}
					F = (int)(sum / SoundFQueue.Count) + 1;
					if (F < 1000)
						F = 1000;
					IsAthread = false;
				}
			}
		}
	}





	public void PlayAllVoice (string uid, int levelId, Action playCallback = null, Action failCallback = null)
	{
		StopAllCoroutines ();
		stopVoiceEvent.Invoke ();
		JsonData data = new JsonData ();
		data["uid"] = uid;
		data["levelId"] = levelId;
		DancingWordAPI.Instance.RequestLevelVoiceFromServer (data, (string result) => {

			if ((bool)JsonMapper.ToObject (result)["status"] == false)
			{
				if (failCallback != null) failCallback.Invoke ();
				return;
			}
			//LogManager.Log ("   = " , result);
			List<string> voiceUrls = new List<string> ();
			JsonData json = JsonMapper.ToObject (result)["data"];
			JsonData meteData = json["meteData"];
			for (int i = 0; i < meteData.Count; i++)
			{
				voiceUrls.Add (meteData[i].TryGetString ("voice"));
			}
			_LoadVoiceCallback (voiceUrls, failCallback);
			if (playCallback != null) playCallback.Invoke ();
		}, failCallback);
	}

	void _LoadVoiceCallback (List<string> voiceUrls, Action failCallback)
	{
		if (voiceUrls.Count == 0)
		{
			if (failCallback != null)
				failCallback.Invoke ();
			return;
		}
		StopAllCoroutines ();
		StartCoroutine (_CorPlayVoices (voiceUrls, failCallback));
	}

	IEnumerator _CorPlayVoices (List<string> voiceUrls, Action failCallback)
	{
		yield return null;
		for (int i = 0; i < voiceUrls.Count; i++)
		{
			WWW www = new WWW (voiceUrls[i]);
			yield return www;
			if (www.error != null)
			{
				LogManager.Log (www.error);
				if (failCallback != null)
					failCallback.Invoke ();
				yield break;
			}
			AudioClip ac = www.GetAudioClip (true, true, AudioType.MPEG);
			m_AudioSource.loop = false;
			m_AudioSource.volume = 1;
			m_AudioSource.clip = ac;
			m_AudioSource.Play ();
			yield return new WaitForSeconds (ac.length);
		}
		yield return null;
		stopVoiceEvent.Invoke ();
	}




}
