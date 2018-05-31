using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class TeachButtonControlBase : MonoBehaviour
{

	protected Dropdown m_DropDown;
	protected SongObject m_CurrentSong;
	protected TeachRhythmCreator m_TapCreator;
	protected Queue<CorePlayTimerEvent> m_TimerEvents;

	protected bool m_GameStart;
	// Use this for initialization
	protected virtual void Start()
	{
		CheckPlatformDir();

		m_DropDown = transform.Find("Dropdown").GetComponent<Dropdown>();
		m_DropDown.ClearOptions();
		string[] list = null;
		string filePath = GetPathString("EditFileList.txt");
		if (File.Exists(filePath))
		{
			list = File.ReadAllLines(filePath);
		}
		else
		{
			PromptManager.Instance.MessageBox(PromptManager.Type.FloatingTip, "没有找到 EditFileList.txt 文件！！");
		}
		if (list != null)
		{
			List<string> options = new List<string>(list);
			m_DropDown.AddOptions(options);
		}

		m_TapCreator = new TeachRhythmCreator();
		transform.Find("Listen").GetComponent<Button>().onClick.AddListener(ListenMusic);
		transform.Find("Pause").GetComponent<Button>().onClick.AddListener(Stop);
		transform.Find("Record").GetComponent<Button>().onClick.AddListener(RecordMusic);
		transform.Find("Return").GetComponent<Button>().onClick.AddListener(Return);
		m_GameStart = false;
	}

	protected void Return()
	{
		TeachToolPage.Instance.BackToChoosePage();
	}

	protected void ClearChildContent(Transform trans)
	{
		for (int i = trans.childCount - 1; i >= 0; i--)
		{
			Destroy(trans.GetChild(i).gameObject);
		}
	}

#if UNITY_STANDALONE_WIN
    protected string Platform = "/Windows";
#elif UNITY_STANDALONE_OSX
    protected string Platform = "/MacOS";
#else
	protected string Platform = "";
#endif

	protected string GetPathString(string fileName)
	{
		string path = "";
		path = Application.dataPath + "/TeachAudioBundles" + Platform + "/" + fileName;
		return path;
	}

	protected void CheckPlatformDir()
	{
		string dirPath = Application.dataPath + "/TeachAudioBundles" + Platform;
		if (!Directory.Exists(dirPath))
		{
			Directory.CreateDirectory(dirPath);
		}
	}

	protected float m_Timer;
	protected const float Second2MilliSecond = 1000;
	// Update is called once per frame
	protected virtual void Update()
	{
		if (m_GameStart)
		{
			m_Timer = CorePlayMusicPlayer.Instance.GetBgmTime() * Second2MilliSecond;

			while (m_TimerEvents.Count > 0 && m_Timer >= m_TimerEvents.ToArray()[0].m_Timer)
			{
				ProcessEvents(m_TimerEvents.Dequeue());
			}
		}
	}

	protected virtual void ProcessEvents(CorePlayTimerEvent te)
	{
		switch (te.m_Type)
		{
			case CorePlayTimerEvent.EventType.CreateTapSentence:
				m_TapCreator.CreateWords(CorePlayData.CurrentSong.SentenceObjs[te.m_SentenceIndex], te.m_SentenceIndex);
				break;

			case CorePlayTimerEvent.EventType.HighLightTap:
				m_TapCreator.HighLightWord(CorePlayData.CurrentSong.SentenceObjs[te.m_SentenceIndex].ClickAndHOList.HitObjects, te.m_HOIndex, 0);
				AudioController.Play("dong");
				break;

			case CorePlayTimerEvent.EventType.StartPlayMusic:
				StartCurrentSong();
				break;

		}
	}

	public virtual void StartCurrentSong()
	{
		CorePlayMusicPlayer.Instance.PlaySong(Path.GetFileNameWithoutExtension(CorePlayData.CurrentSong.AudioFileName.Trim()));
		AudioListener.volume = 1f;

		WriteLog.Instance.fileName(CorePlayData.CurrentSong.Title);
		WriteLog.Instance.writelog("======================" + CorePlayData.CurrentSong.Title + "===================");
	}

	protected virtual void ListenMusic()
	{
		if (!m_GameStart)
		{
			string songName = m_DropDown.options[m_DropDown.value].text;
			CorePlayData.CurrentSong = BeatmapParse.Parse(songName);
			CorePlayMusicPlayer.Instance.LoadSong(Path.GetFileNameWithoutExtension(CorePlayData.CurrentSong.AudioFileName.Trim()));

			TeachCreateEvents createEvents = new TeachCreateEvents();
			m_TimerEvents = createEvents.GetTimerEventQueue();

			m_GameStart = true;
		}
	}

	protected virtual void Stop()
	{
		m_CurrentSong = null;
		m_GameStart = false;
		CorePlayMusicPlayer.Instance.StopMusic();
	}

	protected virtual void RecordMusic()
	{

	}
}
