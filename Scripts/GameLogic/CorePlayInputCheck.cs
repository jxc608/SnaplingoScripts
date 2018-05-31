using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Events;
using System.Text;

public class CheckSentence
{
	public List<string> m_Content = new List<string> ();
	public List<int> m_StartMS = new List<int> ();
}

public class CorePlayInputCheck
{
	public static UnityEvent SentenceAllRightEvent = new UnityEvent ();
	public static UnityEvent GetVoiceComplete = new UnityEvent ();
	private CorePlayManager m_Manager;
	public CorePlayInputCheck (CorePlayManager manager)
	{
		m_Manager = manager;
		m_SentenceMap = new Dictionary<int, CheckSentence> ();
		SentenceAllRightEvent.RemoveAllListeners ();
		GetVoiceComplete.RemoveAllListeners ();
	}

	public bool CalcSentence (int sentenceIndex, bool changeLife = false)
	{
		if (!m_SentenceMap.ContainsKey (sentenceIndex))
			return false;
		if (m_SentenceMap[sentenceIndex].m_Content.Count > 0 && m_CheckIndexMap[sentenceIndex] < m_SentenceMap[sentenceIndex].m_Content.Count)
		{
			//时间到了 还没check完，肯定有没完成的，combo断掉，结算分数
			m_Manager.ComboBreak (true);
			if (changeLife)
				m_Manager.ChangeLife (RuntimeConst.LifeCutUnit);
			return false;
		}
		else
		{
			return true;
		}
	}

	public void RefreshSentenceCheck (int sentenceIndex)
	{
		m_CheckFinishedMap[sentenceIndex] = false;
		m_CheckIndexMap[sentenceIndex] = 0;
	}

	#region CreateSentence

	public void StartCheckSentenceWithStartMS (List<HitObject> hoList, int sentenceIndex)
	{
		CheckSentence cs = new CheckSentence ();
		for (int i = 0; i < hoList.Count; i++)
		{
			cs.m_Content.Add (hoList[i].Word);
			cs.m_StartMS.Add (hoList[i].StartMilliSecond);
		}
		m_SentenceMap.Add (sentenceIndex, cs);
	}

	public void StartCheckSentenceWithoutMS (List<string> content, int sentenceIndex)
	{
		CheckSentence cs = new CheckSentence ();
		for (int i = 0; i < content.Count; i++)
		{
			cs.m_Content.Add (content[i]);
		}
		m_SentenceMap.Add (sentenceIndex, cs);
	}

	//生成一个没有时间需求 只有顺序需求的判定句子 返回句子ID
	public void CreateCheckSentence (List<string> sentence, int sentenceIndex, int sentenceLength)
	{
		InitSentence (sentenceIndex, sentenceLength);
		StartCheckSentenceWithoutMS (sentence, sentenceIndex);
		WriteLog.Instance.writelog ("--------------------" + sentenceIndex + "----------------------");
	}

	//生成一个有时间需求 和顺序需求的判定句子，返回句子ID
	public void CreateCheckSentenceWithMS (List<HitObject> sentence, int sentenceIndex, int sentenceLength)
	{
		InitSentence (sentenceIndex, sentenceLength);
		StartCheckSentenceWithStartMS (sentence, sentenceIndex);
		WriteLog.Instance.writelog ("--------------------" + sentenceIndex + "----------------------");
	}

	void InitSentence (int sentenceIndex, int sentenceLength)
	{
		m_CheckFinishedMap.Add (sentenceIndex, false);
		m_CheckIndexMap.Add (sentenceIndex, 0);
		m_HOIndexMap.Add (sentenceIndex, 0);
		m_SentenceLengthMap.Add (sentenceIndex, sentenceLength);
	}

	#endregion

	public void SetCurHOIndex (int sentenceIndex, int index)
	{
		m_HOIndexMap[sentenceIndex] = index;
	}

