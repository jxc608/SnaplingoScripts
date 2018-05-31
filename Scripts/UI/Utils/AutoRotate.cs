using System;
using UnityEngine;
using DG.Tweening;

namespace UI
{
	public class AutoRotate: MonoBehaviour
	{
		public float m_RotationSpeed =  1;
		public float m_ScaleSize = 1;
		public float m_ScaleTimeLength = 3;
		public enum ScaleStatus { None, Revert, Expansion };
		private Vector3 m_ScaleFormer = new Vector3();
		private ScaleStatus m_ScaleStatus;
		private float m_Time;
		void Start ()
		{
			m_Time = 0;
			m_ScaleStatus = ScaleStatus.Revert;
			m_ScaleFormer = transform.GetComponent<RectTransform>().localScale;
			//LogManager.Log(m_ScaleStatus);
		}
		void Update ()	
		{
			
			transform.eulerAngles += new Vector3 (0, 0, m_RotationSpeed);
			if (m_ScaleSize != 1)
			{
				m_Time += Time.deltaTime;
				if (m_Time > m_ScaleTimeLength && m_ScaleStatus == ScaleStatus.Revert)
				{
					transform.DOScale (m_ScaleSize * m_ScaleFormer, m_ScaleTimeLength);
					m_Time -= m_ScaleTimeLength;
					m_ScaleStatus = ScaleStatus.Expansion;
				}
				if (m_Time > m_ScaleTimeLength && m_ScaleStatus == ScaleStatus.Expansion)
				{
					transform.DOScale (m_ScaleFormer, m_ScaleTimeLength);
					m_Time -= m_ScaleTimeLength;
					m_ScaleStatus = ScaleStatus.Revert;

				}

			}
				
		}
	}
}

