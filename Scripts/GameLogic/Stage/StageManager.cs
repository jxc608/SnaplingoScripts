using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using DG.Tweening;
using Snaplingo.UI;
using Spine.Unity;
using System.Text;

[System.Serializable]
public class ChangeDanceSectionEvent : UnityEvent<DanceSection>
{
}

[System.Serializable]
public class DanceBeatEvent : UnityEvent<int>
{
}

public class StageManager : MonoBehaviour
{
	public static StageManager Instance;

	float m_BPM = 0.5f;
	public float BPM {
		get { return m_BPM; }
	}

	int m_DanceBeatNumber;
	public int DanceBeatNumber {
		get { return m_DanceBeatNumber; }
	}

	int m_DanceBeatNumberOnThird;
	public int DanceBeatNumberOnThird {
		get { return m_DanceBeatNumberOnThird; }
	}

	StageAnimationBase[] m_AllStageAnimation;
	public SkeletonAnimation m_Tornado;
	Dancer[] m_AllDancer;
	ChoreographerData m_ChoreographerData;
	public AudioSource m_AudioSource;
	public bool m_Debug;
	public GameObject m_MiddleOriginPoint;
	public GameObject m_RightOriginPoint;
	public GameObject m_LeftOriginPoint;
	public BoneFollower m_MiddleFollower;
	public BoneFollower m_LeftFollower;
	public BoneFollower m_RightFollower;
	public StageNameTitle m_MiddleTitle;
	public StageNameTitle m_LeftTitle;
	public StageNameTitle m_RightTitle;
	public BlowEffect blowEffect;
	public SpriteRenderer m_CenterLight;
	public Text m_TimerText;
	public Text m_SubTitle;
	public SpriteRenderer stageTitle;
	public SpriteRenderer stageKing;
	public Sprite[] spriteArray;
	#region events
	public static ChangeDanceSectionEvent ChangeDanceSectionEvent = new ChangeDanceSectionEvent ();
	public static UnityEvent DanceStartEvent = new UnityEvent ();
	public static UnityEvent DanceEndEvent = new UnityEvent ();
	public static UnityEvent StopTornadoEvent = new UnityEvent ();
	public static DanceBeatEvent DanceBeat = new DanceBeatEvent ();
	#endregion

	#region AnimationName
	private static string[] m_AnimationGroup = {"attack_01","attack_02","bear_01", "bear_02","bear_03","bear_down","bear_up",
										 "jump_lv1_01","jump_lv1_02","jump_lv1_03","jump_lv2_01","jump_lv2_02","jump_lv2_03","jump_lv3_01","jump_lv3_02","jump_lv3_03",
										 "over","pose_1","pose_2","start","tx_over","tx_start", "null", "jump_lv3_04","jump_lv3_05"};
	private static int[] m_AnimationBeatGroup = {1,1,1,1,1,1,1,
												 2,2,2,1,2,2,2,2,2,
												 1,1,1,1,1,1,1,2,2};

	public static string[] AnimationGroup {
		get { return m_AnimationGroup; }
	}

	private const int HeadAttack = 0;
	private const int HipAttack = 1;
	private const int GettingUp1 = 2;
	private const int GettingUp2 = 3;
	private const int GettingUp3 = 4;
	private const int KnockDown = 5;
	private const int BeatUp = 6;
	private const int Jump_Lv1_01 = 7;
	private const int Jump_Lv1_02 = 8;
	private const int Jump_Lv1_03 = 9;
	private const int Jump_Lv2_01 = 10;
	private const int Jump_Lv2_02 = 11;
	private const int Jump_Lv2_03 = 12;
	private const int Jump_Lv3_01 = 13;
	private const int Jump_Lv3_02 = 14;
	private const int Jump_Lv3_03 = 15;
	private const int OverKillBeaten = 16;
	private const int DancingPose1 = 17;
	private const int DancingPose2 = 18;
	private const int Start = 19;
	private const int TX_Over = 20;
	private const int TX_Start = 21;
	private const int Freeze = 22;
    private const int Jump_Lv3_04 = 23;
    private const int Jump_Lv3_05 = 24;
	#endregion
	bool m_StartDance;
	float m_Timer;
	float m_TotalTimer;
	int m_CurrentBeatNumber;
	DanceSection m_CurrentDanceSection;


	public void Init (float bpm)
	{
		if (bpm < 0.4f)
		{
			m_BPM = bpm * 2;
		}
		else
		{
			m_BPM = bpm;
		}

		m_ChoreographerData = new ChoreographerData ();
		CalcBeatNumber ();
		m_SubTitle.text = "";
		InitAllComponents ();
		m_StartDance = false;
		m_Tornado.gameObject.SetActive (false);
	}

	public void ShowStage ()
	{
		transform.position = new Vector3 (0, -15.8f, 0);
		m_CenterLight.gameObject.SetActive (false);
		SetStageTitle ();
		StartCoroutine (StageComeUp ());
	}
	public void SetStageTitle ()
	{
		if (m_ChoreographerData.ChampionStage)
		{
			//是冠军舞台
			stageKing.gameObject.SetActive (true);
			stageTitle.sprite = LanguageManager.languageType == LanguageType.Chinese ? spriteArray[0] : spriteArray[1];
		}
		else
		{
			stageKing.gameObject.SetActive (false);
			stageTitle.sprite = LanguageManager.languageType == LanguageType.Chinese ? spriteArray[2] : spriteArray[3];
		}
	}

    private AudioObject m_Music;
	public void StartDance ()
	{
		DanceStartEvent.Invoke ();
		DanceBeat.AddListener (ProcessKOByBeat);

		if (m_Debug)
			m_AudioSource.Play ();
		else
        {
            m_Music = AudioController.PlayMusic("StageMusic");
            m_Music.audioItem.Loop = AudioItem.LoopMode.LoopSequence;
        }
			
		m_AllDancer = new Dancer[3];
		m_AllDancer[0] = m_MiddleDancer;
		m_AllDancer[1] = m_RightDancer;
		m_AllDancer[2] = m_LeftDancer;

		m_StartDance = true;
		m_Timer = 0;
		m_CurrentBeatNumber = 0;
		m_CurrentDanceSection = DanceSection.SlowSection;
		ChangeDanceSectionEvent.Invoke (m_CurrentDanceSection);
		m_TotalTimer = 0;
	}

	private const float StageComeUpDuration = 4f;
	IEnumerator StageComeUp ()
	{
		transform.DOMoveY (0, StageComeUpDuration);
		yield return new WaitForSeconds (StageComeUpDuration);
		StartDance ();
	}

