using UnityEngine.UI;
using UnityEngine;
using DG.Tweening;
using Snaplingo.SaveData;
using System.Collections.Generic;

public class TutorialScene : MonoBehaviour
{
	public enum TutorialStatus
	{
		ErJi,
		StartPlayMusic,
		PlayBatchKey,
		ClickBatchKey,
		RetryPlayBatchKey,
		StartSound,
		PlaySound,
		RetryPlaySound,
		CheckSound,
		Nothing
	}

	public enum Status
	{
		Sound,
		Click
	}

	public static TutorialScene Instance;
	public TutorialStatus m_Status = TutorialStatus.Nothing;
	public Status m_PlayStatus = Status.Sound;
	private float m_Beat;
	private float BPM;
	private float m_Timer;
	public TutotialScaler m_TutotialScaler;
	public GameObject m_Skip;
	public AudioSource m_DrumFast;
	public GameObject m_Zhuyi;
	public GameObject obj_hand;
	public bool HasHand;
	public RhythmTap m_Tap;
	public GameObject m_Erji;

	public RhythmController m_TapController;

	public bool Pause;

	private int RepetNum;
	private bool ClickPefect;

	private AudioObject obj_ClickSound;

	private void Start()
	{
		Instance = this;
		Pause = false;
		BPM = 60;
		//m_Status = TutorialStatus.StartPlayMusic;
		m_Status = TutorialStatus.ErJi;
		m_Beat = 60000f / BPM;
		m_Skip.transform.GetComponent<Button>().onClick.AddListener(() =>
		{
			AnalysisManager.Instance.OnEvent("100005", null, "新手", "跳过新手");
			Skip(null);
		});
		m_Zhuyi.SetActive(false);
		RepetNum = 0;
		ClickPefect = false;
	}

