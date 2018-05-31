using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

class UserInfoOtherView : UserInfoViewBasic
{
	//姓名
	private Text mUserNameTF;

	//地区
	private Text mUserCityTF;

	//性别
	private Button mUserGenderBoy;
	private Button mUserGenderGirl;

	//年龄
	private Text mUserAgeTF;

	//private 

	public UserInfoOtherView(GameObject _basic, UserInfoNode _manager)
	{
		mBasic = _basic;
		mManager = _manager;

		mBasic.gameObject.SetActive(false);
	}

	protected override void initViewSub()
	{
		mUserNameTF = mManager.mOtherNameTF;
		mUserGenderBoy = mManager.mOtherGenderBoyBtn;
		mUserGenderGirl = mManager.mOtherGenderGirlBtn;
		mUserAgeTF = mManager.mOtherAgeTF;

		mUserCityTF = mManager.mOtherCityTF;

		mUserGenderBoy.enabled = mUserGenderGirl.enabled = false;
	}

	protected override void refreshViewSub()
	{
		mBasic.gameObject.SetActive(!mManager.isSelf);

		mUserNameTF.text = mUserVo.UserName;

		if (mUserVo.UserGender == UserVoBasic.GENDER_BOY)
		{
			mUserGenderBoy.gameObject.SetActive(true);
			mUserGenderGirl.gameObject.SetActive(false);
		}
		else
		{
			mUserGenderBoy.gameObject.SetActive(false);
			mUserGenderGirl.gameObject.SetActive(true);
		}

		mUserAgeTF.text = mUserVo.UserAge + " 岁";

		mUserCityTF.text = mUserVo.UserCity;

	}

}
