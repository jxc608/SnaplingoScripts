using UnityEngine;
using UnityEngine.UI;
using Snaplingo.UI;
using Snaplingo.SaveData;

public class Login : MonoBehaviour
{

	public enum LoginType
	{
		Login = 0,
		NotLogin = 1
	}

	public delegate void LoginDelegate();
	public LoginDelegate loginDelegate;


	public Button btn_newplay;
	public Button btn_register;
	public Button btn_login;
	//public Button btn_service;
	public Button btn_forget_pwd;
	public InputField Input_account;
	public InputField Input_pwd;
	public GameObject Object_forget_pwd;

	private void Awake()
	{
		//print("OKOKO");
		initEvents();
	}
	// Use this for initialization
	void Start()
	{
		//initEvents();
		getUserInfoFromCache();
	}

	void initEvents()
	{
		btn_newplay.onClick.AddListener(newPlayClickHandler);
		btn_register.onClick.AddListener(registerClickHandler);
		btn_login.onClick.AddListener(loginClickHandler);
		//btn_service.onClick.AddListener(serviceClickHandler);
		btn_forget_pwd.onClick.AddListener(forgetPwdClickHandler);
		//print("OKOK");
		RegisterNode.afterRegisterDelgate += afterRegisterHandler;
	}

	private void getUserInfoFromCache()
	{
		SaveDataUtils.Load<SelfPlayerData>();
        Input_account.text = PlayerPrefs.GetString("user_cache_name");
        Input_pwd.text = PlayerPrefs.GetString("user_cache_pwd");
    }

	//注册完成后，由注册回调事件，做登陆操作
	public void afterRegisterHandler(string userName, string pwd)
	{
		RegisterNode.afterRegisterDelgate -= afterRegisterHandler;
		Input_account.text = userName;
		Input_pwd.text = pwd;
		loginClickHandler();
	}

	//试玩，不去注册
	private void newPlayClickHandler()
	{

		if (loginDelegate != null)
		{
			loginDelegate();
		}
	}

	//打开注册面板
	private void registerClickHandler()
	{
		//PageManager.Instance.CurrentPage.AddNode<ShareActivityNode>(true);
		//return;
		PageManager.Instance.CurrentPage.AddNode<RegisterNode>(true);
	}

	//private void registerUserCallback()
	//{
	//	Input_account.text = userName;
	//	Input_pwd.text = password;
	//}

	//登陆操作
	private void loginClickHandler()
	{
		//if(btn_login!=null)
		//btn_login.enabled = false;
		bool login = LoginRpcProxy.getInstance().LoginToApp(Input_account.text, Input_pwd.text, (int code) =>
		{
			//if (btn_login != null)
			//btn_login.enabled = true;
			if (code == 1)
			{
				if (loginDelegate != null)
				{
					loginDelegate.Invoke();
				}
			}

		});

	}

	private void serviceClickHandler()
	{
        HttpHandler.UploadScore();
	}

	private void forgetPwdClickHandler()
	{

	}

	// Update is called once per frame
	void Update()
	{

	}

}
