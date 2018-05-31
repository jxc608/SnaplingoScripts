using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using System.Collections;
using TMPro;
using Chinese2PinYin;

public class RhythmController : MonoBehaviour, IPointerDownHandler
{
	public enum State
	{
		Null,
		Default,
		Right,
		Wrong,
		Perfect,
	}

	#region [ --- Property --- ]
	string EnglishWord {
		get { return (int)first_word[0] <= 127 ? first_word : second_word; }
	}
	string ChineseWord {
		get { return (int)first_word[0] > 127 ? first_word : second_word; }
	}

	float ComboRate {
		get { return (float)CorePlayManager.Combo / EffectSettings.Instance.maxCombo; }
	}

	[Header (" --- Setting ---")]
	public Color colorRight;
	public Color colorWrong;
	public Color[] textColors;

	//0.4f
	public const float RightAnimDelay = 0.25f;
	public const float WrongAnimDelay = 0.25f;
	public const float BossWordWrongAnimDelay = 1f;
	const float EdgeWidth = 0.1f;
	float preShowTime;
	string first_word;
	string second_word;
	public State curState;
	#endregion


	#region [ --- Object References --- ]
	[Header (" --- Object References ---")]
	public SpriteRenderer sp_base;
	public SpriteRenderer sp_textField;

	//
	public ColorBlink colorBlink;
	public TextMeshPro text_word;
	public AlignTextBoundPro align;


	public Transform tran_base, tran_wordField;
	public GameObject prefab_highLight;


	// State
	public GameObject obj_wrongDisplay;
	public SpriteRenderer[] sp_RightColors, sp_WrongColors;
	public MonoBehaviour[] comp_WrongEnables;
	public Sprite sp_Right, sp_Default;

	BoxCollider coll;
	RhythmObject _tap;
	#endregion


	#region [ --- Implement --- ]
	public void OnPointerDown (PointerEventData eventData)
	{
		if (_tap != null && gameObject.activeInHierarchy) _tap.OnPointerDown ();
	}
	#endregion


	#region [ --- Mono --- ]
	void Awake ()
	{
		obj_wrongDisplay.SetActive (false);
		CorePlaySceneManager.bossWordClickWrongEvent.AddListener (WordGoBack);
		CorePlaySceneManager.bossWordRe_EnterEvent.AddListener (WordEnter);
		CorePlaySceneManager.bossSentenceCompleteEvent.AddListener (OnBossSentenceComplete);
		coll = GetComponent<BoxCollider> ();
	}
	void OnDisable ()
	{
		DOTween.Complete (transform);
		//LogManager.Log(" --- [ {0} ] 句子消失 - {1}", first_word, Time.frameCount);
	}
	void OnDestroy ()
	{
		CorePlaySceneManager.bossWordClickWrongEvent.RemoveListener (WordGoBack);
		CorePlaySceneManager.bossWordRe_EnterEvent.RemoveListener (WordEnter);
		CorePlaySceneManager.bossSentenceCompleteEvent.RemoveListener (OnBossSentenceComplete);
	}
	#endregion




	#region [ --- Init --- ]

	public void Init (RhythmObject tap,
					 string _word,
					 string _ShowWord,
					 Vector3 targetPos,
					 EffectInOutTiming inOut = EffectInOutTiming.Default)
	{
		//print(_word);
		//print(_ShowWord);
		// 存数值
		_tap = tap;
		first_word = _word;
		second_word = _ShowWord;

		preShowTime = 0.001f * CorePlaySettings.Instance.m_PreShowTimeLength;

		_SetState (State.Default);

		// 设置文字组建
		text_word.text = string.Empty;
		text_word.color = Color.white;
		tran_base.gameObject.SetActive (false);

		// 处理提前进场
		if (inOut == EffectInOutTiming.Both || inOut == EffectInOutTiming.AheadIn)
		{
			//LogManager.Log("Go = 处理提前进场");
			WordEnter ();
		}
		else
		{
			WordFadeIn ();
		}

		transform.localPosition = targetPos;
		coll.enabled = true;
	}
	#endregion


