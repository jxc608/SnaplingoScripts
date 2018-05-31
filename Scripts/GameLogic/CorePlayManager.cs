using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Snaplingo.UI;
using System;
using TMPro;
using Snaplingo.SaveData;
using DG.Tweening;
using LitJson;

public class CorePlayTimerEvent
{
	public enum EventType
	{
		StartCount,
		StartPlayMusic,
		CreateTapSentence,
		CreateVoiceSentence,
		StartVoiceUI,
		EndVoiceUI,
		StartTapChecker,
		StartVoiceChecker,
		EndVoiceChecker,
		HighLightTap,
		HighLightVoice,
		StartBoss,
		StartKeyVoice,
	}

	public int m_Timer;
	public int m_SentenceIndex;
	public int m_HOIndex;
	public EventType m_Type;
	public float m_HighLightTime;
	public CorePlayTimerEvent (int timer, EventType type, int sentenceIndex, int hoIndex, float highLightTime = 0)
	{
		m_Timer = timer;
		m_Type = type;
		m_SentenceIndex = sentenceIndex;
		m_HOIndex = hoIndex;
		m_HighLightTime = highLightTime;
	}
}

public class CorePlayManager : MonoBehaviour
{
    #region [ --- Object References --- ]
    [Header(" --- Object References ---")]
    public TextMeshPro text_bigCombo;
    public GameObject m_Horn;
    StageManager stageManager;
    #endregion

    [Header(" --- Settings ---")]

    private CorePlayInputCheck m_InputCheck;
    public RhythmTapCreator m_TapCreator;
    private RhythmVoiceCreator m_VoiceCreator;
    private CorePlayBossWar m_BossWar;
    private KeyAndVoiceLogic m_KVLogic;
    public static CorePlayManager Instance;
    private CorePlayAutoPlay m_AutoPlay;

    private int m_Score;
    //public static int Combo { get; private set; }
    // 测试用
    public static int Combo { get; set; }

    public int m_MaxCombo;
    public int m_VoiceNum;
    public float m_SumVoiceScore;
    public int m_AllWordNum;
    public string m_ProcessData;

    public enum GameStatus { Idle, StartTutorial, InTutorial, GameStart, BossWar, KeyAndVoice, Pause, Finish }
    private GameStatus m_Status = GameStatus.GameStart;

    private enum IntradaStatus { WaitForPlaying, Playing, Played }
    private IntradaStatus m_IntradaStatus = IntradaStatus.WaitForPlaying;

    private int m_CorrectNum = 0;
    private int m_WrongNum = 0;
    private float m_Timer;
    private float m_SFXTimer;

    public float Timer
    {
        get { return m_Timer; }
    }
    private int m_Life;
    public int Life
    {
        get { return m_Life; }
    }
    private float m_CurrentTB;
    public float CurrentTB
    {
        get { return m_CurrentTB; }
    }

    private Queue<CorePlayTimerEvent> m_TimerEvents = new Queue<CorePlayTimerEvent>();
    private List<CorePlayTimerEvent> m_TimerEventsCopy = new List<CorePlayTimerEvent>();
    private float m_RhythmBeat;
    private float m_ScoreParam;
    public float ScoreParam
    {//don需要每一关的分数尽量递增，即相同的水平（准确率），越往后面的关卡，得分越高
        get { return m_ScoreParam; }
    }
    private bool m_StartAutoPlay;
    private bool m_EditMode = false;
    public bool EDITMODE
    {
        get { return m_EditMode; }
    }
    public float RhythmBeat
    {
        get
        {
            if (CorePlaySettings.Instance.m_GameHeartBeat)
            {
                return m_RhythmBeat;
            }
            else
            {
                return 0;
            }
        }
    }
    private float m_RhythmBeatInterval;
    public float RhythmBeatInterval
    {
        get
        {
            return m_RhythmBeatInterval;
        }
    }
    private SongMaxScoreInfo m_SongMaxScoreInfo;
    // Use this for initialization
    void Start()
    {
        m_AllWordNum = 0;
        if (Instance != null)
        {
            Destroy(Instance);
            LogManager.Log("Clear CorePlayManager");
        }
        Instance = this;
        m_Status = GameStatus.Idle;
        m_Horn.SetActive(false);
        m_InputCheck = new CorePlayInputCheck(this);
        m_AutoPlay = Instantiate(ResourceLoadUtils.Load<GameObject>("CorePlay/AutoPointer")).GetComponent<CorePlayAutoPlay>();
        m_AutoPlay.Close();
        CorePlayInputCheck.SentenceAllRightEvent.RemoveAllListeners();
        m_TapCreator = new RhythmTapCreator();
        m_TapCreator.SetManager(this);
        m_VoiceCreator = new RhythmVoiceCreator();
		PageManager.Instance.CurrentPage.GetNode<CalculateNode>().ShowMainSliderAndPauseButton();
        Reset();
        if (!m_EditMode)
        {
            CalcCorePlayData();
            StartLogic();
        }


    }

    void CalcCorePlayData()
    {
        CorePlayData.BMPDelta = CorePlayData.CurrentSong.BPM / 60f / 2f;
        m_CurrentTB = 60f / CorePlayData.CurrentSong.BPM;
        while (m_CurrentTB < 0.5f || m_CurrentTB > 1)
        {
            if (m_CurrentTB < 0.5f)
                m_CurrentTB *= 2f;
            else if (m_CurrentTB > 1)
                m_CurrentTB /= 2f;
        }
        CorePlayData.TB = m_CurrentTB;

        m_RhythmBeatInterval = 60000f / CorePlayData.CurrentSong.BPM;
        Debug.LogWarning("m_RhythmBeatInterval coreplay: " + m_RhythmBeatInterval);

        //计算分数
        int levelNum = StaticData.LevelID - RuntimeConst.LevelBase;
        m_SongMaxScoreInfo = BeatmapParse.GetSongObjectMaxScore(CorePlayData.CurrentSong);
        int currentMaxScore = m_SongMaxScoreInfo.MaxScore;
        m_AllWordNum = m_SongMaxScoreInfo.MaxCombo;
        float destScore = (Mathf.Log10(levelNum) + RuntimeConst.ScoreAddOnParam) * CorePlayData.FirstLevelMaxScore;
        m_ScoreParam = destScore / currentMaxScore;
        PageManager.Instance.CurrentPage.GetNode<CalculateNode>().levelMaxScore = currentMaxScore;
        PageManager.Instance.CurrentPage.GetNode<CalculateNode>().scoreParam = m_ScoreParam;
        PageManager.Instance.CurrentPage.GetNode<BossWarNode>().scoreParam = m_ScoreParam;
        //Debug.Log(m_ScoreParam);
    }

