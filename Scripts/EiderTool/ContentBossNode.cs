using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Snaplingo.UI;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class ContentBossNode : Node
{
	public enum ContentBossType { AddNew, Editer, Insert }
	private ContentBossType m_ContentType;
	private string m_AudioPath = "Assets/Resources/Audio/Words";
	public BossLineInfo m_EditerLine = new BossLineInfo();
	private int m_BossLineNum;
	public Dropdown m_MusicFile;
	public InputField m_KeepTime;
	public InputField m_SoundText;
	public InputField m_ClickText;
	public Dropdown m_Type;
	public EditBossNode m_BossNode;
	public Dropdown m_IsEnd;

	public override void Init(params object[] args)
	{
		base.Init(args);
		transform.Find("OK").GetComponent<Button>().onClick.AddListener(OkClose);
		transform.Find("Cancle").GetComponent<Button>().onClick.AddListener(Cancle);

		m_MusicFile.onValueChanged.AddListener(ResetMusicFile);
		m_KeepTime.onValueChanged.AddListener(ResetKeepTime);
		m_SoundText.onValueChanged.AddListener(ResetSoundText);
		m_ClickText.onValueChanged.AddListener(ResetClickText);
		m_Type.onValueChanged.AddListener(ResetType);
		m_IsEnd.onValueChanged.AddListener(IsEnd);
	}

	public override void Open()
	{
		base.Open();
		m_MusicFile.ClearOptions();
		List<string> options = GetAudioFileName();
		if (options != null)
		{
			m_MusicFile.AddOptions(options);
		}
		m_ContentType = ContentBossType.AddNew;
		

	}

	private void ResetMusicFile( int m )
	{
		m_EditerLine.MusicFileName = m_MusicFile.options[m].text;
	}

	private void ResetKeepTime( string content )
	{
		m_EditerLine.KeepTimeLength = int.Parse(content);
	}

	private void ResetSoundText( string content )
	{
		m_EditerLine.SoundText = content;
	}

	private void ResetClickText( string content )
	{
		m_EditerLine.ClickText = content;
	}

	private void ResetType(int m)
	{
		string content = m_Type.options[m].text;
		if (content == "Click")
			m_EditerLine.Type = SentenceInfo.SentenceType.ClickNode;
		else
			m_EditerLine.Type = SentenceInfo.SentenceType.SoundNode;
	}

	private void IsEnd(int m)
	{
		if (m == 0)
		{
			m_EditerLine.IsEnd = false;
		}
		else
		{
			m_EditerLine.IsEnd = true;
		}
	}

	private List<string> GetAudioFileName()
	{
		List<string> audioFileName = new List<string>();
		DirectoryInfo dirInfo = new DirectoryInfo(m_AudioPath);
		if (dirInfo.Exists)
		{
			FileInfo[] fis = dirInfo.GetFiles("*.mp3");
			if (fis.Length > 0)
			{
				foreach (FileInfo file in fis)
				{
					audioFileName.Add(Path.GetFileNameWithoutExtension(file.Name));
				}
			}
			else
				return null;
		}
		return audioFileName;
	}

	public void Eider(int k)
	{
		m_ContentType = ContentBossType.Editer;
		m_BossLineNum = k;
		BossLineInfo temp = EiderToolPage.Instance.SongObject.BossInfos.BossLineObject[m_BossLineNum];
		m_EditerLine = new BossLineInfo();
		m_EditerLine.KeepTimeLength = temp.KeepTimeLength;
		m_EditerLine.SoundText = temp.SoundText;
		m_EditerLine.ClickText = temp.ClickText;
		m_EditerLine.MusicFileName = temp.MusicFileName;
		m_EditerLine.Type = temp.Type;
		m_EditerLine.IsEnd = temp.IsEnd;
		DisPlayInfo();
	}

	public void AddNew()
	{
		m_ContentType = ContentBossType.AddNew;
		m_EditerLine = new BossLineInfo();
		m_EditerLine.KeepTimeLength = 0;
		m_EditerLine.SoundText = "";
		m_EditerLine.ClickText = "";
		m_EditerLine.MusicFileName = "";
		m_EditerLine.Type = SentenceInfo.SentenceType.ClickNode;
		m_EditerLine.IsEnd = false;
		DisPlayInfo();
	}

	public void Insert(int k)
	{
		m_ContentType = ContentBossType.Insert;
		m_BossLineNum = k;
		m_EditerLine = new BossLineInfo();
		m_EditerLine.KeepTimeLength = 0;
		m_EditerLine.SoundText = "";
		m_EditerLine.ClickText = "";
		m_EditerLine.MusicFileName = "";
		m_EditerLine.Type = SentenceInfo.SentenceType.ClickNode;
		m_EditerLine.IsEnd = false;
		DisPlayInfo();
	}

	private void DisPlayInfo()
	{
		m_KeepTime.text = m_EditerLine.KeepTimeLength.ToString();
		m_SoundText.text = m_EditerLine.SoundText;
		m_ClickText.text = m_EditerLine.ClickText;
		m_MusicFile.transform.Find("Label").GetComponent<Text>().text = m_EditerLine.MusicFileName;
		if (m_EditerLine.Type == SentenceInfo.SentenceType.SoundNode)
			m_Type.value = 1;
		else
			m_Type.value = 0;

		if (m_EditerLine.IsEnd == false)
			m_IsEnd.value = 0;
		else
			m_IsEnd.value = 1;
	}

	private void OkClose()
	{
		if (OnRight() == false)
		{
			return;
		}
		else
		{
			if (m_ContentType == ContentBossType.AddNew)
				EiderToolPage.Instance.SongObject.BossInfos.BossLineObject.Add(m_EditerLine);
			else if (m_ContentType == ContentBossType.Insert)
				EiderToolPage.Instance.SongObject.BossInfos.BossLineObject.Insert(m_BossLineNum, m_EditerLine);
			else
				EiderToolPage.Instance.SongObject.BossInfos.BossLineObject[m_BossLineNum] = m_EditerLine;

			Close();
			m_BossNode.transform.Find("BossNode").gameObject.SetActive(true);
			m_BossNode.DisPlay();
		}
	}

	private void Cancle()
	{
		Close();
		m_BossNode.transform.Find("BossNode").gameObject.SetActive(true);
		m_BossNode.DisPlay();
	}

	private bool OnRight()
	{
		if (m_EditerLine.KeepTimeLength <= 0)
			return false;
		if (string.IsNullOrEmpty(m_EditerLine.SoundText))
			return false;
		if (string.IsNullOrEmpty(m_EditerLine.ClickText))
			return false;
		if (string.IsNullOrEmpty(m_EditerLine.MusicFileName))
			return false;
		if (string.IsNullOrEmpty(m_EditerLine.Type.ToString()))
			return false;

		string sound = m_EditerLine.SoundText;
		string click = m_EditerLine.ClickText;

		string[] soundList = sound.Split('$');
		string[] clickList = click.Split('$');

		if (soundList.Length != clickList.Length)
			return false;

		string soundTemp = "";
		string clickTemp = "";
		for (int i = 0; i < soundList.Length; i++)
		{
			string temp = "";
			if (string.IsNullOrEmpty(soundList[i]))
				temp = " ";
			else
				temp = soundList[i];
			if (soundTemp == "")
				soundTemp = temp;
			else
				soundTemp = soundTemp + "$" + temp;
		}

		for (int i = 0; i < clickList.Length; i++)
		{
			string temp = "";
			if (string.IsNullOrEmpty(clickList[i]))
				temp = " ";
			else
				temp = clickList[i];
			if (clickTemp == "")
				clickTemp = temp;
			else
				clickTemp = clickTemp + "$" + temp;
		}

		m_EditerLine.SoundText = soundTemp;
		m_EditerLine.ClickText = clickTemp;

		return true;
	}
}
