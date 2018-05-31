using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MiniForgetFoolEyeControl : MonoBehaviour 
{
    public Sprite m_AngryEye;
    public Sprite m_BigEye;
    public Sprite m_TearEye;
    public Sprite m_WuzzyEye;

    private Image m_Eye;
	// Use this for initialization
	void Start () 
    {
        m_Eye = GetComponent<Image>();
	}

    const float AngryState = 0.75f;
    const float BigEyeState = 0.50f;
    const float TearEyeState = 0.25f;
    const float WuzzyState = 0f;
    public void UpdateEye(float progress)
    {
        if(progress > AngryState)
        {
            m_Eye.sprite = m_AngryEye;
        }
        else if(progress > BigEyeState)
        {
            m_Eye.sprite = m_BigEye;
        }
        else if (progress > TearEyeState)
        {
            m_Eye.sprite = m_TearEye;
        }
        else if (progress > WuzzyState)
        {
            m_Eye.sprite = m_WuzzyEye;
        }
    }
}