	private Dictionary<int, bool> m_CheckFinishedMap = new Dictionary<int, bool> ();
	private Dictionary<int, int> m_SentenceLengthMap = new Dictionary<int, int> ();
	private Dictionary<int, int> m_HOIndexMap = new Dictionary<int, int> ();
	private Dictionary<int, CheckSentence> m_SentenceMap = new Dictionary<int, CheckSentence> ();
	private Dictionary<int, int> m_CheckIndexMap = new Dictionary<int, int> ();
	private Action<int, int, float> m_ClickRightCallback;
	private Action<int, int> m_ClickWrongCallback;
	/// <summary>
	/// 返回0 表示点准了
	/// 返回1 表示点对了
	/// 返回2 表示点错了
	/// </summary>
	/// <param name="hitWord"></param>
	/// <param name="scale"></param>
	/// <param name="sentenceIndex"></param>
	/// <returns></returns>
	public int GotHit (string hitWord, int sentenceIndex, int curMS)
	{
		int result = RuntimeConst.HitWrong;
		if (m_CheckFinishedMap[sentenceIndex])
		{
			ClickWrong (sentenceIndex, RuntimeConst.CheckFinished);
			return RuntimeConst.HitWrong;
		}

		if (m_SentenceMap[sentenceIndex].m_Content.Count == 0)
		{
			WriteLog.Instance.writelog ("点击单词：" + hitWord + ", 点击过早");
			ClickWrong (sentenceIndex, RuntimeConst.ClickTooEarly);
			return RuntimeConst.HitWrong;//只有开始播了 才能开始点击
		}

		//LogManager.Log("点击单词：" , hitWord , " // m_HighNum = " , m_HighNum , " // HitObjects.Count = " , CorePlayData.CurrentSentence.HitObjects.Count);
		if (string.Equals (hitWord, m_SentenceMap[sentenceIndex].m_Content[m_CheckIndexMap[sentenceIndex]]))
		{//点击匹配上了当前顺序的下一个词
			bool needCalcAccuracy = false;
			if (m_SentenceMap[sentenceIndex].m_StartMS.Count > 0)
			{
				result = CheckSentenceWithStartMS (m_CheckIndexMap[sentenceIndex], sentenceIndex, curMS);
				needCalcAccuracy = true;
			}
			else
			{//不用判断时间
				result = RuntimeConst.HitPerfect;
			}

			if (result == RuntimeConst.HitPerfect || result == RuntimeConst.HitRight)
			{
				float accuracy = 1;
				if (needCalcAccuracy)
					accuracy = 1 - Mathf.Abs (curMS - m_SentenceMap[sentenceIndex].m_StartMS[m_CheckIndexMap[sentenceIndex]]) / (float)m_SentenceLengthMap[sentenceIndex];

				ClickRight (sentenceIndex, m_CheckIndexMap[sentenceIndex], accuracy);
				m_CheckIndexMap[sentenceIndex]++;
				WriteLog.Instance.writelog ("点击单词：" + hitWord + ", 正常点");
			}
			else
			{
				ClickWrong (sentenceIndex, m_CheckIndexMap[sentenceIndex]);
			}
		}
		else if (m_SentenceMap[sentenceIndex].m_StartMS.Count >= 0 &&
				 m_CheckIndexMap[sentenceIndex] <= m_HOIndexMap[sentenceIndex] &&
				 string.Equals (hitWord, m_SentenceMap[sentenceIndex].m_Content[m_HOIndexMap[sentenceIndex]]))
		{//点击匹配了当前语音的圈圈
			result = CheckSentenceWithStartMS (m_HOIndexMap[sentenceIndex], sentenceIndex, curMS);
			float accuracy = 1 - Mathf.Abs (curMS - m_SentenceMap[sentenceIndex].m_StartMS[m_CheckIndexMap[sentenceIndex]]) / (float)m_SentenceLengthMap[sentenceIndex];

			ClickWrong (sentenceIndex, m_CheckIndexMap[sentenceIndex]);
			ClickRight (sentenceIndex, m_HOIndexMap[sentenceIndex], accuracy);
			m_CheckIndexMap[sentenceIndex] = m_HOIndexMap[sentenceIndex] + 1;
			WriteLog.Instance.writelog ("点击单词：" + hitWord + ", 正常点");
		}
		else
		{
			WriteLog.Instance.writelog ("点击单词：" + hitWord + ", 点击错误");
			ClickWrong (sentenceIndex, m_CheckIndexMap[sentenceIndex]);
			return RuntimeConst.HitWrong;
		}

		if (m_CheckIndexMap[sentenceIndex] == m_SentenceMap[sentenceIndex].m_Content.Count)
		{
			WriteLog.Instance.writelog ("点击完成");
			WriteLog.Instance.writelog ("*****************************************************************");
			m_CheckFinishedMap[sentenceIndex] = true;
			SentenceAllRightEvent.Invoke ();
		}

		return result;
	}

	public void SetClickCallback (Action<int, int, float> rightAction, Action<int, int> wrongAction)
	{
		m_ClickRightCallback = rightAction;
		m_ClickWrongCallback = wrongAction;
	}

	int CheckSentenceWithStartMS (int index, int sentenceIndex, int curMS)
	{
		if (curMS < m_SentenceMap[sentenceIndex].m_StartMS[index] - CorePlaySettings.Instance.m_ClickOffset)
		{
			WriteLog.Instance.writelog ("点击单词：" + m_SentenceMap[sentenceIndex].m_Content[index] + ", 点击过早");
			return RuntimeConst.HitWrong;//只有开始播了 才能开始点击
		}
		else if (curMS >= m_SentenceMap[sentenceIndex].m_StartMS[index] - CorePlaySettings.Instance.m_PerfectOffsetBefore &&
				 curMS <= m_SentenceMap[sentenceIndex].m_StartMS[index] + CorePlaySettings.Instance.m_PerfectOffsetAfter)
		{
			return RuntimeConst.HitPerfect;
		}
		else
		{
			return RuntimeConst.HitRight;
		}
	}

	public void ClickWrong (int sentenceIndex, int checkIndex)
	{
		if (m_ClickWrongCallback != null)
			m_ClickWrongCallback.Invoke (sentenceIndex, checkIndex);
	}

	public void ClickRight (int sentenceIndex, int checkIndex, float accuracy)
	{
		if (m_ClickRightCallback != null)
		{
			m_ClickRightCallback.Invoke (sentenceIndex, checkIndex, accuracy);
		}
	}