	public void Restart ()
	{
		foreach (Dancer dancer in m_AllDancer)
		{
			dancer.Restart ();
		}
		CreateDancerActionData ();
		if (m_Debug)
		{
			m_AudioSource.Stop ();
			m_AudioSource.Play ();
		}
		StartDance ();
	}

	#region init
	Dancer m_MiddleDancer;
	Dancer m_LeftDancer;
	Dancer m_RightDancer;
	void InitAllComponents ()
	{
		m_AllStageAnimation = GetComponentsInChildren<StageAnimationBase> ();

		foreach (StageAnimationBase sb in m_AllStageAnimation)
		{
			sb.m_BPM = m_BPM;
			sb.Init ();
		}
	}

	public void SetChampion (bool championStage)
	{
		m_ChoreographerData.ChampionStage = championStage;


	}

	public enum DancerPos { Middle, Left, Right }
	public void CreateDancer (DancerInfo dancer, DancerPos pos)
	{
		Transform circleParent = transform.Find ("MiddleCircle"); ;

		GameObject obj = Instantiate (Resources.Load<GameObject> ("RoleCreate/Role/" + dancer.ModelID));
		SkeletonAnimation sa = obj.GetComponent<SkeletonAnimation> ();
		sa.skeleton.SetSkin (dancer.FaceID);
		obj.transform.localScale = Vector3.one * 0.5f;

		obj.transform.SetParent (circleParent);
		obj.AddComponent<BeanCollider> ();

		RoleModelClick click = obj.GetComponent<RoleModelClick> ();
		if (click != null)
			Destroy (click);

		switch (pos)
		{
			case DancerPos.Left:
				m_LeftDancer = obj.AddComponent<Dancer> ();
				m_LeftDancer.Init (m_BPM);
				m_LeftFollower.SkeletonRenderer = sa;
				m_LeftFollower.SetBone ("point");
				obj.transform.position = m_LeftOriginPoint.transform.position;
				m_LeftTitle.Init (dancer.Name, dancer.m_Country, m_LeftFollower.gameObject);
				m_ChoreographerData.m_LeftDancer = dancer;
				break;
			case DancerPos.Right:
				m_RightDancer = obj.AddComponent<Dancer> ();
				m_RightDancer.Init (m_BPM);
				m_RightFollower.SkeletonRenderer = sa;
				m_RightFollower.SetBone ("point");
				obj.transform.position = m_RightOriginPoint.transform.position;
				m_RightTitle.Init (dancer.Name, dancer.m_Country, m_RightFollower.gameObject);
				m_ChoreographerData.m_RightDancer = dancer;
				break;
			case DancerPos.Middle:
				m_MiddleDancer = obj.AddComponent<Dancer> ();
				m_MiddleDancer.Init (m_BPM);
				m_MiddleFollower.SkeletonRenderer = sa;
				m_MiddleFollower.SetBone ("point");
				obj.transform.position = m_MiddleOriginPoint.transform.position;
				m_MiddleTitle.Init (dancer.Name, dancer.m_Country, m_MiddleFollower.gameObject);
				m_ChoreographerData.m_MiddleDancer = dancer;
				break;
		}
	}


	public void ClearDancers ()
	{
		Destroy (m_MiddleDancer.gameObject);
		Destroy (m_RightDancer.gameObject);
		Destroy (m_LeftDancer.gameObject);
		m_MiddleFollower.SkeletonRenderer = null;
		m_RightFollower.SkeletonRenderer = null;
		m_LeftFollower.SkeletonRenderer = null;
		m_ChoreographerData = null;
	}


