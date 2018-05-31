using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;
using DG.Tweening;

public class Dancer : MonoBehaviour
{
	public enum DancerPos { Middle, Left, Right }
	public DancerPos m_Pos;
	SkeletonAnimation m_SkeletonAnimation;
	bool m_StartDance;
	float m_BPM;
	int m_AlreadyDanceBeats;
	List<DanceActionData> m_DanceActionList = new List<DanceActionData> ();
	public int m_CurrentEditBeatIndex;
	public List<DanceActionData> DanceActionList {
		get { return m_DanceActionList; }
		set { m_DanceActionList = value; }
	}
	bool m_Finish;
	public bool Finish {
		get { return m_Finish; }
	}
	public void Init (float bpm)
	{
		if (m_SkeletonAnimation == null)
		{
			m_SkeletonAnimation = GetComponent<SkeletonAnimation> ();
		}

		m_BPM = bpm;

		m_AlreadyDanceBeats = 0;
		m_CurrentEditBeatIndex = 0;

		StageManager.DanceStartEvent.AddListener (StartDance);
		StageManager.DanceEndEvent.AddListener (EndDance);
		StageManager.DanceBeat.AddListener (CheckDance);
		//StageManager.StopTornadoEvent.AddListener (OnStopTornado);
	}

	public void Restart ()
	{
		StopAllCoroutines ();
		m_DanceActionList.Clear ();
		m_AlreadyDanceBeats = 0;
		m_CurrentEditBeatIndex = 0;
	}

	void StartDance ()
	{
		m_StartDance = true;
		m_Finish = false;
	}

	void EndDance ()
	{
		m_StartDance = false;
	}

	//void OnStopTornado ()
	//{
	//}

	// Use this for initialization
	void Awake ()
	{
		m_StartDance = false;
	}
	void OnDestroy ()
	{
		if (stunned != null)
		{
			stunned.transform.SetParent (EffectManager.instance.transform);
		}
	}



	void CheckDance (int beatNumber)
	{
		while (m_DanceActionList.Count > 0 && beatNumber >= m_DanceActionList[0].StartBeat)
		{
			ProcessDanceAction (m_DanceActionList[0]);

			m_DanceActionList.RemoveAt (0);
			if (m_DanceActionList.Count == 0)
			{
				m_Finish = true;
			}
		}
	}

	void ProcessDanceAction (DanceActionData data)
	{
		switch (data.m_AnimationType)
		{
			case DanceActionData.AnimationType.Attack:
				StartCoroutine (PlayAttack (data));
				break;
			case DanceActionData.AnimationType.Beaten:
				StartCoroutine (PlayBeaten (data));
				break;
			case DanceActionData.AnimationType.Dance:
				if (data.MoveToTarget)
				{
                    transform.DOLocalJump (data.TargetPos, data.JumpPower, data.NumberOfJumps, m_BPM);
				}
				if (!string.Equals (data.AnimationName, "null"))
					PlayAnimation (data.AnimationName, data.FlipXAxis, data.Loop, data.AnimationBeatCount);

				break;
		}
	}

	void PlayAnimation (string animation, bool flipx, bool loop, int beatNumber)
	{
		Spine.TrackEntry entry = m_SkeletonAnimation.AnimationState.SetAnimation (0, animation, true);
		entry.Loop = loop;
		if (flipx)
			transform.eulerAngles = new Vector3 (0, 180, 0);
		else
			transform.eulerAngles = Vector3.zero;
		float animationLength = entry.Animation.Duration;
		float speed = animationLength / m_BPM / beatNumber;
		entry.TimeScale = speed;

	}

