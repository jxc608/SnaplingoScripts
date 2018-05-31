using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LitJson;

using UnityEngine;
using UnityEngine.UI;
using Snaplingo.UI;
using LitJson;
using System;

class UserInfoRpcProxy
{
	private static Action<SnapRpcDataVO> mLoadUserVoCallback;
	/// <summary>
	/// 更新用户信息
	/// </summary>
	/// <param name="paramObj">参数</param>
	/// <param name="callback">回调</param>
	public static void UpdateUserInfo(JsonData paramObj, Action<SnapRpcDataVO> callback = null)
	{
		mLoadUserVoCallback = callback;

		SnapAppApi.Request_SnapAppApi(SnapAppApiInterface.Request_UpdateUserInfo, SnapHttpConfig.NET_REQUEST_PUT, paramObj, LoadUserVoCallback);
	}
	private static void LoadUserVoCallback(SnapRpcDataVO rpcData)
	{
		if (rpcData.status == true)
		{
			LogManager.Log("更新信息成功");
		}
		else
		{
			LogManager.Log("更新信息失败:" , rpcData.code , "(" , rpcData.message , ")");
		}
		if (mLoadUserVoCallback != null)
		{
			mLoadUserVoCallback(rpcData);
			mLoadUserVoCallback = null;
		}
	}

}