	#region [ --- Event --- ]
	public void On_Show ()
	{
		if (!gameObject.activeInHierarchy)
			gameObject.SetActive (true);

		//LogManager.Log("OnShowWord = " , Time.frameCount);
		_SetState (State.Default);
		coll.enabled = true;
	}
	void OnBossSentenceComplete ()
	{
		if (!gameObject.activeInHierarchy)
			return;
		//LogManager.Log("Go = OnBossSentenceComplete");
		SceneController.instance.OnLoadMissile (transform.position);
	}
	#endregion



	#region [ --- Public --- ]
	public void WordEnter ()
	{
		if (!gameObject.activeInHierarchy)
			return;
		StartCoroutine (_CorWordEnter ());
	}
	IEnumerator _CorWordEnter ()
	{
		tran_base.gameObject.SetActive (true);
		_SetState (State.Default);
		coll.enabled = true;
		yield return null;
		align.ReSize ();
		coll.size = new Vector3 (sp_textField.size.x, sp_textField.size.y, 1);
	}

	public void WordFadeIn () { StartCoroutine (_CorWordFadeIn ()); }
	IEnumerator _CorWordFadeIn ()
	{
		curState = State.Default;
		//LogManager.Log(" [ {0} ] 生成 - {1}", first_word, Time.frameCount);
		tran_base.gameObject.SetActive (true);
		tran_base.localScale = 0.2f * Vector3.one;
		tran_base.DOScale (Vector3.one, 0.2f)
					 .SetEase (Ease.InQuart);
		yield return new WaitForSeconds (0.2f);
		_SetState (State.Default);
		yield return null;
		align.ReSize ();
		coll.size = new Vector3 (sp_textField.size.x, sp_textField.size.y, 1);
		yield return new WaitForSeconds (preShowTime - 0.2f);
		coll.enabled = true;
		//LogManager.Log(" [ {0} ] Enter 完成 - {1}", first_word, Time.frameCount);
	}
	public void WordGoBack ()
	{
		if (!gameObject.activeInHierarchy)
			return;
		text_word.text = string.Empty;
		coll.enabled = false;
		tran_base.DOScale (0.2f * Vector3.one, BossWordWrongAnimDelay)
			 .SetEase (Ease.InQuart);
	}
	public void On_HighLight (string word, float duration = 0)
	{
		if (first_word != word || curState != State.Default) return;
		//LogManager.Log("Go = On_HighLight ",duration);
		StartCoroutine (_CorHighLight (duration));
	}
	IEnumerator _CorHighLight (float duration)
	{
		float delay = Mathf.Max (0, CorePlaySettings.Instance.m_PreShowTimeLength - CorePlaySettings.Instance.m_highLightAheadTime);
		yield return new WaitForSeconds (delay * .001f);

		GameObject obj = Instantiate (prefab_highLight, transform.position, Quaternion.identity, transform);
		var rander = obj.GetComponentInChildren<SpriteRenderer> ();
		Color color = rander.color;
		rander.DOFade (0.2f, duration);
		Destroy (obj, duration);
	}
	/// 完美
	public void On_Perfect ()
	{
		if (curState != State.Default) return;
		CorePlaySceneManager.tapRightEvent.Invoke (transform.position);
		StopAllCoroutines ();
		StartCoroutine (_CorPerfect ());
	}
	IEnumerator _CorPerfect ()
	{
		_SetState (State.Perfect);
		yield return new WaitForSeconds (RightAnimDelay);
		_SetState (State.Default);
	}
	/// 正确 
	public void On_Right ()
	{
		if (curState != State.Default) return;
		CorePlaySceneManager.tapRightEvent.Invoke (transform.position);
		StopAllCoroutines ();
		StartCoroutine (_CorCorrect ());
	}
	IEnumerator _CorCorrect ()
	{
		_SetState (State.Right);
		yield return new WaitForSeconds (RightAnimDelay);
		_SetState (State.Default);
	}
	/// 错误
	public void On_Wrong ()
	{
		StopAllCoroutines ();
		StartCoroutine (_CorWrong ());
	}
	IEnumerator _CorWrong ()
	{
		_SetState (State.Wrong);
		yield return new WaitForSeconds (WrongAnimDelay);
		_SetState (State.Default);
	}






