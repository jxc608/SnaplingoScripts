using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Snaplingo.SaveData;
using UnityEngine.Events;
using System;
using Snaplingo.UI;
using System.Text;

public class CorePlayBossWar
{
	public static UnityEvent BossAttackFinishEvent = new UnityEvent();
	public static UnityEvent BossFinishEvent = new UnityEvent();
	public enum Status { Idle, ClickWrongInterval, ClickRightInterval, FirstTimeCG, BossAppearCG, Counting, ReadContent, BossReadyToAttack, SentenceCalcAnimation, FinishingAnimation, Finish, Pause }
	private Status m_Status;
	private CorePlayInputCheck m_InputCheck;
	public CorePlayInputCheck InputCheck
	{
		get { return m_InputCheck; }
	}
	private CorePlayBossTapCreator m_TapCreator;
	private BossWarVoiceCheck m_VoiceCheck;
	private BossWarNode m_BossWarNode;
	private BossLife m_BossLife;
	private float m_Timer;
	private float m_CurrentSentenceLength;
	private int m_CurrentSentenceIndex;
	private int m_RoundNumber;
	private float m_MoreTimeParam = 1;
	public CorePlayBossWar(CorePlayInputCheck inputCheck)
	{
		m_Timer = 0;
		m_CurrentSentenceIndex = 0;
		m_TapCreator = new CorePlayBossTapCreator();
		m_VoiceCheck = new BossWarVoiceCheck(this);
		m_BossLife = new BossLife(CorePlayData.BossLife);

		CorePlaySceneManager.bossEnterFinishEvent.AddListener(CGFinishCallback);
		SetEventListener(BossAttackFinishEvent, BossAttackFinishCallback);
		SetEventListener(BossFinishEvent, BossFinishCallback);
		m_InputCheck = inputCheck;
		m_Status = Status.Idle;

	}

	void SetEventListener(UnityEvent theEvent, UnityAction action)
	{
		theEvent.RemoveAllListeners();
		theEvent.AddListener(action);
	}

	public void StartLogic()
	{
		//AudioManager.Instance.StopMusic();
		AudioController.StopAll();
		AudioController.SetGlobalVolume(1);

		m_RoundNumber = 1;
		m_BossWarNode = PageManager.Instance.CurrentPage.GetNode<BossWarNode>();
		m_BossWarNode.Open();
		m_BossWarNode.BossHeartInitAnimation(CorePlayData.BossLife);
		m_InputCheck.SetClickCallback(ClickRightCallback, ClickWrongCallback);
		CorePlayInputCheck.SentenceAllRightEvent.RemoveListener(SentenceCompleteCallback);
		CorePlayInputCheck.SentenceAllRightEvent.AddListener(SentenceCompleteCallback);
		int delayTime = CorePlayData.CurrentSong.BossWarMusicDelay;
		if (delayTime < 0)
		{
			//AudioManager.Instance.PlaySong(CorePlayData.BossSongName);
			AudioController.PlayMusic("Song");
			StaticMonoBehaviour.Instance.StartCoroutine(DelayPlayAppearCG(-delayTime / 1000f));
		}
		else
		{
			BossAppearCG();
			StaticMonoBehaviour.Instance.StartCoroutine(DelayPlayMusic(CorePlayData.CurrentSong.BossWarMusicDelay / 1000f));
		}
		CalcMoreTime();
	}

	void CalcMoreTime()
	{
		if (StaticData.Difficulty == 1)
		{
			m_MoreTimeParam = 2f;
			//LogManager.Log("get more time");
		}
		else
		{
			m_MoreTimeParam = 1f;
		}
	}


	IEnumerator DelayPlayAppearCG(float time)
	{
		//LogManager.Log(" 1 = DelayPlayAppearCG " , time);
		yield return new WaitForSeconds(time);
		BossAppearCG();
	}

	IEnumerator DelayPlayMusic(float time)
	{
		yield return new WaitForSeconds(time);
		//AudioManager.Instance.PlaySong(CorePlayData.BossSongName)
		PlaySong();
	}

	void PlaySong()
	{
		if (!AudioController.IsPlaying("Song"))
		{
			AudioObject ao = AudioController.PlayMusic("Song");
			ao.audioItem.Loop = AudioItem.LoopMode.LoopSequence;
		}
	}

	void FirstTimeCG()
	{
		m_Status = Status.FirstTimeCG;
	}

	void BossAppearCG()
	{
		//LogManager.Log(" 2 = BossAppearCG");
		m_Status = Status.BossAppearCG;
		CorePlaySceneManager.bossEnterEvent.Invoke();
	}