	private void Update()
	{
		if (Pause == false)
		{
			if (m_Status == TutorialStatus.ErJi)
			{
				m_Status = TutorialStatus.Nothing;
				m_Erji.SetActive(true);
                AudioObject audioObj = null;
                if(LanguageManager.languageType == LanguageType.Chinese)
                {
                    audioObj = AudioController.Play("Recording_erji");
                }
                else
                {
                    audioObj = AudioController.Play("en_Recording1");
                }
				audioObj.completelyPlayedDelegate = EndErji;
			}

			else if (m_Status == TutorialStatus.StartPlayMusic)
			{
				m_Status = TutorialStatus.Nothing;
                ClickObj co = null;
                if(LanguageManager.languageType == LanguageType.Chinese)
                {
                    co = new ClickObj("Hello", new Vector2(256, 192), "Hello");
                }
                else
                {
                    co = new ClickObj("你好", new Vector2(256, 192), "你好");
                }
              
				obj_hand.SetActive(false);
				m_Tap = CorePlayManager.Instance.m_TapCreator.CreateWord(null, co);
				m_Tap.IsTutorial = true;
				m_Tap.TutNum(1);
				m_TapController = m_Tap.contrller;
				m_Zhuyi.SetActive(true);
                AudioObject audioObj = null;
                if (LanguageManager.languageType == LanguageType.Chinese)
                {
                    audioObj = AudioController.Play("Recording_1");
                }
                else
                {
                    audioObj = AudioController.Play("en_Recording2");
                }
                audioObj.completelyPlayedDelegate = ChangeStatusToPlayBatchKey;
			}
			else if (m_Status == TutorialStatus.PlayBatchKey)
			{
				BPM = 60;
				m_Beat = 60000f / BPM;
				m_Status = TutorialStatus.ClickBatchKey;
				m_PlayStatus = Status.Sound;
				RhyTapHight();
			}
			else if (m_Status == TutorialStatus.RetryPlayBatchKey)
			{
				BPM = 60;
				m_Beat = 60000f / BPM;
				m_Status = TutorialStatus.ClickBatchKey;
				m_PlayStatus = Status.Sound;
				m_Zhuyi.SetActive(true);

                AudioObject audioObj = null;
                if (LanguageManager.languageType == LanguageType.Chinese)
                {
                    audioObj = AudioController.Play("Recording_6");
                }
                else
                {
                    audioObj = AudioController.Play("en_Recording2");
                }
                audioObj.completelyPlayedDelegate = RepetClick;
			}
			else if (m_Status == TutorialStatus.StartSound)
			{
				m_Status = TutorialStatus.Nothing;
                AudioObject audioObj = null;
                if (LanguageManager.languageType == LanguageType.Chinese)
                {
                    audioObj = AudioController.Play("Recording_2");
                }
                else
                {
                    audioObj = AudioController.Play("en_Recording5");
                }
                audioObj.completelyPlayedDelegate = StartPlaySound;
			}
			else if (m_Status == TutorialStatus.PlaySound)
			{
				RepetNum += 1;
				m_Status = TutorialStatus.Nothing;
				FlyWord();
			}
			else if (m_Status == TutorialStatus.RetryPlaySound)
			{
				RepetNum += 1;
				m_Status = TutorialStatus.Nothing;
				List<string> wordList = new List<string>();
              
                if (LanguageManager.languageType == LanguageType.Chinese)
                {
                    wordList.Add("Recording_7");
                }
                else
                {
                    wordList.Add("en_Recording6");
                }

				var audioObj = AudioController.PlayList(wordList);
				audioObj.completelyPlayedDelegate = FlyWord;

			}
			else if (m_Status == TutorialStatus.CheckSound)
			{
				m_Timer += Time.deltaTime * 1000f;
				if (m_Timer >= 5000f)
				{
					if (RepetNum < 2)
					{
						XunFeiSRManager.Instance.StopListen();
						MicManager.Instance.StopRecord();
						m_Status = TutorialStatus.RetryPlaySound;
					}
					else
					{
						m_Status = TutorialStatus.Nothing;
						XunFeiSRManager.Instance.StopListen();
						MicManager.Instance.StopRecord();
                        AudioObject audioObj = null;
                        if (LanguageManager.languageType == LanguageType.Chinese)
                        {
                            audioObj = AudioController.Play("Recording_5");
                        }
                        else
                        {
                            audioObj = AudioController.Play("en_Recording3");
                        }
						AnalysisManager.Instance.OnEvent("100005", null, "新手", "完成新手");
						audioObj.completelyPlayedDelegate = Skip;
					}
				}
			}
		}
	}

	private void EndErji(AudioObject obj)
	{
		m_Erji.SetActive(false);
		m_Status = TutorialStatus.StartPlayMusic;
	}

	#region 听点引导

	private void ChangeStatusToPlayBatchKey(AudioObject obj)
	{
		m_Status = TutorialStatus.PlayBatchKey;
	}


	private void RepetClick(AudioObject obj)
	{
		RhyTapHight();
	}

	private void RhyTapHight()
	{
		m_Zhuyi.SetActive(false);
		TapHight();
	}

	private void TapHight()
	{
		m_PlayStatus = Status.Click;
		m_TutotialScaler.Hight();
		m_TutotialScaler.beat = m_Beat;
	}

	public void ClickSound()
	{
		obj_hand.SetActive(true);
		obj_ClickSound = AudioController.Play("Recording_4");
	}

	public int GetResult(int k)
	{
		if (m_PlayStatus == Status.Sound)
		{
			return -1;
		}
		if (m_Status == TutorialStatus.ClickBatchKey)
		{
			if (HasHand && m_TutotialScaler.Click == false)
			{
				obj_hand.SetActive(false);
				if (obj_ClickSound != null)
					obj_ClickSound.Stop();

				m_TutotialScaler.Click = true;

				RepetNum += 1;

				if (m_TutotialScaler.IsPerfect)
				{
					ClickPefect = true;
					return 0;
				}
				else
				{
					if (RepetNum == 3)
						return 0;
					return 1;
				}
			}
			else
			{
				return -1;
			}
		}
		return -1;
	}

