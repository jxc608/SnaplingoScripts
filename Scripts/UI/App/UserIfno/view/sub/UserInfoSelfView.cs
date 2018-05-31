using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using LitJson;

class UserInfoSelfView : UserInfoViewBasic
{
	//用户名
	private Text mUserNameTF;
	//用户名编辑按钮,未注册用户显示,点击跳转注册
	private Button mUserNameEditBtn;

	//地区
	private InputField mUserCityImportTF;

	//性别
	private Button mUserGenderBoyBtn;
	private Button mUserGenderGirlBtn;
	private Image mUserGenderSelectMc;

	//年龄
	private InputField mUserAgeImportTF;

	//经验
	private Image mUserExpProgressBar;
	private float mUserExpProgressBarWidth;
	private Text mUserExpTF;

	public UserInfoSelfView(GameObject _basic, UserInfoNode _manager)
	{
		mBasic = _basic;
		mManager = _manager;
		mBasic.gameObject.SetActive(false);
	}

	protected override void initViewSub()
	{
		mUserNameTF = mManager.mSelfNameTF;
		mUserNameEditBtn = mManager.mSelfNameTFEditBtn;
		mUserCityImportTF = mManager.mUserCityImportTF;
		mUserGenderBoyBtn = mManager.mSelfGenderBoyBtn;
		mUserGenderGirlBtn = mManager.mSelfGenderGirlBtn;
		mUserGenderSelectMc = mManager.mSelfGenderSelectMc;
		mUserAgeImportTF = mManager.mSelfAgeImportTF;

		mUserExpProgressBar = mManager.mSelfExpProgressBar;
		mUserExpTF = mManager.mSelfExpTF;
		mUserExpProgressBarWidth = mUserExpProgressBar.rectTransform.sizeDelta.x;
	}

	protected override void refreshViewSub()
	{
		mBasic.gameObject.SetActive(mManager.isSelf);

		//姓名
		mUserNameTF.text = mUserVo.UserName;
		mUserNameEditBtn.gameObject.SetActive(!mUserVo.UserIsRegister);
		mUserNameEditBtn.onClick.AddListener(onClickEditMyNameHandle);

		//性别
		if (mUserVo.UserGender <= 0)
		{
			mUserGenderSelectMc.gameObject.SetActive(true);
			mUserGenderBoyBtn.enabled = mUserGenderGirlBtn.enabled = true;
			mUserGenderBoyBtn.onClick.AddListener(onSelectBoyHandle);
			mUserGenderGirlBtn.onClick.AddListener(onSelectGirlHandle);
		}
		else if (mUserVo.UserGender == UserVoBasic.GENDER_BOY)
		{
			mUserGenderBoyBtn.gameObject.SetActive(true);
			mUserGenderGirlBtn.gameObject.SetActive(false);
			mUserGenderSelectMc.gameObject.SetActive(false);

			Vector3 newPosition = mUserGenderGirlBtn.transform.position;
			newPosition.x = newPosition.x + 30;
			mUserGenderBoyBtn.transform.position = newPosition;

			mUserGenderBoyBtn.enabled = mUserGenderGirlBtn.enabled = false;
		}
		else
		{
			mUserGenderBoyBtn.gameObject.SetActive(false);
			mUserGenderGirlBtn.gameObject.SetActive(true);

			Vector3 newPosition = mUserGenderGirlBtn.transform.position;
			newPosition.x = newPosition.x + 30;
			mUserGenderGirlBtn.transform.position = newPosition;

			mUserGenderSelectMc.gameObject.SetActive(false);
			mUserGenderBoyBtn.enabled = mUserGenderGirlBtn.enabled = false;
		}

		//地区
		mUserCityImportTF.text = mUserVo.UserCity.ToString();
		if (mUserVo.UserCityIsChange)
		{
			mUserCityImportTF.enabled = false;
		}
		else
		{
			mUserCityImportTF.enabled = true;
			mUserCityImportTF.onEndEdit.AddListener(onSetCityHandle);
		}

		//年龄
		mUserAgeImportTF.text = mUserVo.UserAge.ToString();
		if (mUserVo.UserAge <= 0)
		{
			mUserAgeImportTF.enabled = true;
			//mUserAgeImportTF.onSelect += onSelectAgeTFHandle;
			mUserAgeImportTF.onEndEdit.AddListener(onSetAgeHandle);
		}
		else
		{
			mUserAgeImportTF.enabled = false;
		}

		//经验
		int expTemp = SelfPlayerData.Experience;
		ExpBasic expItem = ExpConfig.Instance.getExpInfo(expTemp);

		float value = (float)expTemp / (float)expItem.expEnd;
		mUserExpProgressBar.rectTransform.sizeDelta = new Vector2(value * mUserExpProgressBarWidth, mUserExpProgressBar.rectTransform.sizeDelta.y);
		mUserExpTF.text = "   " + expTemp + " / " + expItem.expEnd;
	}

