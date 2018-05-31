using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenShake : Manager
{
	public static ScreenShake Instance { get { return GetManager<ScreenShake>(); } }


	void Update()
	{
		if (m_Shake > 0)
		{
			Camera.main.transform.localPosition = m_StartPos + Random.insideUnitSphere * m_ShakeAmount * m_Shake / m_Max;
			m_Shake -= Time.deltaTime * m_ShakeFactor;
		}
		else
		{
			m_Shake = 0f;
			//Camera.main.transform.localPosition = new Vector3(0, 0, -10);
		}
	}

	private float m_Shake;
	private float m_ShakeAmount;
	private float m_ShakeFactor = 1f;
	private float m_Max;
	private Vector3 m_StartPos;
	public void Shake(float shakeAmount = .5f)
	{
		m_ShakeAmount = shakeAmount;
		m_Shake = 1;
		m_Max = m_Shake;
		m_StartPos = Camera.main.transform.localPosition;
	}




}
