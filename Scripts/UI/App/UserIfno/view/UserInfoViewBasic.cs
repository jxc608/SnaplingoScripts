using UnityEngine;
using UnityEngine.UI;
using Snaplingo.UI;
using LitJson;
using System;

class UserInfoViewBasic
{
	//用户数据
	protected UserVoBasic mUserVo;

	//Basic
	protected GameObject mBasic;

	//头像
	protected Image mHeadImg;

	//等级
	protected Text mLevelTF;

	//国旗
	protected Image mCityFlag;

	//关于我
	protected InputField AboutMeTF;

	protected Image mIsToDoMc;
	protected Text mIsToDoHintTF;
	protected Button mIsToDoOkBtn;
	protected Button mIsToDoNoBtn;

	//管理者
	protected UserInfoNode mManager;

	public UserInfoViewBasic() {
		
	}


	/// <summary>
	/// 获取资源
	/// </summary>
	public void initView() {
		mHeadImg = mManager.mHeadImg;
		mLevelTF = mManager.mLevelTF;
		AboutMeTF = mManager.mAboutMeTF;
		mCityFlag = mManager.mCityFlagImg;

		mIsToDoMc = mManager.mIsToDoMc;
		mIsToDoHintTF = mManager.mIsToDoHintTF;
		mIsToDoOkBtn = mManager.mIsToDoOkBtn;
		mIsToDoNoBtn = mManager.mIsToDoNoBtn;

		mIsToDoMc.gameObject.SetActive(false);

		initViewSub();
	}

	protected virtual void initViewSub() { }

	/// <summary>
	/// 刷新显示
	/// </summary>
	/// <param name="_userVo"></param>
	public void refreshView(UserVoBasic _userVo)
	{
		mUserVo = _userVo;

		AboutMeTF.text = mUserVo.UserAboutMe;
		AboutMeTF.enabled = mManager.isSelf;
		AboutMeTF.onEndEdit.AddListener(onSetAboutMeHandle);

		mLevelTF.text = "Lv " + SelfPlayerData.Level;

		string key = I18NConfig.getCityFlagByInt(mUserVo.UserCountry);
		mCityFlag.sprite = Resources.Load<Sprite>("CityFlag/" + key + "");
		Vector3 cityPosition = mCityFlag.transform.position;
		cityPosition.z += 5;
		mCityFlag.transform.position = cityPosition;

		//if (mUserVo.UserIconUrl.IndexOf("http:") != -1)
		//{
		//	AsyncImageDownload.GetInstance().SetAsyncImage(mUserVo.UserIconUrl, mHeadImg);
		//}
		//else if (mUserVo.UserIconUrl.IndexOf("Avatar") != -1)
		//{
			
		//	var sprite = Resources.Load<Sprite>("UI/_Avatars/" + mUserVo.UserIconUrl);
		//	mHeadImg.sprite = sprite;
		//}
		

		refreshViewSub();
	}
	protected virtual void refreshViewSub() { }


	#region [ 编辑关于我 ]
	private void onSetAboutMeHandle(string aboutMe)
	{
		if (mUserVo.UserIsRegister)
		{
			if (AboutMeTF.text != null && AboutMeTF.text != mUserVo.UserAboutMe.ToString())
			{
				JsonData info = new JsonData();
				info["profile"] = AboutMeTF.text;
				UserInfoRpcProxy.UpdateUserInfo(info);
			}
		}
		else
		{
			showHint("注册后才能修改信息哦!", mManager.GoRegisterHandle);
		}
	}
	#endregion

	#region [提示 ]

	private Action mOkCallback;
	private Action mNoCallback;
	/// <summary>
	/// 出现提示
	/// </summary>
	/// <param name="str"></param>
	/// <param name="okCallback"></param>
	/// <param name="noCallback"></param>
	protected void showHint(string str, Action okCallback = null, Action noCallback = null)
	{
		mOkCallback = okCallback;
		mNoCallback = noCallback;
		mIsToDoMc.gameObject.SetActive(true);
		mIsToDoHintTF.text = str;
		mIsToDoOkBtn.onClick.AddListener(onClickOkCallback);
		mIsToDoNoBtn.onClick.AddListener(onClickNoCallback);
	}
	private void onClickOkCallback()
	{
		mIsToDoMc.gameObject.SetActive(false);
		if (mOkCallback != null)
		{
			mOkCallback();
		}
	}
	private void onClickNoCallback()
	{
		mIsToDoMc.gameObject.SetActive(false);
		if (mNoCallback != null)
		{
			mNoCallback();
		}
	}

	#endregion

	/// <summary>
	/// 隐藏显示
	/// </summary>
	/// <param name="_visible"></param>
	public void visible(bool _visible)
	{
		mBasic.gameObject.SetActive(_visible);
	}

}
