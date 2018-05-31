using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;

public class BossWarVoiceCheck
{
	private CorePlayBossWar m_CorePlayBossWar;
	public BossWarVoiceCheck(CorePlayBossWar bossWar)
	{
		m_CorePlayBossWar = bossWar;
	}

	private bool m_GetVoice = false;
	private bool m_CheckFinished = false;
	private bool m_CreateMissle = false;
	private float m_CurrentSentenceAccuracy = 0;
	// ___  Boss战 "语音"  每句提前结束 
	public void VoiceXunfeiCallback(float accuracy)
	{
		if (!m_CheckFinished)
		{
			m_CheckFinished = true;
			m_GetVoice = true;
			if (!m_CreateMissle)
			{
				m_CreateMissle = true;
				VoiceController.OnBossSentenceComplete();
			}
			m_CurrentSentenceAccuracy = accuracy;
			m_CorePlayBossWar.CalcCurSentence();
		}
	}

	// Boss战 "语音"  每句开始 
	public void CreateVoiceSentence(List<string> list)
	{
		m_GetVoice = false;
		m_CheckFinished = false;
		m_CreateMissle = false;
		m_CurrentSentenceAccuracy = 0;
		MicManager.Instance.StartRecord(GetVoiceCallback, m_CorePlayBossWar.InputCheck.SoundWave, SilentCallback);
		XunFeiSRManager.Instance.StartListen(m_CorePlayBossWar.InputCheck.GetSRCallback);
		AudioController.GetCurrentMusic().FadeOut(1.5f, 0, 0.5f, false);
		m_CorePlayBossWar.InputCheck.GetVoiceCheckSentence(list, VoiceXunfeiCallback);
	}

	public void Close()
	{

	}

	/// <summary>
	/// -1 = fail; 0 = win; 1 = draw;
	/// </summary>
	/// <returns>The result.</returns>
	public int CalculateResult()
	{
		if (!m_GetVoice)
		{
			CorePlayManager.Instance.ComboBreak(true);
			CorePlayManager.Instance.ChangeLife(-1);
			m_CheckFinished = true;
			return RuntimeConst.BossWarLose;
		}
		else
		{
			if (m_CurrentSentenceAccuracy >= CorePlaySettings.Instance.m_RightAccuracyValve)
			{
				CorePlayManager.Instance.AddVoicePoint((int)(m_CurrentSentenceAccuracy * 100f));//0~1转化为0~100的整数
				return RuntimeConst.BossWarWin;
			}
			else
			{
				return RuntimeConst.BossWarDraw;
			}
		}
	}

	public void Restart()
	{
		Close();
		StopListen();
	}

	// Boss战 "语音"  每句结束 ()
	public void StopListen()
	{
		LogManager.Log("  ____ \"语音\"  每句结束 ");

		m_CheckFinished = true;
		XunFeiSRManager.Instance.StopListen();
		//AudioManager.Instance.StartAudioFadeIn();
		AudioController.UnpauseAll(0.2f);
		MicManager.Instance.StopRecord();
	}

	private void SilentCallback()
	{
		//LogManager.Log("silent callback");
		if (!m_CreateMissle)
		{
			m_CreateMissle = true;
			VoiceController.OnBossSentenceComplete();
		}
	}

	public void CheckCreateMissle()
	{
		if (m_GetVoice && !m_CreateMissle)
		{
			m_CreateMissle = true;
			VoiceController.OnBossSentenceComplete();
		}
	}
	private void GetVoiceCallback()
	{
		m_GetVoice = true;
	}
}