	public void PointerWrong()
	{
		if (m_Status == TutorialStatus.ClickBatchKey && m_PlayStatus == Status.Click)
		{
			if (ClickPefect == false && RepetNum < 3)
			{
				m_PlayStatus = Status.Sound;
				m_TutotialScaler.Restart();
				m_Status = TutorialStatus.RetryPlayBatchKey;
			}
			else
			{
				m_PlayStatus = Status.Sound;
				EndClick();
			}
		}
	}

	private void EndClick()
	{
		AudioObject obj;
		if (ClickPefect)
		{
            if (LanguageManager.languageType == LanguageType.Chinese)
            {
                obj = AudioController.Play("Recording_5");
            }
            else
            {
                obj = AudioController.Play("en_Recording4");
            }
		}
		else
		{
            if (LanguageManager.languageType == LanguageType.Chinese)
            {
                obj = AudioController.Play("Recording_8");
            }
            else
            {
                obj = AudioController.Play("en_Recording4");
            }
		}
		obj.completelyPlayedDelegate = ChangeStatusToStartSound;
	}

	#endregion

	#region 跟读引导
	private void ChangeStatusToStartSound(AudioObject obj)
	{
		RepetNum = 0;
		m_Status = TutorialStatus.StartSound;
		if (m_Tap != null)
		{
			m_Tap.Delete();
		}
	}

	private void StartPlaySound(AudioObject audioObj)
	{
		VoiceController.Show();
		m_Status = TutorialStatus.PlaySound;
	}

	private void FlyWord(AudioObject audio = null)
	{
		m_Timer = 0;
		m_Status = TutorialStatus.CheckSound;

        if(LanguageManager.languageType == LanguageType.Chinese)
        {
            VoiceController.instance.CreateWord("Hello", 5f);
        }
        else
        {
            VoiceController.instance.CreateWord("你好", 5f);
        }

		XunFeiSRManager.Instance.StartListen(GetSRCallback);
		MicManager.Instance.StartRecord(FlyStar, SoundWaveCh);
		VoiceController.Start_RecordingAnim();
	}

	public void GetSRCallback(string content)
	{
        if(LanguageManager.languageType == LanguageType.Chinese)
        {
            MatchVoiceCheckItem("Hello".ToLower(), content.ToLower());
        }
        else
        {
            MatchVoiceCheckItem("你好", content);
        }
	}

	private void MatchVoiceCheckItem(string item, string content)
	{
		if (content == item)
		{
			VoiceController.CreateVoiceStar();
			LogManager.Log("讯飞获得：" , content , ", 匹配单词：" , item);
			m_Status = TutorialStatus.Nothing;
			XunFeiSRManager.Instance.StopListen();
			MicManager.Instance.StopRecord();
            AudioObject obj = null;
            if (LanguageManager.languageType == LanguageType.Chinese)
            {
                obj = AudioController.Play("Recording_3");
            }
            else
            {
                obj = AudioController.Play("en_Recording8");
            }
			AnalysisManager.Instance.OnEvent("100005", null, "新手", "完成新手");
            obj.completelyPlayedDelegate = Skip;

			return;
		}
		else
		{
			LogManager.Log("讯飞获得：" , content , ", 匹配单词：" , item);
			return;
		}
	}

	private void FlyStar()
	{

	}

	public void SoundWaveCh(float value)
	{
		VoiceController.InputVolume = value;
	}

	#endregion

	private void Skip(AudioObject audioObject)
	{
		obj_hand.SetActive(false);
		if (m_Tap != null)
		{
			m_Tap.Delete();
		}
		SelfPlayerData.NewPlayer = false;
		VoiceController.instance.Restart();
		SaveDataUtils.Save<SelfPlayerData>();
		AudioController.StopCategory("Other");
		XunFeiSRManager.Instance.StopListen();
		MicManager.Instance.StopRecord();
		Instance = null;
		Destroy(transform.gameObject);
	}

	public void Reset()
	{
		if (m_Tap != null)
		{
			m_Tap.Delete();
		}
		obj_hand.SetActive(false);
		VoiceController.instance.Restart();
		AudioController.StopCategory("Other");
		XunFeiSRManager.Instance.StopListen();
		MicManager.Instance.StopRecord();
		Instance = null;
		Destroy(transform.gameObject);
	}
}
