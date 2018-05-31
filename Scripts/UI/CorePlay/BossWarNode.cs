using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Snaplingo.UI;
using UnityEngine.UI;
using DG.Tweening;
using System;
using Chinese2PinYin;

public class BossWarNode : Node
{

	#region [ --- Object References --- ]
	[Header(" --- Object References ---")]
	[SerializeField]
	SpriteToParticlesAsset.SpriteToParticles sToParticle_combo;
	[SerializeField]
	UiEffect.GradientColor gradient_Combo;
	[SerializeField]
	GameObject warningPanel;
	[SerializeField]
	GameObject obj_DisPlayRoot, obj_WarningDisPlayRoot;
	[SerializeField]
	Button btn_Pause;
	[SerializeField]
	Button btn_Skip;
	[SerializeField]
	Text text_Combo;
	[SerializeField]
	Text text_Score;
	[SerializeField]
	Transform tran_PlayerLifeRoot;
	[SerializeField]
	Transform tran_BossLifeRoot;
	[SerializeField]
	StarPanel starPanel;
	//[SerializeField]
	//Button showPinYin;
	#endregion


	//private UIMaskProgressBar m_ProgressBar;
	private BossWarRound m_Round;
    public float scoreParam;

	#region [ --- Mono --- ]
	void Start()
	{
		CorePlaySceneManager.tapRightEvent.AddListener(OnTapRight);
		CorePlaySceneManager.bossEnterEvent.AddListener(OnBossEnter);
		CorePlaySceneManager.bossEnterFinishEvent.AddListener(OnBossEnterFinish);
	}
	void OnDestroy()
	{
		CorePlaySceneManager.tapRightEvent.RemoveListener(OnTapRight);
		CorePlaySceneManager.bossEnterEvent.RemoveListener(OnBossEnter);
		CorePlaySceneManager.bossEnterFinishEvent.RemoveListener(OnBossEnterFinish);
	}
	#endregion


	public override void Init(params object[] args)
	{
		base.Init();
		m_Round = GetComponentInChildren<BossWarRound>();
		m_Round.Init();
		m_LastScore = 0;
		//UpdateChangePinYinButton(PinYin.isShowPinYin);
		//showPinYin.onClick.AddListener(()=>
		//{
		//	PinYin.isShowPinYin = !PinYin.isShowPinYin;
		//	UpdateChangePinYinButton(PinYin.isShowPinYin);
		//});
		//btn_Pause.onClick.AddListener(PauseGame);
		btn_Skip.onClick.AddListener(()=> {
			AnalysisManager.Instance.OnEvent("100005", null, "游戏中", "Boss游戏跳过片头|" + StaticData.LevelID);
			OnSkip();
		});
		btn_Skip.gameObject.SetActive(false);
		obj_DisPlayRoot.SetActive(false);
		obj_WarningDisPlayRoot.SetActive(false);

		UpdateScoreAndCombo(0, 0);
		//UpdateBossHeart(0);
	}
	//private void UpdateChangePinYinButton(bool isShow)
   // {
   //     if (LanguageManager.languageType == LanguageType.Chinese)
   //     {
			//showPinYin.gameObject.SetActive(false);
   //     }
   //     else
   //     {
			//showPinYin.gameObject.SetActive(true);
			//showPinYin.transform.GetChild(0).GetComponent<Text>().text = isShow != true ? "显示拼音" : "不显示拼音";
    //    }
    //}
	public void InitBossLife(int maxLife)
	{
		for (int i = 0; i < maxLife; i++)
		{
			GameObject obj = Instantiate(ResourceLoadUtils.Load<GameObject>("CorePlay/HeartItem"));
			obj.name = "HeartItem";
			obj.transform.SetParent(tran_BossLifeRoot.transform);
			obj.transform.localScale = Vector2.one;
		}
	}

	private void PauseGame()
	{
		AudioController.PauseAll();
		CorePlayManager.Instance.PauseGame();
	}

	//public void SetLifeBar(float progress)
	//{
	//    m_ProgressBar.SetProgress(progress);
	//}

	public void SetTipContent(string content)
	{
	}

	public void ShowRound(int round, Action action)
	{
		m_Round.SetRound(round, action);
	}



	private int m_LastScore;
	public void UpdateScoreAndCombo(int score, int combo)
	{
		if (m_LastScore < score)
		{
			m_LastScore = score;
			text_Combo.fontSize = 120 + (int)(110 * Mathf.Clamp01(combo / 10f));
			text_Combo.transform.localScale = Vector3.one;
			text_Combo.transform.DOScale(1.2f * Vector3.one, 0.2f).SetEase(Ease.OutBack).OnComplete(() =>
			{ text_Combo.transform.localScale = Vector3.one; });
		}

        text_Score.text = Mathf.Clamp((int)(score*1.0f/scoreParam), 0, 999999999).ToString();
		text_Combo.text = combo.ToString();


		if (score > 0)
		{
			text_Combo.GetComponent<UiEffect.GradientColor>().enabled = true;
			text_Combo.color = Color.white;
		}
		else
		{
			text_Combo.GetComponent<UiEffect.GradientColor>().enabled = false;
			text_Combo.color = Color.gray;
		}

		sToParticle_combo.EmissionRate = 300 * Mathf.Clamp01((combo - 9) / 10f);
	}

