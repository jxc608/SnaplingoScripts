using Snaplingo.UI;
using System.Collections.Generic;
using Snaplingo.SaveData;
using UnityEngine.UI;
using UnityEngine;
using LitJson;

class StartSceneNode : Node
{
	bool clicked;
	Login login_app;
    #region 多语言游戏物体
    public Text loginUserName;
    public Text loginPassWord;
    public Text loginUserNameTip;
    public Text loginPassWordTip;
    public Image guestButton;
    public Image registerButton;
    public Text version;
    public Button changeLanguageButton;
    public GameObject login_Object;
    public Button button_PlayGame;
	public LoadingFlower m_loadflower;
	#endregion

	[SerializeField] private Sprite m_sure;
    [SerializeField] private Sprite m_recontact;
    [SerializeField] private Sprite m_sure_en;
    [SerializeField] private Sprite m_recontact_en;

	private static bool m_bLoadConfig = false;

	//public RectTransform[] m_ResizeGroup;
	public override void Init(params object[] args)
	{
		AudioController.PlayMusic("StartMusic");
		clicked = false;
		base.Init();
		ResizeGroup();
		GetVersion();

		GlobalConst.CurrentServerTime = SystemTime.timeSinceLaunch;
        changeLanguageButton.onClick.AddListener(() =>
        {
            if(LanguageManager.languageType == LanguageType.Chinese)
            {
                LanguageManager.languageType = LanguageType.English;
                InitGameObjectUI();
				changeLanguageButton.GetComponent<Image>().sprite = Resources.Load<Sprite>("language/UI_enter_012");
                //changeLanguageButton.transform.GetChild(0).GetComponent<Text>().text = "English";
            }
            else
            {
                LanguageManager.languageType = LanguageType.Chinese;
                InitGameObjectUI();
				changeLanguageButton.GetComponent<Image>().sprite = Resources.Load<Sprite>("language/UI_enter_011");
                //changeLanguageButton.transform.GetChild(0).GetComponent<Text>().text = "简体中文"; //6 245 440
            }
            if(FindObjectOfType<RegisterNode>()!=null)
            {
                FindObjectOfType<RegisterNode>().InitGameObjectUI();
            }
        });

		login_Object.SetActive(false);
		button_PlayGame.gameObject.SetActive(false);

		CheckVersion();

	}

	private void CheckVersion()
    {
        //hide login ui 
        login_Object.SetActive(false);
        button_PlayGame.gameObject.SetActive(false);

#if UNITY_IOS || UNITY_EDITOR
        JsonData data = new JsonData();
        DancingWordAPI.Instance.GetGameInfo(data, OnGetGameInfoSuccessiOS, OnGetGameInfoFailiOS);
#endif

    }

    /// <summary>
    /// 获取游戏信息的回调函数，这个版本只有版本号，之后热更新一类的可以放在里面
    /// </summary>
    /// <param name="result">Result.</param>
    private void OnGetGameInfoSuccessiOS(string result)
    {
        //success
        JsonData resultData = JsonMapper.ToObject(result);

        bool bSuccess = false;
        if (null == resultData["status"] || !bool.TryParse(resultData["status"].ToString(), out bSuccess))
        {
			LogManager.LogError("[OnGetGameInfoSuccess]result has no statues\n" ,
                           "server result" , result);
            return;
        }

        JsonData data = resultData["data"];
        if (bSuccess)
        {
            string[] curVersion = AppInfo.AppVersion.Split('.');
            string[] serverVersion = data["version"].ToString().Split('.');
            string[] serverMinVersion = data["minVersion"].ToString().Split('.');
            //测试代码
            //string testVersion = "1.05.02";
            //serverMinVersion = testVersion.Split('.');

            if (curVersion.Length != 3 || serverVersion.Length != 3 || serverMinVersion.Length != 3)
            {
				LogManager.LogError("[OnGetGameInfoSuccess]version invalid\n" ,
                "cur app version:" , AppInfo.AppVersion + "\n" ,
                "server result" , result);
                return;
            }
            //version com
            bool bMatch = true;
            for (int i = 0; i < 3; ++i)
            {
                if (int.Parse(curVersion[i]) < int.Parse(serverMinVersion[i]))
                {
                    bMatch = false;
                    break;
                }
            }

            if (bMatch)
            {
                ShowMainGame();
            }
            else
            {
                CommonPromptNode node = PageManager.Instance.CurrentPage.AddNode<CommonPromptNode>(true) as CommonPromptNode;
                node.InitPrompt(true, SelfType.Prompt, SelfType.NeedUpdateApp,
                                LanguageManager.languageType == LanguageType.Chinese ? m_sure : m_sure_en, null,
                                onClickUpdateApp, null, false);
            }

        }
        else
        {
            //not success
            CommonPromptNode node = PageManager.Instance.CurrentPage.AddNode<CommonPromptNode>(true) as CommonPromptNode;
            node.InitPrompt(true, SelfType.Prompt, SelfType.NeedRecontact,
                            LanguageManager.languageType == LanguageType.Chinese ? m_recontact : m_recontact_en, null,
                            onClickRecontact, null, true);
        }
    }

