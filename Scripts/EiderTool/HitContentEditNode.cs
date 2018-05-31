using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Snaplingo.UI;
using UnityEngine.UI;
using System;

public class HitContentEditNode : Node
{
    private InputField m_SoundContent;
    private InputField m_ClickContent;
    private bool m_Editing;
    public override void Init(params object[] args)
    {
        base.Init(args);
        m_SoundContent = transform.Find("SoundInputField").GetComponent<InputField>();
        m_ClickContent = transform.Find("ClickInputField").GetComponent<InputField>();
        m_SoundContent.onValueChanged.AddListener(SoundValueChanged);
        m_ClickContent.onValueChanged.AddListener(ClickValueChanged);
        UIUtils.RegisterButton("Button", OKButton, transform);
        m_Editing = false;
		EiderSong.Instance.m_Editing = false;

	}


    Action<string> m_SoundCallback;
    Action<string> m_ClickCallback;
    public void SetCallback(string currentSound, string currentClick, Action<string> soundCallback , Action<string> clickCallback)
    {
        m_Editing = true;
		EiderSong.Instance.m_Editing = true;
		m_SoundCallback = soundCallback;
        m_ClickCallback = clickCallback;
        m_SoundContent.text = currentSound;
        m_ClickContent.text = currentClick;
    }

    void SoundValueChanged(string content)
    {
        if(m_Editing)
        {
            m_SoundCallback.Invoke(content);
        }
    }

    void ClickValueChanged(string content)
    {
        if (m_Editing)
        {
            m_ClickCallback.Invoke(content);
        }
    }

    void OKButton()
    {
        m_Editing = false;
		EiderSong.Instance.m_Editing = false;
		Close();
    }

}
