using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LitJson;
using Snaplingo.SaveData;
using UnityEngine;
using UnityEngine.UI;
using Snaplingo.UI;
using LitJson;
using System;
using UnityEngine.UI;


class UserVoProxy
{
	private static UserVoProxy _instance;
	public static UserVoProxy instance()
	{
		_instance = _instance == null ? (new UserVoProxy()) : _instance;
		return _instance;
	}

	public UserVoProxy()
	{
		mRequestPool = new List<UserVoRequest>();
		mAllUserVoTemp = new Dictionary<string, UserVoBasic>();
	}

	/** 个人信息 **/
	//public static UserVoBasic SelfUserVo = new UserVoBasic();

	/** 用户信息缓存器 **/
	private Dictionary<string, UserVoBasic> mAllUserVoTemp;

	/** 请求回调 **/
	private Action<UserVoBasic> mRequestUserVoCallback;

	/** 请求池 **/
	private List<UserVoRequest> mRequestPool;

	/// <summary>
	/// 请求用户信息
	/// </summary>
	/// <param name="_userID"></param>
	/// <param name="_callback"></param>
	/// <param name="isFromTemp"></param>
	public void RequestUserVo(string _userID, Action<UserVoBasic> _callback = null, bool isFromTemp = true)
	{
		mRequestUserVoCallback = _callback;

		UserVoRequest itemRequest;
		if (isFromTemp)
		{
			LogManager.Log("从本地");
			if (mAllUserVoTemp.ContainsKey(_userID))
			{
				Callback(mAllUserVoTemp[_userID]);
			}
			else
			{
				itemRequest = new UserVoRequest();
				mRequestPool.Add(itemRequest);
				itemRequest.RequestUserVo(_userID, RequestUserVoCallback);
			}
		}
		else
		{
			LogManager.Log("从服务器");
			itemRequest = new UserVoRequest();
			mRequestPool.Add(itemRequest);
			itemRequest.RequestUserVo(_userID, RequestUserVoCallback);
		}
	}

	/// <summary>
	/// 请求完成
	/// </summary>
	/// <param name="_rpcResultObj"></param>
	private void RequestUserVoCallback(UserVoBasic _rpcResultObj)
	{
		//if (_rpcResultObj.UserID == GlobalConst.Player_ID)
		//{
		//	SelfUserVo = _rpcResultObj;

		//	//交换数据
		//	UserVoProxy.SelfUserVo.UserLevel = SelfPlayerData.Level;
		//	UserVoProxy.SelfUserVo.UserEnergy = SelfPlayerData.Energy;
		//	UserVoProxy.SelfUserVo.UserExp = SelfPlayerData.Experience;

		//	//SelfPlayerData.Uuid = "";
		//	//SelfPlayerData.DeviceID = "";
		//	SelfPlayerData.Nickname = _rpcResultObj.UserName;
		//	SelfPlayerData.AvatarUrl = _rpcResultObj.UserIconUrl;
		//	SaveDataUtils.Save<SelfPlayerData>();
		//}

		Callback(_rpcResultObj);
	}

	/// <summary>
	/// 单个请求完成
	/// </summary>
	/// <param name="itemRequest"></param>
	public void FinishItemRequest(UserVoRequest itemRequest)
	{
		if (mRequestPool != null && itemRequest != null)
		{
			mRequestPool.Remove(itemRequest);
		}
	}

	/// <summary>
	/// 回调上层
	/// </summary>
	/// <param name="_userVo"></param>
	private void Callback(UserVoBasic _userVo)
	{
		if (mRequestUserVoCallback != null)
		{
			mRequestUserVoCallback(_userVo);
			mRequestUserVoCallback = null;
		}
	}


	/// <summary>
	/// 格式化一个数据
	/// </summary>
	/// <param name="_rpcResultObj"></param>
	/// <returns></returns>
	public UserVoBasic formatVo(JsonData _rpcResultObj)
	{
		UserVoBasic userVo = new UserVoBasic();

		userVo.UserID = _rpcResultObj["appUID"].ToString();
		userVo.UserName = _rpcResultObj["nickname"].ToString();
		if (_rpcResultObj["avatar"].ToString() != null && _rpcResultObj["avatar"].ToString() != "")
		{
			userVo.UserIconUrl = _rpcResultObj["avatar"].ToString();
		}
		userVo.UserAge = int.Parse(_rpcResultObj["age"].ToString());
		userVo.UserCity = (_rpcResultObj["city"].ToString());
		if (int.Parse(_rpcResultObj["count"].ToString()) == 1)
		{
			userVo.UserCityIsChange = false;
		}
		else
		{
			userVo.UserCityIsChange = true;
		}
		userVo.UserCountry = int.Parse(_rpcResultObj["country"].ToString());
		userVo.UserAboutMe = _rpcResultObj["profile"].ToString();
		userVo.UserGender = int.Parse(_rpcResultObj["sex"].ToString());

		userVo.TempUserID = _rpcResultObj["id"].ToString();

		userVo.UserExp = int.Parse(_rpcResultObj["experience"].ToString());
		userVo.UserLevel = int.Parse(_rpcResultObj["level"].ToString());
		userVo.UserEnergy = int.Parse(_rpcResultObj["energy"].ToString());
		//userVo.TempUserLevel = (int)_rpcResultObj["level"];
		//userVo.TempUserExp = (int)_rpcResultObj["experience"];
		//userVo.TempUserEnergy = (int)_rpcResultObj["energy"];

		userVo.UserIsRegister = int.Parse(_rpcResultObj["appUID"].ToString()) > 0;

		//存储
		mAllUserVoTemp[userVo.TempUserID] = userVo;

		return userVo;
	}

	public void ClearUserVoTemp()
	{
		mAllUserVoTemp.Clear();
	}

}
