using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class ShowLoading : GeneralUIManager
{

	float fadeInDuration = .5f;
	float fadeOutDuration = .5f;

	#region [ --- Object References --- ]
	[Header(" --- Object References ---")]
	public Image image_leaf;
    #endregion
    //#region
    //public Text loadText;
    //public Text tipText;
    //#endregion

    #region Instance

    private static ShowLoading _instance;

	public static ShowLoading Instance
	{
		get
		{
			if (_instance == null)
			{
				m_Loading = Instantiate(ResourceLoadUtils.Load<GameObject>("UI/Utils/Loading"));
				_instance = m_Loading.GetComponent<ShowLoading>();
				m_Loading.name = _instance.GetType().ToString();
				_instance.AddToUIManager();
				_instance.SetPriority(Priority.Loading);
				_instance.Init();
				RefreshSortingOrder();
			}
			return _instance;
		}
	}

	public void OnDestory()
	{
		_instance = null;
	}

	private Image m_GeneralBG;
	private Image m_TeachBG;
	private Image m_Spinner;
	private Text m_Text;
	private GameObject m_General;
	private GameObject m_Teach;
	private GameObject m_TeachImages;
	private Text m_TeachTitle;
	private Text m_TeachContent;
	private static GameObject m_Loading;

	void Init()
	{
		m_General = transform.Find("General").gameObject;
		m_Teach = transform.Find("Teach").gameObject;
		m_TeachImages = transform.Find("Teach/Images").gameObject;
		m_GeneralBG = m_General.GetComponent<Image>();
		m_TeachBG = m_Teach.GetComponent<Image>();
		m_Spinner = transform.Find("General/Image").GetComponent<Image>();
		m_Text = transform.Find("General/Text").GetComponent<Text>();
		m_TeachTitle = transform.Find("Teach/Title").GetComponent<Text>();
		m_TeachContent = transform.Find("Teach/Content").GetComponent<Text>();
		m_Status = FadeStatus.None;
	}
    //private void InitGameObjectUI()
    //{
    //    loadText.text = LanguageManager.Instance.GetValueByKey(loadText.GetComponent<LanguageFlagClass>().selfType.ToString(), LanguageManager.languageType);
    //    tipText.text = LanguageManager.Instance.GetValueByKey(tipText.GetComponent<LanguageFlagClass>().selfType.ToString(), LanguageManager.languageType);
    //}
	#endregion

	public void Show()
	{
		if (m_Loading != null)
		{
			m_Loading.SetActive(true);
            //InitGameObjectUI();
		}
	}

	public void Close()
	{
		m_Status = FadeStatus.None;
		if (m_Loading != null)
		{
			m_Loading.SetActive(false);
		}
	}

	private enum FadeStatus { None, FadingIn, Faded, FadingOut, TeachFadingIn, Teaching, TeachingFadeOut }
	private FadeStatus m_Status;
	void Update()
	{
		switch (m_Status)
		{
			case FadeStatus.FadingIn:
				FadingIn();
				break;
			case FadeStatus.TeachFadingIn:
				TeachFadingIn();
				break;
			case FadeStatus.FadingOut:
				FadingOut();
				break;
			case FadeStatus.TeachingFadeOut:
				TeachingFadingOut();
				break;
		}
	}



	#region [ Public --- ]
	public void FadeIn(float duration)
	{
		if (m_Status == FadeStatus.None)
		{
			Show();
			m_General.SetActive(true);
			m_Teach.SetActive(false);
			m_Status = FadeStatus.FadingIn;
			m_Timer = 0;
			fadeInDuration = duration;
		}
	}
	public void TeachFadeIn(string content, float duration)
	{
		if (m_Status == FadeStatus.None)
		{
			Show();
			m_General.SetActive(false);
			m_Teach.SetActive(true);
			m_TeachContent.text = content;
			m_Status = FadeStatus.TeachFadingIn;
			m_Timer = 0;
			fadeInDuration = duration;
            //InitGameObjectUI();
		}
	}
	private Action m_FadeOutCallback;
	public void FadeOut(Action callback)
	{
		m_FadeOutCallback = callback;
		m_Timer = 0;
		if (m_Status == FadeStatus.FadingIn || m_Status == FadeStatus.Faded)
		{
			m_Status = FadeStatus.FadingOut;
		}
		else if (m_Status == FadeStatus.TeachFadingIn || m_Status == FadeStatus.Teaching)
		{
			m_Status = FadeStatus.TeachingFadeOut;
		}
	}
	#endregion


	private float m_Timer;
	void FadingIn()
	{
		m_Timer += Time.deltaTime;
		float lerp = m_Timer / fadeInDuration;
		m_GeneralBG.color = Color.Lerp(new Color(0, 0, 0, 0), new Color(0, 0, 0, 1), lerp);
		Color temp = Color.Lerp(new Color(1, 1, 1, 0), new Color(1, 1, 1, 1), lerp);
		m_Spinner.color = temp;
		image_leaf.color = temp;
		m_Text.color = temp;
		if (lerp >= 1)
		{
			m_Status = FadeStatus.Faded;
		}
	}
	void TeachFadingIn()
	{
		m_Timer += Time.deltaTime;
		float lerp = m_Timer / fadeInDuration;
		Color temp = Color.Lerp(new Color(1, 1, 1, 0), new Color(1, 1, 1, 1), lerp);
		m_TeachBG.color = Color.Lerp(new Color(0, 0, 0, 0), new Color(0, 0, 0, 1), lerp);
		m_TeachTitle.color = temp;
		m_TeachContent.color = temp;
		foreach (Transform t in m_TeachImages.transform)
		{
			Image image = t.GetComponent<Image>();
			image.color = temp;
		}
		if (lerp >= 1)
		{
			m_Status = FadeStatus.Teaching;
		}
	}

	void FadingOut()
	{
		m_Timer += Time.deltaTime;
		float lerp = m_Timer / fadeOutDuration;
		m_GeneralBG.color = Color.Lerp(new Color(0, 0, 0, 1), new Color(0, 0, 0, 0), lerp);
		Color temp = Color.Lerp(new Color(1, 1, 1, 1), new Color(1, 1, 1, 0), lerp);
		m_Spinner.color = temp;
		image_leaf.color = temp;
		m_Text.color = temp;
		if (lerp >= 1)
		{
			if (m_FadeOutCallback != null)
			{
				m_FadeOutCallback.Invoke();
			}
			Close();
		}

		//LogManager.Log("FadingOut 6>>>> " , Time.realtimeSinceStartup);

	}

	void TeachingFadingOut()
	{
		m_Timer += Time.deltaTime;
		float lerp = m_Timer / fadeOutDuration;
		Color temp = Color.Lerp(new Color(1, 1, 1, 1), new Color(1, 1, 1, 0), lerp);
		m_TeachBG.color = Color.Lerp(new Color(0, 0, 0, 1), new Color(0, 0, 0, 0), lerp);
		m_TeachTitle.color = temp;
		m_TeachContent.color = temp;
		foreach (Transform t in m_TeachImages.transform)
		{
			Image image = t.GetComponent<Image>();
			image.color = temp;
		}
		if (lerp >= 1)
		{
			if (m_FadeOutCallback != null)
			{
				m_FadeOutCallback.Invoke();
			}
			Close();
		}
	}


}
