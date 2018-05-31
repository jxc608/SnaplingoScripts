using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Snaplingo.UI;


/// <summary>
/// 游戏的核心管理类，之后所有的逻辑都依赖于这个类执行
/// 现有的逻辑之后慢慢往里挪
/// </summary>
public class GameManager : MonoBehaviour
{
	private static LanguageManager m_languageManager;
	private static ResourcesManager m_resourceManager;

	//重连提示框的重连按钮引用，现在先扔到这，之后换了NGUI后采用NGUI的换name方式来做
	[SerializeField] private Sprite SpriteSure;
	[SerializeField] private Sprite SpriteSuer_EN;
	private static Sprite m_spriteSure;
	private static Sprite m_spriteSure_EN;

	void Awake()
	{
		DontDestroyOnLoad(this.gameObject);

		m_resourceManager = ResourcesManager.Instance;
		m_languageManager = LanguageManager.Instance;

        
        //这种代码以后肯定要干掉
		m_spriteSure = SpriteSure;
		m_spriteSure_EN = SpriteSuer_EN;

		StartCoroutine(InitManagers());
	}
    
	private IEnumerator InitManagers()
	{
		yield return m_languageManager.Init(this);
        //resource manager的初始化也应该改为异步 TODO
		m_resourceManager.Init(this, StartScene.GameStart);
	}

	// Use this for initialization
	void Start()
	{
		
	}

	// Update is called once per frame
	void Update()
	{
		if(m_bNeedLogout)
		{
			m_bNeedLogout = false;
			LoginOnOtherDivices();
		}
	}

	#region 重新登录
	public static void GameLogOut()
	{
		SnapHttpConfig.LOGINED_APP_TOKEN = "";
		UserVoProxy.instance().ClearUserVoTemp();
		CorePlayData.isFirstTime = true;
		LoadSceneManager.Instance.LoadNormalScene("LoginScene");
	}

	public static bool m_bNeedLogout = false;
	public static void LoginOnOtherDivices(bool bReloadNow = true)
	{
		if(bReloadNow)
		{
			CommonPromptNode node = PageManager.Instance.CurrentPage.AddNode<CommonPromptNode>(true) as CommonPromptNode;
            node.InitPrompt(true, SelfType.Prompt, SelfType.LoginOnOtherDivice,
                            LanguageManager.languageType == LanguageType.Chinese ? m_spriteSure : m_spriteSure_EN, null,
                            GameLogOut, null, true);
		}
		else
		{
			m_bNeedLogout = true;
		}

	}
    #endregion
}
