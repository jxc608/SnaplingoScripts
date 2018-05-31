using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using Snaplingo.UI;
using DG.Tweening;
using System.Collections;
using Chinese2PinYin;
using Spine.Unity;
using Spine;

public class CalculateNode : Node
{
	public enum UIType { Boss, Common }
	public enum SliderSpriteName { slider_1, slider_2, slider_3, slider_4, slider_5 }
	#region [ --- Property --- ]
	const float OSU_WIDTH = 512f;
	const float OSU_HEIGHT = 384f;

	UIType m_Type;
	int m_LastScore;
	#endregion


	#region [ --- Object References --- ]
	[Header (" --- Object References ---")]
	public ParticleSystem particle_combo;
	[SerializeField]
	SpriteToParticlesAsset.SpriteToParticles sToParticle_combo;
	[SerializeField]
	UiEffect.GradientColor gradient_Combo;
	[SerializeField]
	Button btn_Pause;
	[SerializeField]
	Text text_Combo;
	[SerializeField]
	Text text_Score;
	[SerializeField]
	GameObject obj_Life;
	[SerializeField]
	StarPanel starPanel;
	#endregion
	/// <summary>
	/// 汉子转化拼音按钮
	/// </summary>
	//public Button but_ShowPinYin;
	/// <summary>
	/// 关卡最高分
	/// </summary>
	public int levelMaxScore;
	/// <summary>
	/// 系数
	/// </summary>
	public float scoreParam;
	public Image scoreSlider;
	public GameObject m_mainSlider;
	[SerializeField]
	private Sprite[] m_sliderValue;
	[SerializeField]
	private Image[] m_rankImage;
	private SkeletonGraphic m_scoreBarActor = null;
	#region [ --- Mono --- ]
	void Start ()
	{
		CorePlaySceneManager.tapRightEvent.AddListener (OnTapRight);
	}
	void OnDestroy ()
	{
		CorePlaySceneManager.tapRightEvent.RemoveListener (OnTapRight);
		DOTween.CompleteAll ();
	}
	#endregion

	public override void Init (params object[] args)
	{
		base.Init (args);
		m_LastScore = 0;
		InitScoreBarActor ();
		UpdateScoreAndCombo (0, 0);
		btn_Pause.onClick.AddListener (() => {
			AnalysisManager.Instance.OnEvent ("100005", null, "游戏中", "游戏暂停|" + StaticData.LevelID);
			CorePlayManager.Instance.PauseGame ();
		});
		//UpdateChangePinYinButton(PinYin.isShowPinYin);
		//but_ShowPinYin.onClick.AddListener(() =>
		//{
		//	PinYin.isShowPinYin = !PinYin.isShowPinYin;
		//	UpdateChangePinYinButton(PinYin.isShowPinYin);
		//});

		//位置不准确
		//以后等策划规范具体位置
		//m_rankImage[0].transform.localPosition = new Vector3 (m_mainSlider.rectTransform.rect.width * 0.5f, m_rankImage[0].transform.localPosition.y, m_rankImage[0].transform.localPosition.z);
		//m_rankImage[1].transform.localPosition = new Vector3 (m_mainSlider.rectTransform.rect.width * 0.66f, m_rankImage[1].transform.localPosition.y, m_rankImage[1].transform.localPosition.z);
		//m_rankImage[2].transform.localPosition = new Vector3 (m_mainSlider.rectTransform.rect.width * 0.77f, m_rankImage[2].transform.localPosition.y, m_rankImage[2].transform.localPosition.z);
		//m_rankImage[3].transform.localPosition = new Vector3 (m_mainSlider.rectTransform.rect.width * 0.84f, m_rankImage[3].transform.localPosition.y, m_rankImage[3].transform.localPosition.z);
		//m_rankImage[4].transform.localPosition = new Vector3 (m_mainSlider.rectTransform.rect.width * 0.9f, m_rankImage[4].transform.localPosition.y, m_rankImage[4].transform.localPosition.z);
		HideScoreAndCombo ();

	}
	//private void UpdateChangePinYinButton(bool isShow)
	//{
	//	if (LanguageManager.languageType == LanguageType.Chinese)
	//	{
	//		but_ShowPinYin.gameObject.SetActive(false);
	//	}
	//	else
	//	{
	//		but_ShowPinYin.gameObject.SetActive(true);
	//		but_ShowPinYin.transform.GetChild(0).GetComponent<Text>().text = isShow != true ? "显示拼音" : "不显示拼音";
	//	}
	//}
	private void HideScoreAndCombo ()
	{
		text_Score.gameObject.SetActive (false);
		text_Combo.transform.parent.gameObject.SetActive (false);
		obj_Life.gameObject.SetActive (false);
	}
    public void ShowMainSliderAndPauseButton()
	{
		m_mainSlider.SetActive(true);
		btn_Pause.gameObject.SetActive(true);
	}