	int[] m_SlowDanceGroup = { Jump_Lv1_01, Jump_Lv1_02, Jump_Lv1_03 };
	int[] m_MediumDanceGroup = { Jump_Lv2_01, Jump_Lv2_02, Jump_Lv2_03, DancingPose1, DancingPose2 };
	int[] m_FeverDanceGroup = { Jump_Lv3_01, Jump_Lv3_02, Jump_Lv3_03, Jump_Lv3_04, Jump_Lv3_05};
	private const int MiddleWithLeft = 0;
	private const int MiddleWithRight = 1;
	private const int RightWithLeft = 2;
	private const int SectionNumber = 3;
	private const int SlowDanceSection = 0;
	private const int MediumDanceSection = 1;
	private const int FeverDanceSection = 2;
	private const float BadValve = 0.6f;
	private const float GoodValve = 0.8f;
	public void CreateDancerActionData ()
	{
		bool hasCrash = false;
		for (int j = 0; j < SectionNumber; j++)
		{
			hasCrash = false;
			for (int i = 0; i < DanceBeatNumberOnThird; i += 2)
			{
				if (!hasCrash && (m_MiddleDancer.m_CurrentEditBeatIndex < (i + j * DanceBeatNumberOnThird) &&
								  m_LeftDancer.m_CurrentEditBeatIndex < (i + j * DanceBeatNumberOnThird) &&
								  m_RightDancer.m_CurrentEditBeatIndex < (i + j * DanceBeatNumberOnThird)))
				{
					int temp = 1;
					if (j == 1)
					{
						temp = Random.Range (0, 10);
					}
					else if (j == 2)
					{
						temp = Random.Range (0, 5);
					}

					if (temp == 0)
					{
						int crashBeatNumber = i + j * DanceBeatNumberOnThird;
						if (crashBeatNumber < MinStartSpellIndex)
						{
							m_ChoreographerData.StartSpellIndex = MinStartSpellIndex;
						}
						else
						{
							m_ChoreographerData.StartSpellIndex = crashBeatNumber + 6;
						}

						hasCrash = true;
						int type = Random.Range (0, 3);
						LogManager.Log ("There wil be a crash at beat :" + (i + DanceBeatNumberOnThird * j).ToString () + "..and the type is :" + type);
						switch (type)
						{
							case MiddleWithLeft:
								if (m_ChoreographerData.m_MiddleDancer.ClickScore >= m_ChoreographerData.m_LeftDancer.ClickScore)
								{
									DanceActionData attackData = new DanceActionData ("", 0, crashBeatNumber);
									attackData.m_AnimationType = DanceActionData.AnimationType.Attack;
									attackData.m_AttackDir = DanceActionData.AttackDir.FromMiddleToLeft;
									DanceActionData beatenData = new DanceActionData ("", 0, crashBeatNumber);
									beatenData.m_AnimationType = DanceActionData.AnimationType.Beaten;
									beatenData.m_AttackDir = DanceActionData.AttackDir.FromMiddleToLeft;
									m_MiddleDancer.m_CurrentEditBeatIndex += 6;
									m_LeftDancer.m_CurrentEditBeatIndex += 6;
									m_LeftDancer.DanceActionList.Add (beatenData);
									m_MiddleDancer.DanceActionList.Add (attackData);
									//ProcessCrash (m_MiddleDancer, m_LeftDancer, m_MiddleOriginPoint.transform.position, m_LeftOriginPoint.transform.position,
									//false, true, crashBeatNumber);
								}
								else
								{
									DanceActionData attackData = new DanceActionData ("", 0, crashBeatNumber);
									attackData.m_AnimationType = DanceActionData.AnimationType.Attack;
									attackData.m_AttackDir = DanceActionData.AttackDir.FromLeftToMiddle;
									DanceActionData beatenData = new DanceActionData ("", 0, crashBeatNumber);
									beatenData.m_AnimationType = DanceActionData.AnimationType.Beaten;
									beatenData.m_AttackDir = DanceActionData.AttackDir.FromLeftToMiddle;
									m_LeftDancer.m_CurrentEditBeatIndex += 6;
									m_MiddleDancer.m_CurrentEditBeatIndex += 6;
									m_LeftDancer.DanceActionList.Add (attackData);
									m_MiddleDancer.DanceActionList.Add (beatenData);
									//ProcessCrash (m_LeftDancer, m_MiddleDancer, m_LeftOriginPoint.transform.position, m_MiddleOriginPoint.transform.position,
									//true, false, crashBeatNumber);
								}
								break;
							case MiddleWithRight:
								if (m_ChoreographerData.m_MiddleDancer.ClickScore >= m_ChoreographerData.m_RightDancer.ClickScore)
								{
									DanceActionData attackData = new DanceActionData ("", 0, crashBeatNumber);
									attackData.m_AnimationType = DanceActionData.AnimationType.Attack;
									attackData.m_AttackDir = DanceActionData.AttackDir.FromMiddleToRight;
									DanceActionData beatenData = new DanceActionData ("", 0, crashBeatNumber);
									beatenData.m_AnimationType = DanceActionData.AnimationType.Beaten;
									beatenData.m_AttackDir = DanceActionData.AttackDir.FromMiddleToRight;
									m_MiddleDancer.m_CurrentEditBeatIndex += 6;
									m_RightDancer.m_CurrentEditBeatIndex += 6;
									m_RightDancer.DanceActionList.Add (beatenData);
									m_MiddleDancer.DanceActionList.Add (attackData);
									//ProcessCrash (m_MiddleDancer, m_RightDancer, m_MiddleOriginPoint.transform.position, m_RightOriginPoint.transform.position,
									//true, false, crashBeatNumber);
								}
								else
								{
									DanceActionData attackData = new DanceActionData ("", 0, crashBeatNumber);
									attackData.m_AnimationType = DanceActionData.AnimationType.Attack;
									attackData.m_AttackDir = DanceActionData.AttackDir.FromRightToMiddle;
									DanceActionData beatenData = new DanceActionData ("", 0, crashBeatNumber);
									beatenData.m_AnimationType = DanceActionData.AnimationType.Beaten;
									beatenData.m_AttackDir = DanceActionData.AttackDir.FromRightToMiddle;
									m_RightDancer.m_CurrentEditBeatIndex += 6;
									m_MiddleDancer.m_CurrentEditBeatIndex += 6;
									m_RightDancer.DanceActionList.Add (attackData);
									m_MiddleDancer.DanceActionList.Add (beatenData);
									//ProcessCrash (m_RightDancer, m_MiddleDancer, m_RightOriginPoint.transform.position, m_MiddleOriginPoint.transform.position,
									//false, true, crashBeatNumber);
								}
								break;
							case RightWithLeft:
								if (m_ChoreographerData.m_RightDancer.ClickScore >= m_ChoreographerData.m_LeftDancer.ClickScore)
								{
									DanceActionData attackData = new DanceActionData ("", 0, crashBeatNumber);
									attackData.m_AnimationType = DanceActionData.AnimationType.Attack;
									attackData.m_AttackDir = DanceActionData.AttackDir.FromRightToLeft;
									DanceActionData beatenData = new DanceActionData ("", 0, crashBeatNumber);
									beatenData.m_AnimationType = DanceActionData.AnimationType.Beaten;
									beatenData.m_AttackDir = DanceActionData.AttackDir.FromRightToLeft;
									m_RightDancer.m_CurrentEditBeatIndex += 6;
									m_LeftDancer.m_CurrentEditBeatIndex += 6;
									m_LeftDancer.DanceActionList.Add (beatenData);
									m_RightDancer.DanceActionList.Add (attackData);
									//ProcessCrash (m_RightDancer, m_LeftDancer, m_RightOriginPoint.transform.position, m_LeftOriginPoint.transform.position,
									//false, true, crashBeatNumber);
								}
								else
								{
									DanceActionData attackData = new DanceActionData ("", 0, crashBeatNumber);
									attackData.m_AnimationType = DanceActionData.AnimationType.Attack;
									attackData.m_AttackDir = DanceActionData.AttackDir.FromLeftToRight;
									DanceActionData beatenData = new DanceActionData ("", 0, crashBeatNumber);
									beatenData.m_AnimationType = DanceActionData.AnimationType.Beaten;
									beatenData.m_AttackDir = DanceActionData.AttackDir.FromLeftToRight;
									m_LeftDancer.m_CurrentEditBeatIndex += 6;
									m_RightDancer.m_CurrentEditBeatIndex += 6;
									m_LeftDancer.DanceActionList.Add (attackData);
									m_RightDancer.DanceActionList.Add (beatenData);
									//ProcessCrash (m_LeftDancer, m_RightDancer, m_LeftOriginPoint.transform.position, m_RightOriginPoint.transform.position,
									//true, false, crashBeatNumber);
								}
								break;
						}
					}
				}

				if (m_MiddleDancer.m_CurrentEditBeatIndex < (i + j * DanceBeatNumberOnThird) || m_MiddleDancer.m_CurrentEditBeatIndex == 0)
				{
					int animationID = 0;
					switch (j)
					{
						case SlowDanceSection:
							if (m_ChoreographerData.m_MiddleDancer.ClickScorePercent < BadValve)
							{
								animationID = GetRandomAnimation (SlowDanceSection, Bad);
							}
							else if (m_ChoreographerData.m_MiddleDancer.ClickScorePercent < GoodValve)
							{
								animationID = GetRandomAnimation (SlowDanceSection, Good);
							}
							else
							{
								animationID = GetRandomAnimation (SlowDanceSection, Perfect);
							}
							break;
						case MediumDance:
							if (m_ChoreographerData.m_MiddleDancer.VoiceScorePercent < BadValve)
							{
								animationID = GetRandomAnimation (MediumDance, Bad);
							}
							else if (m_ChoreographerData.m_MiddleDancer.VoiceScorePercent < GoodValve)
							{
								animationID = GetRandomAnimation (MediumDance, Good);
							}
							else
							{
								animationID = GetRandomAnimation (MediumDance, Perfect);
							}
							break;
						case FeverDance:
							if (m_ChoreographerData.m_MiddleDancer.WholeRankingPercent < BadValve)
							{
								animationID = GetRandomAnimation (FeverDance, Bad);
							}
							else if (m_ChoreographerData.m_MiddleDancer.WholeRankingPercent < GoodValve)
							{
								animationID = GetRandomAnimation (FeverDance, Good);
							}
							else
							{
								animationID = GetRandomAnimation (FeverDance, Perfect);
							}
							break;
					}


					DanceActionData data = new DanceActionData (m_AnimationGroup[animationID], animationID, i + j * DanceBeatNumberOnThird);
					data.AnimationBeatCount = m_AnimationBeatGroup[animationID];
					m_MiddleDancer.m_CurrentEditBeatIndex = i + j * DanceBeatNumberOnThird;
					m_MiddleDancer.DanceActionList.Add (data);
				}

				if (m_LeftDancer.m_CurrentEditBeatIndex < (i + j * DanceBeatNumberOnThird) || m_LeftDancer.m_CurrentEditBeatIndex == 0)
				{
					int animationID = 0;
					switch (j)
					{
						case SlowDanceSection:
							if (m_ChoreographerData.m_LeftDancer.ClickScorePercent < BadValve)
							{
								animationID = GetRandomAnimation (SlowDanceSection, Bad);
							}
							else if (m_ChoreographerData.m_LeftDancer.ClickScorePercent < GoodValve)
							{
								animationID = GetRandomAnimation (SlowDanceSection, Good);
							}
							else
							{
								animationID = GetRandomAnimation (SlowDanceSection, Perfect);
							}
							break;
						case MediumDance:
							if (m_ChoreographerData.m_LeftDancer.VoiceScorePercent < BadValve)
							{
								animationID = GetRandomAnimation (MediumDance, Bad);
							}
							else if (m_ChoreographerData.m_LeftDancer.VoiceScorePercent < GoodValve)
							{
								animationID = GetRandomAnimation (MediumDance, Good);
							}
							else
							{
								animationID = GetRandomAnimation (MediumDance, Perfect);
							}
							break;
						case FeverDance:
							if (m_ChoreographerData.m_LeftDancer.WholeRankingPercent < BadValve)
							{
								animationID = GetRandomAnimation (FeverDance, Bad);
							}
							else if (m_ChoreographerData.m_LeftDancer.WholeRankingPercent < GoodValve)
							{
								animationID = GetRandomAnimation (FeverDance, Good);
							}
							else
							{
								animationID = GetRandomAnimation (FeverDance, Perfect);
							}
							break;
					}

					DanceActionData data = new DanceActionData (m_AnimationGroup[animationID], animationID, i + j * DanceBeatNumberOnThird);
					data.AnimationBeatCount = m_AnimationBeatGroup[animationID];
					m_LeftDancer.m_CurrentEditBeatIndex = i + j * DanceBeatNumberOnThird;
					m_LeftDancer.DanceActionList.Add (data);
				}

				if (m_RightDancer.m_CurrentEditBeatIndex < (i + j * DanceBeatNumberOnThird) || m_RightDancer.m_CurrentEditBeatIndex == 0)
				{
					int animationID = 0;
					switch (j)
					{
						case SlowDanceSection:
							if (m_ChoreographerData.m_RightDancer.ClickScorePercent < BadValve)
							{
								animationID = GetRandomAnimation (SlowDanceSection, Bad);
							}
							else if (m_ChoreographerData.m_RightDancer.ClickScorePercent < GoodValve)
							{
								animationID = GetRandomAnimation (SlowDanceSection, Good);
							}
							else
							{
								animationID = GetRandomAnimation (SlowDanceSection, Perfect);
							}
							break;
						case MediumDance:
							if (m_ChoreographerData.m_RightDancer.VoiceScorePercent < BadValve)
							{
								animationID = GetRandomAnimation (MediumDance, Bad);
							}
							else if (m_ChoreographerData.m_RightDancer.VoiceScorePercent < GoodValve)
							{
								animationID = GetRandomAnimation (MediumDance, Good);
							}
							else
							{
								animationID = GetRandomAnimation (MediumDance, Perfect);
							}
							break;
						case FeverDance:
							if (m_ChoreographerData.m_RightDancer.WholeRankingPercent < BadValve)
							{
								animationID = GetRandomAnimation (FeverDance, Bad);
							}
							else if (m_ChoreographerData.m_RightDancer.WholeRankingPercent < GoodValve)
							{
								animationID = GetRandomAnimation (FeverDance, Good);
							}
							else
							{
								animationID = GetRandomAnimation (FeverDance, Perfect);
							}
							break;
					}

					DanceActionData data = new DanceActionData (m_AnimationGroup[animationID], animationID, i + j * DanceBeatNumberOnThird);
					data.AnimationBeatCount = m_AnimationBeatGroup[animationID];
					m_RightDancer.m_CurrentEditBeatIndex = i + j * DanceBeatNumberOnThird;
					m_RightDancer.DanceActionList.Add (data);
				}
			}
		}

		ProcessKO ();
	}

