using UnityEngine;
using UnityEngine.UI;
using Snaplingo.UI;
using LitJson;
using System;
using System.Text.RegularExpressions;

class RegisterNode : Node
{

	public delegate void AfterRegisterDelegate (string userName, string pwd);
	public static AfterRegisterDelegate afterRegisterDelgate;

	//用户名是否ok	密码是否ok
	private bool mUserNameIsOk, mUserPasswordIsOk = false;
	private Action<string, string> mRegisterCallback;

	private byte[] mDefaultHeadBytr;

	public Image mHeadImg;
	public InputField mUserNameImportTF, mUserPasswordTF;
	public Text mUserNameImportHintTF, mPasswordImportHintTF;

	public GameObject mRecommendMc;
	public Button mRecommendName1, mRecommendName2;
	public Text mRecommendNameTF1, mRecommendNameTF2;

	public Button mSubmitBtn, mCancleBtn, mCloseBtn;
	#region
	public Image registerTitleImage;
	public Text registerUserName;
	public Text registerUserNameTip;
	public Text registerPassWord;
	public Text registerPassWordTip;
	public Text registerTitleInfo;
	public Image registerTipImage;
	#endregion

	public override void Init (params object[] args)
	{
		base.Init ();
	}
	public void InitGameObjectUI ()
	{
		registerUserName.text = LanguageManager.Instance.GetEnumValue (registerUserName.gameObject);
		registerUserNameTip.text = LanguageManager.Instance.GetEnumValue (registerUserNameTip.gameObject);
		registerPassWord.text = LanguageManager.Instance.GetEnumValue (registerPassWord.gameObject);
		registerPassWordTip.text = LanguageManager.Instance.GetEnumValue (registerPassWordTip.gameObject);
		registerTitleInfo.text = LanguageManager.Instance.GetEnumValue (registerTitleInfo.gameObject);
		registerTitleImage.sprite = Resources.Load<Sprite> ("language/" + LanguageManager.Instance.GetEnumValue (registerTitleImage.gameObject));
		registerTitleImage.SetNativeSize ();
		//registerTipImage.sprite = Resources.Load<Sprite>("language/" + LanguageManager.Instance.GetEnumValue(registerTipImage.gameObject));
	}
	public override void Open ()
	{
		mRecommendMc.SetActive (false);

		//mUserNameImportHintTF.enabled = mPasswordImportHintTF.enabled = false;
		mUserNameImportTF.text = mUserPasswordTF.text = "";
		mUserNameImportTF.characterLimit = 11;
		mUserNameImportTF.contentType = InputField.ContentType.Alphanumeric;
		//mUserNameImportTF.onEndEdit.AddListener(onUserNameTFFocusOutCallback);

		if (GlobalConst.Player_IsChina)
		{
			mUserPasswordTF.contentType = InputField.ContentType.DecimalNumber;
			mUserPasswordTF.characterLimit = 11;
		}
		else
		{
			mUserPasswordTF.characterLimit = 100;
		}
		//mUserPasswordTF.onEndEdit.AddListener(onPasswordTFFocusOutCallback);

		mSubmitBtn.onClick.AddListener (onClickSubmitHandle);
		mCancleBtn.onClick.AddListener (onCloseNodeHandle);
		mCloseBtn.onClick.AddListener (onCloseNodeHandle);
		mHeadImg.sprite = RoleManager.Instance.roleAvatarList[int.Parse (RoleManager.Instance.currentRoleModelName.Split ('_')[1]) - 1];
		InitGameObjectUI ();
	}


	#region [ UserName Logic ]

	/// <summary>
	/// 文本聚焦
	/// </summary>
	private void onUserNameTFFocusInCallback ()
	{
	}

	/// <summary>
	/// 文本失去焦点
	/// </summary>
	private void startCheckUserName (string info)
	{
		if (info != "")
		{
			//mUserNameImportHintTF.text = "";
			//开始检测
			checkUserNameServer (info);
		}
		else
		{
			PromptManager.Instance.MessageBox (PromptManager.Type.FloatingTip, "请输入用户名");

			string hintStr = GlobalConst.Player_IsChina ? RegisterConfig.Hint_UserNameCN : RegisterConfig.Hint_UserNameEN;
			//mUserNameImportHintTF.text = hintStr;

			mUserNameIsOk = false;
			checkIsCanRegister ();
		}

	}

