using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class TutotialScaler : MonoBehaviour
{
	#region [ --- Property --- ]
	public float scaleSize = 0.1f;
	public float beat = 1000f;
	Vector3 startScale;
	public AudioSource m_Hello;
    public AudioSource m_Hai;
	public bool PlayWord = false;
	public bool Click = false;
	public bool IsClickKey;
	public TutorialScene _tutorialScene;
	#endregion

	private float m_Timer;
	private float beatScale;
	private float lastbeatScale = 0f;
	private bool HightLight;
	private bool Insert;
	private bool Sound_Click;
	private bool PlayMusic = false;
	public bool IsPerfect;

	float StartTime = 0;
	float EndTime = 0;
	float SoundTime = 900;

	#region [ --- Mono --- ]
	void Start()
	{
		startScale = transform.localScale;
		m_Timer = 0;
		Insert = false;
		IsPerfect = false;
		StartTime = SoundTime + CorePlaySettings.Instance.m_PerfectOffsetBefore*2;
		EndTime = SoundTime - CorePlaySettings.Instance.m_PerfectOffsetAfter*2;
	}

	public void Restart()
	{
		m_Timer = 0;
		lastbeatScale = 0f;
		HightLight = false;
		Insert = false;
		IsPerfect = false;
		IsClickKey = false;
	}

	private void Update()
	{
		if (_tutorialScene.Pause == false)
		{		
			m_Timer += Time.deltaTime * 1000f;
			float timer = m_Timer % beat;
			beatScale = timer / beat;

			if (HightLight)
			{
				Sound_Click = false;
				IsClickKey = true;
				if (PlayMusic == false && beatScale > (1 - SoundTime / beat))
				{
                    if (LanguageManager.languageType == LanguageType.Chinese)
                    {
                        m_Hello.Play();
                    }
                    else
                    {
                        m_Hai.Play();
                    }
                 
					PlayMusic = true;
					_tutorialScene.HasHand = true;
				}
				if (Insert == false && beatScale > (1 - 800f / beat))
				{
					Insert = true;
					Click = false;
					_tutorialScene.m_TapController.On_HighLight("Hello", 0.8f);
				}
				if (lastbeatScale > beatScale)
				{
					Insert = false;
					PlayMusic = false;
					HightLight = false;
					m_Timer = 0;
				}
				if (beatScale > (1 - StartTime / beat) && beatScale < (1 - EndTime / beat))
				{
					IsPerfect = true;
				}
				else
				{
					IsPerfect = false;
				}
			}
			else if(IsClickKey == true)
			{
				IsPerfect = false;
				if (m_Timer > 500f && Click == false && Sound_Click==false)
				{
					_tutorialScene.ClickSound();
					Sound_Click = true;
				}
				
				if (m_Timer > 500f && Click)
				{
					_tutorialScene.PointerWrong();
					IsClickKey = false;
				}
				
			}
			
			lastbeatScale = beatScale;
		
		}
		else
		{
		}
	}

	public void Hight()
	{
		Restart();
		HightLight = true;
		IsClickKey = true;
	}

	public void ClearHight()
	{
		HightLight = false;
		IsClickKey = false;
	}

	#endregion
}