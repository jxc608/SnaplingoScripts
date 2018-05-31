using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class StageAnimationBase : MonoBehaviour
{
    public float m_BPM = 0.5f;
    protected float m_T;
    protected float m_Timer;
    protected SpriteRenderer m_Render;
    protected bool m_StartAnimation;
    // Use this for initialization
    void Start()
    {
        m_StartAnimation = false;
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        if(m_StartAnimation)
        {
            m_Timer += Time.deltaTime;
            if (m_Timer >= 0 && m_Timer <= m_BPM)
            {// 
                m_T = m_Timer / m_BPM;
                AnimationFirstHalf();
            }
            else if (m_Timer > m_BPM && m_Timer < m_BPM * 2f)
            {// fading out color
                m_T = (m_Timer - m_BPM) / m_BPM;
                AnimationSecondHalf();
            }
            else
            {
                m_Timer -= m_BPM * 2f;
                AnimationStatusReset();
            }
        }
    }

    public virtual void Init()
    {
        m_StartAnimation = false;
        StageManager.DanceStartEvent.AddListener(StartAnimation);
    }

    public virtual void StartAnimation()
    {
        m_StartAnimation = true;
    }

    protected abstract void AnimationFirstHalf();
    protected abstract void AnimationSecondHalf();
    protected abstract void AnimationStatusReset();
}
