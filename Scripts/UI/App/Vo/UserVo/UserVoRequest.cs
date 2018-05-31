using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LitJson;
using TA;
using UnityEngine;
using UnityEngine.SceneManagement;
class UserVoRequest
{

	private UserVoBasic mUserVo;

	private string mUserID;

	private Action<UserVoBasic> mCallback;

	public void RequestUserVo (string _userID, Action<UserVoBasic> _callback)
	{
		mUserID = _userID;
		mCallback = _callback;

		JsonData paramObj = new JsonData ();
		paramObj["uid"] = _userID;
		paramObj["urlTypeExt"] = "/";
		SnapAppApi.Request_SnapAppApi (SnapAppApiInterface.Request_GetUserInfo, SnapHttpConfig.NET_REQUEST_GET, paramObj, RequestUserVoCallback);
	}

	protected void RequestUserVoCallback (SnapRpcDataVO _rpcResultObj)
	{
		if (_rpcResultObj.status == true)
		{
			UserVoProxy.instance ().FinishItemRequest (this);
			JsonData userData = _rpcResultObj.data["user"];
			UserVoBasic userVo = UserVoProxy.instance ().formatVo (userData);
			//把自己登陆的信息扔份数据给SelfPlayerData
			if (userData["id"].ToString () == GlobalConst.Player_IDTemp)
			{
				GameUserService.SaveGameUser (userData);
				LogManager.Log ("登录用户的模型ID" + _rpcResultObj.data.TryGetString ("modelID"));
				LogManager.Log ("登录用户的表情ID" + _rpcResultObj.data.TryGetString ("emotionID"));
				LogManager.Log ("登录用户的省份" + userData.TryGetString ("province"));
				LogManager.Log ("登录用户的城市" + userData.TryGetString ("city"));
				SelfPlayerData.Country = int.Parse (userData.TryGetString ("country"));
				if (DebugConfigController.Instance._debugLanguage == false)
					LanguageManager.languageType = SelfPlayerData.Country == 0 ? LanguageType.Chinese : LanguageType.English;
				else
				{
					if (DebugConfigController.Instance._isChinese)
						LanguageManager.languageType = LanguageType.Chinese;
					else
						LanguageManager.languageType = LanguageType.English;
				}
				XunFeiSRManager.OnRuntimeMethodLoad (LanguageManager.languageType);
				SelfPlayerData.Province = userData.TryGetString ("province");
				SelfPlayerData.City = userData.TryGetString ("city");
				SelfPlayerData.ModelId = _rpcResultObj.data.TryGetString ("modelID");
				SelfPlayerData.EmotionId = _rpcResultObj.data.TryGetString ("emotionID");
				BookSceneManager.GetCourseFormServer ();
				if (SelfPlayerData.ModelId.Equals ("0") || SelfPlayerData.EmotionId.Equals ("0"))
				{
					//用户还没创建角色则更新角色信息到服务器
					RoleManager.Instance.SbmitRoleInfoToServer ();
				}
				else
				{
					//已经创建过角色了则已经是第二次登录了直接进选关界面
					LogManager.Log (SceneManager.GetActiveScene ().name);
					if (SceneManager.GetActiveScene ().name.Equals ("LoginScene"))
					{
						LoadSceneManager.Instance.LoadNormalScene ("BookScene");
					}
				}
			}
			if (mCallback != null)
			{
				mCallback (userVo);
				mCallback = null;
			}
		}
	}

}
