using UnityEngine;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
#endif
public class CorePlaySettings : ScriptableObject
{
	public static string CorePlaySettingPath = "Settings/CorePlaySettings";
#if UNITY_EDITOR
	[MenuItem("自定义/工具/导出脚本对象/CorePlay数据", false, 1)]
	static void CreateDebugController()
	{
		EditorUtils.CreateAsset<CorePlaySettings>(CorePlaySettingPath);
	}
#endif

	private static CorePlaySettings _instance = null;

	public static CorePlaySettings Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = ResourceLoadUtils.Load<CorePlaySettings>(CorePlaySettingPath);
			}
			return _instance;
		}
	}

	[Header("是否显示调试bar")]
	public bool m_ShowProgressBar = true;

	[Header("游戏内是否跳动")]
	public bool m_GameHeartBeat = false;

    [Header("是否使用内存池")]
    public bool m_UseMemoryPool = false;

	[Header("节奏点FadeIn时间长")]
	public int m_StartFadeInTimeLength = 500;
	[Header("节奏点FadeOut时长（毫秒）")]
	public int m_StartFadeOutTimeLength = 500;

	[Header("VoiceNode FadeIn时间长")]
	public int m_VoiceNodeFadeInTimeLength = 500;
	[Header("VoiceNode FadeOut时间长")]
	public int m_VoiceNodeFadeOutTimeLength = 500;
	[Header("VoiceNode check offset")]
	public int m_VoiceCheckOffset = 500;

	[Header("念对单词得分")]
	public int m_VoiceRightPoint = 300;
	[Header("声音检测等待时间（毫秒）")]
	public int m_VoiceCheckWaitLength = 1000;

	[Header("提前显示点击点的时间间隔（毫秒）")]
	[Range(0, 400)]
	public int m_PreShowTimeLength = 400;//毫秒

	[Header("引导圈提前时间（毫秒）")]
	[Range(0, 400)]
	public int m_highLightAheadTime = 400;//毫秒

	[Header("提前点击容错时间（毫秒）")]
	public int m_ClickOffset = 300;//毫秒

	[Header("perfect（毫秒）")]
	public int m_PerfectOffsetBefore = 100;//毫秒
	public int m_PerfectOffsetAfter = 200;//毫秒

	[Header("combo加分")]
	public float m_ComboPoint = 25f;

	[Header("准确率判断等级")]
	public int m_LevelSS = 9;
	public int  m_LevelS = 8;
	public int m_LevelA = 7;
	public int m_LevelB = 6;
	public int m_LevelC = 0;

	[Header("生命值debug")]
	public bool m_LifeDebug = false;

	[Header("开启语音输入检测")]
	public bool m_CheckVoice = true;


	[Header("当前界面UI消失时间")]
	public float m_DisappearTime = 1f;

	[Header("GameOver掉落")]
	public float m_FallDownWordTime = 0.2f;
	public float m_FallDownWordTimeLength = 1f;
	public float m_FallDownTime = 1f;
	public float m_BGFall = 0.5f;

	[Header("Voice Word Node 运动最小速度")]
	public float m_VoiceSpeedMin = 500f;
	public float m_VoiceDisMin = 100f;
	public float m_ArtWordDelta = 100f;

	[Header("外圈放大倍数")]
	public float m_VoiceOutCircleScaler = 2;
	[Header("TB倍数")]
	public float m_VoiceTBParam = 2;

	[Header("ClickNode的消失")]
	public float m_StarSize = 0.25f;
	public int m_StarSizeTime = 200;
	public int m_StarMoveTime = 200;
	public int m_StarDisappearTime = 100;
	public int m_ScorePunchTime = 200;
	public float m_ScorePunchSize = 1.2f;

	[Header("音效放小比例")]
	public float m_AudioVolume = 0.4f;
	public float m_AudioTimeLength = 0.1f;
	[Header("录音计算数据取样长度，2的幂次")]
	public int m_GetVolumeData = 4096;
	[Header("音量检测单词提前毫秒数")]
	public int m_VoiceWordAheadMS = 300;

	[Header("判断是否应该延迟删除和提前出现的时间阈值")]
	public int m_MinIntervalBeforeSentenceEnd = 800;

	[Header("boss战点错的机会数")]
	public int m_BossWarWrongChances = 1;

	[Header("是否跳过片头")]
	public bool m_TutorialJump = false;

	[Header("片头和音乐是否分开")]
	public bool m_SeperateAnimationAudio = false;


	[Header("Boss战难度")]
	public int m_Easy = 0;
	public int m_Normal = 1;
	public int m_Hard = 2;

    [Header("Boss语音句子判断正确的正确率阈值")]
    public float m_RightAccuracyValve = 0.5f;

	[Header("连续点错出提示的错误次数")]
	public int m_MaxContinuoslyWrongTime = 5;
	[Header("连续点对出提示的错误次数")]
	public int m_MaxContinuoslyRightTime = 3;

    [Header("连击语音音量变小，时间参数(秒)")]
    public float m_ComboTimeValve = .1f;
    [Header("在背景读单词的时候，combo连击语音音量")]
    public float m_VolumeValve = .5f;
}