	public void CreateDataFromWholeChoreographer (ChoreographerData data)
	{
		m_ChoreographerData = data;

		CreateDancer (data.m_MiddleDancer, DancerPos.Middle);
		CreateDancer (data.m_LeftDancer, DancerPos.Left);
		CreateDancer (data.m_RightDancer, DancerPos.Right);

		m_MiddleDancer.DanceActionList = data.m_MiddleDancer.m_DanceActionList;
		m_LeftDancer.DanceActionList = data.m_LeftDancer.m_DanceActionList;
		m_RightDancer.DanceActionList = data.m_RightDancer.m_DanceActionList;
	}

	private const float CrashHeight = 1.95f;
	private const float BeatenLowestHeight = -0.3f;
	private const float BeatenLength = 0.5f;
	//private const string OverKillAnimationName = "attack3";
	//private const string OverKillBeatenAnimationName = "over";
	private const int OverKillBeatLength = 10;
	private int m_WinningPos;//0 = Middle, 1 = Left, 2 = Right;
	private const int MinStartSpellIndex = 28;
	void ProcessKO ()
	{
		if (m_ChoreographerData.StartSpellIndex < MinStartSpellIndex)
		{
			m_ChoreographerData.StartSpellIndex = MinStartSpellIndex;
		}


		if (m_ChoreographerData.m_MiddleDancer.m_RankingType == DancerInfo.RankingType.Winner)
		{
			m_WinningPos = Middle;

			MiddleWinProcess ();
		}
		else if (m_ChoreographerData.m_LeftDancer.m_RankingType == DancerInfo.RankingType.Winner)
		{
			m_WinningPos = Left;
			LeftWinProcess ();
		}
		else if (m_ChoreographerData.m_RightDancer.m_RankingType == DancerInfo.RankingType.Winner)
		{
			m_WinningPos = Right;
			RightWinProcess ();
		}
		else
		{
			if (m_ChoreographerData.m_LeftDancer.WholeScore > m_ChoreographerData.m_RightDancer.WholeScore)
			{
				m_WinningPos = Left;
				LeftWinProcess ();
			}
			else
			{
				m_WinningPos = Right;
				RightWinProcess ();
			}
		}

		m_ChoreographerData.m_MiddleDancer.m_DanceActionList = m_MiddleDancer.DanceActionList;
		m_ChoreographerData.m_LeftDancer.m_DanceActionList = m_LeftDancer.DanceActionList;
		m_ChoreographerData.m_RightDancer.m_DanceActionList = m_RightDancer.DanceActionList;

		UploadDanceActionData ();
	}

