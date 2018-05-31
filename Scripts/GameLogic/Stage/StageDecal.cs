using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageDecal : StageAnimationBase 
{
    Color m_RandomColor;
	void Start () 
    {
        m_RandomColor = new Color(Random.Range(0, 1f), Random.Range(0, 1f), Random.Range(0, 1f));
        m_Render = GetComponent<SpriteRenderer>();
	}
	
	protected override void AnimationFirstHalf()
    {
        m_Render.color = Color.Lerp(Color.white, m_RandomColor, m_T);
    }

    protected override void AnimationSecondHalf()
    {
        m_Render.color = Color.Lerp(m_RandomColor, Color.white, m_T);
    }

    protected override void AnimationStatusReset()
    {
        m_RandomColor = new Color(Random.Range(0, 1f), Random.Range(0, 1f), Random.Range(0, 1f));
    }
}