    void StartLogic()
    {
        CreateTimerEvents();
        m_InputCheck.SetClickCallback(ClickRight, ClickWrong);
        if (StaticData.LevelID == RuntimeConst.FirstLevelID && (SelfPlayerData.NewPlayer || CorePlaySettings.Instance.m_TutorialJump == false))
            m_Status = GameStatus.StartTutorial;
        else
        {
            AnalysisManager.Instance.OnEvent("100002", null, StaticData.LevelID.ToString(), "进入游戏人次");
            AnalysisManager.Instance.OnLevelBegin(StaticData.LevelID.ToString());
            m_Status = GameStatus.GameStart;
        }
    }

    public void SetEditMode()
    {
        m_EditMode = true;
    }

    private void SetComboTextActive(bool active, string text = null)
    {
        if (!m_EditMode)
        {
            if (text != null)
                text_bigCombo.text = text;
            text_bigCombo.gameObject.SetActive(active);
        }
    }
    public void ShowHorn()
    {
        m_Horn.SetActive(true);
    }

    public void CloseHorn()
    {
        m_Horn.SetActive(false);
    }

    public void Retry()
    {
        if (Tutorial != null)
        {
            Tutorial.Reset();
        }
        Reset();
        StartLogic();
    }

    private List<int> m_AllHOPoint = new List<int>();
    void CreateTimerEvents()
    {
        CorePlayCreateEvents eventsCreator = new CorePlayCreateEvents();
        m_TimerEvents = eventsCreator.GetTimerEventQueue();
        m_TimerEventsCopy = new List<CorePlayTimerEvent>(m_TimerEvents.ToArray());

        m_AllHOPoint.Clear();
        for (int i = 0; i < m_TimerEventsCopy.Count; i++)
        {
            if (m_TimerEventsCopy[i].m_Type == CorePlayTimerEvent.EventType.HighLightTap)
            {
                m_AllHOPoint.Add(m_TimerEventsCopy[i].m_Timer - CorePlaySettings.Instance.m_PreShowTimeLength);
            }
        }
    }

    #region TimerEventCallbacks
    private void ProcessEvents(CorePlayTimerEvent te)
    {
        switch (te.m_Type)
        {
            case CorePlayTimerEvent.EventType.CreateTapSentence:
                //Debug.Log("CreateTapSentence = " + te.m_Timer + " / time = " + Time.time);
                m_TapCreator.CreateWords(CorePlayData.CurrentSong.SentenceObjs[te.m_SentenceIndex], te.m_SentenceIndex);
                break;
            case CorePlayTimerEvent.EventType.CreateVoiceSentence:
                m_VoiceCreator.CreateWords(CorePlayData.CurrentSong.SentenceObjs[te.m_SentenceIndex], te.m_SentenceIndex);
                //Debug.Log(" 0 = 普通模式 开始录音" + "  / " + Time.frameCount);
                StartMicRecord();
                break;
            case CorePlayTimerEvent.EventType.EndVoiceChecker:
                Debug.Log(" 1 = 普通模式 结束录音");
                StopMicRecord();
                break;
            case CorePlayTimerEvent.EventType.EndVoiceUI:
                m_VoiceCreator.BGFadeOut();
                if (m_StartAutoPlay)
                    m_AutoPlay.Show();
                break;
            case CorePlayTimerEvent.EventType.HighLightTap:
                string word = CorePlayData.CurrentSong.SentenceObjs[te.m_SentenceIndex].ClickAndHOList.HitObjects[te.m_HOIndex].Word.ToLower();
                m_TapCreator.HighLightWord(CorePlayData.CurrentSong.SentenceObjs[te.m_SentenceIndex].ClickAndHOList.HitObjects, te.m_HOIndex, te.m_HighLightTime);
                m_InputCheck.SetCurHOIndex(te.m_SentenceIndex, te.m_HOIndex);
                if (m_StartAutoPlay)
                {
                    ClickObj co = CorePlayData.CurrentSong.SentenceObjs[te.m_SentenceIndex].ClickAndHOList.ClickObjs[te.m_HOIndex];
                    m_AutoPlay.Simulate(CorePlaySettings.Instance.m_PreShowTimeLength * RuntimeConst.MSToSecond, co.m_Position);
                }
                StartCoroutine(HOPointTimeLogic(CorePlayData.CurrentSong.SentenceObjs[te.m_SentenceIndex].ClickAndHOList.HitObjects[te.m_HOIndex]));
                break;
            case CorePlayTimerEvent.EventType.HighLightVoice:
                m_VoiceCreator.HighLightWord(CorePlayData.CurrentSong.SentenceObjs[te.m_SentenceIndex].ClickAndHOList.HitObjects, te.m_HOIndex);
                break;
            case CorePlayTimerEvent.EventType.StartCount:
                CorePlayIntrada intrada = new CorePlayIntrada(CorePlayData.BMPDelta, () => { m_IntradaStatus = IntradaStatus.Played; });
                intrada.Show();
                m_IntradaStatus = IntradaStatus.Playing;
                break;
            case CorePlayTimerEvent.EventType.StartPlayMusic:
                StartCurrentSong();
                break;
            case CorePlayTimerEvent.EventType.StartTapChecker:
                // Temp Event
                CorePlaySceneManager.checkSentenceEvent.Invoke();
                int sentenceLength = CorePlayData.CurrentSong.SentenceObjs[te.m_SentenceIndex].m_InOutTime.EndTime -
                                     CorePlayData.CurrentSong.SentenceObjs[te.m_SentenceIndex].m_InOutTime.StartTime;
                int timeLength = sentenceLength +
                                 CorePlaySettings.Instance.m_PreShowTimeLength;
                m_InputCheck.CreateCheckSentenceWithMS(CorePlayData.CurrentSong.SentenceObjs[te.m_SentenceIndex].ClickAndHOList.HitObjects, te.m_SentenceIndex, sentenceLength);
                float duration = timeLength * 0.001f;
                StartCoroutine(SentenceCheck(te.m_SentenceIndex, duration));
                break;
            case CorePlayTimerEvent.EventType.StartVoiceChecker:
                CreateVoiceCheck(te.m_SentenceIndex);
                break;
            case CorePlayTimerEvent.EventType.StartVoiceUI:
                m_VoiceCreator.BGFadeIn();
                if (m_StartAutoPlay)
                    m_AutoPlay.Close();
                break;
            case CorePlayTimerEvent.EventType.StartBoss:
                StartBossWarLogic();
                break;
            case CorePlayTimerEvent.EventType.StartKeyVoice:
                StartKeyAndVoice();
                break;
        }
    }
    #endregion