	void MiddleWinProcess ()
	{
		DanceActionData flyToMiddle = new DanceActionData (m_AnimationGroup[Start], Start, m_ChoreographerData.StartSpellIndex + 8);
		flyToMiddle.MoveToTarget = true;
		flyToMiddle.Loop = false;
		flyToMiddle.TargetPos = new Vector3 (m_MiddleOriginPoint.transform.localPosition.x, CrashHeight, 0);
		DanceActionData flyToLeft = new DanceActionData (m_AnimationGroup[Start], Start, m_ChoreographerData.StartSpellIndex + 8);
		flyToLeft.Loop = false;
		flyToLeft.MoveToTarget = true;
		flyToLeft.TargetPos = flyToMiddle.TargetPos + Vector3.left;
		DanceActionData flyToRight = new DanceActionData (m_AnimationGroup[Start], Start, m_ChoreographerData.StartSpellIndex + 8);
		flyToRight.Loop = false;
		flyToRight.MoveToTarget = true;
		flyToRight.TargetPos = flyToMiddle.TargetPos + Vector3.right;


		DanceActionData attack = new DanceActionData (m_AnimationGroup[TX_Start], TX_Start, m_ChoreographerData.StartSpellIndex + 9);
		attack.Loop = false;
		DanceActionData beatenLeft = new DanceActionData (m_AnimationGroup[OverKillBeaten], OverKillBeaten, m_ChoreographerData.StartSpellIndex + 9);
		beatenLeft.FlipXAxis = true;
		DanceActionData beatenRight = new DanceActionData (m_AnimationGroup[OverKillBeaten], OverKillBeaten, m_ChoreographerData.StartSpellIndex + 9);

		DanceActionData winBack = new DanceActionData (m_AnimationGroup[Start], Start, m_ChoreographerData.StartSpellIndex + 13);
		winBack.MoveToTarget = true;
		winBack.TargetPos = m_MiddleOriginPoint.transform.localPosition;
		winBack.Loop = false;
		DanceActionData knockLeft = new DanceActionData (m_AnimationGroup[KnockDown], KnockDown, m_ChoreographerData.StartSpellIndex + 13);
		knockLeft.Loop = false;
		knockLeft.MoveToTarget = true;
		knockLeft.TargetPos = m_LeftOriginPoint.transform.localPosition + Vector3.left * BeatenLength;
		knockLeft.NumberOfJumps = 3;
		knockLeft.JumpPower = 3;
		knockLeft.FlipXAxis = true;
		DanceActionData knockRight = new DanceActionData (m_AnimationGroup[KnockDown], KnockDown, m_ChoreographerData.StartSpellIndex + 13);
		knockRight.Loop = false;
		knockRight.MoveToTarget = true;
		knockRight.TargetPos = m_RightOriginPoint.transform.localPosition + Vector3.right * BeatenLength;
		knockRight.JumpPower = 3;
		knockRight.NumberOfJumps = 3;

		DanceActionData winPose = new DanceActionData (m_AnimationGroup[DancingPose2], DancingPose2, m_ChoreographerData.StartSpellIndex + 14);

		m_MiddleDancer.DanceActionList.Add (flyToMiddle);
		m_MiddleDancer.DanceActionList.Add (attack);
		m_MiddleDancer.DanceActionList.Add (winBack);
		m_MiddleDancer.DanceActionList.Add (winPose);

		m_RightDancer.DanceActionList.Add (flyToRight);
		m_RightDancer.DanceActionList.Add (beatenRight);
		m_RightDancer.DanceActionList.Add (knockRight);

		m_LeftDancer.DanceActionList.Add (flyToLeft);
		m_LeftDancer.DanceActionList.Add (beatenLeft);
		m_LeftDancer.DanceActionList.Add (knockLeft);
	}

