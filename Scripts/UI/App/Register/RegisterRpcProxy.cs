using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LitJson;

class RegisterRpcProxy
{


	private static Action<SnapRpcDataVO> mCheckUserNameCallback;
	/// <summary>
	/// 通过服务器检查用户名
	/// </summary>
	/// <param name="paramObj">参数</param>
	/// <param name="callback">回调</param>
	public static void CheckUserName(JsonData paramObj, Action<SnapRpcDataVO> callback)
	{
		mCheckUserNameCallback = callback;

		SnapAppApi.Request_SnapAppApi(SnapAppApiInterface.Request_CheckUserNameApp, SnapHttpConfig.NET_REQUEST_GET, paramObj, CheckUserNameCallback);
	}
	private static void CheckUserNameCallback(SnapRpcDataVO rpcData)
	{
		if (mCheckUserNameCallback != null)
		{
			mCheckUserNameCallback(rpcData);
			mCheckUserNameCallback = null;
		}
	}

	private static Action<SnapRpcDataVO> mRegisterUserCallback;
	/// <summary>
	/// 注册用户
	/// </summary>
	/// <param name="paramObj">参数</param>
	/// <param name="callback">回调</param>
	public static void RegisterUser(JsonData paramObj, Action<SnapRpcDataVO> callback)
	{
		mRegisterUserCallback = callback;

		SnapAppApi.Request_SnapAppApi(SnapAppApiInterface.Request_RegisterApp, SnapHttpConfig.NET_REQUEST_POST, paramObj, RegisterUserCallback);
	}
	private static void RegisterUserCallback(SnapRpcDataVO rpcData)
	{
		if (mRegisterUserCallback != null)
		{
			mRegisterUserCallback(rpcData);
			mRegisterUserCallback = null;
		}
	}

}