    IEnumerator DelayPlayAudioWord(string word)
    {
        yield return new WaitForSeconds(CorePlaySettings.Instance.m_PreShowTimeLength / 1000f);
        if (word.Contains(" "))
        {
            //string[] array = word.Split(' ');
            //List<string> list = new List<string>(array);
            //AudioController.PlayList(list, CorePlayData.SongID.ToString());
            string content = word.Replace(" ", "_");
            AudioController.GetCategory("AudioWords").Play(content);
        }
        else
        {
            AudioController.GetCategory("AudioWords").Play(word);
        }
    }

    IEnumerator HOPointTimeLogic(HitObject ho)
    {
        yield return new WaitForSeconds(CorePlaySettings.Instance.m_PreShowTimeLength / 1000f);
        if (!ho.SonudFile.Equals(""))
        {
            AudioController.GetCategory("AudioWords").Play(ho.SonudFile);
        }
        RefreshSFXTimer();
    }

    private void RefreshSFXTimer()
    {
        m_SFXTimer = 0;
    }

    public void UIRestart()
    {
        if (PageManager.Instance.CurrentPage == null) return;

        if (CorePlayData.BossLife > 0)
        {
            PageManager.Instance.CurrentPage.GetNode<CalculateNode>().Close();
            PageManager.Instance.CurrentPage.GetNode<BossWarNode>().Restart();
        }
        else
        {
            PageManager.Instance.CurrentPage.GetNode<CalculateNode>().Restart();
            PageManager.Instance.CurrentPage.GetNode<CalculateNode>().gameObject.SetActive(true);
            PageManager.Instance.CurrentPage.GetNode<BossWarNode>().Close();
        }
    }

    public void AudioAndMicRestart()
    {
        AudioController.StopAll();

        XunFeiSRManager.Instance.StopListen();
        MicManager.Instance.StopRecord();
    }

    public void DataRestart()
    {
        m_Life = RuntimeConst.InitLife;
        CorePlayData.PlayerReadingData.Clear();
        ChangeLife(0, true);
        m_Score = 0;
        Combo = 0;
        m_CorrectNum = 0;
        m_WrongNum = 0;
        m_ContinuoslyWrongTime = 0;
        m_ContinuoslyRightTime = 0;
        m_MaxCombo = 0;
        m_VoiceNum = 0;
        m_SumVoiceScore = 0;
    }

    public void Reset()
    {
        m_Timer = 0;
        m_StartAutoPlay = false;
        m_IntradaStatus = IntradaStatus.WaitForPlaying;
        UIRestart();
        AudioAndMicRestart();
        DataRestart();
        m_InputCheck.Restart();
        m_TapCreator.Restart();
        m_VoiceCreator.Restart();
        if (m_BossWar != null)
            m_BossWar.Restart();
        if (m_KVLogic != null)
            m_KVLogic.Restart();
        ClearIntrada();
        StaticMonoBehaviour.Instance.StopAllCoroutines();
        SceneController.instance.Reset();
        MicManager.Instance.Reset();
        SetComboTextActive(false);
		if (LoginScene.isFirstGoingGame)
        {
            PageManager.Instance.CurrentPage.GetComponent<CorePlayPage>().HidePauseButton();
        }
        else
        {
            PageManager.Instance.CurrentPage.GetComponent<CorePlayPage>().ShowPauseButton();
        }
        if (stageManager != null)
        {
            Destroy(stageManager.gameObject);
            stageManager = null;
        }
    }

    void ClearIntrada()
    {
        GameObject[] objs = GameObject.FindGameObjectsWithTag("intrada");

        for (int i = 0; i < objs.Length; i++)
        {
            Destroy(objs[i]);
        }
    }

    public void StartCurrentSong()
    {
        //AudioManager.Instance.PlaySong(Path.GetFileNameWithoutExtension(CorePlayData.CurrentSong.AudioFileName.Trim()));
        //if (CorePlayData.CurrentSong.LyricName != "0")
        //{
        //	AudioManager.Instance.PlayLyric(CorePlayData.CurrentSong.LyricName);
        //}
        //AudioListener.volume = 1f;
        AudioController.StopAll();
        AudioController.SetGlobalVolume(1);
        AudioController.PlayMusic("Song");


        WriteLog.Instance.fileName(CorePlayData.CurrentSong.Title);
        WriteLog.Instance.writelog("======================" + CorePlayData.CurrentSong.Title + "===================");
    }

    void StartKeyAndVoice()
    {
        m_Status = GameStatus.KeyAndVoice;
        if (m_KVLogic == null)
            m_KVLogic = new KeyAndVoiceLogic(m_InputCheck);
        m_KVLogic.StartLogic();
    }

    void KeyAndVoiceLogic()
    {
        m_KVLogic.Update();
    }

    void StartBossWarLogic()
    {
        m_Status = GameStatus.BossWar;
        if (m_BossWar == null)
            m_BossWar = new CorePlayBossWar(m_InputCheck);
        m_BossWar.StartLogic();
    }

    void BossWarLogic()
    {
        m_BossWar.Update();
    }

    public void VoiceUIFadeIn(Action callback = null)
    {
        m_VoiceCreator.BGFadeIn(callback);
    }

    public void VoiceUIFadeOut(Action callback = null)
    {
        m_VoiceCreator.BGFadeOut(callback);
    }

    public void CreateVoiceString(string sentence, float duration)
    {
        m_VoiceCreator.CreateWords(sentence, duration);
    }