	void LeftWinProcess ()
	{
		DanceActionData flyToMiddle = new DanceActionData (m_AnimationGroup[Start], Start, m_ChoreographerData.StartSpellIndex + 8);
		flyToMiddle.MoveToTarget = true;
		flyToMiddle.Loop = false;
		flyToMiddle.TargetPos = new Vector3 (m_MiddleOriginPoint.transform.localPosition.x, CrashHeight, 0);
		DanceActionData flyToLeft = new DanceActionData (m_AnimationGroup[Start], Start, m_ChoreographerData.StartSpellIndex + 8);
		flyToLeft.Loop = false;
		flyToLeft.MoveToTarget = true;
		flyToLeft.TargetPos = flyToMiddle.TargetPos + Vector3.left;
		DanceActionData flyToRight = new DanceActionData (m_AnimationGroup[Start], Start, m_ChoreographerData.StartSpellIndex + 8);
		flyToRight.Loop = false;
		flyToRight.MoveToTarget = true;
		flyToRight.TargetPos = flyToMiddle.TargetPos + Vector3.right;


		DanceActionData attack = new DanceActionData (m_AnimationGroup[TX_Start], TX_Start, m_ChoreographerData.StartSpellIndex + 9);
		attack.Loop = false;
		DanceActionData beatenMiddle = new DanceActionData (m_AnimationGroup[OverKillBeaten], OverKillBeaten, m_ChoreographerData.StartSpellIndex + 9);
		DanceActionData beatenRight = new DanceActionData (m_AnimationGroup[OverKillBeaten], OverKillBeaten, m_ChoreographerData.StartSpellIndex + 9);

		DanceActionData winBack = new DanceActionData (m_AnimationGroup[Start], Start, m_ChoreographerData.StartSpellIndex + 13);
		winBack.MoveToTarget = true;
		winBack.TargetPos = m_LeftOriginPoint.transform.localPosition;
		winBack.Loop = false;
		DanceActionData knockMiddle = new DanceActionData (m_AnimationGroup[KnockDown], KnockDown, m_ChoreographerData.StartSpellIndex + 13);
		knockMiddle.Loop = false;
		knockMiddle.MoveToTarget = true;
		knockMiddle.TargetPos = m_MiddleOriginPoint.transform.localPosition + Vector3.right * BeatenLength;
		knockMiddle.NumberOfJumps = 3;
		knockMiddle.JumpPower = 3;
		DanceActionData knockRight = new DanceActionData (m_AnimationGroup[KnockDown], KnockDown, m_ChoreographerData.StartSpellIndex + 13);
		knockRight.Loop = false;
		knockRight.MoveToTarget = true;
		knockRight.TargetPos = m_RightOriginPoint.transform.localPosition + Vector3.right * BeatenLength;
		knockRight.JumpPower = 3;
		knockRight.NumberOfJumps = 3;

		DanceActionData winPose = new DanceActionData (m_AnimationGroup[DancingPose2], DancingPose2, m_ChoreographerData.StartSpellIndex + 14);

		m_LeftDancer.DanceActionList.Add (flyToLeft);
		m_LeftDancer.DanceActionList.Add (attack);
		m_LeftDancer.DanceActionList.Add (winBack);
		m_LeftDancer.DanceActionList.Add (winPose);

		m_RightDancer.DanceActionList.Add (flyToRight);
		m_RightDancer.DanceActionList.Add (beatenRight);
		m_RightDancer.DanceActionList.Add (knockRight);

		m_MiddleDancer.DanceActionList.Add (flyToMiddle);
		m_MiddleDancer.DanceActionList.Add (beatenMiddle);
		m_MiddleDancer.DanceActionList.Add (knockMiddle);
	}

	void RightWinProcess ()
	{
		DanceActionData flyToMiddle = new DanceActionData (m_AnimationGroup[Start], Start, m_ChoreographerData.StartSpellIndex + 8);
		flyToMiddle.MoveToTarget = true;
		flyToMiddle.Loop = false;
		flyToMiddle.TargetPos = new Vector3 (m_MiddleOriginPoint.transform.localPosition.x, CrashHeight, 0);
		DanceActionData flyToLeft = new DanceActionData (m_AnimationGroup[Start], Start, m_ChoreographerData.StartSpellIndex + 8);
		flyToLeft.Loop = false;
		flyToLeft.MoveToTarget = true;
		flyToLeft.TargetPos = flyToMiddle.TargetPos + Vector3.left;
		DanceActionData flyToRight = new DanceActionData (m_AnimationGroup[Start], Start, m_ChoreographerData.StartSpellIndex + 8);
		flyToRight.Loop = false;
		flyToRight.MoveToTarget = true;
		flyToRight.TargetPos = flyToMiddle.TargetPos + Vector3.right;


		DanceActionData attack = new DanceActionData (m_AnimationGroup[TX_Start], TX_Start, m_ChoreographerData.StartSpellIndex + 9);
		attack.Loop = false;
		DanceActionData beatenMiddle = new DanceActionData (m_AnimationGroup[OverKillBeaten], OverKillBeaten, m_ChoreographerData.StartSpellIndex + 9);
		beatenMiddle.FlipXAxis = true;
		DanceActionData beatenLeft = new DanceActionData (m_AnimationGroup[OverKillBeaten], OverKillBeaten, m_ChoreographerData.StartSpellIndex + 9);
		beatenLeft.FlipXAxis = true;

		DanceActionData winBack = new DanceActionData (m_AnimationGroup[Start], Start, m_ChoreographerData.StartSpellIndex + 13);
		winBack.MoveToTarget = true;
		winBack.TargetPos = m_RightOriginPoint.transform.localPosition;
		winBack.Loop = false;
		DanceActionData knockMiddle = new DanceActionData (m_AnimationGroup[KnockDown], KnockDown, m_ChoreographerData.StartSpellIndex + 13);
		knockMiddle.Loop = false;
		knockMiddle.MoveToTarget = true;
		knockMiddle.TargetPos = m_MiddleOriginPoint.transform.localPosition + Vector3.left * BeatenLength;
		knockMiddle.NumberOfJumps = 3;
		knockMiddle.JumpPower = 3;
		knockMiddle.FlipXAxis = true;
		DanceActionData knockLeft = new DanceActionData (m_AnimationGroup[KnockDown], KnockDown, m_ChoreographerData.StartSpellIndex + 13);
		knockLeft.Loop = false;
		knockLeft.MoveToTarget = true;
		knockLeft.TargetPos = m_LeftOriginPoint.transform.localPosition + Vector3.left * BeatenLength;
		knockLeft.JumpPower = 3;
		knockLeft.NumberOfJumps = 3;
		knockLeft.FlipXAxis = true;

		DanceActionData winPose = new DanceActionData (m_AnimationGroup[DancingPose2], DancingPose2, m_ChoreographerData.StartSpellIndex + 14);

		m_RightDancer.DanceActionList.Add (flyToRight);
		m_RightDancer.DanceActionList.Add (attack);
		m_RightDancer.DanceActionList.Add (winBack);
		m_RightDancer.DanceActionList.Add (winPose);

		m_LeftDancer.DanceActionList.Add (flyToLeft);
		m_LeftDancer.DanceActionList.Add (beatenLeft);
		m_LeftDancer.DanceActionList.Add (knockLeft);

		m_MiddleDancer.DanceActionList.Add (flyToMiddle);
		m_MiddleDancer.DanceActionList.Add (beatenMiddle);
		m_MiddleDancer.DanceActionList.Add (knockMiddle);

	}