	#region [ 填写地区 ]
	private void onSetCityHandle(string city)
	{
		if (mUserVo.UserIsRegister)
		{
			if (mUserCityImportTF.text != null && mUserCityImportTF.text != "")
			{
				showHint("确定后只能通过客服修改哦!", onSetCityOkCalllback, onSetCityONoClalback);
			}
			else
			{
				mUserCityImportTF.text = mUserVo.UserCity.ToString();
			}
		}
		else
		{
			showHint("注册后才能修改信息哦!", mManager.GoRegisterHandle);
		}
	}
	private void onSetCityOkCalllback()
	{
		mUserCityImportTF.enabled = false;

		JsonData info = new JsonData();
		info["city"] = (mUserCityImportTF.text);
		UserInfoRpcProxy.UpdateUserInfo(info);
		mUserVo.UserCityIsChange = true;
	}
	private void onSetCityONoClalback()
	{
		mUserCityImportTF.text = "";
	}

	#endregion

	#region [ 填写年龄 ]
	private void onSetAgeHandle(string age)
	{
		if (mUserVo.UserIsRegister)
		{
			if (mUserAgeImportTF.text != null && mUserAgeImportTF.text != mUserVo.UserAge.ToString())
			{
				showHint("确定后只能通过客服修改哦!", onSetAgeOkCalllback, onSetAgeONoClalback);
			}
			else
			{
				mUserAgeImportTF.text = mUserVo.UserAge.ToString();
			}
		}
		else
		{
			showHint("注册后才能修改信息哦!", mManager.GoRegisterHandle);
		}
	}
	private void onSetAgeOkCalllback()
	{
		mUserAgeImportTF.enabled = false;

		JsonData info = new JsonData();
		info["age"] = int.Parse(mUserAgeImportTF.text);
		UserInfoRpcProxy.UpdateUserInfo(info);
	}
	private void onSetAgeONoClalback()
	{
		mUserAgeImportTF.text = mUserVo.UserAge.ToString();
	}

	#endregion

	#region [ 选择性别 ]

	private void onSelectBoyHandle()
	{
		if (mUserVo.UserIsRegister)
		{
			showHint("确定后只能通过客服修改哦!", onSelectBoyCallback);
		}
		else
		{
			showHint("注册后才能修改信息哦!", mManager.GoRegisterHandle);
		}
	}
	private void onSelectBoyCallback()
	{
		mUserGenderSelectMc.gameObject.SetActive(true);
		Vector3 newPosition = mUserGenderBoyBtn.transform.position;
		newPosition.x = newPosition.x + 20;
		newPosition.y = newPosition.y - 20;
		mUserGenderSelectMc.transform.position = newPosition;
		mUserGenderBoyBtn.enabled = mUserGenderGirlBtn.enabled = false;

		JsonData info = new JsonData();
		info["sex"] = UserVoBasic.GENDER_BOY;
		UserInfoRpcProxy.UpdateUserInfo(info);
	}

	private void onSelectGirlHandle()
	{
		showHint("确定后只能通过客服修改哦!", onSelectGirlCallback);
	}
	private void onSelectGirlCallback()
	{
		mUserGenderSelectMc.gameObject.SetActive(true);
		Vector3 newPosition = mUserGenderGirlBtn.transform.position;
		newPosition.x = newPosition.x + 20;
		newPosition.y = newPosition.y - 20;
		mUserGenderSelectMc.transform.position = newPosition;
		mUserGenderBoyBtn.enabled = mUserGenderGirlBtn.enabled = false;

		JsonData info = new JsonData();
		info["sex"] = UserVoBasic.GENDER_GIRL;
		UserInfoRpcProxy.UpdateUserInfo(info);
	}

	#endregion

	#region [ 点击编辑名字 ]
	private void onClickEditMyNameHandle()
	{
		if (!mUserVo.UserIsRegister)
		{
			showHint("快去注册账号\n取一个棒棒的名字!", mManager.GoRegisterHandle);
		}
	}
	#endregion

}

