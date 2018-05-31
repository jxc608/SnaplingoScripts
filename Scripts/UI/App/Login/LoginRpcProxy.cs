using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LitJson;
using Snaplingo.SaveData;
using UnityEngine;
using UnityEngine.UI;

class LoginRpcProxy
{
	public delegate void ShareDelegate ();
	public ShareDelegate shareDelegate;

	private static LoginRpcProxy instance;
	public static LoginRpcProxy getInstance ()
	{
		return instance == null ? instance = new LoginRpcProxy () : instance;
	}

	public LoginRpcProxy ()
	{
		if (instance != null)
		{
			throw new Exception ("LoginRpcProxy不能被重复实例化");
		}
	}

	string _userName;
	string _pwd;
	Action<int> _loginActionCallBack;

	/// <summary>
	/// 帐号，密码登陆app
	/// </summary>
	/// <param name="userName"></param>
	/// <param name="pwd"></param>
	/// <param name="device"></param>
	/// <param name="platform"></param>
	/// <param name="loginActionCallBack"></param>
	/// <returns></returns>
	public bool LoginToApp (string userName, string pwd, Action<int> loginActionCallBack = null)
	{
		_loginActionCallBack = loginActionCallBack;
		if (string.IsNullOrEmpty (userName))
		{
			PromptManager.Instance.MessageBox (PromptManager.Type.FloatingTip, "用户名不能为空");
			_loginActionCallBack.Invoke (2);
			return false;
		}
		if (string.IsNullOrEmpty (pwd))
		{
			PromptManager.Instance.MessageBox (PromptManager.Type.FloatingTip, "密码不能为空");
			_loginActionCallBack.Invoke (2);
			return false;
		}
		//btn_login.enabled = false;
		_userName = userName;
		_pwd = pwd;

		PlayerPrefs.SetString ("user_cache_name", _userName);
		PlayerPrefs.SetString ("user_cache_pwd", _pwd);

		JsonData rpcObj = new JsonData ();
		rpcObj["clientDate"] = SnapHttpManager.GetTimeStamp ();
		rpcObj["username"] = userName;
		rpcObj["password"] = pwd;
		rpcObj["version"] = MiscUtils.GetVersion ();
		rpcObj["device"] = SystemInfo.deviceType.ToString ();
		rpcObj["platform"] = SystemInfo.operatingSystem;
		rpcObj["deviceNew"] = SystemInfo.deviceType.ToString ();
		rpcObj["gameDeviceID"] = SelfPlayerData.DeviceID;
		SnapAppApi.Request_SnapAppApi (SnapAppApiInterface.Request_LoginApp, SnapHttpConfig.NET_REQUEST_POST, rpcObj, onLoginRequestFinish);
		return true;
	}

	/// <summary>
	/// 登陆成功的回调
	/// </summary>
	/// <param name="_rpcResultObj"></param>
	private void onLoginRequestFinish (SnapRpcDataVO _rpcResultObj)
	{
		JsonData data = _rpcResultObj.data;
		if (_rpcResultObj.code == 1)
		{
			LogManager.Log ("登录成功!!!!");
			GlobalConst.LoginToApp = true;
			SelfPlayerData.UserName = _userName;
			SelfPlayerData.Password = _pwd;
			SnapHttpConfig.LOGINED_APP_TOKEN = data["token"].ToString ();
			GlobalConst.Player_ID = data.TryGetString ("appUserID");
			GlobalConst.Player_IDTemp = data.TryGetString ("uid");
			SelfPlayerData.Uuid = data.TryGetString ("uid");
			SelfPlayerData.DeviceID = PlayerPrefs.GetString ("DeviceID");

			//GlobalConst.CurrentServerTime = (long)data["currentTime"];
			GetLoginUserInfo ();



		}
		else if (_rpcResultObj.code == RpcDataCode.AccoutLock)
		{
			PromptManager.Instance.MessageBox (PromptManager.Type.FloatingTip, "帐号被锁定，请联系客服人员");
		}
		else if (_rpcResultObj.code == RpcDataCode.PasswordError)
		{
			PromptManager.Instance.MessageBox (PromptManager.Type.FloatingTip, "密码输入有误");
		}
		if (_loginActionCallBack != null)
		{
			_loginActionCallBack.Invoke (_rpcResultObj.code);
		}
	}

	/// <summary>
	/// 登陆成功，获取玩家的具体信息
	/// </summary>
	public void GetLoginUserInfo ()
	{
		UserVoProxy.instance ().RequestUserVo (GlobalConst.Player_IDTemp);
	}

	/// <summary>
	/// 玩家试玩，走游戏内部注册接口
	/// </summary>
	public void LoginByAnonymous ()
	{
		SnapAppApi.Request_SnapAppApi (SnapAppApiInterface.Request_RegisterGuestApp, SnapHttpConfig.NET_REQUEST_POST, null, (SnapRpcDataVO data) => {
			JsonData user = data.data["user"];
			//临时
			//UserData.Instance.curUserID = SelfPlayerData.Uuid;
			//LogManager.LogWarning(UserData.Instance.curUserID);
			// 本地初始化 当前用户数据
			//UserData.Instance.GetCurrentUserData();

			SelfPlayerData.Uuid = user.TryGetString ("id");
			SelfPlayerData.DeviceID = user.TryGetString ("deviceID");

			//SaveDataUtils.Save<UserData>();
			GetGuestUserInfoByDeviceId ();
		});
	}

