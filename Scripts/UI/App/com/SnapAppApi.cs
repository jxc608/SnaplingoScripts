using System;
using LitJson;
using UnityEngine;
using System.Collections;

public enum SnapAppApiInterface
{
	Request_LoginApp = 0,
	Request_GetUserInfo,
	Request_RegisterApp,
	Request_RegisterGuestApp,
	Request_ForgetPwdApp,
	Request_ModifyPwdApp,
	Request_CheckUserNameApp,
	Request_AliyunOSSApp,
	Request_ThirdWebToApp,
	Request_WeChatShareGame,
	Request_UploadGameInfo,
	Request_UpdateUserInfo,
	Request_GetGuestUserInfoByDeviceId,
	Request_GetLevelScore,
	Request_GetRankScoresByLevelID,
	Request_GetRankOfTenByIDs,
	Request_UploadGameScores,
	Request_IAPGetGoodsInfo,
	Request_IAPGetGoodsVerify,
	Request_CurAppVersion,
	Request_UploadLevelVoices,
	Request_GetLevelVoices
}

public enum SnapAppApiThirdRequestFace
{
	No_Third = 0,
	Third_Aliyun_OSS = 1,
	Third_AWS = 2
}

public class SnapAppApi
{

	//--------------------- API  --------------------
	private const string User_Registered_Header = "registered/";
	private const string User_Anonymous_Header = "anonymous/";
	// 登录
	private const string User_Service_Login = User_Anonymous_Header + "users/login";
	// 1_12 获取用户信息
	private const string User_Service_Get_Info = User_Anonymous_Header + "users/user_by_uid";
	// 1_2 上传游戏用户信息
	private const string User_Service_UploadGame_Info = User_Anonymous_Header + "users/game_info";
	// 更新用户信息
	private const string User_Service_UpdateUserInfo = User_Registered_Header + "users/my_info";
	// 1_9 注册
	private const string User_Service_Register = User_Anonymous_Header + "users/registration";
	// 玩家试玩
	private const string Guest_Service_Register = User_Anonymous_Header + "users/guest/registration";
	// 1_3 根据 deviceID 获取用户信息
	private const string Guest_Service_GetInfoByDeviceId = User_Anonymous_Header + "users/user_by_device";
	// 1_10 检测用户名 
	private const string User_Service_CheckUserName = User_Anonymous_Header + "users/check_user_name";
	// 修改密码 
	private const string User_Service_ModifyPwd = User_Registered_Header + "modifyPwd";
	// 获取阿里云oss信息 
	private const string Request_Aliyun_OSS = User_Anonymous_Header + "oss/auth_header";

	// 1_11 分享接口
	private const string User_Service_WeChatShare = User_Anonymous_Header + "share/wechat_share";

	// 1_5 上传通关成绩
	private const string User_Service_UploadGameScores = User_Anonymous_Header + "level_scores";
	// 1_6 通过用户id和关卡id获取用户成绩
	private const string User_Service_GetLevelScore = User_Anonymous_Header + "users/level_scores/my_level_score";
	// 1_7 通过关卡id获取排行榜
	private const string User_Service_GetRankScores = User_Anonymous_Header + "level_scores/ranking_scores";
	// 1_8 通过关卡id获取排行榜
	private const string User_Service_GetRankOfTenByID = User_Anonymous_Header + "level_scores/scores_by_levelid_uid";

	//支付 - 获取商品信息
	private const string User_IAP_GetGoodsInfo = User_Anonymous_Header + "iap/goods";
	//支付 - 验证交易接口
	private const string User_IAP_Verify = User_Anonymous_Header + "iap/IapVerifyReceipt";

	// 2_11 获取游戏版本信息
	private const string App_GetCurrentVersion = User_Anonymous_Header + "game_info";
	private const string User_Service_UploadLevelVoices = User_Anonymous_Header + "game_level/voice_url";

	//委托事件
	public delegate void HttpErrorCodeDelegate (int errorCode);
	public static HttpErrorCodeDelegate RequestErrorCodeBack = null;//登录回调