	void UploadDanceActionData ()
	{
		LitJson.JsonData data = new LitJson.JsonData ();
		data["historyID"] = SelfPlayerLevelData.TempHistoryId;
		data["dance"] = m_ChoreographerData.ToJson ();
		DancingWordAPI.Instance.SubmitDancDataToServer (data, UploadCallback, null);
	}

	void UploadCallback (string content)
	{

	}

	private const int Middle = 0;
	private const int Left = 1;
	private const int Right = 2;
	void PlayTornadoAnimation ()
	{
		switch (m_WinningPos)
		{
			case Middle:
				m_MiddleDancer.gameObject.SetActive (false);
				m_Tornado.transform.position = m_MiddleDancer.transform.position;
				break;
			case Left:
				m_LeftDancer.gameObject.SetActive (false);
				m_Tornado.transform.position = m_LeftDancer.transform.position;
				break;
			case Right:
				m_RightDancer.gameObject.SetActive (false);
				m_Tornado.transform.position = m_RightDancer.transform.position;
				break;
		}

		m_Tornado.gameObject.SetActive (true);
		Spine.TrackEntry entry = m_Tornado.AnimationState.SetAnimation (0, "tx_ljf", true);
		entry.Loop = true;
		float animationLength = entry.Animation.Duration;
		float speed = animationLength / m_BPM;
		entry.TimeScale = speed;
	}

	void StopTornado ()
	{
		switch (m_WinningPos)
		{
			case Middle:
				m_MiddleDancer.gameObject.SetActive (true);
				break;
			case Left:
				m_LeftDancer.gameObject.SetActive (true);
				break;
			case Right:
				m_RightDancer.gameObject.SetActive (true);
				break;
		}
		m_Tornado.gameObject.SetActive (false);
		EffectManager.Play ("MysticExplosion", m_MiddleDancer.transform.position);
	}

	private const int SlowDance = 0;
	private const int MediumDance = 1;
	private const int FeverDance = 2;
	private const int Bad = 0;
	private const int Good = 1;
	private const int Perfect = 2;
	int GetRandomAnimation (int type, int score = Good)
	{
		int animationID = 0;

		switch (type)
		{
			case SlowDance:
				switch (score)
				{
					case Bad: animationID = GetAnimationIDByOdds (80, 95); break;
					case Good: animationID = GetAnimationIDByOdds (70, 95); break;
					case Perfect: animationID = GetAnimationIDByOdds (60, 90); break;
				}
				break;
			case MediumDance:
				switch (score)
				{
					case Bad: animationID = GetAnimationIDByOdds (30, 90); break;
					case Good: animationID = GetAnimationIDByOdds (15, 85); break;
					case Perfect: animationID = GetAnimationIDByOdds (5, 80); break;
				}
				break;
			case FeverDance:
				switch (score)
				{
					case Bad: animationID = GetAnimationIDByOdds (0, 70); break;
					case Good: animationID = GetAnimationIDByOdds (0, 60); break;
					case Perfect: animationID = GetAnimationIDByOdds (0, 40); break;
				}
				break;
		}

		return animationID;
	}

	private int GetAnimationIDByOdds (int badValve, int middleValve)
	{
		int animationID;
		int range = Random.Range (0, 100);

		if (range < badValve)
		{//一级动作
			int randomIndex = Random.Range (0, m_SlowDanceGroup.Length);
			animationID = m_SlowDanceGroup[randomIndex];
		}
		else if (range >= badValve && range < middleValve)
		{//二级动作
			int randomIndex = Random.Range (0, m_MediumDanceGroup.Length);
			animationID = m_MediumDanceGroup[randomIndex];
		}
		else
		{//三级动作
			int randomIndex = Random.Range (0, m_FeverDanceGroup.Length);
			animationID = m_FeverDanceGroup[randomIndex];
		}

		return animationID;
	}

	private const float DanceTimeLength = 120f;
	private const int BeatNumberForEachSection = 12;
	void CalcBeatNumber ()
	{//跳舞分成3个部分 每个部分接近5秒钟
	 //int tempNumber = (int)(DanceTimeLength / m_BPM / 3);
	 //if(tempNumber % 2 != 0)
	 //{//保证每个部分的节拍数是偶数 ，这样比较容易计算舞蹈动作的编排
	 //    m_DanceBeatNumber = tempNumber + 1;
	 //}
	 //else
	 //{
	 //    m_DanceBeatNumber = tempNumber;
	 //}

		//m_DanceBeatNumberOnThird = m_DanceBeatNumber;
		//m_DanceBeatNumber *= 3;

		m_DanceBeatNumberOnThird = BeatNumberForEachSection;
		m_DanceBeatNumber = m_DanceBeatNumberOnThird * 3;
	}
	#endregion

	void Awake ()
	{
		if (Instance != null)
		{
			LogManager.LogError ("More Than One Stage Manager Exists!!");
			Destroy (this);
			return;
		}
		else
		{
			Instance = this;
		}

		Random.InitState ((int)(Time.realtimeSinceStartup * 1000f));
		blowEffect = Camera.main.GetComponent<BlowEffect> ();
	}

	private void OnDestroy ()
	{
		Instance = null;
		ClearAllEventsListeners ();
	}

	void ClearAllEventsListeners ()
	{
		DanceStartEvent.RemoveAllListeners ();
		DanceEndEvent.RemoveAllListeners ();
		ChangeDanceSectionEvent.RemoveAllListeners ();
		DanceBeat.RemoveAllListeners ();
	}

	// Update is called once per frame
	void Update ()
	{
		if (m_StartDance)
		{
			m_Timer += Time.deltaTime;
			m_TotalTimer += Time.deltaTime;
			if (m_Timer >= m_BPM)
			{
				m_Timer -= m_BPM;
				m_CurrentBeatNumber++;
				DanceBeat.Invoke (m_CurrentBeatNumber);
				//AudioController.Play("bpm-01");
			}

			ProcessBeatNumber ();

			if (m_TimerText != null)
				m_TimerText.text = m_TotalTimer.ToString ();
		}
	}

	void ProcessBeatNumber ()
	{
		bool allDancerFinish = true;
		foreach (Dancer dancer in m_AllDancer)
		{
			if (!dancer.Finish)
			{
				allDancerFinish = false;
				break;
			}
		}

		if (allDancerFinish)
		{
			m_StartDance = false;
			StartCoroutine (_DelayFinish ());
            if(m_Music != null)
                m_Music.volume = 0.5f;
		}
	}