	void CGFinishCallback()
	{
		if (m_Status == Status.FirstTimeCG || m_Status == Status.BossAppearCG)
		{
			if (!SelfPlayerData.FirstTimeBossCG)
			{
				SelfPlayerData.FirstTimeBossCG = true;
				SaveDataUtils.Save<SelfPlayerData>();
			}
			m_SentenceCheckResult = RuntimeConst.BossWarLose;
			PlaySong();
			StartCoungting();
			//StartReadCurSentence();
		}
	}

	void StartCoungting()
	{
		if (m_CurrentSentenceIndex >= CorePlayData.CurrentSong.StreamSentences.Count)
		{
			if (m_BossLife.BossLose())
			{
				BossLoseCG();
			}
			else
			{
				BossWinCG();
			}
			return;
		}

		m_Status = Status.Counting;
		PageManager.Instance.CurrentPage.GetNode<BossWarNode>().ShowRound(m_RoundNumber, StartReadCurSentence);
		CorePlaySceneManager.bossPrepareAttackEvent.Invoke();
	}


	public void IgnoreBossCG()
	{
		StartReadCurSentence();
	}


	private StreamSentenceGroup m_CurGroup;
	private StreamSentence m_CurrentBossSentence;
	private int m_SentenceCheckResult;
	private int m_WrongNumber;
	List<string> m_AudioRepeatList = new List<string>();
	void StartReadCurSentence()
	{
		m_Status = Status.ReadContent;
		m_WrongNumber = 0;
		m_CurGroup = CorePlayData.CurrentSong.StreamSentences[m_CurrentSentenceIndex];
		if (m_CurrentSentenceIndex > 0 && CorePlayData.CurrentSong.StreamSentences[m_CurrentSentenceIndex - 1].CalckSentence)
		{
			m_SentenceCheckResult = RuntimeConst.BossWarLose;
		}
		m_CurrentBossSentence = m_CurGroup.GetOneSentence();
		m_CurrentSentenceLength = m_CurGroup.TimeLength * 0.001f * m_MoreTimeParam;
		switch (m_CurGroup.Type)

		{
			case SentenceType.Common:
				CreateSentence();
				//m_BossWarNode.ShowTip();
				//m_TapCreator.ShowWord();
				m_TapCreator.SetClickable();
				PlayAudio();
				break;
			case SentenceType.Voice:
				if (m_CurrentSentenceIndex == 0 ||
					(m_CurrentSentenceIndex > 0 && CorePlayData.CurrentSong.StreamSentences[m_CurrentSentenceIndex - 1].Type == SentenceType.Common))
				{
					//CorePlayManager.Instance.VoiceUIFadeIn(CreateVoice);
					VoiceController.Show(CreateVoice);
				}
				else
				{
					CreateVoice();
				}
				ReadContentFinish(null);
				StaticMonoBehaviour.Instance.StartCoroutine(DelayCheckMissleCreate(m_CurrentSentenceLength - 1));
				break;
		}
	}

	IEnumerator DelayCheckMissleCreate(float duration)
	{
		yield return new WaitForSeconds(duration);
		m_VoiceCheck.CheckCreateMissle();
	}

	void PlayAudio()
	{
		var obj = AudioController.GetCategory("AudioWords").Play(m_CurrentBossSentence.FileName);
		if (obj == null)
			LogManager.LogError("   没有句子录音 : " , m_CurrentBossSentence.FileName);
		else
			obj.completelyPlayedDelegate = ReadContentFinish;

		m_AudioRepeatList.Add(m_CurrentBossSentence.FileName);
	}

	void CreateSentence()
	{
		List<string> sentence = new List<string>();
		List<string> showWords = new List<string>();
		StringBuilder sb = new StringBuilder();
		for (int i = 0; i < m_CurrentBossSentence.Sentence.Count; i++)
		{
			sentence.Add(m_CurrentBossSentence.Sentence[i].Key);
			showWords.Add(m_CurrentBossSentence.Sentence[i].Value);
			sb.Append(m_CurrentBossSentence.Sentence[i].Value + " ");
		}
		m_InputCheck.CreateCheckSentence(sentence, m_CurrentSentenceIndex + BossSentenceOffset, m_CurGroup.TimeLength);
		m_BossWarNode.SetTipContent(sb.ToString());
		m_TapCreator.CreateTap(sentence, showWords, m_CurGroup.TimeLength * 0.001f, m_CurrentSentenceIndex + BossSentenceOffset);
	}