    private void onClickUpdateApp()
    {
        Application.OpenURL("https://itunes.apple.com/cn/app/dancingword/id1347690130?mt=8");
    }

    private void onClickRecontact()
    {
        CheckVersion();
    }

    private void OnGetGameInfoFailiOS()
    {
        CommonPromptNode node = PageManager.Instance.CurrentPage.AddNode<CommonPromptNode>(true) as CommonPromptNode;
        node.InitPrompt(true, SelfType.Prompt, SelfType.NeedRecontact,
                        LanguageManager.languageType == LanguageType.Chinese ? m_recontact : m_recontact_en, null,
                        onClickRecontact, null, true);
    }

    private void ShowMainGame()
    {
		GetLogin();
		m_loadflower.gameObject.SetActive(true);
        m_loadflower.Init();

        if (!m_bLoadConfig)
        {
            ResourcesManager.Instance.LoadAllConfig(onConfigload);
        }
        else
        {
            onConfigload();
        }

    }


	private void onConfigload()
    {
		LogManager.LogWarning("config loaded");
        CorePlayData.CalcFirstLevelScore();
		m_loadflower.gameObject.SetActive(false);
		IsFirstIn();
        button_PlayGame.onClick.AddListener(FirstLevel);
		m_bLoadConfig = true;
    }

	private void FirstLevel()
	{
		//LevelData levelData= LevelConfig.AllLevelDic[1001];
		//LoadSceneManager.Instance.LoadPlayScene(levelData);

		LoadSceneManager.Instance.LoadNormalScene("CreateUserRole");
	}
    private void IsFirstIn()
    {
		if(LoginScene.isFirstGoingGame)
        {
            //第一次打开应用
            login_Object.SetActive(false);   
            button_PlayGame.gameObject.SetActive(true);
        }
        else
        {
            //不是第一次打开应用
            login_Object.SetActive(true);
            button_PlayGame.gameObject.SetActive(false);
        }
    }
    private void InitGameObjectUI()
    {
        loginUserName.text = LanguageManager.Instance.GetEnumValue(loginUserName.gameObject);
        loginPassWord.text = LanguageManager.Instance.GetEnumValue(loginPassWord.gameObject);
        loginUserNameTip.text = LanguageManager.Instance.GetEnumValue(loginUserNameTip.gameObject);
        loginPassWordTip.text = LanguageManager.Instance.GetEnumValue(loginPassWordTip.gameObject);
        version.text = LanguageManager.Instance.GetEnumValue(version.gameObject);
        guestButton.sprite = Resources.Load<Sprite>("language/" + LanguageManager.Instance.GetEnumValue(guestButton.gameObject));
        guestButton.SetNativeSize();
        registerButton.sprite = Resources.Load<Sprite>("language/" + LanguageManager.Instance.GetEnumValue(registerButton.gameObject));
        registerButton.SetNativeSize();
        button_PlayGame.transform.GetChild(0).GetComponent<Text>().text = LanguageManager.Instance.GetEnumValue(button_PlayGame.transform.GetChild(0).gameObject);
    }
	private const float DefaultWidth = 2048f;
	private const float DefaultHeight = 1536f;
	void ResizeGroup()
	{
		float screenWidth = PageManager.Instance.GetComponent<RectTransform>().sizeDelta.x;
		float screenHeight = PageManager.Instance.GetComponent<RectTransform>().sizeDelta.y;

		float ratio = screenWidth / DefaultWidth;
		float reSizeRatio = screenHeight / DefaultHeight / ratio;

		//foreach (RectTransform rt in m_ResizeGroup)
		//{
		//	rt.localScale = Vector3.one / reSizeRatio;
		//}
	}