	private void InitScoreBarActor ()
	{
		m_RhythmBeatInterval = m_RhythmBeatInterval = 60000f / CorePlayData.CurrentSong.BPM;
		LogManager.LogWarning ("m_RhythmBeatInterval calcula: " , m_RhythmBeatInterval);

		int nDefaultModelId = 20001;
		int nDefaultMotinId = 30003;

		//get id from selfdata

		int nModelId = string.IsNullOrEmpty (SelfPlayerData.ModelId) || SelfPlayerData.ModelId == "0" ? nDefaultModelId : int.Parse (SelfPlayerData.ModelId);
		int nMotionId = string.IsNullOrEmpty (SelfPlayerData.EmotionId) || SelfPlayerData.ModelId == "0" ? nDefaultMotinId : int.Parse (SelfPlayerData.EmotionId);

		string strModelName = RoleModelConfig.Instance.GetNameById (nModelId);
		string strMotionName = RoleEmotionConfig.Instance.GetNameById (nMotionId);

		SkeletonAnimation obj = Resources.Load<SkeletonAnimation> ("RoleCreate/Role/" + strModelName);
		m_scoreBarActor = SkeletonGraphic.NewSkeletonGraphicGameObject (obj.SkeletonDataAsset, m_mainSlider.transform);
		m_scoreBarActor.transform.localPosition = new Vector3 (-855f, -20f, 0);

		m_scoreBarActor.transform.localScale = new Vector3 (0.15f, 0.12f, 0.15f);
		m_scoreBarActor.Skeleton.SetSkin (strMotionName);
		PlayBarActorAnimation ();
		m_scoreBarActor.AnimationState.Complete += OnAniCom;


	}

	private string[] m_arrAnimationNames = {"jump_lv1_01","jump_lv1_02","jump_lv1_03",
		"jump_lv2_01","jump_lv2_02","jump_lv2_03","jump_lv3_01","jump_lv3_02","jump_lv3_03",
		"pose_1","pose_2","tx_over","tx_start"};
	private float m_RhythmBeatInterval = 1;

	private void PlayBarActorAnimation ()
	{
		Random.seed = (int)(Time.time);
		int nRandomIndex = Random.RandomRange (0, m_arrAnimationNames.Length - 1);
		Spine.TrackEntry entry = m_scoreBarActor.AnimationState.SetAnimation (0, m_arrAnimationNames[nRandomIndex], false);
		m_scoreBarActor.timeScale = entry.Animation.Duration / (m_RhythmBeatInterval * 0.001f);
	}

	private void OnAniCom (TrackEntry trackEntry)
	{
		PlayBarActorAnimation ();
	}


	public void UpdateScoreAndCombo (int score, int combo)
	{
		// Score
		if (m_LastScore < score)
		{
			ScorePunch ();
			m_LastScore = score;
		}
		text_Score.text = Mathf.Clamp (score, 0, 999999999).ToString ();

		// Combo
		text_Combo.text = combo.ToString ();
		text_Combo.fontSize = 100 + (int)(110 * Mathf.Clamp01 (combo / 10f));
		text_Combo.transform.localScale = Vector3.one;
		text_Combo.transform.DOScale (1.2f * Vector3.one, 0.2f).SetEase (Ease.OutBack)
				  .OnComplete (() => {
					  if (text_Combo != null)
						  text_Combo.transform.localScale = Vector3.one;
				  });

		if (combo > 0)
		{
			gradient_Combo.enabled = true;
			text_Combo.color = Color.white;
		}
		else
		{
			gradient_Combo.enabled = false;
			text_Combo.color = Color.gray;
		}

		sToParticle_combo.EmissionRate = 300 * Mathf.Clamp01 ((combo - 9) / 10f);
		//LogManager.Log("开始进度条的值" , levelMaxScore);
		float sliderValue = 0;
		if (levelMaxScore != 0)
		{
			sliderValue = (score * 1.0f / scoreParam) / levelMaxScore;
		}
		//LogManager.Log("开始进度条的值" , sliderValue);
		UpdateSliderBack (sliderValue);
		//LogManager.Log("更新进度条：" , (score) , "/" , levelMaxScore);
	}