	// 玩家 血量
	IEnumerator CorUpdatePlayerLife(int life)
	{
		yield return new WaitForSeconds(1.8f);
		UpdateLife(life);
	}

	void UpdateLife(int life)
	{
		int lifeNum = 0;
		if (life > tran_PlayerLifeRoot.childCount)
		{
			lifeNum = tran_PlayerLifeRoot.childCount;
		}
		else
		{
			lifeNum = life;
		}
		for (int i = 0; i < tran_PlayerLifeRoot.childCount; i++)
		{
			if (i < lifeNum)
			{
				tran_PlayerLifeRoot.GetChild(i).GetChild(HeartIndex).gameObject.SetActive(true);
			}
			else
			{
				tran_PlayerLifeRoot.GetChild(i).GetChild(HeartIndex).gameObject.SetActive(false);
			}
		}
	}

	private const int HeartIndex = 1;
	public void UpdatePlayerLife(int life, bool immediate = false)
	{
		if (immediate)
		{
			UpdateLife(life);
		}
		else
		{
			StartCoroutine(CorUpdatePlayerLife(life));
		}
	}


	// Boss 血量增长
	private int m_HeartNumberInAnimation;
	IEnumerator CorAddBossHeart(float duration, int maxLife)
	{
		yield return new WaitForSeconds(duration);

		if (m_HeartNumberInAnimation <= maxLife)
		{
			duration = Duration;
			UpdateBossHeart(m_HeartNumberInAnimation);
			StartCoroutine(CorAddBossHeart(duration, maxLife));
			m_HeartNumberInAnimation++;
		}
	}

	private const float Duration = 0.06f;
	public void BossHeartInitAnimation(int target)
	{
		UpdateBossHeart(0);
		m_HeartNumberInAnimation = 0;
		var song = AudioController.GetAudioItem("EnterTalking");
		var songLength = song.subItems[0].Clip.length;
		// warning 时间
		var auio1 = AudioController.GetAudioItem("Warning");
		float warningDuration = auio1.subItems[0].Clip.length * 3;
		float enterAnimDelay = 11f + warningDuration;
		StartCoroutine(CorAddBossHeart(songLength + enterAnimDelay, target));
	}


	// Boss 血量
	public IEnumerator CorUpdateBossHeart(int life, float delay)
	{
		yield return new WaitForSeconds(delay);
		UpdateBossHeart(life);

	}
	public void UpdateBossHeart(int life, float delay = 0)
	{
		if (delay > 0)
		{
			StartCoroutine(CorUpdateBossHeart(life, delay));
			return;
		}

		int lifeNum = 0;
		if (life > tran_BossLifeRoot.childCount)
		{
			lifeNum = tran_BossLifeRoot.childCount;
		}
		else
		{
			lifeNum = life;
		}

		for (int i = 0; i < tran_BossLifeRoot.childCount; i++)
		{
			if (i < lifeNum)
			{
				tran_BossLifeRoot.GetChild(i).GetChild(HeartIndex).gameObject.SetActive(true);
			}
			else
			{
				tran_BossLifeRoot.GetChild(i).GetChild(HeartIndex).gameObject.SetActive(false);
			}
		}
	}


	public void Restart()
	{
		m_LastScore = 0;
		UpdateScoreAndCombo(0, 0);
		UpdateBossHeart(0);
		btn_Skip.gameObject.SetActive(true);
		obj_DisPlayRoot.SetActive(false);
		starPanel.SetDefault();
		m_Round.Restart();
	}

	#region [ --- Private --- ]
	void OnSkip()
	{
		StopAllCoroutines();
		SceneController.instance.OnSkipEnter();
	}
	void OnBossEnter()
	{
		StartCoroutine(CorEnter());
	}
	IEnumerator CorEnter()
	{
		btn_Skip.gameObject.SetActive(true);
		obj_DisPlayRoot.SetActive(false);
		obj_WarningDisPlayRoot.SetActive(true);

		// warning 时间
		var auio1 = AudioController.Play("Warning");
		float warningDuration = auio1.clipLength * 3 - 0.1f;
		AudioController.Play("Warning");
		yield return new WaitForSeconds(warningDuration);
		obj_DisPlayRoot.SetActive(true);
		obj_WarningDisPlayRoot.SetActive(false);
		AudioController.Stop("Warning");
	}
	void OnBossEnterFinish()
	{
		btn_Skip.gameObject.SetActive(false);
		StopAllCoroutines();
		UpdateBossHeart(CorePlayData.BossLife);
		obj_DisPlayRoot.SetActive(true);
		obj_WarningDisPlayRoot.SetActive(false);
		AudioController.Stop("Warning");
	}
	void OnTapRight(Vector3 worldPos)
	{
		if (!gameObject.activeInHierarchy)
			return;

		starPanel.LoadStar(worldPos, text_Combo.transform.position,null);
	}
	#endregion
}
