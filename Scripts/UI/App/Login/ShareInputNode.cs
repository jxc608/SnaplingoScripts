using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using Snaplingo.UI;
using LitJson;
using Snaplingo.SaveData;
using System.Text.RegularExpressions;

class ShareInputNode : Node
{
	//public Button btn_register;
	public Button btn_login;
	public Button btn_close;
	public Image ImgInputTelOK;
	public Image ImgInputTelError;
	//public InputField Input_account;
	public InputField Input_pwd;

    #region 对语言游戏物体
    //public Image shareTitleImage;
    //public Image shareSureImage;
    //public Text shareInputNumberTip;
    //private void InitGameObjectUI()
    //{
    //    shareInputNumberTip.text = LanguageManager.Instance.GetValueByKey(shareInputNumberTip.GetComponent<LanguageFlagClass>().selfType.ToString(),LanguageManager.languageType);
    //}
    #endregion

    public override void Init(params object[] args)
	{
		base.Init();

		btn_login.onClick.AddListener(onClickLoginHandle);
		btn_close.onClick.AddListener(onClickCloseHandle);
		Input_pwd.onEndEdit.AddListener(onTelTFFocusOutCallback);
        //InitGameObjectUI();
	}

	public override void Open()
	{
		base.Open();

		//btn_register.onClick.AddListener(onClickRegisterHandle);
		Input_pwd.text = SelfPlayerData.TelphoneNum;
	}

	private void onTelTFFocusOutCallback(string result)
	{
        Regex Reg_Phone = new Regex(@"^1[2|3|4|5|6|7|8|9][0-9]\d{4,8}$");
		if (result.Length < 11 || Reg_Phone.IsMatch(result) == false)
		{
			ImgInputTelOK.transform.gameObject.SetActive(false);
			ImgInputTelError.transform.gameObject.SetActive(true);
			PromptManager.Instance.MessageBox(PromptManager.Type.FloatingTip, "手机号输入有误，请重新输入");
			return;
		}
		ImgInputTelError.transform.gameObject.SetActive(false);
		ImgInputTelOK.transform.gameObject.SetActive(true);
	}

	void onClickLoginHandle()
	{
        Regex Reg_Phone = new Regex(@"^1[2|3|4|5|6|7|8|9][0-9]\d{4,8}$");
        if (Input_pwd.text.Length < 11 || Reg_Phone.IsMatch(Input_pwd.text) == false)
		{
			ImgInputTelOK.transform.gameObject.SetActive(false);
			ImgInputTelError.transform.gameObject.SetActive(true);
			PromptManager.Instance.MessageBox(PromptManager.Type.FloatingTip, "手机号输入有误，请重新输入");
			return;
		}
		SelfPlayerData.TelphoneNum = Input_pwd.text;
		SaveDataUtils.Save<SelfPlayerData>();
		LoginRpcProxy.getInstance().ShareWeChatGameByLevelID();
        PageManager.Instance.CurrentPage.GetNode<ShareActivityNode>().Close(false);
        PageManager.Instance.CurrentPage.GetNode<WinNode>().Open();
		Close(true);
	}

	void onClickCloseHandle()
	{
		Close(true);
	}

}