    public void CreateVoiceCheck(int sentenceIndex)
    {
        List<string> list = new List<string>();
        List<HitObject> hoList = CorePlayData.CurrentSong.SentenceObjs[sentenceIndex].ClickAndHOList.HitObjects;
        for (int i = 0; i < hoList.Count; i++)
        {
            list.Add(hoList[i].Word);
        }
        m_InputCheck.GetVoiceCheckSentence(list, VoiceSentenceCalc);
    }

    private const int Second2MilliSecond = 1000;
    void Update()
    {
        switch (m_Status)
        {
            case GameStatus.StartTutorial:
                //PageManager.Instance.CurrentPage.GetNode<CalculateNode>().AllFadeOut();
                PageManager.Instance.CurrentPage.GetNode<CalculateNode>().gameObject.SetActive(false);
                StartCoroutine(InstantTutorial());
                break;
            case GameStatus.InTutorial:
                if (Tutorial == null)
                {
                    Reset();
                    StartLogic();
                    m_Status = GameStatus.GameStart;
                }
                break;
            case GameStatus.GameStart:
                //m_Timer = AudioManager.Instance.GetBgmTime() * Second2MilliSecond;
                m_Timer += Time.deltaTime * Second2MilliSecond;
                m_SFXTimer += Time.deltaTime;
                while (m_TimerEvents.Count > 0 && m_Timer >= m_TimerEvents.ToArray()[0].m_Timer + CorePlayData.SongOffset)
                {
                    ProcessEvents(m_TimerEvents.Dequeue());
                }
                break;
            case GameStatus.BossWar:
                BossWarLogic();
                break;
            case GameStatus.KeyAndVoice:
                KeyAndVoiceLogic();
                break;
        }

        CalcRhythmBeat();
    }

    private TutorialScene Tutorial;
    IEnumerator InstantTutorial()
    {
        m_Status = GameStatus.Idle;
        yield return new WaitForSeconds(1f);
        m_Status = GameStatus.InTutorial;
        Tutorial = Instantiate(ResourceLoadUtils.Load<TutorialScene>("CorePlay/TutorialNode"));
    }

    private void CalcRhythmBeat()
    {
        float timer = m_Timer % m_RhythmBeatInterval;
        m_RhythmBeat = timer / m_RhythmBeatInterval;
    }

    public void StartMicRecord()
    {
        XunFeiSRManager.Instance.StartListen(m_InputCheck.GetSRCallback);
        MicManager.Instance.StartRecord(VoiceController.CreateVoiceStar, m_InputCheck.SoundWave, m_InputCheck.SilentCallback);
        AudioController.GetCurrentMusic().FadeOut(1.5f, 0, 0.5f, false);
    }

    public void StopMicRecord()
    {
        StartCoroutine(StopXunFeiRecorder());
        MicManager.Instance.StopRecord();
        var audio = AudioController.GetCurrentMusic();
        if (audio != null)
            audio.FadeIn(1.5f);
    }

    public void CleanAll()
    {
        if (Tutorial != null)
            Tutorial.Reset();
        StopMicRecord();
        m_TapCreator.Restart();
        if (m_BossWar != null)
            m_BossWar.Restart();
    }

    public bool HasIntrada = false;

    public void Continue()
    {
        Time.timeScale = 1f;
        switch (m_IntradaStatus)
        {
            case IntradaStatus.WaitForPlaying:
            case IntradaStatus.Playing:
                GameRecover();
                break;
            case IntradaStatus.Played:
                if (HasIntrada == false)
                {
                    CorePlayIntrada intrada = new CorePlayIntrada(CorePlayData.BMPDelta, GameRecover);
                    HasIntrada = true;
                    intrada.Show();
                }
                break;
        }

        if (Tutorial != null)
        {
            Tutorial.Pause = false;
        }
    }

    private void GameRecover()
    {
        //AudioManager.Instance.ResumeSound();
        AudioController.UnpauseAll();

        HasIntrada = false;
        m_Status = m_LastStatus;
        if (m_BossWar != null)
        {
            m_BossWar.Recover();
        }
    }

    private GameStatus m_LastStatus;
    public void PauseGame()
    {
        //AudioManager.Instance.PauseSound();
        // 不能直接加 fade 参数
        AudioController.PauseAll();


        Time.timeScale = 0;
        PageManager.Instance.CurrentPage.GetNode<PauseNode>().Open();
        if (m_Status != GameStatus.Pause)
            m_LastStatus = m_Status;
        m_Status = GameStatus.Pause;
        if (m_BossWar != null)
        {
            m_BossWar.Pause();
        }

        if (Tutorial != null)
        {
            Tutorial.Pause = true;
        }
    }

    private void OnApplicationFocus(bool focus)
    {
        if ((m_Status != GameStatus.Pause && m_Status != GameStatus.Finish) && !focus && !m_EditMode && !LoginScene.isFirstGoingGame)
        {
            PauseGame();
        }
    }

    public int GotHit(string word, int sentenceIndex)
    {
        return m_InputCheck.GotHit(word, sentenceIndex, (int)m_Timer);
    }

    IEnumerator StopXunFeiRecorder()
    {
        yield return new WaitForSeconds(2f);
        XunFeiSRManager.Instance.StopListen();
    }

    IEnumerator SentenceCheck(int sentenceIndex, float duration)
    {
        yield return new WaitForSeconds(duration);
        m_InputCheck.CalcSentence(sentenceIndex);
    }

    public void VoiceGetAndStartNextSentence()
    {
        if (m_TimerEvents.Count > 0)
            m_Timer = m_TimerEvents.ToArray()[0].m_Timer;
    }

    private int m_ContinuoslyWrongTime;
    private int m_ContinuoslyRightTime;
    public void ClickRight(int sentenceIndex, int checkIndex, float accuracy)
    {
        m_ContinuoslyWrongTime = 0;
        m_ContinuoslyRightTime++;
        if (m_ContinuoslyRightTime >= CorePlaySettings.Instance.m_MaxContinuoslyRightTime)
        {
            m_AutoPlay.Close();
        }
        //Debug.Log(accuracy);
        float score = CorePlaySettings.Instance.m_ComboPoint + Combo * RuntimeConst.ScoreParam * accuracy;
        score *= m_ScoreParam;
        UpdateComboAndScore((int)score, 1);
    }

