using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using LitJson;
using UnityEngine;
/// <summary>
/// Http请求消息头
/// </summary>
public enum HttpHeader
{
	registered,
	anonymous,
}
/// <summary>
/// 静态字段
/// </summary>
public class DancingWordConst
{
	public static string appVersion = "api/game_app/v2/";
	public static string roleTitlehttpAction = "/user_title/titles";
	public static string courseAction = "/game_courses";
	public static string dancProgressAndFinallyDataAction = "/game_dance/dance_by_uid_levelId";
	public static string dancDataAction = "/game_dance";
	public static string roleDataAction = "/user_role";
	public static string userInfo = "/users/user_by_uid";
	public static string gameinfo = "/game_info";
	public static string levelVoice = "/game_level/voice_url";
	public static string levelSupport = "/game_level/like";
}
public class DancingWordAPI : MonoBehaviour
{
	private static DancingWordAPI _instance = null;
	public static DancingWordAPI Instance
	{
		get
		{
			if (_instance == null)
			{
				GameObject go = new GameObject();
				_instance = go.AddComponent<DancingWordAPI>();
				go.name = _instance.GetType().ToString();
			}
			return _instance;
		}
	}
	/// <summary>
	/// 获取用户的所有称号
	/// </summary>
	/// <param name="data">请求参数</param>
	/// <param name="successCallBack">请求成功回调</param>
	/// <param name="failCallBack">请求失败回调</param>
	public void RequestRoleTitleFromServer(JsonData data, Action<string> successCallBack, Action failCallBack)
	{
		StartCoroutine(HttpRequest.Instance.WebRequest(HttpRequest.HttpReqType.GET, DancingWordConst.appVersion + HttpHeader.anonymous + DancingWordConst.roleTitlehttpAction + CreateHttpGetValue(data), null, successCallBack, failCallBack));
	}
	/// <summary>
	/// 更新用户的某一个称号
	/// </summary>
	/// <param name="data">提交参数</param>
	/// <param name="successCallBack">提交成功回调</param>
	/// <param name="failCallBack">提交失败回调</param>
	public void SubmitRoleTitleToServer(JsonData data, Action<string> successCallBack, Action failCallBack)
	{
		StartCoroutine(HttpRequest.Instance.WebRequest(HttpRequest.HttpReqType.POST, DancingWordConst.appVersion + HttpHeader.anonymous + DancingWordConst.roleTitlehttpAction, CreateHttpPostValue(data), successCallBack, failCallBack));
	}
	/// <summary>
	/// 获取当前用户的所有课程信息
	/// </summary>
	/// <param name="data">请求参数</param>
	/// <param name="successCallBack">请求成功回调</param>
	/// <param name="failCallBack">请求失败回调</param>
	public void RequestCourseFromServer(JsonData data, Action<string> successCallBack, Action failCallBack)
	{
		StartCoroutine(HttpRequest.Instance.WebRequest(HttpRequest.HttpReqType.GET, DancingWordConst.appVersion + HttpHeader.anonymous + DancingWordConst.courseAction + CreateHttpGetValue(data), null, successCallBack, failCallBack));
	}
	/// <summary>
	/// 更新用户的课程信息
	/// </summary>
	/// <param name="data">提交参数</param>
	/// <param name="successCallBack">提交成功回调</param>
	/// <param name="failCallBack">提交失败回调</param>
	public void SubmitCourseToServer(JsonData data, Action<string> successCallBack, Action failCallBack)
	{
		StartCoroutine(HttpRequest.Instance.WebRequest(HttpRequest.HttpReqType.POST, DancingWordConst.appVersion + HttpHeader.anonymous + DancingWordConst.courseAction, CreateHttpPostValue(data), successCallBack, failCallBack));
	}
	/// <summary>
	/// 获取舞蹈过程数据和最终数据
	/// </summary>
	/// <param name="data">请求参数</param>
	/// <param name="successCallBack">请求成功回调</param>
	/// <param name="failCallBack">请求失败回调</param>
	public void RequestLevelDancDataFromServer(JsonData data, Action<string> successCallBack, Action failCallBack)
	{
		StartCoroutine(HttpRequest.Instance.WebRequest(HttpRequest.HttpReqType.GET, DancingWordConst.appVersion + HttpHeader.anonymous + DancingWordConst.dancProgressAndFinallyDataAction + CreateHttpGetValue(data), null, successCallBack, failCallBack));
	}
    /// <summary>
	/// 提交舞蹈最终数据到服务器
    /// </summary>
    /// <param name="data">提交参数</param>
    /// <param name="successCallBack">提交成功的回调</param>
    /// <param name="failCallBack">提交失败的回调</param>
	public void SubmitDancDataToServer(JsonData data, Action<string> successCallBack, Action failCallBack)
    {
		StartCoroutine(HttpRequest.Instance.WebRequest(HttpRequest.HttpReqType.POST, DancingWordConst.appVersion + HttpHeader.anonymous + DancingWordConst.dancDataAction, CreateHttpPostValue(data), successCallBack, failCallBack));
    }
    /// <summary>
    /// 提交用户角色的信息到服务器
    /// </summary>
    /// <param name="data">Data.</param>
    /// <param name="successCallBack">Success call back.</param>
    /// <param name="failCallBack">Fail call back.</param>
	public void SubmitRoleDataToServer(JsonData data,Action<string> successCallBack,Action failCallBack)
	{
		StartCoroutine(HttpRequest.Instance.WebRequest(HttpRequest.HttpReqType.POST,DancingWordConst.appVersion+HttpHeader.anonymous+DancingWordConst.roleDataAction,CreateHttpPostValue(data),successCallBack,failCallBack));
	}
    /// <summary>
	/// 请求用户的角色信息(只有角色的信息)
    /// </summary>
    /// <param name="data">Data.</param>
    /// <param name="successCallBack">Success call back.</param>
    /// <param name="failCallBack">Fail call back.</param>
	public void RequestRoleDataFromServer(JsonData data,Action<string> successCallBack,Action failCallBack)
	{
		StartCoroutine(HttpRequest.Instance.WebRequest(HttpRequest.HttpReqType.GET,DancingWordConst.appVersion+HttpHeader.anonymous+DancingWordConst.roleDataAction+CreateHttpGetValue(data),null,successCallBack,failCallBack));
	}
    /// <summary>
    /// 根据用户ID获取用户信息
    /// </summary>
    /// <param name="data">Data.</param>
    /// <param name="successCallBack">Success call back.</param>
    /// <param name="failCallBack">Fail call back.</param>
	public void RequestUserInforFromServer(JsonData data,Action<string> successCallBack,Action failCallBack)
	{
		StartCoroutine(HttpRequest.Instance.WebRequest(HttpRequest.HttpReqType.GET,DancingWordConst.appVersion+HttpHeader.anonymous+DancingWordConst.userInfo+CreateOldHttpGetValue(data),null,successCallBack,failCallBack));
	}
    /// <summary>
    /// 获取App版本号
    /// </summary>
	/// <param name="data">Data.</param>
    /// <param name="successCallBack">Success call back.</param>
    /// <param name="failCallBack">Fail call back.</param>
	public void GetGameInfo(JsonData data, Action<string> successCallBack, Action failCallBack)
	{
		StartCoroutine(HttpRequest.Instance.WebRequest(HttpRequest.HttpReqType.GET, DancingWordConst.appVersion + HttpHeader.anonymous + DancingWordConst.gameinfo + CreateHttpGetValue(data), null, successCallBack, failCallBack));
	}
    /// <summary>
    /// 提交关卡的录音地址到服务器
    /// </summary>
    /// <param name="data">Data.</param>
    /// <param name="successCallBack">Success call back.</param>
    /// <param name="failCallBack">Fail call back.</param>
	public void SubmitLevelVoiceAddrToServer(JsonData data,Action<string> successCallBack,Action failCallBack)
	{
		StartCoroutine(HttpRequest.Instance.WebRequest(HttpRequest.HttpReqType.POST,DancingWordConst.appVersion+HttpHeader.anonymous+DancingWordConst.levelVoice,CreateHttpPostValue(data),successCallBack,failCallBack));
	}
    /// <summary>
    /// 获取关卡的录音数据的地址
    /// </summary>
    /// <param name="data">Data.</param>
    /// <param name="successCallBack">Success call back.</param>
    /// <param name="failCallBack">Fail call back.</param>
	public void RequestLevelVoiceFromServer(JsonData data,Action<string> successCallBack,Action failCallBack)
	{
		StartCoroutine(HttpRequest.Instance.WebRequest(HttpRequest.HttpReqType.GET,DancingWordConst.appVersion+HttpHeader.anonymous+DancingWordConst.levelVoice+CreateHttpGetValue(data),null,successCallBack,failCallBack));
	}
    /// <summary>
    /// 提交关卡舞蹈的点赞数到服务器
    /// </summary>
    /// <param name="data">Data.</param>
    /// <param name="successCallBack">Success call back.</param>
    /// <param name="failCallBack">Fail call back.</param>
	public void SubmitLevelSupportToServer(JsonData data,Action<string> successCallBack,Action failCallBack)
	{
		StartCoroutine(HttpRequest.Instance.WebRequest(HttpRequest.HttpReqType.POST,DancingWordConst.appVersion+HttpHeader.anonymous+DancingWordConst.levelSupport,CreateHttpPostValue(data),successCallBack,failCallBack));
	}
    /// <summary>
    /// 获取被点赞的总数和增长情况
    /// </summary>
    /// <param name="data">Data.</param>
    /// <param name="successCallBack">Success call back.</param>
    /// <param name="failCallBack">Fail call back.</param>
	public void RequestLevelSupportFromServer(JsonData data,Action<string> successCallBack,Action failCallBack)
	{
		StartCoroutine(HttpRequest.Instance.WebRequest(HttpRequest.HttpReqType.GET,DancingWordConst.appVersion+HttpHeader.anonymous+DancingWordConst.levelSupport+CreateHttpGetValue(data),null,successCallBack,failCallBack));
	}
	public string CreateOldHttpGetValue(JsonData data)
	{
		StringBuilder str = new StringBuilder();
        str.Append("/");
		List<string> list = new List<string>(data.Keys);
		foreach (string key in list)
        {
            str.Append(data[key] + "/");
        }
        return str.Remove(str.Length - 1, 1).ToString();
	}
	/// <summary>
	/// 创建Url参数(Get请求)
	/// </summary>
	/// <returns>返回参数列表字符串</returns>
	/// <param name="data">参数</param>
	public string CreateHttpGetValue(JsonData data)
	{
		StringBuilder str = new StringBuilder();
		str.Append("?");
		List<string> list = new List<string>(data.Keys);
		foreach (string key in list)
		{
			str.Append(key + "=" + data[key] + "&");
		}
		return str.Remove(str.Length - 1, 1).ToString();
	}
	/// <summary>
	/// 创建Url参数(Post请求)
	/// </summary>
	/// <returns>返回参数的字节数组</returns>
	/// <param name="data">参数</param>
	private byte[] CreateHttpPostValue(JsonData data)
	{
		return Encoding.UTF8.GetBytes(data.ToJson());
	}
	/// <summary>
	/// 更新服务端的称号数据
	/// </summary>
	/// <param name="roleTitleItemObjects">Role title item objects.</param>
	public void UpDateServerRoleTitleInfo(List<RoleTitleItemObject> roleTitleItemObjects)
	{
		foreach (RoleTitleItemObject roleTitleItemObject in roleTitleItemObjects)
		{
			JsonData data = new JsonData();
			data["titleID"] = roleTitleItemObject.roletitleId;
			data["userID"] = SelfPlayerData.Uuid;
			data["isFinished"] = true;
			data["isUsed"] = roleTitleItemObject.roleTitleStatus == RoleTitleStatus.Weared ? true : false;
			data["progress"] = roleTitleItemObject.roletitleCount;
			SubmitRoleTitleToServer(data, Successful, Failed);
		}
	}   
	/// <summary>
	/// 请求成功的回调
	/// </summary>
	/// <returns>The successful.</returns>
	/// <param name="result">Result.</param>
	private void Successful(string result)
	{
		LogManager.Log("请求服务器成功 = " ,result);
	}
	/// <summary>
	/// 请求失败的回调
	/// </summary>
	private void Failed()
	{
		LogManager.Log("请求服务器失败!");
	}

}
