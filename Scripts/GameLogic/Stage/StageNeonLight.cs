using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageNeonLight : StageAnimationBase {

    public SpriteRenderer m_Neon1;
    public SpriteRenderer m_Neon2;

	// Use this for initialization
	void Start () 
    {
		
	}


    protected override void AnimationFirstHalf()
    {
        m_Neon1.color = new Color(1, 1, 1, m_T);
        m_Neon2.color = new Color(1, 1, 1, 1 - m_T);
    }

    protected override void AnimationSecondHalf()
    {
        m_Neon1.color = new Color(1, 1, 1, 1 - m_T);
        m_Neon2.color = new Color(1, 1, 1, m_T);
    }

    protected override void AnimationStatusReset()
    {
        // do nothing
    }
}