    public void ClickWrong(int sentenceIndex, int checkIndex)
    {
        m_ContinuoslyWrongTime++;
        m_ContinuoslyRightTime = 0;
        if (m_ContinuoslyWrongTime >= CorePlaySettings.Instance.m_MaxContinuoslyWrongTime)
        {
            m_StartAutoPlay = true;
            m_AutoPlay.Show();
            if (!AudioController.IsPlaying("bell_tip"))
                AudioController.Play("bell_tip");
        }

        ComboBreak(true);
    }

    public void VoiceSentenceCalc(float accuracy)
    {
        AddVoicePoint((int)(accuracy * 100));
    }

    public void AddVoicePoint(int accuracy)
    {
        float difficultyAddOn = 1f;
        if (StaticData.Difficulty == CorePlaySettings.Instance.m_Normal)
        {
            difficultyAddOn = 0.8f;
        }

        float score = accuracy / 100f * CorePlaySettings.Instance.m_VoiceRightPoint * difficultyAddOn;
        score *= m_ScoreParam;
        m_Score += (int)score;
        Combo++;
        if (m_MaxCombo < Combo)
            m_MaxCombo = Combo;
        print("录音算分。。。。。。。");
        UpdateComboScore();
    }


    public void ComboBreak(bool clickWrong = false)
    {
        if (clickWrong)
            m_WrongNum++;
        Combo = 0;
        if (m_Status != GameStatus.BossWar)
            ChangeLife(-1);
        //AudioController.Play("combobreak");
        //WriteLog.Instance.writelog("得分：" + m_Score + ", Combo：" + m_Combo);
        EffectManager.instance.ResetCamera();
        UpdateComboScore();
    }

    private void UpdateComboScore()
    {
        if (m_Status == GameStatus.BossWar)
        {
            PageManager.Instance.CurrentPage.GetNode<BossWarNode>().UpdateScoreAndCombo(m_Score, Combo);
        }
        else
        {
            PageManager.Instance.CurrentPage.GetNode<CalculateNode>().UpdateScoreAndCombo(m_Score, Combo);
        }
    }

    private const int ComboEnoughValve = 10;
    public void UpdateComboAndScore(int scoreIncrease, int comboIncrease)
    {
        //Debug.Log("combo加分");
        Combo += comboIncrease;
        ProcessComboTip();

        //if (m_Life > 0)
        //{
        m_Score += scoreIncrease;
        //}
        m_CorrectNum++;
        //WriteLog.Instance.writelog("得分：" + m_Score + ", Combo：" + m_Combo);
        if (m_MaxCombo < Combo)
            m_MaxCombo = Combo;
        UpdateComboScore();
    }

    private void ProcessComboTip()
    {
        if (Combo <= 100 && (Combo % 10 == 0) && Combo > 0)
        {
            string comboString = Combo.ToString();
            SetComboTextActive(true, comboString);
            if (!CheckInHighlightTiming())
            {
                AudioController.Play("combo_" + comboString);

            }
            else
            {
                AudioController.Play("combo_" + comboString, CorePlaySettings.Instance.m_VolumeValve);
            }
        }
    }

    bool CheckInHighlightTiming()
    {
        for (int i = 0; i < m_AllHOPoint.Count; i++)
        {
            if (m_Timer >= m_AllHOPoint[i] - CorePlaySettings.Instance.m_ComboTimeValve * 1000f &&
               m_Timer <= m_AllHOPoint[i] + CorePlaySettings.Instance.m_ComboTimeValve * 1000f)
            {
                return true;
            }
        }

        return false;
    }


    private const int AlmostLoseValve = 1;
    public void ChangeLife(int count, bool immediate = false)
    {
        m_Life += count;
        if (m_Life < 0)
        {
            m_Life = 0;
            //StartCoroutine(GameFailed());
        }
        else if (m_Life == AlmostLoseValve)
        {
            //AudioController.Play("be_careful");
        }

        UpdateLife(immediate);
    }

    private void UpdateLife(bool immediate = false)
    {
        if (PageManager.Instance.CurrentPage == null) return;
        if (CorePlayData.BossLife > 0)
        {
            PageManager.Instance.CurrentPage.GetNode<BossWarNode>().UpdatePlayerLife(m_Life, immediate);
        }
        else
        {
            PageManager.Instance.CurrentPage.GetNode<CalculateNode>().UpdateLifeBar(m_Life);
        }
    }


    public void BossWarPlayerFail()
    {
        m_Status = GameStatus.Finish;
        AudioController.Play("Oh_no");
        XunFeiSRManager.Instance.StopListen();
        MicManager.Instance.StopRecord();
        SaveResultToLocal(-1);
        AnalysisManager.Instance.OnEvent("100002", null, StaticData.LevelID.ToString(), "完成游戏");
        AnalysisManager.Instance.OnEvent("100001", null, StaticData.LevelID.ToString(), "BOSS关卡失败");
        AnalysisManager.Instance.OnLevelFailed(StaticData.LevelID.ToString(), "Boss关卡");
        PageManager.Instance.CurrentPage.GetNode<GameOverNode>().Open();
    }

