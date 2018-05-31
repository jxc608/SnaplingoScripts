using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitObject
{
	private Vector2 m_Position;
	public Vector2 Position {
		get { return m_Position; }
	}
	private int m_StartMilliSecond;
	public int StartMilliSecond {
		get { return m_StartMilliSecond; }
	}
	private int m_Type;//节奏点类型
	public int Type {
		get { return m_Type; }
	}
	private int m_SoundType;//音效类型
	public int SoundType {
		get { return m_SoundType; }
	}

	private string m_Word;
	public string Word {
		get { return m_Word; }
		set { m_Word = value; }
	}

	private string m_SoundFile;
	public string SonudFile {
		get { return m_SoundFile; }
		set { m_SoundFile = value; }
	}

	public HitObject (Vector2 pos, int startTime, int type, int soundType)
	{
		m_Position = pos;
		m_StartMilliSecond = startTime;
		m_Type = type;
		m_SoundType = soundType;
		m_SoundFile = "";
	}
}
