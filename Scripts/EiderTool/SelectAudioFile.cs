using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using System.IO;


public class SelectAudioFile : MonoBehaviour
{
	private Dropdown m_DropDown;
	private string m_AudioPath = "Assets/Resources/Audio/Songs";
	private string m_RunTimeAudioPath = "/Resources/Audio/Songs";

	void Start()
	{
		m_DropDown = transform.Find("Dropdown").GetComponent<Dropdown>();
		m_DropDown.ClearOptions();
		List<string> options = GetAudioFileName();
		
		if (options != null)
		{
			m_DropDown.AddOptions(options);
		}

		transform.Find("Enter").GetComponent<Button>().onClick.AddListener(Enter);
		transform.Find("Back").GetComponent<Button>().onClick.AddListener(Back);
	}

	private void Back()
	{
		EiderToolPage.Instance.BackToBegin();
	}

	private void Enter()
	{
		EiderToolPage.Instance.OpenEiderPage();
		string audioName = m_DropDown.transform.Find("Label").GetComponent<Text>().text;
		EiderToolPage.Instance.SongObject.SongInfos.AudioFileName = audioName;
		EiderSong.Instance.AudioFileName = audioName;
		EiderSong.Instance.transform.Find("Bpm").GetComponent<InputField>().text = EiderToolPage.Instance.SongObject.SongInfos.BPM.ToString();
		CorePlayMusicPlayer.Instance.LoadSongTest(audioName);
		float audioLen = CorePlayMusicPlayer.Instance.GetAudioTimeLengh(audioName);
		EiderSong.Instance.transform.Find("PlayAudio").Find("PlayTime").GetComponent<Slider>().maxValue = audioLen;
	}

	private List<string> GetAudioFileName()
	{
		List<string> audioFileName = new List<string>();
		if (DebugConfigController.Instance.RunTimeEditor)
		{
			m_AudioPath =  Application.dataPath + m_RunTimeAudioPath;
		}
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

}