    public void GameFinish()
    {
        m_Status = GameStatus.Finish;
        AudioController.Play("applause");

        if (m_WrongNum == 0)
        {
            AudioController.Play("combo_perfect");
        }
        if (m_Life > 0)
        {
            AnalysisManager.Instance.OnEvent("100002", null, StaticData.LevelID.ToString(), "完成游戏");
            AnalysisManager.Instance.OnEvent("100001", null, StaticData.LevelID.ToString(), m_Life.ToString());
            AnalysisManager.Instance.OnLevelCompleted(StaticData.LevelID.ToString());
        }
        else
        {
            AnalysisManager.Instance.OnEvent("100002", null, StaticData.LevelID.ToString(), "完成游戏");
            AnalysisManager.Instance.OnEvent("100001", null, StaticData.LevelID.ToString(), "空血通关");
            AnalysisManager.Instance.OnLevelFailed(StaticData.LevelID.ToString(), "没血通关");
        }
        // 记录成绩
        SelfPlayerLevelData.TempRankIncrement = SelfPlayerLevelData.CurRank;
        Debug.LogWarning("  记录成绩 = " + SelfPlayerLevelData.TempRankIncrement);
        Debug.Log("<======记录舞蹈数据 boss战舞蹈数据为空 ======>");
        // 打开UI
        if (CorePlayData.BossLife > 0)
        {
            m_ProcessData = null;
            SelfPlayerRoleTitleData.Instance.UpdateRoleTitleCount(50002);
        }
        else
        {
            SelfPlayerRoleTitleData.Instance.UpdateRoleTitleCount(50001);
            PlayerOperationData operData = new PlayerOperationData();
            operData.wholeScore = m_Score;
            operData.clickAccuracy = (float)m_CorrectNum / m_AllWordNum;
            operData.clickNumber = m_AllWordNum;
            operData.clickScore = m_Score - (int)(m_SumVoiceScore * CorePlaySettings.Instance.m_VoiceRightPoint);
            operData.wrongNumber = m_WrongNum;
            operData.rightNumber = m_CorrectNum;
            operData.m_ReadingData = CorePlayData.PlayerReadingData;

            //临时测试上传操作过程数据
            m_ProcessData = operData.GetJson();
        }
        SaveDataUtils.Save<SelfPlayerRoleTitleData>();
        DancingWordAPI.Instance.UpDateServerRoleTitleInfo(SelfPlayerRoleTitleData.RoleTitleList);
        SaveResultToLocal(m_Life);
        int grade = SetLevel.setLevel(SelfPlayerLevelData.CurAccuracy);
        SelfPlayerData.Instance.AddExpAndSaveToLocal(grade);
        // 关录音
        XunFeiSRManager.Instance.StopListen();
        MicManager.Instance.StopRecord();
        LoginRpcProxy.getInstance().SaveLevelVoices(MicManager.Instance.voiceDic);

		PageManager.Instance.CurrentPage.GetComponent<CorePlayPage>().HidePauseButton();
        if (CorePlayData.BossLife > 0)
        {
            PageManager.Instance.CurrentPage.GetNode<WinNode>().Open();
        }
        else
        {
            HttpHandler.UploadScore(ServerDataCallback, true);
            m_AlreadyRuning = false;
            LoadStage();
        }
    }

	#region [--- Http Call Back ---]
	void OnReceive_ScoreData ()
	{
		if (MiscUtils.IsOnline ())
		{
			HttpHandler.Request_10_Before_And_10_After (OnReceive_10_10_RankData);
		}
	}

	void OnReceive_10_10_RankData ()
	{
		if (SelfPlayerLevelData.TempRankIncrement < 0)
		{
			SelfPlayerLevelData.TempRankIncrement = 0;
		}
		else
		{
			SelfPlayerLevelData.TempRankIncrement = SelfPlayerLevelData.TempRankIncrement - SelfPlayerLevelData.CurRank;
		}
		//Debug.LogWarning (" >>>> = " + SelfPlayerLevelData.TempRankIncrement);
		//SetupRankPanel ();
	}
	#endregion

    private const float WaitingLength = 3f;
    private bool m_AlreadyRuning;
    void ServerDataCallback()
    {
		OnReceive_ScoreData();
        if (!m_AlreadyRuning)
        {
            StopCoroutine(LoadStageData());

            m_AlreadyRuning = true;
            //从服务器获取的真实数据


            int songMax = m_SongMaxScoreInfo.MaxScore;
            int clickMax = (int)(m_SongMaxScoreInfo.MaxScore * m_SongMaxScoreInfo.ClickScorePercent);
            int voiceMax = (int)(m_SongMaxScoreInfo.MaxScore * m_SongMaxScoreInfo.VoiceScorePercent);

            DancerInfo left = new DancerInfo();
            DancerInfo right = new DancerInfo();

            if (!string.IsNullOrEmpty(SelfPlayerLevelData.TempOtherLevelProcessData))
            {
                JsonData otherPlayers = JsonMapper.ToObject(SelfPlayerLevelData.TempOtherLevelProcessData);

                JsonData leftJson = otherPlayers[0];
                JsonData rightJson = otherPlayers[1];

                if (int.Parse(leftJson.TryGetString("Country")) == China)
                {
                    left.m_Country = DancerInfo.Country.China;
                }
                else
                {
                    left.m_Country = DancerInfo.Country.America;
                }

                left.ModelID = RoleModelConfig.Instance.GetNameById(int.Parse(leftJson.TryGetString("ModelId")));
                left.Name = leftJson.TryGetString("UserName");
                left.FaceID = RoleEmotionConfig.Instance.GetNameById(int.Parse(leftJson.TryGetString("EmotionId")));
                left.PlayerID = leftJson.TryGetString("uid");

                PlayerOperationData leftOperation = PlayerOperationData.GetDataFromJson(leftJson.TryGetString("Process"));

                left.WholeScore = leftOperation.wholeScore;
                left.ClickScore = leftOperation.clickScore;
                left.VoiceScore = left.WholeScore - left.ClickScore;

                left.WholeRankingPercent = (float)left.WholeScore / songMax;
                left.ClickScorePercent = (float)left.ClickScore / clickMax;
                left.VoiceScorePercent = (float)left.VoiceScore / voiceMax;

                if (int.Parse(rightJson.TryGetString("Country")) == China)
                {
                    right.m_Country = DancerInfo.Country.China;
                }
                else
                {
                    right.m_Country = DancerInfo.Country.America;
                }

                right.ModelID = RoleModelConfig.Instance.GetNameById(int.Parse(rightJson.TryGetString("ModelId")));
                right.Name = leftJson.TryGetString("UserName");
                right.FaceID = RoleEmotionConfig.Instance.GetNameById(int.Parse(rightJson.TryGetString("EmotionId")));
                right.PlayerID = rightJson.TryGetString("uid");

                PlayerOperationData rightOperation = PlayerOperationData.GetDataFromJson(rightJson.TryGetString("Process"));

                right.WholeScore = rightOperation.wholeScore;
                right.ClickScore = rightOperation.clickScore;
                right.VoiceScore = right.WholeScore - right.ClickScore;

                right.WholeRankingPercent = (float)right.WholeScore / songMax;
                right.ClickScorePercent = (float)right.ClickScore / clickMax;
                right.VoiceScorePercent = (float)right.VoiceScore / voiceMax;

            }
            else
            {
                if (m_SelfWinner)
                {
                    left = GetNPCDancer(false, m_Score, DancerInfo.Country.China, DancerInfo.Sex.Female, songMax, clickMax, m_TempTypeCache);
                    right = GetNPCDancer(false, m_Score, DancerInfo.Country.America, DancerInfo.Sex.Male, songMax, clickMax, m_TempTypeCache);
                }
                else
                {
                    int temp = UnityEngine.Random.Range(0, 2);
                    if (temp == 0)
                    {
                        left = GetNPCDancer(true, m_Score, DancerInfo.Country.China, DancerInfo.Sex.Female, songMax, clickMax, m_TempTypeCache);
                        right = GetNPCDancer(false, m_Score, DancerInfo.Country.America, DancerInfo.Sex.Male, songMax, clickMax, m_TempTypeCache);
                    }
                    else
                    {
                        left = GetNPCDancer(false, m_Score, DancerInfo.Country.China, DancerInfo.Sex.Female, songMax, clickMax, m_TempTypeCache);
                        right = GetNPCDancer(true, m_Score, DancerInfo.Country.America, DancerInfo.Sex.Male, songMax, clickMax, m_TempTypeCache);
                    }
                }
            }

            stageManager.CreateDancer(left, StageManager.DancerPos.Left);
            stageManager.CreateDancer(right, StageManager.DancerPos.Right);
            stageManager.CreateDancerActionData();
        }
    }

