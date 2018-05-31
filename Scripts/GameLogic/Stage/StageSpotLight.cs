using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageSpotLight : StageAnimationBase {

    public bool m_Left;
	// Use this for initialization
	void Start () {
		
	}
	


    protected override void AnimationFirstHalf()
    {
        float axis_z;
        if(m_Left)
        {
            axis_z = QuadEaseInOut(m_T, 0, -80, m_BPM);
        }
        else
        {
            axis_z = QuadEaseInOut(m_T, 0, 80, m_BPM);
        }

        transform.eulerAngles = new Vector3(0, 0, axis_z);
    }

    protected override void AnimationSecondHalf()
    {
        float axis_z;
        if (m_Left)
        {
            axis_z = QuadEaseInOut(m_T, -80, 0, m_BPM);
        }
        else
        {
            axis_z = QuadEaseInOut(m_T, 80, 0, m_BPM);
        }

        transform.eulerAngles = new Vector3(0, 0, axis_z);
    }

    protected override void AnimationStatusReset()
    {
        // do nothing
    }

    /// <summary>
    /// Quads the ease in out.
    /// </summary>
    /// <returns>The ease in out.</returns>
    /// <param name="t">current value</param>
    /// <param name="b">init value</param>
    /// <param name="c">total change</param>
    /// <param name="d">duration</param>
    public static float QuadEaseInOut(float t, float b, float c, float d)
    {
        if ((t /= d / 2) < 1) return c / 2 * t * t + b;
        return -c / 2 * ((--t) * (t - 2) - 1) + b;
    }
}
