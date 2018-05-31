using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Snaplingo.UI;
using DG.Tweening;

public class GameOverNode : Node
{
	//RectTransform m_transform;
	//Vector3 m_LocalPosition = new Vector3();

	public override void Init(params object[] args)
	{
		base.Init(args);

		//m_transform = transform.Find("GameOver").GetComponent<RectTransform>();
		//m_LocalPosition = m_transform.localPosition;
		//m_transform.localPosition = new Vector3(m_LocalPosition[0], Screen.height/2f, m_LocalPosition[2]);
		UIUtils.RegisterButton("Retry", Retry, transform);
		UIUtils.RegisterButton("Back", Back, transform);
	}

	public override void Open()
	{
		base.Open();
		transform.SetAsLastSibling();
	}

	void Retry()
	{
		AnalysisManager.Instance.OnEvent("100005", null, "游戏中", "游戏失败，重新开始|" + StaticData.LevelID);
		Close();
		Time.timeScale = 1f;
		CorePlayManager.Instance.Retry();
	}

	void Back()
	{
		AnalysisManager.Instance.OnEvent("100005", null, "游戏中", "游戏失败，返回列表|" + StaticData.LevelID);
		Close();
        CorePlayPage.BackToMainProcess();
	}

	public void FallDown()
	{
		float downTime = CorePlaySettings.Instance.m_FallDownTime;
		float bgFall = CorePlaySettings.Instance.m_BGFall;
		Image tempRetry = transform.Find("Retry").GetComponent<Image>();
		Image tempBack = transform.Find("Back").GetComponent<Image>();
		tempRetry.DOFade(1, downTime);
		tempBack.DOFade(1, downTime);
		//m_transform.DOLocalMove(m_LocalPosition, downTime, true);
		transform.parent.Find("BG").GetComponent<Image>().DOColor(new Color(bgFall, bgFall, bgFall, 1f), downTime);
	}

}