	void CreateVoice()
	{
		List<string> sentence = new List<string>();
		StringBuilder sb = new StringBuilder();
		for (int i = 0; i < m_CurrentBossSentence.Sentence.Count; i++)
		{
			sentence.Add(m_CurrentBossSentence.Sentence[i].Key);
			sb.Append(m_CurrentBossSentence.Sentence[i].Value + " ");
		}
		m_VoiceCheck.CreateVoiceSentence(sentence);
		CorePlayManager.Instance.CreateVoiceString(sb.ToString(), m_CurGroup.TimeLength * 0.001f);
	}

	void ReadContentFinish(AudioObject audioObj)
	{
		if (m_Status == Status.ReadContent)
		{
			//switch (m_Group.Type)
			//{
			//    case BossSentenceGroup.BossSentenceType.Common:
			//        StaticMonoBehaviour.Instance.StartCoroutine(DelayAttack(m_CurrentSentenceLength - AttackAnimationDuration));
			//        m_TapCreator.ShowWord();
			//        m_TapCreator.SetClickable();
			//        break;
			//}
			m_Status = Status.BossReadyToAttack;
			m_Timer = 0;
		}
	}

	private const float AttackAnimationDuration = 0.5f;
	IEnumerator DelayAttack(float duration)
	{
		yield return new WaitForSeconds(duration);
	}

	public void Update()
	{
		switch (m_Status)
		{
			case Status.BossAppearCG:
			case Status.FirstTimeCG:
			case Status.ReadContent:
			case Status.SentenceCalcAnimation:
				break;
			case Status.BossReadyToAttack:
				BossAttacking();
				break;
		}
	}

	void BossAttacking()
	{
		CalcTimer();
	}

	void CalcTimer()
	{
		m_Timer += Time.deltaTime;
		if (m_Timer > m_CurrentSentenceLength)
		{
			m_Timer = 0;
			CalcCurSentence();
		}
	}

	void SentenceCompleteCallback()
	{
		m_Status = Status.ClickRightInterval;
		StaticMonoBehaviour.Instance.StartCoroutine(SentenceCompleteDelay());
	}

	IEnumerator SentenceCompleteDelay()
	{
		yield return new WaitForSeconds(RhythmController.RightAnimDelay);
		//LogManager.Log("Go = SentenceCompleteCallback");
		CorePlaySceneManager.bossSentenceCompleteEvent.Invoke();
		if (m_Status == Status.ClickRightInterval)
		{
			m_Timer = 0;
			m_Status = Status.SentenceCalcAnimation;
			m_TapCreator.Delete();
			m_SentenceCheckResult = RuntimeConst.BossWarWin;
			CheckGroupType();
		}
	}

	public void CalcCurSentence()
	{
		m_Status = Status.SentenceCalcAnimation;
		bool correct = false;
		switch (m_CurGroup.Type)
		{
			case SentenceType.Common:
				m_TapCreator.Delete();
				correct = m_InputCheck.CalcSentence(m_CurrentSentenceIndex + BossSentenceOffset, true);
				if (!correct)
				{
					BossBeatPlayer();
					return;
				}
				else
				{
					m_SentenceCheckResult = RuntimeConst.BossWarWin;
				}
				break;

			case SentenceType.Voice:
				m_SentenceCheckResult = m_VoiceCheck.CalculateResult();
				m_VoiceCheck.Close();
				m_VoiceCheck.StopListen();
				if (m_CurrentSentenceIndex < CorePlayData.CurrentSong.StreamSentences.Count - 1)
				{
					if (CorePlayData.CurrentSong.StreamSentences[m_CurrentSentenceIndex + 1].Type != SentenceType.Voice)
					{
						CorePlayManager.Instance.VoiceUIFadeOut();
					}
				}
				else
				{
					CorePlayManager.Instance.VoiceUIFadeOut();
				}
				RepeatVoice();

				if (m_SentenceCheckResult == RuntimeConst.BossWarLose)
				{
					BossBeatPlayer();
					return;
				}
				break;
		}

		CheckGroupType();
	}

	private void BossBeatPlayer()
	{
		m_SentenceCheckResult = RuntimeConst.BossWarLose;
		BossBeatPlayerCG();
		while (m_CurrentSentenceIndex < CorePlayData.CurrentSong.StreamSentences.Count &&
			   !CorePlayData.CurrentSong.StreamSentences[m_CurrentSentenceIndex].CalckSentence)
		{
			m_CurrentSentenceIndex++;
		}
		m_AudioRepeatList.Clear();
	}