	IEnumerator _DelayFinish ()
	{
		yield return new WaitForSeconds (1f);
		EffectManager.PlayRandomOnScene ("ConfettiBlast");
		yield return new WaitForSeconds (1f);
		EffectManager.PlayRandomOnScene ("ConfettiBlast");
		yield return new WaitForSeconds (1f);
		EffectManager.PlayRandomOnScene ("ConfettiBlast");

		DanceEndEvent.Invoke ();
		if (PageManager.Instance.CurrentPage != null)
		{
			//第一次进游戏舞台结束直接先到登录界面
			//if (LoginScene.isFirstGoingGame)
			//{
			//	LoadSceneManager.Instance.LoadNormalScene ("LoginScene");
			//}
			//else
			//{
			PageManager.Instance.CurrentPage.GetNode<StageOverNode> ().Open ();
			PageManager.Instance.CurrentPage.GetNode<StageOverNode> ().SetBinding (m_MiddleFollower.gameObject, m_LeftFollower.gameObject, m_RightFollower.gameObject);
			PageManager.Instance.CurrentPage.GetNode<StageOverNode> ().SetID (m_ChoreographerData.m_MiddleDancer.PlayerID,
																			m_ChoreographerData.m_LeftDancer.PlayerID,
																			m_ChoreographerData.m_RightDancer.PlayerID);
			m_StartDance = false;
			//}
		}
		else
		{
			LogManager.LogWarning (" Restart !! ");
			Restart ();
		}
	}



	void ProcessKOByBeat (int beatNumber)
	{
		if (beatNumber >= m_ChoreographerData.StartSpellIndex)
		{
			if (beatNumber >= m_ChoreographerData.StartSpellIndex && beatNumber < m_ChoreographerData.StartSpellIndex + 4)
			{
				AudioController.Play ("Beng");

				foreach (var item in m_AllDancer)
				{
					if (item.m_Pos == Dancer.DancerPos.Middle)
					{
						var bc = item.GetComponentInChildren<BeanCollider> ();
						bc.PlayBounceEffect ();
						break;
					}
				}
				int number = beatNumber - m_ChoreographerData.StartSpellIndex + 1;
				StringBuilder sb = new StringBuilder ();

				for (int i = 0; i < number; i++)
				{
					sb.Append ("蹦 ");
				}
				m_SubTitle.text = sb.ToString ();
				//var particle = EffectManager.GetPoolParticle ("MagicCharge");
				//if (particle != null) particle.Play ();
			}
			else if (beatNumber >= m_ChoreographerData.StartSpellIndex + 4 && beatNumber < m_ChoreographerData.StartSpellIndex + 8)
			{
				AudioController.Play ("Bounce");

				foreach (var item in m_AllDancer)
				{
					if (item.m_Pos == Dancer.DancerPos.Middle)
					{
						var bc = item.GetComponentInChildren<BeanCollider> ();
						bc.PlayBounceEffect ();
						break;
					}
				}

				int number = beatNumber - m_ChoreographerData.StartSpellIndex - 4 + 1;
				StringBuilder sb = new StringBuilder ();
				for (int i = 0; i < number; i++)
				{
					sb.Append ("Bounce ");
				}
				m_SubTitle.text = sb.ToString ();
				//var particle = EffectManager.GetPoolParticle ("MagicCharge");
				//if (particle != null) particle.Play ();
			}
			else if (beatNumber == m_ChoreographerData.StartSpellIndex + 8)
			{
				m_SubTitle.text = "";

			}
			else if (beatNumber == m_ChoreographerData.StartSpellIndex + 10)
			{
				PlayTornadoAnimation ();
				AudioController.Play ("Boom");
				m_SubTitle.text = "Boooooooooom!!!!";
			}
			else if (beatNumber == m_ChoreographerData.StartSpellIndex + 14)
			{
				if (blowEffect != null) blowEffect.Blow ();
				StopTornado ();
				m_SubTitle.text = "";
				//StopTornadoEvent.Invoke ();
			}
		}
	}

	#region debug


	private void OnGUI ()
	{
		if (DebugConfigController.Instance._Debug)
		{
			if (GUI.Button (new Rect (50, 50, 50, 50), "Start"))
			{
				Init (0.6316f);
				CreateDataFromWholeChoreographer (CreateFakeData ());
				CreateDancerActionData ();
				ShowStage ();
				//StartDance();
			}

			if (GUI.Button (new Rect (50, 100, 50, 50), "restart"))
			{
				transform.position = Vector3.zero;
				m_CenterLight.gameObject.SetActive (true);
				Restart ();
			}
		}
	}

	private ChoreographerData CreateFakeData ()
	{
		ChoreographerData data = new ChoreographerData ();

		data.StartSpellIndex = -1;
		data.HasPlayerSelf = true;

		DancerInfo self = new DancerInfo ();
		DancerInfo left = new DancerInfo ();
		DancerInfo right = new DancerInfo ();

		self.Name = SelfPlayerData.UserName;
		self.m_Sex = DancerInfo.Sex.Female;
		self.m_Country = DancerInfo.Country.China;
		List<string> tempCache = new List<string> ();

		self.ModelID = RoleModelConfig.Instance.GetNameById (UnityEngine.Random.Range (20001, 20009));
		self.FaceID = RoleEmotionConfig.Instance.GetNameById (UnityEngine.Random.Range (30001, 30009));
		tempCache.Add (self.ModelID);
		int songMax = 10000;
		int clickMax = 8000;
		int voiceMax = 2000;
		self.WholeScore = 6000;
		self.VoiceScore = 1000;
		self.ClickScore = self.WholeScore - self.VoiceScore;
		self.WholeRankingPercent = (float)self.WholeScore / songMax;
		self.ClickScorePercent = (float)self.ClickScore / clickMax;
		self.VoiceScorePercent = (float)self.VoiceScore / voiceMax;

		data.m_MiddleDancer = self;
		data.m_LeftDancer = CorePlayManager.GetNPCDancer (true, self.WholeScore, DancerInfo.Country.China, DancerInfo.Sex.Female, songMax, clickMax, tempCache);
		data.m_RightDancer = CorePlayManager.GetNPCDancer (true, self.WholeScore, DancerInfo.Country.America, DancerInfo.Sex.Male, songMax, clickMax, tempCache);

		return data;
	}
	#endregion
}


public enum DanceSection { SlowSection, MediumSection, FeverSection }