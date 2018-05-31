using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;
using System;
using UnityEngine.UI;
using TMPro;
using Chinese2PinYin;

public class VoiceController : MonoBehaviour
{
	#region [ --- Property --- ]
	public static Vector3 WordCenterPos {
		get { return instance.tran_WordCenter.position; }
	}
	public static float InputVolume {
		set {
			micFillAmount = value * 7;
			instance.soundWave.Volume = value * 5;

			//var em = instance.ps_Voice.emission;
			//em.rateOverTime = Mathf.Clamp (value * 2000, 0, 300);
		}
	}
	static float micFillAmount;

	float rightPosX;
	float leftPosX;
	RectTransform rectTran_Voice;
	static float fadeDuration;
	Sequence wordSequence;
	#endregion


	#region [ --- Object References --- ]
	[Header (" --- Object References ---")]
	public SoundWave soundWave;
	public ParticleSystem ps_Voice;
	public Image image_micOn;
	public Transform tran_WordCenter;
	[Header (" Fade Group")]
	public Transform[] tran_fadeGroup;
	public Image[] image_fadeArray;
	[Header (" Mic Anim")]
	public DOTweenAnimation[] do_micAnims;
	[Header (" Canvas")]
	public Canvas canvas_Base;
	public Canvas canvas_Front;

	public TextMeshProUGUI text_Voice;


	#endregion



	#region [ --- Mono --- ]
	public static VoiceController instance;
	void Awake ()
	{
		instance = this;
	}
	void Start ()
	{
		fadeDuration = CorePlaySettings.Instance.m_VoiceNodeFadeInTimeLength / 1000f;
		MatFader.DoFade (0, 0, null, instance.tran_fadeGroup);
		foreach (var item in image_fadeArray) {
			item.color = new Color (1, 1, 1, 0);
		}

		canvas_Base.worldCamera = Camera.main;
		canvas_Front.worldCamera = Camera.main;
		text_Voice.text = string.Empty;
		rectTran_Voice = text_Voice.GetComponent<RectTransform> ();
		rightPosX = rectTran_Voice.anchoredPosition.x + (canvas_Front.transform as RectTransform).rect.width;
		leftPosX = rectTran_Voice.anchoredPosition.x - (canvas_Front.transform as RectTransform).rect.width;
	}
	void Update ()
	{
		if (image_micOn.fillAmount > micFillAmount)
			image_micOn.fillAmount = Mathf.Lerp (image_micOn.fillAmount, micFillAmount, Time.deltaTime * 2f);
		else
			image_micOn.fillAmount = micFillAmount;
	}
	void OnDestroy ()
	{
		instance = null;
	}
	#endregion


	#region [ --- Public --- ]
	public static void OnBossSentenceComplete ()
	{
		SceneController.instance.OnLoadMissile (instance.image_micOn.transform.position, true);
	}
	public static void Show (Action callback = null)
	{
		//if (!instance.isShown)
		//{
		//AudioController.Play("SpeekLoud");
		//instance.isShown = true;
		//}

		foreach (var item in instance.do_micAnims) {
			item.DORewind ();
		}
		micFillAmount = instance.image_micOn.fillAmount = 0;
		foreach (var item in instance.image_fadeArray) {
			item.DOFade (1, fadeDuration);
		}
		MatFader.DoFade (1, fadeDuration, callback, instance.tran_fadeGroup);
	}
	public static void Hide (Action callback = null)
	{
		foreach (var item in instance.image_fadeArray) {
			item.DOFade (0, fadeDuration);
		}
		MatFader.DoFade (0, fadeDuration, callback, instance.tran_fadeGroup);
		Stop_RecordingAnim ();
	}
	public static void Start_RecordingAnim ()
	{
		//AudioController.Play("SpeekLoud");
		foreach (var item in instance.do_micAnims) {
			item.DORestart ();
		}
	}
	public static void Stop_RecordingAnim ()
	{
		foreach (var item in instance.do_micAnims) {
			item.DORewind ();
		}
	}
	private void ChangeFont ()
	{
		if (PinYin.isShowPinYin) {
			//加载拼音字体
			text_Voice.GetComponent<TextMeshProUGUI> ().font = Resources.Load<TMP_FontAsset> ("Font/PinYin_SDF");
			text_Voice.GetComponent<TextMeshProUGUI> ().fontStyle = FontStyles.Italic ^ FontStyles.Bold;

		}
		else {
			//加载中文字体
			text_Voice.GetComponent<TextMeshProUGUI> ().font = Resources.Load<TMP_FontAsset> ("Font/Book01_SDF");
			text_Voice.GetComponent<TextMeshProUGUI> ().fontStyle = FontStyles.Normal ^ FontStyles.Italic;
		}
	}
	public void CreateWord (string text, float duration)
	{
		//print(text);
		ChangeFont ();
		text_Voice.text = PinYin.GetShowText (text);
		MoveWord (rightPosX, leftPosX, duration);
	}
	public void DeleteWord ()
	{
		text_Voice.text = string.Empty;
	}

	public static void CreateVoiceStar ()
	{
		if (instance == null)
			return;

		instance._CreateVoiceStar ();
	}
	void _CreateVoiceStar ()
	{
		ps_Voice.Clear ();
		ps_Voice.Play ();
	}


	public static void HighLightWord (List<HitObject> hoList, int index, float preshowLength = 0)
	{
		instance._HighLightWord (hoList, index, preshowLength);
	}
	public void _HighLightWord (List<HitObject> hoList, int index, float preshowLength)
	{
		//float time = (hoList[index].StartMilliSecond - AudioManager.Instance.GetBgmTime() * 1000) / 1000f;

		//tran_OuterCircle.localScale = Vector3.one * CorePlaySettings.Instance.m_VoiceOutCircleScaler;
		//float standardLength = CorePlayManager.Instance.CurrentTB * CorePlaySettings.Instance.m_VoiceTBParam;
		//float length = time > standardLength ? standardLength : time;
		//sp_LightCircle.transform.DOScale(endScale, length)
		//							 .OnComplete(() =>
		//						   {
		//							   if (CorePlayData.CurrentSong.AutoCreateWordAudio)
		//								   AudioManager.Instance.PlayWord(hoList[index].Word);
		//							   sp_LightCircle.color = new Color(1, 1, 1, 0);
		//						   });
	}

	public void Restart ()
	{
		Hide ();
		DeleteWord ();
	}
	#endregion



	void MoveWord (float fromPosX, float toPosX, float duration)
	{
		wordSequence.Complete ();
		wordSequence = DOTween.Sequence ();
		rectTran_Voice.anchoredPosition = new Vector2 (fromPosX, rectTran_Voice.anchoredPosition.y);

		wordSequence.Append (rectTran_Voice.DOAnchorPosX (0, 0.5f).SetEase (Ease.Linear));
		wordSequence.Append (rectTran_Voice.DOAnchorPosX (toPosX, duration - 0.5f).SetEase (Ease.InQuart));

	}

}
//VoiceController













