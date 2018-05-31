using UnityEngine;
using UnityEngine.UI;
using Snaplingo.UI;
using LitJson;
using System;
using UnityEngine.UI;

class UserInfoNode : Node
{

	private string mUserID;
	private UserVoBasic mUserVo;

	//显示层 有数据之前先隐藏
	public GameObject mDisplay;

	//关闭按钮
	public Button mCloseBnt;

	//是否是自己
	public bool isSelf;

	//自己
	public UserInfoViewBasic mSelfView;
	//别人
	public UserInfoViewBasic mOtherView;

	public UserInfoViewBasic mCurrrentView;

    //#region
    //public Image playInfoTitleImage;
    //public Text playInfoSelfName;
    //public Text playInfoSelfChenghao;
    //public Text playInfoDesc;
    //public Text playInfoSelfSui;
    //public Text playInfoOtherName;
    //public Text playInfoOtherChenghao;
    //public Text playInfoOtherSex;
    //public Text playInfoOtherAge;
    //public Text playInfoTouxiang;
    //#endregion
    //private void InitGameObjectUI()
    //{
    //    playInfoSelfName.text = LanguageManager.Instance.GetValueByKey(playInfoSelfName.GetComponent<LanguageFlagClass>().selfType.ToString(), LanguageManager.languageType);
    //    playInfoSelfChenghao.text = LanguageManager.Instance.GetValueByKey(playInfoSelfChenghao.GetComponent<LanguageFlagClass>().selfType.ToString(), LanguageManager.languageType);
    //    playInfoDesc.text = LanguageManager.Instance.GetValueByKey(playInfoDesc.GetComponent<LanguageFlagClass>().selfType.ToString(), LanguageManager.languageType);
    //    playInfoSelfSui.text = LanguageManager.Instance.GetValueByKey(playInfoSelfSui.GetComponent<LanguageFlagClass>().selfType.ToString(), LanguageManager.languageType);
    //    playInfoOtherName.text = LanguageManager.Instance.GetValueByKey(playInfoOtherName.GetComponent<LanguageFlagClass>().selfType.ToString(), LanguageManager.languageType);
    //    playInfoOtherChenghao.text = LanguageManager.Instance.GetValueByKey(playInfoOtherChenghao.GetComponent<LanguageFlagClass>().selfType.ToString(), LanguageManager.languageType);
    //    playInfoOtherSex.text = LanguageManager.Instance.GetValueByKey(playInfoOtherSex.GetComponent<LanguageFlagClass>().selfType.ToString(), LanguageManager.languageType);
    //    playInfoOtherAge.text = LanguageManager.Instance.GetValueByKey(playInfoOtherAge.GetComponent<LanguageFlagClass>().selfType.ToString(), LanguageManager.languageType);
    //    playInfoTouxiang.text = LanguageManager.Instance.GetValueByKey(playInfoTouxiang.GetComponent<LanguageFlagClass>().selfType.ToString(), LanguageManager.languageType);
    //}

    public override void Open()
	{
		base.Open();

		mCloseBnt.onClick.AddListener(onClose);
		mDisplay.gameObject.SetActive(false);
		if (mUserVo == null)
		{
			bool isFromTemp = true;
			if (mUserID == SelfPlayerData.Uuid)
			{
				isFromTemp = false;
			}

			UserVoProxy.instance().RequestUserVo(mUserID, LoadUserVoCallback, isFromTemp);
		}
		else
		{
			GoGoGo();
		}
		//InitGameObjectUI();
		LogManager.Log("个人信息" , mUserID);
		PlayerInfoPanel.SetAvatar(mHeadImg, mUserID);
	}

	private void LoadUserVoCallback(UserVoBasic _userVo)
	{
		mUserVo = _userVo;
		GoGoGo();
	}

	public void GoRegisterHandle()
	{
		PageManager.Instance.CurrentPage.AddNode<RegisterNode>(true);
		onClose();
	}

	private void GoGoGo()
	{
		mDisplay.gameObject.SetActive(true);
		mSelfView = new UserInfoSelfView(mSelfBasic, this);
		mOtherView = new UserInfoOtherView(mOtherBasic, this);
		isSelf = mUserVo.TempUserID == SelfPlayerData.Uuid;
		//mUserVo.UserIsRegister = false;
		//isSelf = true;
		if (isSelf)
		{
			mCurrrentView = mSelfView;
		}
		else
		{
			mCurrrentView = mOtherView;
		}
		mCurrrentView.initView();
		mCurrrentView.refreshView(mUserVo);
	}

	private void onClose()
	{
		Close(true);
	}

	public override void Init(params object[] args)
	{
		base.Init(args);

		mUserID = (string)args[0];
		if (args.Length > 1)
		{
			mUserVo = (UserVoBasic)args[1];
		}
	}

	/**--------------------- 舞台元素,懒得再Get了  ---------------------**/

	/** 头像 **/
	public Image mHeadImg;
	/** 等级 **/
	public Text mLevelTF;
	/** 国家 **/
	public Image mCityFlagImg;
	/** 关于我 **/
	public InputField mAboutMeTF;

	/** 我的 Basic **/
	public GameObject mSelfBasic;
	/** 我的名字文本 **/
	public Text mSelfNameTF;
	// 我的地区文本
	public InputField mUserCityImportTF;
	/** 我的名字编辑按钮 **/
	public Button mSelfNameTFEditBtn;
	/** 我的性别mc **/
	public Button mSelfGenderBoyBtn;
	public Button mSelfGenderGirlBtn;
	public Image mSelfGenderSelectMc;
	/** 我的地区文本 **/
	public InputField mCityImportTF;
	/** 我的年龄文本 **/
	public InputField mSelfAgeImportTF;
	/** 我的经验 **/
	public Image mSelfExpProgressBar;
	public Text mSelfExpTF;
	/** 是否干什么 **/
	public Image mIsToDoMc;
	public Text mIsToDoHintTF;
	public Button mIsToDoOkBtn;
	public Button mIsToDoNoBtn;

	/** 别人 Basic **/
	public GameObject mOtherBasic;
	/** 别人名字文本 **/
	public Text mOtherNameTF;
	/** 别人地区文本 **/
	public Text mOtherCityTF;
	/** 别人性别mc **/
	public Button mOtherGenderBoyBtn;
	public Button mOtherGenderGirlBtn;
	/** 别人年龄文本 **/
	public Text mOtherAgeTF;

}