	private bool m_WaitingForRepeatFinish;
	private float m_WatingTimer;
	private const float WaitTimeLength = 6;
	void RepeatVoice()
	{
		//LogManager.Log("Start play voice repeat!");
		CorePlaySceneManager.voiceRepeatEvent.Invoke(true);

		m_WaitingForRepeatFinish = true;
		CorePlayManager.Instance.ShowHorn();
		AudioController.GetCurrentMusic().FadeOut(1.5f, 0, 0.5f, false);

		//LogManager.LogWarning("PlayWordList = ");
		//foreach (var item in m_AudioRepeatList) { LogManager.Log(item); }

		if (m_AudioRepeatList.Count > 0)
		{
			// 暂时不支持拼字
			var ac = AudioController.GetCategory("AudioWords");
			var obj = ac.Play(m_AudioRepeatList[0]);
			obj.completelyPlayedDelegate = (v) =>
			{
				//LogManager.Log("Play word list callback");
				MicManager.Instance.Play(false, RepeatVoiceFinish);
			};
		}
		else
		{
			MicManager.Instance.Play(false, RepeatVoiceFinish);
		}
	}

	void RepeatVoiceFinish()
	{
		//LogManager.Log("end play voice repeat!");
		CorePlaySceneManager.voiceRepeatEvent.Invoke(false);

		m_WaitingForRepeatFinish = false;
		var audio = AudioController.GetCurrentMusic();
		if (audio != null) audio.FadeIn(1.5f);

		m_AudioRepeatList.Clear();
		CorePlayManager.Instance.CloseHorn();
	}

	void CheckGroupType()
	{
		if (m_CurGroup.CalckSentence)
		{
			switch (m_SentenceCheckResult)
			{
				case RuntimeConst.BossWarWin:
					PlayerBeatBossCG();
					break;
				case RuntimeConst.BossWarDraw:
					CorePlaySceneManager.bossDrawEvent.Invoke();
					break;
			}
		}
		else
		{
			BossAttackFinishCallback();
		}
	}

	void BossBeatPlayerCG()
	{
		CorePlaySceneManager.bossAttackEvent.Invoke();
	}

	void PlayerBeatBossCG()
	{
		m_BossLife.BossDamage();
		StaticMonoBehaviour.Instance.StartCoroutine(DelayScreenShake());
		CorePlaySceneManager.bossDamageEvent.Invoke();
	}

	IEnumerator DelayScreenShake()
	{
		yield return new WaitForSeconds(1.6f);
		ScreenShake.Instance.Shake();
	}

	void BossAttackFinishCallback()
	{
		if (m_Status == Status.SentenceCalcAnimation)
		{
			if (m_CurGroup.CalckSentence)
				m_AudioRepeatList.Clear();


			if (m_CurGroup.Type == SentenceType.Voice)
			{
				m_WatingTimer = 0;
				StaticMonoBehaviour.Instance.StartCoroutine(WaitingForRepeatVoiceFinish());
			}
			else
			{
				BossAttackFinishProcess();
			}
		}
	}

	void BossAttackFinishProcess()
	{
		if (CorePlayManager.Instance.Life == 0 && CorePlaySettings.Instance.m_LifeDebug)
		{
			m_Status = Status.Idle;
			CorePlayManager.Instance.BossWarPlayerFail();
		}
		else
		{
			StreamSentenceGroup group = CorePlayData.CurrentSong.StreamSentences[m_CurrentSentenceIndex];
			m_CurrentSentenceIndex++;
			if (group.CalckSentence)
			{
				m_RoundNumber++;
				StartCoungting();
			}
			else
			{
				StartReadCurSentence();
			}
		}
	}

	IEnumerator WaitingForRepeatVoiceFinish()
	{
		while (m_WaitingForRepeatFinish)
		{
			m_WatingTimer += Time.deltaTime;
			if (m_WatingTimer >= WaitTimeLength)
			{
				//LogManager.Log("time out end play voice repeat!!");
				CorePlaySceneManager.voiceRepeatEvent.Invoke(false);
				break;
			}

			yield return null;
		}
		BossAttackFinishProcess();
	}

	void BossWinCG()
	{
		if (m_Status != Status.FinishingAnimation)
		{
			m_Status = Status.FinishingAnimation;
			//CorePlayTempHandler.bossFinishEvent.Invoke(true);
			CorePlaySceneManager.bossPrepareAttackEvent.Invoke();
			StaticMonoBehaviour.Instance.StartCoroutine(GameDelayFiinish(Duration));
		}
	}

	void BossLoseCG()
	{
		if (m_Status != Status.FinishingAnimation)
		{
			m_Status = Status.FinishingAnimation;
			//Time.timeScale = 0.1f;
			//StaticMonoBehaviour.Instance.StartCoroutine(DelayRecoverTimeScale());
			CorePlaySceneManager.bossFinishEvent.Invoke(false);
		}
	}

