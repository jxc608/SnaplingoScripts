using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ClickObj
{
	public string m_Word;
	public int m_ClickTimes;
	public Vector2 m_Position;
	public string m_ShowWord;

	public ClickObj(string wordMatch, Vector2 position, string wordShow)
	{
		m_Word = wordMatch;
		m_ClickTimes = 1;
		m_Position = position;
		m_ShowWord = wordShow;
	}
}

// 特效 in out 时机
public enum EffectInOutTiming
{
	Default,
	DelayOut,
	AheadIn,
	Both,
}

public class InOutTime
{
	private EffectInOutTiming m_InOut;
	public EffectInOutTiming InOut
	{
		get { return m_InOut; }
		set { m_InOut = value; }
	}

	private int m_StartTime;
	public int StartTime
	{
		get { return m_StartTime; }
	}

	private int m_EndTime;
	public int EndTime
	{
		get { return m_EndTime; }
	}

	public InOutTime(int startTime, int endTime)
	{
		m_StartTime = startTime;
		m_EndTime = endTime;
	}
}

public class ClickAndHOList
{
	private List<ClickObj> m_ClickObjs = new List<ClickObj>();
	public List<ClickObj> ClickObjs
	{
		get { return m_ClickObjs; }
	}

	private List<HitObject> m_HitObjects = new List<HitObject>();
	public List<HitObject> HitObjects
	{
		get { return m_HitObjects; }
	}
}

public enum SentenceType
{
	Common, Voice, BossStart,
	/// <summary>
	/// 点读开始
	/// </summary>
	KeyVoiceStart,
	/// <summary>
	/// 无节奏 单词
	/// </summary>
	NonRhythmCommon,
	/// <summary>
	/// 无节奏 语音
	/// </summary>
	NonRhythmVoice
}

public class SentenceObj
{
	private SentenceType m_Type;
	public SentenceType Type
	{
		get { return m_Type; }
		set { m_Type = value; }
	}

	public InOutTime m_InOutTime;

	private ClickAndHOList m_ClickAndHOList = new ClickAndHOList();
	public ClickAndHOList ClickAndHOList
	{
		get { return m_ClickAndHOList; }
	}
}

public class StreamSentence
{
	private SentenceType m_Type;
	public SentenceType Type
	{
		get { return m_Type; }
	}

	float m_Duration;
	public float Duration
	{
		get { return m_Duration; }
	}

	List<KeyValuePair<string, string>> m_Sentence = new List<KeyValuePair<string, string>>(); // key: word, value : show word
	public List<KeyValuePair<string, string>> Sentence
	{
		get { return m_Sentence; }
	}

	private ClickAndHOList m_ClickAndHOList;
	public ClickAndHOList ClickAndHOList
	{
		get { return m_ClickAndHOList; }
	}

	string m_Speaker;
	public string Speaker
	{
		get { return m_Speaker; }
		set { m_Speaker = value; }
	}

	string m_AudioFileName;
	public string FileName
	{
		get { return m_AudioFileName; }
	}

	public bool needRecord;


	public StreamSentence()
	{
		m_Sentence.Clear();
		m_ClickAndHOList = new ClickAndHOList();
	}

	public void AddToSentence(KeyValuePair<string, string> kv)
	{
		m_Sentence.Add(kv);
	}

	public void SetAudioFileName(string fileName)
	{
		m_AudioFileName = fileName;
	}

	public void SetDuration(float duration)
	{
		m_Duration = duration;
	}

	public void SetType(SentenceType type)
	{
		m_Type = type;
	}
}

public class StreamSentenceGroup
{
	private SentenceType m_Type;
	public SentenceType Type
	{
		get { return m_Type; }
	}

	List<StreamSentence> m_Group = new List<StreamSentence>();
	public List<StreamSentence> Group
	{
		get { return m_Group; }
	}

	int m_TimeLength;
	public int TimeLength
	{
		get { return m_TimeLength; }
	}
	private bool m_CalcSentence;
	public bool CalckSentence
	{
		get { return m_CalcSentence; }
		set { m_CalcSentence = value; }
	}

	public StreamSentenceGroup()
	{
		m_Group.Clear();
	}

	public void AddSentence(StreamSentence sentence)
	{
		m_Group.Add(sentence);
	}

	public void SetSentenceType(SentenceType type)
	{
		m_Type = type;
	}

	public void SetTimeLength(int timeLength)
	{
		m_TimeLength = timeLength;
	}

	public StreamSentence GetOneSentence()
	{
		if (m_Group.Count > 0)
		{
			int randomNumber = UnityEngine.Random.Range(0, m_Group.Count);
			return m_Group[randomNumber];
		}
		else
		{
			return null;
		}
	}
}


[Serializable]
public class SongObject
{
	List<SentenceObj> m_SentenceObjs = new List<SentenceObj>();
	public List<SentenceObj> SentenceObjs
	{
		get { return m_SentenceObjs; }
	}

	List<StreamSentenceGroup> m_BossSentences = new List<StreamSentenceGroup>();
	public List<StreamSentenceGroup> StreamSentences
	{
		get { return m_BossSentences; }
	}

	List<StreamSentence> m_NonRhythmSentences = new List<StreamSentence>();
	public List<StreamSentence> NonRhythmSenteces
	{
		get { return m_NonRhythmSentences; }
	}

	[SerializeField]
	string m_AudioFileName;
	public string AudioFileName
	{
		get { return m_AudioFileName; }
		set { m_AudioFileName = value; }
	}

	// string m_LyricName;
	//public string LyricName
	//{
	//    get { return m_LyricName; }
	//    set { m_LyricName = value; }
	//}

	[SerializeField]
	int m_BossWarMusicDelay;
	public int BossWarMusicDelay
	{
		get { return m_BossWarMusicDelay; }
		set { m_BossWarMusicDelay = value; }
	}

	[SerializeField]
	string m_Title;
	public string Title
	{
		get { return m_Title; }
	}

	[SerializeField]
	float m_BPM;
	public float BPM
	{
		get { return m_BPM; }
	}


	public void SetContents(Dictionary<string, string> contentList)
	{
		m_AudioFileName = contentList["AudioFileName:"];
		m_Title = contentList["Title:"];
		m_BPM = float.Parse(contentList["BPM:"]);
	}

	//测试用
	public string GetSongInfo()
	{
		return "The song name is:" + m_Title +
			   ".\n Audio File Name is:" + m_AudioFileName +
			   ".\n BPM is:" + m_BPM +
			   ".\n SentenceObjs number is:" + m_SentenceObjs.Count;
	}

	public void WriteToTxt()
	{
		//TODO
	}
}
//SongObject