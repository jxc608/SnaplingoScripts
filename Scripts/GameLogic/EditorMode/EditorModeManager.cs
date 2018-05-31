using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EditorModeManager : MonoBehaviour
{
    public static EditorModeManager Instance;
    #region UI and GameObjects
    private Dropdown m_TimeScaleDropDown;
    private InputField m_BPM;
    private Slider m_TimerSlider;
    #endregion

    #region Managers
    private EditorModeTimeLineCircle m_TimeLineRuler;
    private CorePlayManager m_CorePlayManager;
    private EditorModeOperation m_EditorOperation;
    #endregion

    #region Properties
    private int m_TimeScale;

    private float m_Timer;
    public float Timer
    {
        get { return m_Timer; }
        set { m_Timer = value; }
    }

    private bool m_GetBPMFromData;
    public bool GetBPMFromData
    {
        get { return m_GetBPMFromData; }
        set { m_GetBPMFromData = value; }
    }

    private float m_MaxTimeLength;

    public float TimeLineWidth
    {
        get { return m_TimeLineRuler.ScreenWidth; }
    }
    #endregion

#region Init
    private enum PlayMusicStatus {Idle, Playing, Pausing}
    private PlayMusicStatus m_Status;
    public void Init()
    {
        Instance = this;

        m_Status = PlayMusicStatus.Idle;
        m_Timer = 0; 
        SetListener();
    }

    private const int Ruler = 2;
    private void SetListener()
    {
        transform.Find("PlayAudio/Play").GetComponent<Button>().onClick.AddListener(Play);
        transform.Find("PlayAudio/Pause").GetComponent<Button>().onClick.AddListener(Pause);
        transform.Find("PlayAudio/Stop").GetComponent<Button>().onClick.AddListener(Stop);

        m_TimeScaleDropDown = transform.Find("TimeScale").GetComponent<Dropdown>();
        m_TimeScaleDropDown.onValueChanged.AddListener(GetTimeScaleDrop);

        m_BPM = transform.Find("Bpm").GetComponent<InputField>();
        if(GetBPMFromData)
        {
            m_BPM.text = CorePlayData.CurrentSong.BPM.ToString();
        }
        else
        {
            m_BPM.text = "";
        }

        m_TimerSlider = transform.Find("PlayAudio/PlayTime").GetComponent<Slider>();

        Transform ruler = transform.GetChild(Ruler);
        m_TimeLineRuler = ruler.GetComponent<EditorModeTimeLineCircle>();
        m_TimeLineRuler.Init();

        m_CorePlayManager = gameObject.AddComponent<CorePlayManager>();
        m_CorePlayManager.SetEditMode();

        gameObject.AddComponent<EditorModePointerControl>();

        m_EditorOperation = new EditorModeOperation();
    }
#endregion

#region public interface
    public void SetTimeLineRuler(int number)
    {
        m_TimeLineRuler.ChangeRulerNumber(number);
    }

    public void SetData()
    {
        CalcMaxLength();
        m_TimeLineRuler.GetHOList();
    }

    private void CalcMaxLength()
    {
        int commonSentenceNumber = CorePlayData.CurrentSong.SentenceObjs.Count;
        float audioLen = CorePlayData.CurrentSong.SentenceObjs[commonSentenceNumber - 1].m_InOutTime.EndTime * RuntimeConst.MSToSecond;

        if (CorePlayData.CurrentSong.StreamSentences.Count > 0)
        {
            for (int i = 0; i < CorePlayData.CurrentSong.StreamSentences.Count; i++)
            {
                audioLen += CorePlayData.CurrentSong.StreamSentences[i].Group[0].Duration;
            }
        }

        if (CorePlayData.CurrentSong.NonRhythmSenteces.Count > 0)
        {
            for (int i = 0; i < CorePlayData.CurrentSong.NonRhythmSenteces.Count; i++)
            {
                audioLen += CorePlayData.CurrentSong.NonRhythmSenteces[i].Duration;
            }
        }
        m_MaxTimeLength = audioLen;
        m_TimerSlider.maxValue = audioLen;
    }

    public List<EditorTimeLineCircleNode> GetCircleNodeList(Vector2 startPos, Vector2 endPos)
    {
        return m_TimeLineRuler.GetCircleNodeList(startPos,endPos);
    }

    public void TimeRulerChange(float delta)
    {
        m_Timer += delta;
    }
#endregion

#region Listener
    public void Play()
    {
        m_Status = PlayMusicStatus.Playing;
        m_CorePlayManager.SetStatus(CorePlayManager.GameStatus.GameStart);
        AudioController.PlayMusic(CorePlayData.CurrentSong.AudioFileName);
    }

    public void Pause()
    {
        m_Status = PlayMusicStatus.Pausing;
        m_CorePlayManager.SetStatus(CorePlayManager.GameStatus.Pause);
        AudioController.PauseMusic();
    }

    public void Stop()
    {
        m_Status = PlayMusicStatus.Idle;
        m_CorePlayManager.SetStatus(CorePlayManager.GameStatus.Idle);
        m_CorePlayManager.Reset();
        m_Timer = 0;
        AudioController.StopMusic();
    }

    private void GetTimeScaleDrop(int number)
    {
        m_TimeScale = int.Parse(m_TimeScaleDropDown.options[number].text);
        SetTimeLineRuler(m_TimeScale);
    }
#endregion

#region Update
    void Update () 
    {
        switch(m_Status)
        {
            case PlayMusicStatus.Playing:
                Playing();
                break;
        }
        UpdateTimeLine();
        ProcessUndo();
	}

    void ProcessUndo()
    {
        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKey(KeyCode.Z))
        {
            Pause();
            m_EditorOperation.UndoOperation();
        }
    }

    void Playing()
    {
        m_Timer += Time.deltaTime;
        if (m_Timer >= m_MaxTimeLength)
        {
            m_Timer = m_MaxTimeLength;
            m_Status = PlayMusicStatus.Idle;
        }
     
    }

    void UpdateTimeLine()
    {
        m_TimeLineRuler.UpdateRuler(m_Timer);
        m_TimerSlider.value = m_Timer;
    }

#endregion

    void UpdateTimer()
    {
        
    }
}
