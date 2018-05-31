using Snaplingo.UI;
using UnityEngine;
using System;
using UnityEngine.Video;
using System.Collections;
using System.Collections.Generic;


class StartScene : MonoBehaviour
{



	void Start ()
	{
		AppInfo.AppVersion = MiscUtils.GetVersion ();
		//GameStart();
	}


	public static void GameStart ()
	{
		MicManager.OnRuntimeMethodLoad ();
		StaticData.ApplicationStart = true;
		//CorePlayData.CalcFirstLevelScore();

		DWBuglyAgent.Instance.InitBuglyAgent ();

		AnalysisManager.Instance.OnEvent ("enterGame", null);

		//#if UNITY_IPHONE && !UNITY_EDITOR
		//XunFeiSRManager.OnRuntimeMethodLoad(LanguageType.Chinese);
		//#endif

		//if (DebugConfigController.Instance.AutoTest)
		//{
		//	GameObject obj = new GameObject();
		//	obj.AddComponent<AutoTestRunner>();
		//}

		Application.LoadLevel ("LoginScene");
		//LoadSceneManager.Instance.LoadNormalScene("LoginScene");
	}

	//void AddNode()
	//{
	//	PageManager.Instance.CurrentPage.AddNode<StartSceneNode>(true);

	//	if (CorePlaySettings.Instance.m_UseMemoryPool)
	//	{
	//		VideoClip vc = ResourceLoadUtils.Load<VideoClip>("animation");
	//		StaticMemoryPool.AddIntoPool("startAnimation", vc);
	//	}
	//}

	//private void onListenRuningError(string log, string stack, LogType type)
	//{
	//	if (type == LogType.Error || type == LogType.Exception)
	//	{
	//		string url = "api/game_app/v1/test_logs";
	//		long timestamp;
	//		TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
	//		timestamp = Convert.ToInt64(ts.TotalMilliseconds);
	//		JsonData data = new JsonData();
	//           data["uid"] = SelfPlayerData.Uuid;
	//		data["level"] = "xxx";
	//		data["missionID"] = "xxx";
	//		data["log"] = log + "   |||   " + stack;
	//		data["timestamp"] = timestamp;
	//		byte[] myData = System.Text.Encoding.UTF8.GetBytes(data.ToJson());
	//		StaticMonoBehaviour.Instance.StartCoroutine(HttpRequest.Instance.WebRequest(HttpRequest.HttpReqType.POST, url, myData, callback, errorCallback));
	//	}
	//}


	//private void Update()
	//{
	//	if(Input.GetKeyDown(KeyCode.Space))
	//	{
	//		LoadSceneManager.Instance.LoadNormalScene("LoginScene");

	//	}
	//}




	private void callback (string content)
	{
		//print("callback " + content);
	}
	private void errorCallback ()
	{
		//print("errorCallback ");
	}
}
