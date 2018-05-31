using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeachRhythmTap : RhythmObject
{
	public void SetPosition(ClickObj obj)
	{
		//TeachRhythmRenderer render = new TeachRhythmRenderer();
		//SetRenderer(render);
		//m_ObjectTrans = m_RhythmRenderer.SetTransform(Vector3.zero);
		//m_RhythmRenderer.SetPosition(obj.m_Position);
		//m_RhythmRenderer.Show();

	}

    public void HighLight()
    {
        
    }

	public override void Delete()
	{
		GameObject.Destroy(m_ObjectTrans.gameObject);
	}

	public void StartDim()
	{
		//m_RhythmRenderer.StartDim();
	}

	public override void OnPointerDown()
	{

	}

}