	public SliderSpriteName sliderSpriteName = SliderSpriteName.slider_1;
	private void UpdateSliderBack (float value)
	{
		if (value > 0.90f && sliderSpriteName != SliderSpriteName.slider_5)
		{
			sliderSpriteName = SliderSpriteName.slider_5;
			scoreSlider.sprite = m_sliderValue[4];
		}
		else if (value > 0.80f && sliderSpriteName != SliderSpriteName.slider_4)
		{
			sliderSpriteName = SliderSpriteName.slider_4;
			scoreSlider.sprite = m_sliderValue[3];
		}
		else if (value > 0.70f && sliderSpriteName != SliderSpriteName.slider_3)
		{
			sliderSpriteName = SliderSpriteName.slider_4;
			scoreSlider.sprite = m_sliderValue[2];
		}
		else if (value > 0.60f && sliderSpriteName != SliderSpriteName.slider_2)
		{
			sliderSpriteName = SliderSpriteName.slider_2;
			scoreSlider.sprite = m_sliderValue[1];
		}
		//else if (sliderSpriteName != SliderSpriteName.slider_1)
		//{
		//	sliderSpriteName = SliderSpriteName.slider_1;
		//	scoreSlider.sprite = m_sliderValue[0];
		//}
		scoreSlider.fillAmount = value;
		m_scoreBarActor.transform.localPosition = new Vector3 (1710f * value + (-855f), -20f, 0f);
		//scoreSlider.rectTransform.SetRectWidth ((m_mainSlider.rectTransform.rect.width - 90) * value);
	}

	public void UpdateLifeBar (int lifeNum)
	{
		for (int i = 0; i < 5; i++)
		{
			GameObject Heart = obj_Life.transform.GetChild (i).Find ("Image_Heart").gameObject;
			if (i < lifeNum)
			{
				if (!Heart.activeSelf)
				{
					Heart.SetActive (true);
				}
			}
			else
			{
				if (Heart.activeSelf)
				{
					Heart.SetActive (false);
				}
			}
		}
	}

	public void Restart ()
	{
		UIUtils.Kill (transform);
		UpdateScoreAndCombo (0, 0);
		Fade (1);
		m_LastScore = 0;
		starPanel.SetDefault ();
	}

	public void Fade (float alpha, float duration = 0.1f)
	{
		float disappearTime = duration;
		text_Score.DOFade (alpha, disappearTime);
		//btn_Pause.image.DOFade (alpha, disappearTime);
	}

	public void FadeText (float alpha, float duration = 0.1f)
	{
		float disappearTime = duration;
		text_Score.DOFade (alpha, disappearTime);
	}

	public void AllFadeOut ()
	{
		UIUtils.Fade (transform, 0, CorePlaySettings.Instance.m_StartFadeOutTimeLength / 1000f);
		FadeText (0, CorePlaySettings.Instance.m_StartFadeOutTimeLength / 1000f);
	}

	public void AllFadeIn ()
	{
		UIUtils.Fade (transform, 1, CorePlaySettings.Instance.m_StartFadeInTimeLength / 1000f);
		FadeText (1, CorePlaySettings.Instance.m_StartFadeOutTimeLength / 1000f);
	}

	public void ScorePunch ()
	{
		float scaleTime = CorePlaySettings.Instance.m_ScorePunchTime / 2000f;
		Tweener scoreSize = text_Score.transform.DOScale (CorePlaySettings.Instance.m_ScorePunchSize, scaleTime);
		scoreSize.OnComplete<Tweener> (delegate () {
			text_Score.transform.GetComponent<RectTransform> ().DOScale (1, scaleTime);
		});
	}

	#region [ --- Private --- ]
	void OnTapRight (Vector3 worldPos)
	{
		if (!gameObject.activeInHierarchy)
			return;

		starPanel.LoadStar (worldPos, m_scoreBarActor.transform.position, null);
	}
	#endregion
}