    private void LoadStage()
    {
        SceneController.instance.HideMidground();
        PageManager.Instance.CurrentPage.GetNode<CalculateNode>().Close();
        stageManager = Instantiate(Resources.Load<StageManager>("CorePlay/Stage"));
        stageManager.Init(m_RhythmBeatInterval * 0.001f);
        stageManager.SetChampion(false);
        stageManager.ShowStage();
        stageManager.CreateDancer(GetSelfDancerInfo(), StageManager.DancerPos.Middle);
      

        StartCoroutine(LoadStageData());
    }

    IEnumerator LoadStageData()
    {
        yield return new WaitForSeconds(WaitingLength);
        //模拟数据
        if (!m_AlreadyRuning)
        {
            m_AlreadyRuning = true;

            int songMax = m_SongMaxScoreInfo.MaxScore;
            int clickMax = (int)(m_SongMaxScoreInfo.MaxScore * m_SongMaxScoreInfo.ClickScorePercent);
            int voiceMax = (int)(m_SongMaxScoreInfo.MaxScore * m_SongMaxScoreInfo.VoiceScorePercent);

            DancerInfo left = new DancerInfo();
            DancerInfo right = new DancerInfo();

            if(m_SelfWinner)
            {
                left = GetNPCDancer(false, m_Score, DancerInfo.Country.China, DancerInfo.Sex.Female, songMax, clickMax, m_TempTypeCache);
                right = GetNPCDancer(false, m_Score, DancerInfo.Country.America, DancerInfo.Sex.Male, songMax, clickMax, m_TempTypeCache);
            }
            else
            {
                int temp = UnityEngine.Random.Range(0, 2);
                if(temp == 0)
                {
                    left = GetNPCDancer(true, m_Score, DancerInfo.Country.China, DancerInfo.Sex.Female, songMax, clickMax, m_TempTypeCache);
                    right = GetNPCDancer(false, m_Score, DancerInfo.Country.America, DancerInfo.Sex.Male, songMax, clickMax, m_TempTypeCache);
                }
                else
                {
                    left = GetNPCDancer(false, m_Score, DancerInfo.Country.China, DancerInfo.Sex.Female, songMax, clickMax, m_TempTypeCache);
                    right = GetNPCDancer(true, m_Score, DancerInfo.Country.America, DancerInfo.Sex.Male, songMax, clickMax, m_TempTypeCache);
                }
            }

            stageManager.CreateDancer(left, StageManager.DancerPos.Left);
            stageManager.CreateDancer(right, StageManager.DancerPos.Right);
            stageManager.CreateDancerActionData();
        }
    }

    private bool m_SelfWinner;
    DancerInfo GetSelfDancerInfo()
    {
        DancerInfo self = new DancerInfo();

        if (SelfPlayerData.Sex == Female)
        {
            self.m_Sex = DancerInfo.Sex.Female;
        }
        else
        {
            self.m_Sex = DancerInfo.Sex.Male;
        }

        if (SelfPlayerData.Country == China)
        {
            self.m_Country = DancerInfo.Country.China;
        }
        else
        {
            self.m_Country = DancerInfo.Country.America;
        }

        if(SelfPlayerLevelData.ScoreIncrement > 0)
        {//自己是胜利者
            self.m_RankingType = DancerInfo.RankingType.Winner;
            m_SelfWinner = true;
        }
        else
        {//自己是失败者
            self.m_RankingType = DancerInfo.RankingType.Loser;
            m_SelfWinner = false;
        }

        m_TempTypeCache.Clear();
        self.ModelID = RoleModelConfig.Instance.GetNameById(int.Parse(SelfPlayerData.ModelId));
        m_TempTypeCache.Add(self.ModelID);
        self.FaceID = RoleEmotionConfig.Instance.GetNameById(int.Parse(SelfPlayerData.EmotionId));
        self.Name = SelfPlayerData.UserName;
        self.PlayerID = SelfPlayerData.Uuid;

        int songMax = m_SongMaxScoreInfo.MaxScore;
        int clickMax = (int)(m_SongMaxScoreInfo.MaxScore * m_SongMaxScoreInfo.ClickScorePercent);
        int voiceMax = (int)(m_SongMaxScoreInfo.MaxScore * m_SongMaxScoreInfo.VoiceScorePercent);
        self.WholeScore = m_Score;
        self.VoiceScore = (int)(m_SumVoiceScore * CorePlaySettings.Instance.m_VoiceRightPoint);
        self.ClickScore = self.WholeScore - self.VoiceScore;
        self.WholeRankingPercent = (float)self.WholeScore / songMax;
        self.ClickScorePercent = (float)self.ClickScore / clickMax;
        self.VoiceScorePercent = (float)self.VoiceScore / voiceMax;

        return self;
    }