	void GetVersion()
	{
		GameObject.Find("Version").GetComponent<Text>().text = AppInfo.AppVersion;
	}

	//======================登陆类型==========================
	void GetLogin()
	{
		login_app = transform.Find("Object_Login").GetComponent<Login>();
		if (login_app != null)
		{
			login_app.loginDelegate += LoginDelegateHandler;
		}
	}

	void LoginDelegateHandler()
	{
		login_app.loginDelegate -= LoginDelegateHandler;
		clicked = true;
		if (GlobalConst.LoginToApp == false)
		{
			LogManager.Log(" 未登陆app需要走这段逻辑 ");
			LoadUserData();
		}
		if (!AudioController.IsPlaying("Click"))
			AudioController.Play("Click");
		AudioController.StopMusic();
		//print("okokookokokokok");
		//LoadSceneManager.Instance.LoadNormalScene(LoadSceneManager.BookSceneName);
		LogManager.Log("登录成功");
		SaveDataUtils.Load<IAPVerifyProxy>();
	}
    

	void Update()
	{
		if(m_loadflower.gameObject.active == true)
		{
			m_loadflower.UpdateProgress(ResourcesManager.Instance.ConfigProgress);
		}

		//if (InputUtils.OnPressed() && clicked == false)
		//{
		//    clicked = true;
		//    LoadUserData();
		//    if (!AudioController.IsPlaying("Click"))
		//        AudioController.Play("Click");
		//    AudioController.StopMusic();
		//    LoadSceneManager.Instance.LoadSceneAsync("BookScene");
		//}
	}

	void DeleteNode()
	{
		Close(true);
	}

	private void LoadUserData()
	{
		if (string.IsNullOrEmpty(SelfPlayerData.DeviceID))
		{
			//var preNames = Resources.Load<TextAsset>("Names/preNames").text.Split('\n');
			var postNames = Resources.Load<TextAsset>("Names/postNames").text.Split('\n');

			System.Random ran = new System.Random();
			//int a = ran.Next(0, preNames.Length - 1);
			int b = ran.Next(0, postNames.Length - 1);

			//SelfPlayerData.Nickname = preNames[a] + " " + postNames[b];
			SelfPlayerData.Nickname = postNames[b];
		}

		bool m_NetWork = MiscUtils.IsOnline();
		if (m_NetWork)
		{
			if (string.IsNullOrEmpty(SelfPlayerData.DeviceID))
			{
				LoginRpcProxy.getInstance().LoginByAnonymous();
			}
			else
			{
				LoginRpcProxy.getInstance().GetGuestUserInfoByDeviceId();
			}
		}
		else
		{
			//if(string.IsNullOrEmpty(SelfPlayerData.ModelId)||"0".Equals(SelfPlayerData.ModelId)||string.IsNullOrEmpty(SelfPlayerData.EmotionId) || "0".Equals(SelfPlayerData.EmotionId))
			//{
			//	//当本地也没有角色信息则跳转创建角色场景
			//	LoadSceneManager.Instance.LoadNormalScene("CreateUserRole");
			//}
			//else
			//{
			//	//本地有角色的信息则跳转选关界面
			//	LoadSceneManager.Instance.LoadNormalScene("BookScene");
			//}
		}

		// 临时关闭
		//UpdataLevelScore();
	}

	//private void UpdataLevelScore()
	//{
	//	List<int> levelId = CacheData.Instance.GetCacheDataLevelID();
	//	if (levelId == null)
	//		return;
	//	for (int i = 0; i < levelId.Count; i++)
	//	{
	//		StaticData.LevelID = levelId[i];
	//		if (MiscUtils.IsOnline())
	//			HttpHandler.UploadScore(DeleteCacheData, true, false);
	//	}
	//}


}