	private const float CrashHeight = 0.23f;
	private const float BeatenLowestHeight = -0.3f;
	private const int BeatenLength = 4;
	IEnumerator PlayAttack (DanceActionData data)
	{
		Vector3 targetPos = Vector3.one;
		switch (data.m_AttackDir)
		{
			case DanceActionData.AttackDir.FromLeftToMiddle:
				targetPos = new Vector3 ((StageManager.Instance.m_LeftOriginPoint.transform.position.x +
										 StageManager.Instance.m_MiddleOriginPoint.transform.position.x) / 2 - 1,
										(StageManager.Instance.m_LeftOriginPoint.transform.position.y +
										 StageManager.Instance.m_MiddleOriginPoint.transform.position.y) / 2 + 1, 0);
				break;
			case DanceActionData.AttackDir.FromLeftToRight:
				targetPos = new Vector3 ((StageManager.Instance.m_RightOriginPoint.transform.position.x +
										 StageManager.Instance.m_LeftOriginPoint.transform.position.x) / 2 - 1, CrashHeight, 0);
				break;
			case DanceActionData.AttackDir.FromRightToLeft:
				targetPos = new Vector3 ((StageManager.Instance.m_RightOriginPoint.transform.position.x +
										 StageManager.Instance.m_LeftOriginPoint.transform.position.x) / 2 + 1, CrashHeight, 0);
				break;
			case DanceActionData.AttackDir.FromMiddleToLeft:
				targetPos = new Vector3 ((StageManager.Instance.m_LeftOriginPoint.transform.position.x +
										 StageManager.Instance.m_MiddleOriginPoint.transform.position.x) / 2 + 1,
										(StageManager.Instance.m_LeftOriginPoint.transform.position.y +
										 StageManager.Instance.m_MiddleOriginPoint.transform.position.y) / 2 - 1, 0);
				break;
			case DanceActionData.AttackDir.FromMiddleToRight:
				targetPos = new Vector3 ((StageManager.Instance.m_RightOriginPoint.transform.position.x +
										 StageManager.Instance.m_MiddleOriginPoint.transform.position.x) / 2 - 1,
										(StageManager.Instance.m_RightOriginPoint.transform.position.y +
										 StageManager.Instance.m_MiddleOriginPoint.transform.position.y) / 2 - 1, 0);
				break;
			case DanceActionData.AttackDir.FromRightToMiddle:
				targetPos = new Vector3 ((StageManager.Instance.m_RightOriginPoint.transform.position.x +
										 StageManager.Instance.m_MiddleOriginPoint.transform.position.x) / 2 + 1,
										(StageManager.Instance.m_RightOriginPoint.transform.position.y +
										 StageManager.Instance.m_MiddleOriginPoint.transform.position.y) / 2 + 1, 0);
				break;
		}
		transform.DOJump (targetPos, data.JumpPower, data.NumberOfJumps, m_BPM);


		Spine.TrackEntry moveEntry;
		switch (data.m_AttackDir)
		{
			case DanceActionData.AttackDir.FromMiddleToLeft:
			case DanceActionData.AttackDir.FromMiddleToRight:
			case DanceActionData.AttackDir.FromLeftToMiddle:
			case DanceActionData.AttackDir.FromRightToMiddle:
				moveEntry = m_SkeletonAnimation.AnimationState.SetAnimation (0, "jump_lv2_01", true);
				break;
			default:
				moveEntry = m_SkeletonAnimation.AnimationState.SetAnimation (0, "jump_lv3_01", true);
				break;
		}

		switch (data.m_AttackDir)
		{
			case DanceActionData.AttackDir.FromLeftToMiddle:
			case DanceActionData.AttackDir.FromLeftToRight:
			case DanceActionData.AttackDir.FromMiddleToRight:
				transform.eulerAngles = new Vector3 (0, 180, 0);
				break;
			default:
				transform.eulerAngles = Vector3.zero;
				break;
		}
		float animationLength = moveEntry.Animation.Duration;
		float speed = animationLength / m_BPM;
		moveEntry.timeScale = speed;

		yield return new WaitForSeconds (m_BPM);





		Spine.TrackEntry readyToAttack = m_SkeletonAnimation.AnimationState.SetAnimation (0, "pose_1", false); ;
		animationLength = readyToAttack.Animation.Duration;
		speed = animationLength / m_BPM;
		readyToAttack.timeScale = speed;

		yield return new WaitForSeconds (m_BPM * 2 - 0.3f);




		Spine.TrackEntry attackEntry = null;
		switch (data.m_AttackDir)
		{
			case DanceActionData.AttackDir.FromMiddleToLeft:
			case DanceActionData.AttackDir.FromMiddleToRight:
				attackEntry = m_SkeletonAnimation.AnimationState.SetAnimation (0, "attack_01", false);
				break;
			case DanceActionData.AttackDir.FromLeftToMiddle:
			case DanceActionData.AttackDir.FromRightToMiddle:
				attackEntry = m_SkeletonAnimation.AnimationState.SetAnimation (0, "jump_lv3_03", false);
				break;
			default:
				attackEntry = m_SkeletonAnimation.AnimationState.SetAnimation (0, "attack_02", false);
				break;
		}
		animationLength = attackEntry.Animation.Duration;
		attackEntry.timeScale = 1;

		yield return new WaitForSeconds (animationLength);






		Vector3 originPos;
		switch (data.m_AttackDir)
		{
			case DanceActionData.AttackDir.FromLeftToMiddle:
			case DanceActionData.AttackDir.FromLeftToRight:
				originPos = StageManager.Instance.m_LeftOriginPoint.transform.position;
				break;
			case DanceActionData.AttackDir.FromMiddleToRight:
			case DanceActionData.AttackDir.FromMiddleToLeft:
				originPos = StageManager.Instance.m_MiddleOriginPoint.transform.position;
				break;
			default:
				originPos = StageManager.Instance.m_RightOriginPoint.transform.position;
				break;
		}
		transform.DOJump (originPos, data.JumpPower, data.NumberOfJumps, m_BPM);

		Spine.TrackEntry overEntry = m_SkeletonAnimation.AnimationState.SetAnimation (0, "over", false);
		animationLength = overEntry.Animation.Duration;
		overEntry.timeScale = animationLength / m_BPM;

		yield return new WaitForSeconds (m_BPM);

		Spine.TrackEntry winEntry = m_SkeletonAnimation.AnimationState.SetAnimation (0, "pose_2", true);
		animationLength = winEntry.Animation.Duration;
		winEntry.timeScale = animationLength / m_BPM;
	}