	//IEnumerator DelayRecoverTimeScale ()
	//{
	//    yield return new WaitForSeconds(2f);
	//    Time.timeScale = 1;
	//}

	const float Duration = 2f;
	void BossFinishCallback()
	{
		StaticMonoBehaviour.Instance.StartCoroutine(GameDelayFiinish(Duration));
	}

	IEnumerator GameDelayFiinish(float duration)
	{
		yield return new WaitForSeconds(duration);
		m_Status = Status.Finish;
		CorePlayManager.Instance.GameFinish();
	}

	public void Restart()
	{
		m_CurrentSentenceIndex = 0;
		m_RoundNumber = 1;
		m_WrongNumber = 0;
		m_TapCreator.Restart();
		m_BossLife.Restart();
		m_VoiceCheck.Restart();
		m_BossWarNode.Close();
		m_Status = Status.Idle;
		AudioController.SetGlobalVolume(1);
		CorePlayInputCheck.SentenceAllRightEvent.RemoveListener(SentenceCompleteCallback);
	}

	private const float ScoreParam = 2.4f;
	private const int BossSentenceOffset = 1000;
	public void ClickRightCallback(int sentenceIndex, int checkIndex, float accuracy)
	{
		if (sentenceIndex - BossSentenceOffset == m_CurrentSentenceIndex)
		{
			float score = CorePlaySettings.Instance.m_ComboPoint + CorePlayManager.Combo * ScoreParam * accuracy;
			float difficultyAddOn = 1f;
			if (StaticData.Difficulty == CorePlaySettings.Instance.m_Normal)
			{
				difficultyAddOn = 0.8f;
			}
			score *= difficultyAddOn;
			score *= CorePlayManager.Instance.ScoreParam;
			CorePlayManager.Instance.UpdateComboAndScore((int)score, 1);
		}
	}

	public void ClickWrongCallback(int sentenceIndex, int checkIndex)
	{
		if (sentenceIndex - BossSentenceOffset != m_CurrentSentenceIndex)
			return;

		if (m_Status == Status.ReadContent || m_Status == Status.BossReadyToAttack)
		{
			AudioController.StopCategory(CorePlayData.BossSongName);
			// 间隔  
			m_Status = Status.ClickWrongInterval;
			StaticMonoBehaviour.Instance.StartCoroutine(CorWrongWordDelay());
		}
	}
	IEnumerator CorWrongWordDelay()
	{
		CorePlaySceneManager.bossWordClickWrongEvent.Invoke();
		yield return new WaitForSeconds(RhythmController.BossWordWrongAnimDelay);
		if (m_WrongNumber < CorePlaySettings.Instance.m_BossWarWrongChances)
		{
			m_WrongNumber++;
			RefreshCurrentSentence();
		}
		else
		{
			CalcCurSentence();
		}
		CorePlaySceneManager.bossWordRe_EnterEvent.Invoke();
	}

	void RefreshCurrentSentence()
	{
		m_InputCheck.RefreshSentenceCheck(m_CurrentSentenceIndex + BossSentenceOffset);
		m_TapCreator.ResetButtonPos();
		m_Status = Status.ReadContent;
		PlayAudio();
		m_Timer = 0;
	}

	private Status m_LastStatus;
	public void Pause()
	{
		m_LastStatus = m_Status;
		m_Status = Status.Pause;
	}

	public void Recover()
	{
		//AudioManager.Instance.BGMSource.volume = 1;
		m_Status = m_LastStatus;
	}

	//private int m_RandomSeed = 0;
	//public void Test()
	//{
	//    m_TapCreator.Delete();
	//    m_RandomSeed++;
	//    UnityEngine.Random.InitState(m_RandomSeed);
	//    int index = UnityEngine.Random.Range(0, CorePlayData.CurrentSong.BossSentences.Count);
	//    BossSentenceGroup group = CorePlayData.CurrentSong.BossSentences[index];
	//    BossSentence bossSentence = group.GetOneSentence();
	//    List<string> sentence = new List<string>();
	//    List<string> showWords = new List<string>();
	//    StringBuilder sb = new StringBuilder();
	//    for (int i = 0; i < bossSentence.Sentence.Count; i++)
	//    {
	//        sentence.Add(bossSentence.Sentence[i].Key);
	//        showWords.Add(bossSentence.Sentence[i].Value);
	//        sb.Append(bossSentence.Sentence[i].Value + " ");
	//    }
	//    m_TapCreator.CreateTap(sentence, showWords, group.TimeLength * 0.001f);
	//    m_TapCreator.ShowWord();
	//}
}
