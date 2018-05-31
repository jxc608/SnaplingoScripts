using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Snaplingo.UI;

public class BossLife
{
	private int m_BossLife;
	private int m_MaxLife;
	private UIMaskProgressBar m_ProgressBar;
	//private MiniForgetFoolEyeControl m_EyeControl;
	public BossLife(int maxLife)
	{
		m_MaxLife = maxLife;
		m_BossLife = maxLife;
		//m_ProgressBar = PageManager.Instance.CurrentPage.GetNode<BossWarNode>().GetComponentInChildren<UIMaskProgressBar>();
		//m_EyeControl = PageManager.Instance.CurrentPage.GetNode<BossWarNode>().GetComponentInChildren<MiniForgetFoolEyeControl>();
		PageManager.Instance.CurrentPage.GetNode<BossWarNode>().InitBossLife(m_MaxLife);
	}

	private const int BossAlmostDieValve = 1;
	public void BossDamage(int damage = 1)
	{
		m_BossLife -= damage;
		//float progress = m_BossLife / (float)m_MaxLife;
		//m_ProgressBar.SetProgress(progress);
		//m_EyeControl.UpdateEye(progress);
		PageManager.Instance.CurrentPage.GetNode<BossWarNode>().UpdateBossHeart(m_BossLife, 1.6f);
	}

	public bool BossLose()
	{
		if (m_BossLife <= 0)
		{
			return true;
		}
		else
		{
			return false;
		}
	}

	public void Restart()
	{
		m_BossLife = m_MaxLife;
		PageManager.Instance.CurrentPage.GetNode<BossWarNode>().Restart();
		//m_ProgressBar.Restart();
		//m_EyeControl.UpdateEye(1);
	}
}