	public void OnSentenceCorrect ()
	{
		//LogManager.LogWarning(" 整句全对 " , Time.time);
		coll.enabled = false;
		tran_base.gameObject.SetActive (false);
	}
	public void OnSentenceWrong ()
	{
		//LogManager.LogWarning(gameObject.GetHashCode() , " 整句失败 " , Time.time);
		coll.enabled = false;
		tran_base.gameObject.SetActive (false);
		text_word.text = string.Empty;
	}

	public void Destroy ()
	{
		StopAllCoroutines ();
		ObjectPool.DeleteOne ("RhythmController", gameObject);
	}

	public void SetPosition (Vector3 pos)
	{
		transform.localPosition = pos;
	}
	public Vector2 GetScaler ()
	{
		return sp_textField.size;
	}
	public Vector3 GetPosition ()
	{
		return transform.localPosition;
	}
	#endregion
	private void ChangeFont ()
	{
		if (PinYin.isShowPinYin)
		{
			//加载拼音字体
			text_word.GetComponent<TextMeshPro> ().font = Resources.Load<TMP_FontAsset> ("Font/PinYin_SDF");
			text_word.GetComponent<TextMeshPro> ().fontStyle = FontStyles.Bold;
		}
		else
		{
			//加载中文字体
			text_word.GetComponent<TextMeshPro> ().font = Resources.Load<TMP_FontAsset> ("Font/Book01_SDF");
			text_word.GetComponent<TextMeshPro> ().fontStyle = FontStyles.Normal;
		}
	}
	void _SetState (State state)
	{
		curState = state;
		ChangeFont ();
		DOTween.Kill (tran_base);
		DOTween.Kill (text_word.transform);
		DOTween.Kill (sp_base.transform);

		switch (state)
		{
			case State.Default:
				text_word.text = PinYin.GetShowText (first_word);
				text_word.color = Color.white;

				//text_word.Rebuild();
				text_word.transform.localScale = Vector3.one;
				text_word.transform.localPosition = Vector3.zero;
				sp_textField.transform.localScale = Vector3.one;
				sp_textField.transform.localPosition = Vector3.zero;
				obj_wrongDisplay.SetActive (false);
				sp_base.sprite = sp_Default;
				sp_base.transform.localScale = Vector3.one;
				sp_textField.transform.localPosition = Vector3.zero;
				tran_base.localScale = Vector3.one;
				tran_base.localPosition = Vector3.zero;
				//do_highLight.gameObject.SetActive(false);

				foreach (var item in comp_WrongEnables)
				{
					item.enabled = false;
				}
				foreach (var item in sp_RightColors)
				{
					item.color = Color.white;
				}
				foreach (var item in sp_WrongColors)
				{
					item.color = Color.white;
				}
				break;
			case State.Perfect:
				// Effects
				//EffectManager.Play("TapStar", transform.position, ComboRate);
				if (CorePlayManager.Combo > 0 && (CorePlayManager.Combo % EffectSettings.Instance.comboInterval) == 0)
				{
					PlayComboTenEffect ();
				}
				else
				{
					EffectManager.Play ("LightningExplosion1", transform.position, ComboRate);
				}


				AudioController.Play ("Tap");
				text_word.color = Color.yellow;
				//text_word.Rebuild();
				text_word.text = PinYin.GetShowText (second_word);
				text_word.transform.localScale = Vector3.one;
				text_word.transform.DOScale (2f, RightAnimDelay).SetEase (Ease.OutQuart);

				sp_textField.transform.DOScale (1.5f, RightAnimDelay).SetEase (Ease.OutQuart);
				sp_textField.transform.DOLocalMoveY (.3f, RightAnimDelay).SetEase (Ease.OutQuart);
				sp_textField.transform.DOPunchRotation (new Vector3 (0, 0, -15), RightAnimDelay);

				obj_wrongDisplay.SetActive (false);
				sp_base.sprite = sp_Right;
				sp_base.transform.localScale = Vector3.one;
				sp_base.transform.DOScale (1.2f, 0.5f).SetEase (Ease.OutSine);

				foreach (var item in comp_WrongEnables) { item.enabled = false; }
				foreach (var item in sp_RightColors) { item.color = colorRight; }
				foreach (var item in sp_WrongColors) { item.color = Color.white; }
				break;
			case State.Right:
				// Effects
				//EffectManager.Play("TapStar", transform.position, ComboRate);
				if (CorePlayManager.Combo > 0 && (CorePlayManager.Combo % EffectSettings.Instance.comboInterval) == 0)
				{
					PlayComboTenEffect ();
				}

				text_word.text = PinYin.GetShowText (second_word);
				text_word.transform.localScale = Vector3.one;
				text_word.transform.DOScale (1.3f, RightAnimDelay);
				sp_textField.transform.DOScale (1.2f, RightAnimDelay).SetEase (Ease.OutQuart);
				sp_textField.transform.DOLocalMoveY (.3f, RightAnimDelay).SetEase (Ease.OutQuart);
				//sp_textField.transform.localPosition = new Vector3(0, 1.2f, 0);
				obj_wrongDisplay.SetActive (false);
				//sp_base.sprite = sp_Right;
				sp_base.transform.localScale = Vector3.one;
				sp_base.transform.DOScale (1.1f, 0.5f).SetEase (Ease.OutQuart);

				foreach (var item in comp_WrongEnables) { item.enabled = false; }
				foreach (var item in sp_RightColors) { item.color = colorRight; }
				foreach (var item in sp_WrongColors) { item.color = Color.white; }
				break;
			case State.Wrong:
				AudioController.Play ("TapWrong");
				text_word.text = " ";
				sp_base.sprite = sp_Default;
				obj_wrongDisplay.SetActive (true);
				tran_base.DOShakePosition (1, new Vector3 (.3f, .3f, 0), 15);

				foreach (var item in comp_WrongEnables) { item.enabled = true; }
				foreach (var item in sp_RightColors) { item.color = Color.white; }
				foreach (var item in sp_WrongColors) { item.color = colorWrong; }
				break;
		}
	}


	public static int comboTenEffectIndex = 0;
	void PlayComboTenEffect ()
	{
		//float pro = Random.value;
		//int amount = EffectSettings.Instance.shackRate + EffectSettings.Instance.comboBRate;
		//float proA = (float)EffectSettings.Instance.shackRate / amount;
		//if (pro <= proA)
		//{
		//	ScreenShake.Instance.Shake(.1f);
		//	EffectManager.Play("PillarBlast", transform.position);
		//}
		//else
		//{
		//	ScreenShake.Instance.Shake(.1f);
		//	EffectManager.Play("Confetti", new Vector3(0, 7.5f, 0));
		//}


		ScreenShake.Instance.Shake (.1f);
		switch (comboTenEffectIndex)
		{
			case 0:
				EffectManager.Play ("PillarBlast", transform.position);
				break;
			case 1:
				EffectManager.Play ("Confetti", new Vector3 (0, 7.5f, 0));
				break;
			case 2:
				EffectManager.Play ("LightingWaves3", transform.position);
				break;
			default:
				break;
		}
		EffectManager.instance.SwitchPostProcessingProfile ();
		comboTenEffectIndex = (comboTenEffectIndex + 1) % 3;
	}



}
//RhythmController