	/// <summary>
	/// 请求App新版本的api
	/// </summary>
	/// <param name="requestInterType">请求接口名称，枚举值</param>
	/// <param name="requestMethod">请求接口方法SnapHttpConfig.NET_REQUEST_GET/SnapHttpConfig.NET_REQUEST_POST</param>
	/// <param name="jsonData">发送请求的数据</param>
	/// <param name="callBackAction">服务器返回数据的callback事件</param>
	/// <param name="priority">发送请求的优先级</param>
	static public void Request_SnapAppApi (SnapAppApiInterface requestInterType, string requestMethod,
		JsonData jsonData = null, Action<SnapRpcDataVO> callBackAction = null,
		int priority = 0, SnapAppApiThirdRequestFace third = SnapAppApiThirdRequestFace.No_Third)
	{
		string apiName = "";
		switch (requestInterType)
		{
			case SnapAppApiInterface.Request_LoginApp:
				apiName = User_Service_Login;
				break;
			case SnapAppApiInterface.Request_GetUserInfo:
				apiName = User_Service_Get_Info;
				break;
			case SnapAppApiInterface.Request_RegisterApp:
				apiName = User_Service_Register;
				break;
			case SnapAppApiInterface.Request_RegisterGuestApp:
				apiName = Guest_Service_Register;
				break;
			case SnapAppApiInterface.Request_CheckUserNameApp:
				apiName = User_Service_CheckUserName;
				break;
			case SnapAppApiInterface.Request_ModifyPwdApp:
				apiName = User_Service_ModifyPwd;
				break;
			case SnapAppApiInterface.Request_AliyunOSSApp:
				apiName = Request_Aliyun_OSS;
				break;
			case SnapAppApiInterface.Request_ThirdWebToApp:
				//第三方的都走这个，需要什么数据自己封装
				apiName = jsonData.TryGetString ("apiName");
				break;
			case SnapAppApiInterface.Request_WeChatShareGame:
				apiName = User_Service_WeChatShare;
				break;
			case SnapAppApiInterface.Request_UploadGameInfo:
				apiName = User_Service_UploadGame_Info + "/" + jsonData.TryGetString ("uid");
				break;
			case SnapAppApiInterface.Request_UpdateUserInfo:
				apiName = User_Service_UpdateUserInfo;
				break;
			case SnapAppApiInterface.Request_GetGuestUserInfoByDeviceId:
				apiName = Guest_Service_GetInfoByDeviceId;
				break;
			case SnapAppApiInterface.Request_GetLevelScore:
				apiName = User_Service_GetLevelScore;
				break;
			case SnapAppApiInterface.Request_GetRankScoresByLevelID:
				apiName = User_Service_GetRankScores + "/" + jsonData.TryGetString ("levelID");
				break;
			case SnapAppApiInterface.Request_GetRankOfTenByIDs:
				apiName = User_Service_GetRankOfTenByID;
				break;
			case SnapAppApiInterface.Request_UploadGameScores:
				apiName = User_Service_UploadGameScores + "/";
				break;

			case SnapAppApiInterface.Request_IAPGetGoodsInfo:
				apiName = User_IAP_GetGoodsInfo;
				break;
			case SnapAppApiInterface.Request_IAPGetGoodsVerify:
				apiName = User_IAP_Verify;
				break;
			case SnapAppApiInterface.Request_CurAppVersion:
				apiName = App_GetCurrentVersion;
				break;
			case SnapAppApiInterface.Request_UploadLevelVoices:
				apiName = User_Service_UploadLevelVoices;
				break;
			case SnapAppApiInterface.Request_GetLevelVoices:
				apiName = User_Service_UploadLevelVoices;
				break;
		}
		//SnapHttpConfig.REQUEST_APP_API_TOKEN = (apiName.IndexOf (User_Anonymous_Header) != -1) ?
			//SnapHttpConfig.NOT_LOGINED_APP_TOKEN : SnapHttpConfig.LOGINED_APP_TOKEN;

		SnapHttpManager.getInstance ().Request_SnapAppApi (requestInterType, apiName, requestMethod, jsonData, callBackAction, priority, third);
	}



	//=======================================================游戏内部接口========================================================

	/// <summary>
	/// 上传通关成绩
	/// </summary>
	/// <param name="data"></param>
	/// <param name="callBack"></param>
	public static void UploadGameScores (JsonData data = null, Action<SnapRpcDataVO> callBack = null)
	{
		Request_SnapAppApi (SnapAppApiInterface.Request_UploadGameScores, SnapHttpConfig.NET_REQUEST_POST, data, callBack);
	}

	/// <summary>
	/// 通过用户id和关卡id获取用户成绩
	/// </summary>
	/// <param name="data"></param>
	/// <param name="callBack"></param>
	public static void GetMyLevelScore (JsonData data = null, Action<SnapRpcDataVO> callBack = null)
	{
		if (data != null || ((IDictionary)data).Keys.Count > 0)
		{
			data["urlTypeExt"] = "/";
		}
		Request_SnapAppApi (SnapAppApiInterface.Request_GetLevelScore, SnapHttpConfig.NET_REQUEST_GET, data, callBack);
	}

	/// <summary>
	/// 通过关卡id获取排行榜
	/// </summary>
	/// <param name="data"></param>
	/// <param name="callBack"></param>
	public static void GetRankScoresByLevelID (JsonData data = null, Action<SnapRpcDataVO> callBack = null)
	{
		Request_SnapAppApi (SnapAppApiInterface.Request_GetRankScoresByLevelID, SnapHttpConfig.NET_REQUEST_GET, data, callBack);
	}

	/// <summary>
	/// 通过关卡id 用户id 获取排行榜（此用户成绩的前10名和后10名）
	/// </summary>
	/// <param name="data"></param>
	/// <param name="callBack"></param>
	public static void GetRankOfTenByLevelAndUserID (JsonData data = null, Action<SnapRpcDataVO> callBack = null)
	{
		Request_SnapAppApi (SnapAppApiInterface.Request_GetRankOfTenByIDs, SnapHttpConfig.NET_REQUEST_GET, data, callBack);
	}

	/// <summary>
	/// 本地文件上传到Oss
	/// </summary>
	/// <param name="filePath"></param>
	/// <param name="resultAction"></param>
	static public void UploadFileToAlyOSS (string filePath, Action<string> resultAction)
	{
		//LogManager.Log("UploadFileToAlyOSSUploadFileToAlyOSSUploadFileToAlyOSS");
		//SnapAppUploadFile.getInstance().PutObjectFileToOSS(filePath, resultAction);

		SnapAppUploadFile uploadFile = new SnapAppUploadFile ();
		uploadFile.PutObjectFileToOSS (filePath, resultAction);

	}
}
