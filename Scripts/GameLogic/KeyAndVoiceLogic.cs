using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class KeyAndVoiceLogic
{
	private enum Status { Idle, Reading, SentenceRightDelay, WaitForClick, WaitForSpeech, RepeatingVoice, Finish, Pause }
	private Status m_Status;
	protected CorePlayInputCheck m_InputCheck;
	protected float m_Timer;
	protected int m_CurSentenceIndex;
	protected float m_CurSentenceLength;
	protected StreamTapCreator m_TapCreator;
	public KeyAndVoiceLogic (CorePlayInputCheck input)
	{
		m_InputCheck = input;
	}

	public void StartLogic ()
	{
		m_Timer = 0;
		m_CurSentenceIndex = 0;
		m_Status = Status.Idle;
		m_TapCreator = new StreamTapCreator ();
		//LogManager.Log("key voice logic start!!");
		CorePlayInputCheck.SentenceAllRightEvent.RemoveListener (SentenceAllRightCallback);
		CorePlayInputCheck.SentenceAllRightEvent.AddListener (SentenceAllRightCallback);
		StartCurSentence ();
	}

	void SentenceAllRightCallback ()
	{
		m_Status = Status.SentenceRightDelay;
		StaticMonoBehaviour.Instance.StartCoroutine (DelayDelete ());
	}

	IEnumerator DelayDelete ()
	{
		yield return new WaitForSeconds (0.4f);//right animation delay is 0.4S
		m_TapCreator.Delete ();
		m_CurSentenceIndex++;
		StartCurSentence ();
	}

	protected StreamSentence m_CurrentStreamSentence;
	protected virtual void StartCurSentence ()
	{
		if (m_CurSentenceIndex >= CorePlayData.CurrentSong.NonRhythmSenteces.Count)
		{
			m_Status = Status.Finish;
			CorePlayManager.Instance.GameFinish ();
			return;
		}

		m_CurrentStreamSentence = CorePlayData.CurrentSong.NonRhythmSenteces[m_CurSentenceIndex];
		m_CurSentenceLength = m_CurrentStreamSentence.Duration;
		switch (m_CurrentStreamSentence.Type)
		{
			case SentenceType.NonRhythmCommon:
				m_Status = Status.Reading;

				PlayAudio ();
				break;
			case SentenceType.NonRhythmVoice:
				m_Status = Status.WaitForSpeech;
				if (string.IsNullOrEmpty (m_CurrentStreamSentence.Speaker) == false)
				{
					AudioCategory ac = AudioController.GetCategory ("AudioWords");
					var obj_speaker = ac.Play (m_CurrentStreamSentence.Speaker);
					obj_speaker.completelyPlayedDelegate = PlayNonRhythmVoice;
					m_Timer = 0;
				}
				else
				{
					PlayNonRhythmVoice ();
				}
				break;
		}
	}

	private void PlayNonRhythmVoice (AudioObject obj = null)
	{
		if (m_CurSentenceIndex == 0 ||
				   (m_CurSentenceIndex > 0 && CorePlayData.CurrentSong.NonRhythmSenteces[m_CurSentenceIndex - 1].Type == SentenceType.NonRhythmCommon))
		{
			VoiceController.Show (CreateVoiceSentence);
		}
		else
		{
			CreateVoiceSentence ();
		}
		m_Timer = 0;
	}

	public void Update ()
	{
		switch (m_Status)
		{
			case Status.Reading:
				break;
			case Status.WaitForClick:
			case Status.WaitForSpeech:
				CalcTimer ();
				break;
		}
	}

	void CalcTimer ()
	{
		m_Timer += Time.deltaTime;
		if (m_Timer > m_CurSentenceLength)
		{
			m_Timer = 0;
			CalcOneSentence ();
		}
	}

	void CalcOneSentence ()
	{
		switch (m_CurrentStreamSentence.Type)
		{
			case SentenceType.NonRhythmCommon:
				//无节奏 单词
				m_TapCreator.Delete ();
				m_InputCheck.CalcSentence (m_CurSentenceIndex + SentenceIndexOffset);
				m_CurSentenceIndex++;
				StartCurSentence ();
				break;
			case SentenceType.NonRhythmVoice:
				//无节奏 语音
				if (!m_GetVoice)
				{
					CorePlayManager.Instance.ComboBreak (false);
				}
				if (m_CurSentenceIndex < CorePlayData.CurrentSong.NonRhythmSenteces.Count - 1)
				{
					if (CorePlayData.CurrentSong.NonRhythmSenteces[m_CurSentenceIndex + 1].Type != SentenceType.NonRhythmVoice)
					{
						CorePlayManager.Instance.VoiceUIFadeOut ();
					}
				}
				else
				{
					CorePlayManager.Instance.VoiceUIFadeOut ();
				}
				m_CheckFinished = true;
				XunFeiSRManager.Instance.StopListen ();
				AudioController.SetGlobalVolume (1f);
				MicManager.Instance.StopRecord ();
				RepeatVoice (m_CurrentStreamSentence);
				break;
		}
	}

	private bool m_WaitingForRepeatFinish;
	private float m_WatingTimer;
	private const float WaitTimeLength = 6;
	void RepeatVoice (StreamSentence sentence)
	{
		m_Status = Status.RepeatingVoice;
		m_WaitingForRepeatFinish = true;
		CorePlayManager.Instance.ShowHorn ();

		if (m_AudioRepeatList.Count > 0)
		{
			// 暂时不支持拼字
			AudioCategory ac = AudioController.GetCategory ("AudioWords");
			var obj = ac.Play (m_AudioRepeatList[0]);
			//LogManager.Log ("play : " , m_AudioRepeatList[0]);

			if (obj != null)
			{
				obj.completelyPlayedDelegate = (v) => {
					MicManager.Instance.Play (sentence.needRecord, RepeatVoiceFinish, sentence.Sentence[0].Key);
				};
			}
		}
		else
		{
			MicManager.Instance.Play (sentence.needRecord, RepeatVoiceFinish, sentence.Sentence[0].Key);
		}
	}

	IEnumerator WaitingForRepeatVoiceFinish ()
	{
		while (m_WaitingForRepeatFinish)
		{
			m_WatingTimer += Time.deltaTime;
			if (m_WatingTimer >= WaitTimeLength)
			{
				break;
			}
			yield return null;
		}
		RepeatVoiceFinish ();
	}

	void RepeatVoiceFinish ()
	{
		if (m_Status == Status.RepeatingVoice)
		{
			m_WaitingForRepeatFinish = false;
			StaticMonoBehaviour.Instance.StopCoroutine (WaitingForRepeatVoiceFinish ());
			AudioController.SetGlobalVolume (1f);
			m_AudioRepeatList.Clear ();
			CorePlayManager.Instance.CloseHorn ();

			m_CurSentenceIndex++;
			StartCurSentence ();
			//LogManager.Log("repeat finish");
		}
	}

	private const int SentenceIndexOffset = 1000;
	protected virtual void CreateTapSentence ()
	{
		List<string> checkList = new List<string> ();
		checkList.Add (m_CurrentStreamSentence.Sentence[0].Key);
		m_InputCheck.CreateCheckSentence (checkList, m_CurSentenceIndex + SentenceIndexOffset, (int)(m_CurSentenceLength * RuntimeConst.SecondToMS));
		m_TapCreator.CreateWord (m_CurrentStreamSentence.Sentence[0].Key, m_CurrentStreamSentence.Sentence[0].Value, m_CurSentenceIndex + SentenceIndexOffset);
	}


	#region Voice Check Region
	private bool m_GetVoice = false;
	private bool m_CheckFinished = false;
	protected void CreateVoiceSentence ()
	{
		List<string> sentence = new List<string> ();
		sentence.Add (m_CurrentStreamSentence.Sentence[0].Key);
		m_CheckFinished = false;
		m_GetVoice = false;
		MicManager.Instance.StartRecord (GetVoiceCallback, m_InputCheck.SoundWave);
		XunFeiSRManager.Instance.StartListen (m_InputCheck.GetSRCallback);
		AudioController.SetGlobalVolume (CorePlaySettings.Instance.m_AudioVolume);

		m_InputCheck.GetVoiceCheckSentence (sentence, VoiceXunfeiCallback);

		CorePlayManager.Instance.CreateVoiceString (m_CurrentStreamSentence.Sentence[0].Key, m_CurrentStreamSentence.Duration);
	}

	public void VoiceXunfeiCallback (float accuracy)
	{
		if (!m_CheckFinished)
		{
			m_CheckFinished = true;
			m_GetVoice = true;
			CorePlayManager.Instance.AddVoicePoint ((int)(accuracy * 100f));//0~1转化为0~100的整数
			CalcOneSentence ();
		}
	}

	private void GetVoiceCallback ()
	{
		m_GetVoice = true;
		//LogManager.Log("key and value get voice back");
	}
	#endregion

	protected List<string> m_AudioRepeatList = new List<string> ();
	protected void PlayAudio ()
	{
		AudioCategory ac = AudioController.GetCategory ("AudioWords");
		if (string.IsNullOrEmpty (m_CurrentStreamSentence.Speaker) == false)
		{
			//LogManager.Log ("  m_CurrentStreamSentence.Speaker = " , m_CurrentStreamSentence.Speaker);
			var obj_speaker = ac.Play (m_CurrentStreamSentence.Speaker);
			obj_speaker.completelyPlayedDelegate = PlayWordFor;
		}
		else
		{
			PlayWordFor ();
		}
		m_AudioRepeatList.Add (m_CurrentStreamSentence.FileName);

	}

	private void PlayWordFor (AudioObject obj = null)
	{
		CreateTapSentence ();
		AudioCategory ac = AudioController.GetCategory ("AudioWords");
		ac.Play (m_CurrentStreamSentence.FileName);
		//LogManager.Log ("PlayWordFor : " , m_CurrentStreamSentence.FileName);
	}

	protected void ReadContentFinish ()
	{
		m_Timer = 0;
		m_Status = Status.WaitForClick;
	}

	public void Restart ()
	{
		m_CurSentenceIndex = 0;
		m_CheckFinished = false;
		m_TapCreator.Delete ();

		m_Status = Status.Idle;
		CorePlayInputCheck.SentenceAllRightEvent.RemoveListener (SentenceAllRightCallback);
		AudioController.SetGlobalVolume (1);
		CorePlayManager.Instance.CloseHorn ();
	}
}
