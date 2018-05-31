using UnityEngine;
using System.Collections;
using System;

public class MatFader : MonoBehaviour
{
	#region [ --- Property --- ]
	enum ObjType { SprRenderer, TextMesh, LineRenderer }
	ObjType m_Type;
	float m_Timer;
	SpriteRenderer m_SpriteRender;
	TextMesh m_TextMesh;
	LineRenderer m_LineRenderer;
	Color m_LineRendererColor;
	float m_TargetAlpha;
	float m_StartAlpha;
	float m_TimeLength;
	#endregion



	#region [ --- Mono --- ]
	void Update()
	{
		m_Timer -= Time.deltaTime;
		m_Timer = Mathf.Max(0, m_Timer);
		float alpha = Mathf.Lerp(m_TargetAlpha, m_StartAlpha, m_Timer / m_TimeLength);
		switch (m_Type)
		{
			case ObjType.SprRenderer:
				if (m_SpriteRender)
					m_SpriteRender.color = new Color(m_SpriteRender.color.r, m_SpriteRender.color.g, m_SpriteRender.color.b, alpha);
				break;
			case ObjType.TextMesh:
				if (m_TextMesh)
					m_TextMesh.color = new Color(m_TextMesh.color.r, m_TextMesh.color.g, m_TextMesh.color.b, alpha);
				break;
			case ObjType.LineRenderer:
				if (m_LineRenderer)
					m_LineRenderer.material.SetColor("_TintColor", new Color(m_LineRendererColor.r, m_LineRendererColor.g, m_LineRendererColor.b, alpha));
				break;
		}

		if (m_Timer == 0)
		{
            if (m_CompleteCallback != null)
            {
                m_CompleteCallback.Invoke();
                m_CompleteCallback = null;
            }
              
            Destroy(this);
		}
	}
    #endregion


    #region [ --- Public --- ]
    private static Action m_CompleteCallback;
    public static void DoFade(float target, float timeLength, Action callback = null, params Transform[] trans)
	{
		if (trans == null)
			return;

        m_CompleteCallback = callback;
		for (int i = 0; i < trans.Length; i++)
		{
			var fader = trans[i].GetComponent<MatFader>();
			if (fader == null)
			{
				fader = trans[i].gameObject.AddComponent<MatFader>();
				fader.OldStartFade(trans[i].transform, target, timeLength);
			}
			else
			{
				fader.OldStartFade(trans[i].transform, target, timeLength);
			}
		}
	}
	public static void KillFade(Transform tran)
	{
		var fader = tran.GetComponent<MatFader>();
		if (fader != null)
		{
			Destroy(fader);
		}
	}
	#endregion




	#region [ --- Private --- ]
	void OldStartFade(Transform tran, float target, float timeLength)
	{
		SpriteRenderer sr = tran.GetComponent<SpriteRenderer>();
		if (sr != null)
		{
			m_SpriteRender = sr;
			m_Type = ObjType.SprRenderer;
			m_StartAlpha = sr.color.a;
		}
		else
		{
			TextMesh tm = tran.GetComponent<TextMesh>();
			if (tm != null)
			{
				m_TextMesh = tm;
				m_Type = ObjType.TextMesh;
				m_StartAlpha = m_TextMesh.color.a;
			}
			else
			{
				LineRenderer lr = tran.GetComponent<LineRenderer>();
				if (lr != null)
				{
					m_LineRenderer = lr;
					m_LineRendererColor = lr.material.GetColor("_TintColor");
					m_StartAlpha = m_LineRendererColor.a;
					m_Type = ObjType.LineRenderer;
					//lr.material.SetColor("_TintColor",new Color(color.r, color.g, color.b, m_StartAlpha));
				}
				else
				{
					return;
				}
			}
		}

		m_TargetAlpha = target;
		m_Timer = timeLength;
		m_TimeLength = m_Timer;
		if (Mathf.Approximately(m_TimeLength, 0))
		{
			switch (m_Type)
			{
				case ObjType.SprRenderer:
					m_SpriteRender.color = new Color(m_SpriteRender.color.r, m_SpriteRender.color.g, m_SpriteRender.color.b, m_TargetAlpha);
					break;
				case ObjType.TextMesh:
					m_TextMesh.color = new Color(m_TextMesh.color.r, m_TextMesh.color.g, m_TextMesh.color.b, m_TargetAlpha);
					break;
				case ObjType.LineRenderer:
					m_LineRenderer.material.SetColor("_TintColor", new Color(m_LineRendererColor.r, m_LineRendererColor.g, m_LineRendererColor.b, m_TargetAlpha));
					break;

			}
			Destroy(this);
		}
	}
	#endregion


}
//MatFader