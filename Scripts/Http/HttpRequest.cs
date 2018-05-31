using System;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine;
using System.Collections.Generic;
using LitJson;

public class HttpRequest : Manager
{
	public enum HttpReqType { PUT, POST, GET }
	public static HttpRequest Instance { get { return GetManager<HttpRequest>(); } }
	string HostName;
	string Token = ConfigurationController.Instance.HttpToken;
	string Https;

	protected override void Init()
	{
		base.Init();

		if (DebugConfigController.Instance.FormalData == false)
		{
			// 测试服
			Https = "http://" + ConfigurationController.Instance.HttpHostList[0] + "/";
		}
		else
		{
			// 正式服
			Https = "https://" + ConfigurationController.Instance.HttpHostList[1] + "/";
		}
	}

	public IEnumerator WebRequest(
		HttpReqType type,
		string url,
		byte[] data = null,
		Action<string> successCallback = null,
		Action failCallback = null)
	{
		UnityWebRequest www = null;
		switch (type)
		{
			case HttpReqType.GET:
				www = UnityWebRequest.Get(Https + url);
				break;
			case HttpReqType.PUT:
				www = UnityWebRequest.Put(Https + url, data);
				break;
			case HttpReqType.POST:
				if (data != null)
				{
					www = UnityWebRequest.Put(Https + url, data);
				}
				else
				{
					JsonData json = new JsonData();
					json["AppName"] = "MusicGame";
					byte[] Data = System.Text.Encoding.UTF8.GetBytes(json.ToJson());
					www = UnityWebRequest.Put(Https + url, Data);
				}
				www.method = UnityWebRequest.kHttpVerbPOST;
				break;
		}
		LogManager.Log(Https , url);
		www.SetRequestHeader("Content-Type", "application/json;charset=utf-8");
		www.SetRequestHeader("Token", SnapHttpConfig.NOT_LOGINED_APP_TOKEN);
		www.SetRequestHeader("UserToken", SnapHttpConfig.LOGINED_APP_TOKEN);
		yield return www.Send();

		bool exceptionOccur = false;
		try
		{
			if (!string.IsNullOrEmpty(www.error))
			{
				exceptionOccur = true;
				LogManager.Log(www.error);
			}
			else
			{
				CheckResponseError(Https + url, www.downloadHandler.text,successCallback);
			}
		}
		catch (Exception e)
		{
			LogManager.LogError(e.StackTrace);
			LogManager.LogError(e.Message , "\n webRequest faild: " , url);
			exceptionOccur = true;
		}
		finally
		{
			if (failCallback != null && exceptionOccur)
				failCallback.Invoke();
		}
	}

	private void CheckResponseError(string url, string result,Action<string> successCallback = null)
	{
		JsonData jsonResult = JsonMapper.ToObject(result);
		if(!bool.Parse(jsonResult["status"].ToString()))
		{
			LogManager.LogError("URL:" , url , ",   [UnityWebRequest]Check Response Error! error code: "
						   , jsonResult["code"] , ", err message: " , jsonResult["message"]);

            //login toke 错误，重新返回登录界面
			if(int.Parse(jsonResult["code"].ToString()) == (int)HttpErrorCode.LoginTokenError)
			{
				GameManager.LoginOnOtherDivices();
			}
            
		}

		if (successCallback != null)
        {
			//LogManager.Log("Http收到 = " , www.downloadHandler.text);
			LogManager.LogWarning("UnityWebRequest response-----------\n" ,
			                 "data: " , result);
			successCallback.Invoke(result);
        }
	}
}