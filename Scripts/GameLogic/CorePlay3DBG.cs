using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CorePlay3DBG
{
	private Vector3 m_ScorePosition;
	public CorePlay3DBG()
	{
		//Transform obj = GameObject.Instantiate(ResourceLoadUtils.Load<Transform>("CorePlay/ThreeDBG"));
		m_ScorePosition = CorePlaySceneManager.instance.starTarget.position;
	}
	public Vector3 GetScorePosition()
	{
		return m_ScorePosition;
	}
}
