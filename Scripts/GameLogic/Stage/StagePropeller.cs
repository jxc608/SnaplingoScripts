using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StagePropeller : StageAnimationBase 
{
    public bool m_TopPropeller;
	// Use this for initialization
	void Start () 
    {
		
	}
	
    protected override void AnimationFirstHalf()
    {
        float scale_x;
        if(m_TopPropeller)
        {
            scale_x = Mathf.Lerp(-1, 1, m_T);
        }
        else
        {
            float tempT = m_T + 0.5f;
            if (tempT > 1) tempT -= 1;
            scale_x = Mathf.Lerp(-1, 1, tempT);
        }
        transform.localScale = new Vector3(scale_x, 1, 1);
    }

    protected override void AnimationSecondHalf()
    {
        float scale_x;
        if (m_TopPropeller)
        {
            scale_x = Mathf.Lerp(1, -1, m_T);
        }
        else
        { 
            float tempT = m_T + 0.5f;
            if (tempT > 1) tempT -= 1;
            scale_x = Mathf.Lerp(1, -1, tempT);
        }
        transform.localScale = new Vector3(scale_x, 1, 1);
    }

    protected override void AnimationStatusReset()
    {
        //do nothing
    }
}
