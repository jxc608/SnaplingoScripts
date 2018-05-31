using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class RhythmObject
{
    public RhythmController m_RhythmController;
    protected CorePlayManager m_Manager;
    protected Transform m_ObjectTrans;
    protected const float OSU_WIDTH = 512f;
    protected const float OSU_HEIGHT = 384f;
    protected const float ScreenWidth = 20.54f;
    protected const float ScreenHeight = 15.3f;
    protected const float HalfScreenWidth = 10.27f;
    protected const float HalfScreenHeight = 7.65f;

    protected bool m_ClickAble;
    protected void Init()
    {
        
    }

    public virtual void Delete()
    {
        if (m_RhythmController != null)
            m_RhythmController.Destroy();
    }

    public string m_HitWord;
    protected int m_SentenceIndex;
    public virtual void OnPointerDown()
    {
        if (!m_ClickAble)
            return;

        int result = CorePlayManager.Instance.GotHit(m_HitWord, m_SentenceIndex);

        switch (result)
        {
            case RuntimeConst.HitRight:
                m_RhythmController.On_Right();
                break;
            case RuntimeConst.HitPerfect:
                m_RhythmController.On_Right();
                break;
            case RuntimeConst.HitWrong:
                m_RhythmController.On_Wrong();
                break;
        }
    }
}



