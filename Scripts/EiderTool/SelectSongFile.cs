using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using System.IO;


public class SelectSongFile : MonoBehaviour
{
	private Dropdown m_SongDown;
	private string m_SongPath = "Assets/Resources/Songs";
	private string m_RunTimeSongPath = "/Resources/Songs";

	private Dropdown m_AudioDown;
	private string m_AudioPath = "Assets/Resources/Audio/Songs";
	private string m_RunTimeAudioPath = "/Resources/Audio/Songs";
	private string m_SongName;

	void Start()
	{
		m_SongName = null;
		m_SongDown = transform.Find("Songdown").GetComponent<Dropdown>();
		m_SongDown.ClearOptions();

		m_AudioDown = transform.Find("Audiodown").GetComponent<Dropdown>();
		m_AudioDown.ClearOptions();

		List<string> options = null;
		if (DebugConfigController.Instance.RunTimeEditor)
		{
			options = GetFileName("*.txt", Application.dataPath + m_RunTimeSongPath);
		}
		else
		{
			options = GetFileName("*.txt", m_SongPath);
		}
	
		if (options != null)
		{
			m_SongDown.AddOptions(options);
		}

		if (DebugConfigController.Instance.RunTimeEditor)
		{
			options = GetFileName("*.mp3", Application.dataPath + m_RunTimeAudioPath);
		}
		else
		{
			options = GetFileName("*.mp3", m_AudioPath);
		}
		//options = GetFileName("*.mp3", m_AudioPath);
		if (options != null)
		{
			m_AudioDown.AddOptions(options);
		}

		transform.Find("Enter").GetComponent<Button>().onClick.AddListener(Enter);
		transform.Find("Back").GetComponent<Button>().onClick.AddListener(Back);
	}

	private void Update()
	{
		if (m_SongName != m_SongDown.transform.Find("Label").GetComponent<Text>().text)
		{
			m_SongName = m_SongDown.transform.Find("Label").GetComponent<Text>().text;
			EiderToolPage.Instance.SongObject.SongInfos.SongName = m_SongName;
			SongTxtObject ho = ReadSongEider.GetSongInfo(m_SongName);
			if (ho != null)
			{
				EiderToolPage.Instance.SongObject.SongInfos.BPM = ho.SongInfos.BPM;
				EiderToolPage.Instance.SongObject.HitInfos = ho.HitInfos;
				EiderToolPage.Instance.SongObject.BossInfos = ho.BossInfos;
				m_AudioDown.transform.Find("Label").GetComponent<Text>().text = ho.SongInfos.AudioFileName.Replace(".mp3", "");
			}
		}
	}

	private void Back()
	{
		EiderToolPage.Instance.BackToBegin();
		m_SongName = null;
	}

	private void Enter()
	{
		EiderToolPage.Instance.OpenEiderPage();
		string audioName = m_AudioDown.transform.Find("Label").GetComponent<Text>().text;
		EiderToolPage.Instance.SongObject.SongInfos.AudioFileName = audioName;
		EiderSong.Instance.AudioFileName = audioName;
		EiderSong.Instance.transform.Find("Bpm").GetComponent<InputField>().text = EiderToolPage.Instance.SongObject.SongInfos.BPM.ToString();
		CorePlayMusicPlayer.Instance.LoadSongTest(audioName);
		float audioLen = CorePlayMusicPlayer.Instance.GetAudioTimeLengh(audioName);
		EiderSong.Instance.transform.Find("PlayAudio").Find("PlayTime").GetComponent<Slider>().maxValue = audioLen;
	}

	private List<string> GetFileName(string partFileName, string filePath )
	{
		List<string> audioFileName = new List<string>();
		DirectoryInfo dirInfo = new DirectoryInfo(filePath);
		if (dirInfo.Exists)
		{
			FileInfo[] fis = dirInfo.GetFiles(partFileName);
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

}