	/// <summary>
	/// 根据 deviceID 获取用户信息
	/// </summary>
	public void GetGuestUserInfoByDeviceId ()
	{
		JsonData data = new JsonData ();
		data["uid"] = SelfPlayerData.Uuid;
		data["urlTypeExt"] = "/";
		SnapAppApi.Request_SnapAppApi (SnapAppApiInterface.Request_GetUserInfo, SnapHttpConfig.NET_REQUEST_GET, data, (SnapRpcDataVO rpcData) => {
			if (rpcData.code == 1)
			{
				JsonData user = rpcData.data["user"];
				PlayerPrefs.SetString ("DeviceID", user["deviceID"].ToString ());
				PlayerPrefs.Save ();
				user["nickname"] = SelfPlayerData.Nickname;
				GameUserService.SaveGameUser (user);
				LogManager.Log ("游客用户的模型ID" , rpcData.data.TryGetString ("modelID"));
				LogManager.Log ("游客用户的表情ID" , rpcData.data.TryGetString ("emotionID"));
				LogManager.Log ("登录用户的省份" , user.TryGetString ("province"));
				LogManager.Log ("登录用户的城市" , user.TryGetString ("city"));
				SelfPlayerData.Province = user.TryGetString ("province");
				SelfPlayerData.City = user.TryGetString ("city");
				SelfPlayerData.ModelId = rpcData.data.TryGetString ("modelID");
				SelfPlayerData.EmotionId = rpcData.data.TryGetString ("emotionID");
				//if(LoginScene.isFirstGoingGame)
				//{
				//	LoadSceneManager.Instance.LoadPlayScene(LevelConfig.AllLevelDic[1001]);
				//}
				//else
				//{
				//	LogManager.Log("ok");
				//	LoadSceneManager.Instance.LoadNormalScene("BookScene");
				//}
				//更新操作
				UploadGameUserInfo ();
			}
		});
	}

	/// <summary>
	/// 上传游戏用户信息
	/// </summary>
	public void UploadGameUserInfo (Action action = null)
	{
		JsonData data = new JsonData ();
		data["uid"] = SelfPlayerData.Uuid;
		data["nickname"] = SelfPlayerData.Nickname;
		data["level"] = SelfPlayerData.Level;
		data["energy"] = SelfPlayerData.Energy;
		data["experience"] = SelfPlayerData.Experience;
		data["avatar"] = SelfPlayerData.AvatarUrl;

		//上传数据的时候做次缓存
		SaveDataUtils.Save<SelfPlayerData> ();
		SnapAppApi.Request_SnapAppApi (SnapAppApiInterface.Request_UploadGameInfo, SnapHttpConfig.NET_REQUEST_PUT, data, (SnapRpcDataVO rpcData) => {
			if (action != null)
			{
				action.Invoke ();
			}
		});
	}

	/// <summary>
	/// 分享接口
	/// </summary>
	public void ShareWeChatGameByLevelID ()
	{
		// 为了确保 MicManager 初始化
		var m = MicManager.Instance;
		SnapAppApi.UploadFileToAlyOSS (MicManager.CurRecordFilePath, (string result) => {
			JsonData data = new JsonData ();
			if (GlobalConst.LoginToApp == true)
			{
				data["uid"] = int.Parse (GlobalConst.Player_ID);
				data["name"] = SelfPlayerData.UserName;
			}
			else
			{
				data["device_id"] = SelfPlayerData.DeviceID;
				data["phone"] = SelfPlayerData.TelphoneNum;
			}
			data["mission_num"] = StaticData.LevelID;
			data["audio_mp3"] = result;

			SnapAppApi.Request_SnapAppApi (SnapAppApiInterface.Request_WeChatShareGame, SnapHttpConfig.NET_REQUEST_POST, data,
				(SnapRpcDataVO dataVo) => {
				LogManager.Log ("share success:::" , dataVo.code);
				});
		});
		if (shareDelegate != null)
		{
			shareDelegate.Invoke ();
		}
	}

	public void SaveLevelVoices (Dictionary<string, string> voiceDic)
	{
		//LogManager.LogWarning (" 12 保存关卡语音地址信息  " , Time.time);
		JsonData data = new JsonData ();
		data["uid"] = SelfPlayerData.Uuid;
		data["levelId"] = StaticData.LevelID;
		data["audios"] = new JsonData ();
		foreach (var item in voiceDic)
		{
			JsonData audioData = new JsonData ();
			audioData["voiceAddr"] = item.Key;
			audioData["text"] = item.Value;
			data["audios"].Add (audioData);
		}

		SnapAppApi.Request_SnapAppApi (SnapAppApiInterface.Request_UploadLevelVoices, SnapHttpConfig.NET_REQUEST_POST, data,
										   (SnapRpcDataVO dataVo) => {
			LogManager.Log (" 12 保存关卡语音地址信息  success:::" , dataVo.code);
										   });
	}

	public void LoadLevelVoices (string uid, int levelId, Action<List<string>, Action> callback, Action fallCallback = null)
	{
		JsonData data = new JsonData ();
		//data["uid"] = SelfPlayerData.Uuid;
		//data["levelId"] = StaticData.LevelID;
		data["uid"] = uid;
		data["levelId"] = levelId;
		SnapAppApi.Request_SnapAppApi (SnapAppApiInterface.Request_GetLevelVoices, SnapHttpConfig.NET_REQUEST_GET, data,
								   (SnapRpcDataVO dataVo) => {
			//LogManager.Log (dataVo.data.ToJson ());
									   List<string> voiceUrls = new List<string> ();
									   JsonData meteData = dataVo.data["meteData"];
									   for (int i = 0; i < meteData.Count; i++)
									   {
										   voiceUrls.Add (meteData[i].TryGetString ("voice"));
									   }
									   callback (voiceUrls, fallCallback);
								   });
	}





}
//LoginRpcProxy