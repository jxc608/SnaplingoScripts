using System.Collections.Generic;
using UnityEngine;

public class SongInfo
{
	private string m_SongName;
	private string m_AudioFileName;
	private int m_BPM;

	public string SongName
	{
		get { return m_SongName; }
		set { m_SongName = value; }
	}

	public string AudioFileName
	{
		get { return m_AudioFileName; }
		set { m_AudioFileName = value; }
	}

	public int BPM
	{
		get { return m_BPM; }
		set { m_BPM = value; }
	}

	public SongInfo(string songName = null, string audioName = null, int bpm = 100)
	{
		SongName = songName;
		AudioFileName = audioName;
		BPM = bpm;
	}
}

public class HitInfo
{
	private int m_HitTime;
	private Vector2 m_Position;
	private int m_StartHitTime;
	private int m_EndHitTime;
	private SentenceInfo.SentenceType m_Type;
	private string m_SoundText;
	private string m_ClickText;
	
	public int HitTime
	{
		get { return m_HitTime; }
		set { m_HitTime = value; }
	}
	public Vector2 Position
	{
		get { return m_Position; }
		set { m_Position = value; }
	}
	public int StartHitTime
	{
		get { return m_StartHitTime; }
		set { m_StartHitTime = value; }
	}
	public int EndHitTime
	{
		get { return m_EndHitTime; }
		set { m_EndHitTime = value; }
	}
	public SentenceInfo.SentenceType Type
	{
		get { return m_Type; }
		set { m_Type = value; }
	}
	public string SoundText
	{
		get { return m_SoundText; }
		set { m_SoundText = value; }
	}
	public string ClickText
	{
		get { return m_ClickText; }
		set { m_ClickText = value; }
	}
}

public class SentenceInfo
{
	public enum SentenceType { ClickNode, SoundNode }

	private SentenceType m_Type;
	public SentenceType Type
	{
		get { return m_Type; }
		set { m_Type = value; }
	}

	private int m_HitNum;
	public int HitNum
	{
		get { return m_HitNum; }
		set { m_HitNum = value; }
	}

	private int m_StartTime;
	public int StartTime
	{
		get { return m_StartTime; }
		set { m_StartTime = value; }
	}

	private int m_EndTime;
	public int EndTime
	{
		get { return m_EndTime; }
		set { m_EndTime = value; }
	}
}

public class BossLineInfo
{
	private int m_KeepTimeLength;
	public int KeepTimeLength
	{
		get { return m_KeepTimeLength; }
		set { m_KeepTimeLength = value; }
	}

	private bool m_IsEnd;
	public bool IsEnd
	{
		get { return m_IsEnd; }
		set { m_IsEnd = value; }
	}

	private string m_SoundText;
	public string SoundText
	{
		get { return m_SoundText; }
		set { m_SoundText = value; }
	}

	private string m_ClickText;
	public string ClickText
	{
		get { return m_ClickText; }
		set { m_ClickText = value; }
	}

	private string m_MusicFileName;
	public string MusicFileName
	{
		get { return m_MusicFileName; }
		set { m_MusicFileName = value; }
	}

	private SentenceInfo.SentenceType m_Type;
	public SentenceInfo.SentenceType Type
	{
		get { return m_Type; }
		set { m_Type = value; }
	}
}

public class BossInfo
{
	public bool IsBoss;
	private int m_StartTime;
	public int StartTime
	{
		get { return m_StartTime; }
		set { m_StartTime = value; }
	}

	private List<BossLineInfo> m_BossLineObject = new List<BossLineInfo>();
	public List<BossLineInfo> BossLineObject
	{
		get { return m_BossLineObject; }
		set { m_BossLineObject = value; }
	}

	private int m_DelayTime;
	public int DelayTime
	{
		get { return m_DelayTime; }
		set { m_DelayTime = value; }
	}
}

public class SongTxtObject
{
	private SongInfo m_SongInfos = new SongInfo();
	public SongInfo SongInfos
	{
		get { return m_SongInfos; }
		set { m_SongInfos = value; }
	}

	private List<HitInfo> m_HitInfos = new List<HitInfo>();
	public List<HitInfo> HitInfos
	{
		get { return m_HitInfos; }
		set { m_HitInfos = value; }
	}

	private BossInfo m_BossInfos = new BossInfo();
	public BossInfo BossInfos
	{
		get { return m_BossInfos; }
		set { m_BossInfos = value; }
	}

}