    private const int Female = 0;
    private const int China = 0;
    private List<string> m_TempTypeCache = new List<string>();
    public static DancerInfo GetNPCDancer(bool betterOne, int wholeScore, DancerInfo.Country country, DancerInfo.Sex sex, int maxScore, int clickMaxScore, List<string> typeList)
    {
        DancerInfo dancer = new DancerInfo();
		if (country == DancerInfo.Country.China)
        {
            dancer.Name = RoleManager.RandomUserNickName(LanguageType.Chinese);
        }
        else
        {
            dancer.Name = RoleManager.RandomUserNickName(LanguageType.English);
        }

        dancer.m_Country = country;
        dancer.m_Sex = sex;
        if (betterOne)
        {
            dancer.WholeScore = UnityEngine.Random.Range(wholeScore + 1, maxScore);
            dancer.m_RankingType = DancerInfo.RankingType.Winner;
        }
        else
        {
            dancer.WholeScore = UnityEngine.Random.Range(0, wholeScore);
            dancer.m_RankingType = DancerInfo.RankingType.Loser;
        }

        dancer.ClickScore = UnityEngine.Random.Range(0, dancer.WholeScore);
        dancer.VoiceScore = dancer.WholeScore - dancer.ClickScore;
        dancer.WholeRankingPercent = (float)dancer.WholeScore / maxScore;
        dancer.ClickScorePercent = (float)dancer.ClickScore / clickMaxScore;
        dancer.VoiceScorePercent = (float)dancer.VoiceScore / (maxScore - clickMaxScore);

        string tempType = RoleModelConfig.Instance.GetNameById(UnityEngine.Random.Range(20001, 20009));
        while (typeList.Contains(tempType))
        {
            tempType = RoleModelConfig.Instance.GetNameById(UnityEngine.Random.Range(20001, 20009));
        }
        typeList.Add(tempType);

        dancer.ModelID = tempType;
        dancer.FaceID = RoleEmotionConfig.Instance.GetNameById(UnityEngine.Random.Range(30001, 30009));
        dancer.PlayerID = "npc";

        return dancer;
    }


    public void WatchOtherDance(ChoreographerData data)
    {
        stageManager.ClearDancers();
        stageManager.CreateDataFromWholeChoreographer(data);
        stageManager.SetChampion(true);
        stageManager.ShowStage();
    }

	private IEnumerator Waitsecond (float time)
	{
		yield return new WaitForSeconds (time);
		PageManager.Instance.CurrentPage.GetNode<ShareNode> ().gameObject.SetActive (false);
		PageManager.Instance.CurrentPage.GetNode<ShareActivityNode> ().Open ();
		PageManager.Instance.CurrentPage.GetNode<ShareActivityNode> ().gameObject.SetActive (true);
	}
	private void SaveResultToLocal (int life)
	{
		float CorrectRat = 0;
		//if (m_CorrectNum + m_WrongNum != 0)
		//{
		//	CorrectRat = (float)m_CorrectNum / (float)(m_AllWordNum + m_WrongNum);
		//}
		LogManager.Log("关卡最高分：",m_SongMaxScoreInfo.MaxScore);
		CorrectRat = ((m_Score * 1.0f)/m_ScoreParam) / m_SongMaxScoreInfo.MaxScore;
		int resultScore = (int)Math.Round( (m_Score * 1.0f) / m_ScoreParam,0);
		float wordAccuracy = m_AllWordNum <= m_VoiceNum ? 1 : (float)m_CorrectNum / m_AllWordNum;
		float readAccuracy = m_VoiceNum == 0 ? 0 : m_SumVoiceScore / m_VoiceNum;
		LogManager.Log ("得分：" ,m_Score , "，点击准确率：" ,CorrectRat , "，最大Combo数：" ,m_MaxCombo , "，生命：" , life ,"，读音准确率：" ,readAccuracy ,"，单词准确率：" , wordAccuracy);

		SelfPlayerLevelData.SaveToLocal (resultScore, CorrectRat, m_MaxCombo, life, readAccuracy, wordAccuracy, m_ProcessData);
	}

	public void UpdateTimer (float timer)
	{
		m_Timer = timer;
		UpdateTimerEventQueue ();
	}
	public void SetStatus (CorePlayManager.GameStatus status)
	{
		m_Status = status;
		if (m_Status == GameStatus.GameStart)
		{
			UpdateTimerEventQueue ();
		}
	}

	private void UpdateTimerEventQueue ()
	{
		int millisecond = (int)(m_Timer * RuntimeConst.SecondToMS);

		m_TimerEvents.Clear ();
		for (int i = 0; i < m_TimerEventsCopy.Count; i++)
		{
			if (m_TimerEventsCopy[i].m_Timer >= millisecond)
			{
				m_TimerEvents.Enqueue (m_TimerEventsCopy[i]);
			}
		}
	}

	public float m_HorizontalSlider = 0.0F;
	private void OnGUI ()
	{
		if (CorePlaySettings.Instance.m_ShowProgressBar)
		{
			m_HorizontalSlider = GUI.HorizontalSlider (new Rect (25, 25, 200, 30), m_HorizontalSlider, 0.0F, 1.0F);

			if (GUI.Button (new Rect (50, 50, 50, 50), "set"))
			{
				SetProgress ();
			}
			if (GUI.Button (new Rect (150, 50, 50, 50), "Boss"))
			{
				StartBossWarLogic ();
			}
			if (GUI.Button (new Rect (250, 50, 50, 50), "IgnoreCG"))
			{
				m_BossWar.IgnoreBossCG ();
			}
			if (GUI.Button (new Rect (350, 50, 50, 50), "autoPlay"))
			{
				m_StartAutoPlay = true;
				m_AutoPlay.Show ();
			}
		}
	}

	void SetProgress ()
	{
		UIRestart ();
		AudioAndMicRestart ();
		DataRestart ();
		m_InputCheck.Restart ();
		m_TapCreator.Restart ();
		m_VoiceCreator.Restart ();

		float pointTime = AudioController.GetCurrentMusic ().clipLength * m_HorizontalSlider;
		int millisecond = (int)(pointTime * RuntimeConst.SecondToMS);
		m_Status = GameStatus.GameStart;
		m_Timer = millisecond;
		UpdateTimerEventQueue ();
		AudioController.PlayMusic ("Song", 1, 0, pointTime);
	}
}