	ParticleSystem stunned;
	IEnumerator PlayBeaten (DanceActionData data)
	{
		Vector3 targetPos = Vector3.one;
		switch (data.m_AttackDir)
		{
			case DanceActionData.AttackDir.FromMiddleToLeft:
				targetPos = new Vector3 ((StageManager.Instance.m_LeftOriginPoint.transform.position.x +
										 StageManager.Instance.m_MiddleOriginPoint.transform.position.x) / 2 - 1,
										(StageManager.Instance.m_LeftOriginPoint.transform.position.y +
										 StageManager.Instance.m_MiddleOriginPoint.transform.position.y) / 2 + 1, 0);
				transform.DOMove (targetPos, m_BPM);
				break;
			case DanceActionData.AttackDir.FromMiddleToRight:
				targetPos = new Vector3 ((StageManager.Instance.m_RightOriginPoint.transform.position.x +
										 StageManager.Instance.m_MiddleOriginPoint.transform.position.x) / 2 + 1,
										(StageManager.Instance.m_RightOriginPoint.transform.position.y +
										 StageManager.Instance.m_MiddleOriginPoint.transform.position.y) / 2 + 1, 0);
				transform.DOMove (targetPos, m_BPM);
				break;
			case DanceActionData.AttackDir.FromRightToMiddle:
				targetPos = new Vector3 ((StageManager.Instance.m_RightOriginPoint.transform.position.x +
										 StageManager.Instance.m_MiddleOriginPoint.transform.position.x) / 2 - 1,
										(StageManager.Instance.m_RightOriginPoint.transform.position.y +
										 StageManager.Instance.m_MiddleOriginPoint.transform.position.y) / 2 - 1, 0);
				transform.DOMove (targetPos, m_BPM);
				break;
			case DanceActionData.AttackDir.FromLeftToMiddle:
				targetPos = new Vector3 ((StageManager.Instance.m_LeftOriginPoint.transform.position.x +
										 StageManager.Instance.m_MiddleOriginPoint.transform.position.x) / 2 + 1,
										(StageManager.Instance.m_LeftOriginPoint.transform.position.y +
										 StageManager.Instance.m_MiddleOriginPoint.transform.position.y) / 2 - 1, 0);
				transform.DOMove (targetPos, m_BPM);
				break;
			case DanceActionData.AttackDir.FromLeftToRight:
				targetPos = new Vector3 ((StageManager.Instance.m_RightOriginPoint.transform.position.x +
										 StageManager.Instance.m_LeftOriginPoint.transform.position.x) / 2 + 1, CrashHeight, 0);
				transform.DOJump (targetPos, data.JumpPower, data.NumberOfJumps, m_BPM);
				break;
			case DanceActionData.AttackDir.FromRightToLeft:
				targetPos = new Vector3 ((StageManager.Instance.m_RightOriginPoint.transform.position.x +
										 StageManager.Instance.m_LeftOriginPoint.transform.position.x) / 2 - 1, CrashHeight, 0);
				transform.DOJump (targetPos, data.JumpPower, data.NumberOfJumps, m_BPM);
				break;
		}


		Spine.TrackEntry moveEntry = m_SkeletonAnimation.AnimationState.SetAnimation (0, "jump_lv3_01", true);
		float animationLength = moveEntry.Animation.Duration;
		float speed = animationLength / m_BPM;
		moveEntry.timeScale = speed;

		yield return new WaitForSeconds (m_BPM);

		Spine.TrackEntry readyForBeaten = m_SkeletonAnimation.AnimationState.SetAnimation (0, "over", false); ;
		animationLength = readyForBeaten.Animation.Duration;
		speed = animationLength / m_BPM;
		readyForBeaten.timeScale = speed;

		yield return new WaitForSeconds (m_BPM * 2);

		AudioController.Play ("be_hit");

		Vector3 pos = transform.position + transform.up;
		EffectManager.Play ("RoundHitBlue", pos);
		stunned = EffectManager.Play ("Stunned", Vector3.zero);
		if (stunned != null)
		{
			stunned.transform.SetParent (transform, false);
			stunned.transform.localPosition = Vector3.up * 2;
		}

		Spine.TrackEntry beatenEntry = m_SkeletonAnimation.AnimationState.SetAnimation (0, "bear_down", false);
		animationLength = beatenEntry.Animation.Duration;
		beatenEntry.timeScale = 1;

		switch (data.m_AttackDir)
		{
			case DanceActionData.AttackDir.FromLeftToMiddle:
			case DanceActionData.AttackDir.FromLeftToRight:
			case DanceActionData.AttackDir.FromMiddleToRight:
				transform.eulerAngles = Vector3.zero;
				break;
			default:
				transform.eulerAngles = new Vector3 (0, 180, 0);
				break;
		}

		switch (data.m_AttackDir)
		{
			case DanceActionData.AttackDir.FromLeftToMiddle:
				targetPos = StageManager.Instance.m_MiddleOriginPoint.transform.position + new Vector3 (BeatenLength, 0, 0);
				break;
			case DanceActionData.AttackDir.FromRightToMiddle:
				targetPos = StageManager.Instance.m_MiddleOriginPoint.transform.position - new Vector3 (BeatenLength, 0, 0);
				break;
			case DanceActionData.AttackDir.FromLeftToRight:
			case DanceActionData.AttackDir.FromMiddleToRight:
				targetPos = StageManager.Instance.m_RightOriginPoint.transform.position + new Vector3 (BeatenLength, 0, 0);
				break;
			default:
				targetPos = StageManager.Instance.m_LeftOriginPoint.transform.position - new Vector3 (BeatenLength, 0, 0);
				break;
		}

		transform.DOJump (targetPos, 5, 3, animationLength);

		yield return new WaitForSeconds (animationLength);


		//getting up
		int randomIndex = Random.Range (1, 4);
		string getUpAnimeName = "bear_0" + randomIndex.ToString ();
		Spine.TrackEntry gettingUpEntry = m_SkeletonAnimation.AnimationState.SetAnimation (0, getUpAnimeName, false);
		animationLength = gettingUpEntry.Animation.Duration;
		gettingUpEntry.timeScale = 1;

		yield return new WaitForSeconds (animationLength);

		Vector3 originPos = Vector3.zero;
		switch (data.m_AttackDir)
		{
			case DanceActionData.AttackDir.FromLeftToMiddle:
			case DanceActionData.AttackDir.FromRightToMiddle:
				originPos = StageManager.Instance.m_MiddleOriginPoint.transform.position;
				break;
			case DanceActionData.AttackDir.FromLeftToRight:
			case DanceActionData.AttackDir.FromMiddleToRight:
				originPos = StageManager.Instance.m_RightOriginPoint.transform.position;
				break;
			default:
				originPos = StageManager.Instance.m_LeftOriginPoint.transform.position;
				break;
		}
		transform.DOJump (originPos, data.JumpPower, data.NumberOfJumps, m_BPM);

		Spine.TrackEntry overEntry = m_SkeletonAnimation.AnimationState.SetAnimation (0, "start", false);
		animationLength = overEntry.Animation.Duration;
		overEntry.timeScale = animationLength / m_BPM;

		yield return new WaitForSeconds (m_BPM);

		Spine.TrackEntry loseEntry = m_SkeletonAnimation.AnimationState.SetAnimation (0, "pose_1", true);
		animationLength = loseEntry.Animation.Duration;
		loseEntry.timeScale = animationLength / m_BPM;
	}
}