	#region VoiceInput
	private int m_VoiceCheckNumber;
	private Action<float> m_VoiceCompleteCallback;
	private PlayerReadingData m_CurrentReadingData;
	public void GetVoiceCheckSentence (List<string> list, Action<float> callback)
	{
		m_VoiceCompleteCallback = callback;
		m_VoiceCheckFinished = false;

		m_CurrentReadingData = new PlayerReadingData ();
		StringBuilder standardAnswer = new StringBuilder ();
		for (int i = 0; i < list.Count; i++)
		{
			standardAnswer.Append (list[i] + " ");
		}
		m_CurrentReadingData.standardAnswer = standardAnswer.ToString ();


		StringBuilder sb = new StringBuilder ();
		m_CurVoiceCheckList.Clear ();
		for (int i = 0; i < list.Count; i++)
		{
			if (LanguageManager.languageType == LanguageType.Chinese)
			{
				if (list[i].Contains (" "))
				{
					string[] words = list[i].Split (new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
					for (int j = 0; j < words.Length; j++)
					{
						m_CurVoiceCheckList.Add (words[j].ToLower ());
						sb.Append (words[j].ToLower () + " ");
					}
				}
				else
				{
					m_CurVoiceCheckList.Add (list[i].ToLower ());
					sb.Append (list[i].ToLower () + " ");
				}
			}
			else
			{
				string content = list[i];

				//有些高亮文字带()需要先去除
				if (content.Contains ("("))
				{
					while (content.Contains ("("))
					{
						content = content.Remove (content.IndexOf ('('), 1);
					}
				}
				if (content.Contains (")"))
				{
					while (content.Contains (")"))
					{
						content = content.Remove (content.IndexOf (')'), 1);
					}
				}

				//LogManager.LogError("GetVoiceCheckSentence,context length: " , content.Length);

				for (int j = 0; j < content.Length; ++j)
				{
					m_CurVoiceCheckList.Add (content[j].ToString ());
				}
			}

		}

		//LogManager.Log(" 普通 , Boss模式 检查语音:" , sb.ToString() ,"  / " , Time.frameCount);
		VoiceController.Start_RecordingAnim ();
		m_VoiceCheckNumber = m_CurVoiceCheckList.Count;
	}

	private List<string> m_CurVoiceCheckList = new List<string> ();
	private bool m_VoiceCheckFinished;
	/// <summary>
	/// 既是讯飞语音的回调，也是检查匹配函数
	/// </summary>
	/// <param name="content"></param>
	public void GetSRCallback (string content)
	{
		LogManager.Log ("讯飞got：", content);
		if (!m_VoiceCheckFinished)
		{
			//  "语音"  每句提前结束 
			LogManager.Log ("  ____ \"语音\"  每句提前结束 ");

			m_CurrentReadingData.playerAnswer = content;

			int rightNumber = 0;
			string[] words;
			if (LanguageManager.languageType == LanguageType.Chinese)
			{
				words = content.ToLower ().Split (new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
			}
			else
			{
				LogManager.Log ("讯飞got content length: ", content.Length);
				words = new string[content.Length];
				for (int i = 0; i < content.Length; ++i)
				{
					words[i] = content[i].ToString ();
				}
				LogManager.Log ("讯飞got words length: ", content.Length);
			}

			for (int i = 0; i < words.Length; i++)
			{
				if (m_CurVoiceCheckList.Contains (words[i]))
				{
					rightNumber++;
					m_CurVoiceCheckList.Remove (words[i]);
					LogManager.Log ("voice check right word:", words[i]);
				}
			}
			AnalysisManager.Instance.OnEvent ("100003", null, StaticData.LevelID.ToString (), "发音次数");
			float accuracy = (float)rightNumber / m_VoiceCheckNumber;
			m_CurrentReadingData.accuracy = accuracy;
			m_CurrentReadingData.score = (int)(accuracy * CorePlaySettings.Instance.m_VoiceRightPoint);
			CorePlayData.PlayerReadingData.Add (m_CurrentReadingData);
			m_CurrentReadingData = null;
			AnalysisManager.Instance.OnEvent ("100004", null, StaticData.LevelID.ToString (), accuracy.ToString ());
			CorePlayManager.Instance.m_SumVoiceScore += accuracy;

			m_VoiceCheckFinished = true;
			if (m_VoiceCompleteCallback != null)
			{
				m_VoiceCompleteCallback.Invoke (accuracy);
				m_VoiceCompleteCallback = null;
			}
		}
	}

	public void SilentCallback ()
	{

	}

	public void SoundWave (float value)
	{
		VoiceController.InputVolume = value;
	}

	#endregion

	public void Restart ()
	{
		m_HOIndexMap.Clear ();
		m_CurVoiceCheckList.Clear ();
		m_CheckFinishedMap.Clear ();
		m_SentenceLengthMap.Clear ();
		m_SentenceMap.Clear ();
		m_CheckIndexMap.Clear ();
		m_ClickRightCallback = null;
		m_ClickWrongCallback = null;
	}
}
