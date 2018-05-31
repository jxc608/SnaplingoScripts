using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageFloating : StageAnimationBase
{

	public float m_FloatingDis = 0.2f;
	public Vector3 m_StartPos;
	public Vector3 m_TargetPos;
	// Use this for initialization
	void Start ()
	{
		m_StartPos = transform.localPosition;
		CreateRandomPos ();
	}


	protected override void AnimationFirstHalf ()
	{
		float dis = CubicEaseOut (m_T, 0, m_FloatingDis, m_BPM);
		transform.localPosition = m_StartPos + m_TargetPos * dis;
	}

	protected override void AnimationSecondHalf ()
	{
		float dis = CubicEaseOut (m_T, m_FloatingDis, 0, m_BPM);
		transform.localPosition = m_StartPos + m_TargetPos * dis;
	}

	protected override void AnimationStatusReset ()
	{
		// do nothing
		CreateRandomPos ();
	}

	public static float CubicEaseOut (float t, float b, float c, float d)
	{
		return c * ((t = t / d - 1) * t * t + 1) + b;
	}

	void CreateRandomPos ()
	{
		Vector3 dir = new Vector3 (Random.Range (0, 1f), Random.Range (0, 1f), Random.Range (0, 1f));
		m_TargetPos = dir.normalized;
	}
}