	private void checkUserNameServer (string info)
	{
		LogManager.Log ("输入的文本为:", mUserNameImportTF.text);

		//是否太短
		if (info.Length >= 5)
		{
			//服务器检查
			JsonData paramObj = new JsonData ();
			paramObj["userName"] = mUserNameImportTF.text;
			RegisterRpcProxy.CheckUserName (paramObj, checkUserNameCallback);
		}
		else
		{
			mUserNameIsOk = false;
			checkIsCanRegister ();
			PromptManager.Instance.MessageBox (PromptManager.Type.FloatingTip, "用户名太短");
		}

	}
	private void checkUserNameCallback (SnapRpcDataVO rpcResultObj)
	{
		if (rpcResultObj.code == RpcDataCode.RequestFinish)
		{
			//if (rpcResultObj.data.IsArray)
			//{
			//用户名可用
			if (rpcResultObj.data == null)
			{
				mUserNameIsOk = true;

			}
			else
			{
				mUserNameIsOk = false;
				if (rpcResultObj.data.IsArray)
				{
					//推荐用户名
					if (rpcResultObj.data.Count > 1 && ((string)rpcResultObj.data[0]).Length < 11)
					{
						onShowRecemmendName ((string)rpcResultObj.data[0], (string)rpcResultObj.data[1]);
					}
					else
					{
						PromptManager.Instance.MessageBox (PromptManager.Type.FloatingTip, "用户名已注册");
					}
				}
				else
				{
					PromptManager.Instance.MessageBox (PromptManager.Type.FloatingTip, "用户名已注册");
				}

			}
			//}
			//else
			//{
			//	mUserNameIsOk = false;
			//	PromptManager.Instance.MessageBox(PromptManager.Type.FloatingTip, "用户名已注册");
			//}
		}
		//用户名不合法 || 激活码不能做用户名
		else if (rpcResultObj.code == RpcDataCode.NameDirty || rpcResultObj.code == RpcDataCode.NameActivation)
		{
			mUserNameIsOk = false;
			PromptManager.Instance.MessageBox (PromptManager.Type.FloatingTip, "用户名包含非法内容，请更换");
		}
		else
		{
			//用户名可用
			mUserNameIsOk = true;
		}
		RegisterLogin ();
		checkIsCanRegister ();
	}
	private void RegisterLogin ()
	{
		if (mUserNameIsOk)
		{
			startCheckPassword (mUserPasswordTF.text);
		}
		if (mUserNameIsOk && mUserPasswordIsOk)
		{
			JsonData paramObj = new JsonData ();
			paramObj["userName"] = mUserNameImportTF.text;
			paramObj["password"] = mUserPasswordTF.text;
			paramObj["phoneNum"] = GlobalConst.Player_IsChina ? mUserPasswordTF.text : "";
			paramObj["parentEmail"] = GlobalConst.Player_IsChina ? "" : mUserPasswordTF.text;
			paramObj["country"] = LanguageManager.languageType == LanguageType.Chinese ? 0 : 1;
			//paramObj["country"] = RegisterConfig.Temp_userCity;
			RegisterRpcProxy.RegisterUser (paramObj, registerUserCallback);
		}
	}
	//推荐名字
	private void onShowRecemmendName (string name1, string name2)
	{
		mRecommendMc.SetActive (true);
		mRecommendNameTF1.text = name1;
		mRecommendName1.onClick.AddListener (delegate () {
			this.onSelectRecommentName (name1);
		});
		mRecommendNameTF2.text = name2;
		mRecommendName2.onClick.AddListener (delegate () {
			this.onSelectRecommentName (name2);
		});
	}
	private void onSelectRecommentName (string name)
	{
		mRecommendMc.SetActive (false);
		mUserNameImportTF.text = name;
		mUserNameIsOk = true;
		checkIsCanRegister ();
	}

	#endregion

	#region [ Password Logic ]

	/// <summary>
	/// 文本聚焦
	/// </summary>
	private void onPasswordTFFocusInCallback (string info)
	{
	}

	/// <summary>
	/// 文本失去焦点
	/// </summary>
	private void startCheckPassword (string info)
	{
		if (info != "")
		{
			//Don的需求，密码只检测空格和中文
			onCheckSpaceChineseLocal (info);
			return;
			//mPasswordImportHintTF.text = "";
			//开始检测
			if (GlobalConst.Player_IsChina)
			{
				onCheckPhoneLocal (info);
			}
			else
			{
				onCheckEmailLocal (info);
			}
		}
		else
		{
			PromptManager.Instance.MessageBox (PromptManager.Type.FloatingTip, "请输入密码");
			string hintStr = GlobalConst.Player_IsChina ? RegisterConfig.Hint_PasswordCN : RegisterConfig.Hint_PasswordEN;
			//mPasswordImportHintTF.text = hintStr;

			mUserPasswordIsOk = false;
			checkIsCanRegister ();
		}

	}

	private void onCheckPhoneLocal (string info)
	{
		if (RegisterConfig.Reg_Phone.IsMatch (info))
		{
			mUserPasswordIsOk = true;
			LogManager.Log ("合法的手机号");
		}
		else
		{
			mUserPasswordIsOk = false;
			PromptManager.Instance.MessageBox (PromptManager.Type.FloatingTip, "请填写正确的手机号");
		}
		checkIsCanRegister ();
	}

	private void onCheckEmailLocal (string info)
	{
		if (RegisterConfig.Reg_Email.IsMatch (info))
		{
			mUserPasswordIsOk = true;
			LogManager.Log ("合法的Email");
		}
		else
		{
			mUserPasswordIsOk = false;
			PromptManager.Instance.MessageBox (PromptManager.Type.FloatingTip, "邮件地址错误");
		}
		checkIsCanRegister ();
	}

	private void onCheckSpaceChineseLocal (string info)
	{
		//空格和中文
		Regex Reg_Chinese = new Regex (@"[\u4e00-\u9fa5]");
		if (!Regex.IsMatch (info, "[\u4e00-\u9fa5]"))
		{
			if (!Regex.IsMatch (info, "\u3000") && !Regex.IsMatch (info, "\u0020"))
			{
				mUserPasswordIsOk = true;
			}
			else
			{
				mUserPasswordIsOk = false;
				PromptManager.Instance.MessageBox (PromptManager.Type.FloatingTip, "密码不能包含空格或中文");
			}
		}
		else
		{
			mUserPasswordIsOk = false;
			PromptManager.Instance.MessageBox (PromptManager.Type.FloatingTip, "密码不能包含空格或中文");
		}

		checkIsCanRegister ();
	}

	#endregion


	/** 检测是否可以注册 **/
	private bool checkIsCanRegister ()
	{
		//if (mUserNameIsOk && mUserPasswordIsOk)
		//{
		//	mSubmitBtn.enabled = true;
		//	return true;
		//}
		//else
		//{
		//	mSubmitBtn.enabled = false;
		//	return false;
		//}
		return false;
	}


	/// <summary>
	/// 开始注册
	/// </summary>
	private void onClickSubmitHandle ()
	{
		//mSubmitBtn.enabled = false;
		startCheckUserName (mUserNameImportTF.text);
	}

	/// <summary>
	/// 处理注册的结果
	/// </summary>
	/// <param name="rpcResultObj"></param>
	private void registerUserCallback (SnapRpcDataVO rpcResultObj)
	{
		if (rpcResultObj.code == RpcDataCode.RequestFinish)
		{
			RegisterConfig.Temp_UserName = mUserNameImportTF.text;
			RegisterConfig.Temp_UserPassword = mUserPasswordTF.text;

			LogManager.Log ("注册成功");
			AnalysisManager.Instance.OnEvent ("register", null);

			afterRegisterDelgate (RegisterConfig.Temp_UserName, RegisterConfig.Temp_UserPassword);
			onCloseNodeHandle ();
			mSubmitBtn.enabled = true;
		}
		else
		{
			//注册未成功
			PromptManager.Instance.MessageBox (PromptManager.Type.FloatingTip, "系统繁忙，请稍后再试");
		}
	}


	private void onCloseNodeHandle ()
	{
		Close (true);
	}

}
