using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Snaplingo.UI;

/// <summary>
/// 封装了BuglyAgent类
/// 所有对于bugly的调用都走这个接口
/// </summary>
public class DWBuglyAgent {

	private static DWBuglyAgent _instance;
	public static DWBuglyAgent Instance
	{
		get
		{
			if(null == _instance)
			{
				_instance = new DWBuglyAgent();
			}

			return _instance;
		}
	}

	private DWBuglyAgent()
	{
		
	}

	public void InitBuglyAgent()
	{
      
		string AppKey_ios;
        string AppKey_Android;
        if (DebugConfigController.Instance.FormalData)
        {
            AppKey_Android = ConfigurationController.Instance.BuglyAppKey_AndroidFormal;
            AppKey_ios = ConfigurationController.Instance.BuglyAppKey_iosFormal;
        }
        else
        {
            AppKey_Android = ConfigurationController.Instance.BuglyAppKey_AndroidTest;
            AppKey_ios = ConfigurationController.Instance.BuglyAppKey_iosTest;
        }
        // 开启SDK的日志打印，发布版本请务必关闭
        BuglyAgent.ConfigDebugMode(false);
        // 注册日志回调，替换使用 'Application.RegisterLogCallback(Application.LogCallback)'注册日志回调的方式
		if(DebugConfigController.Instance._Debug)
		{
			BuglyAgent.RegisterLogCallback(LogCallback);
		}


#if UNITY_IPHONE || UNITY_IOS
        BuglyAgent.InitWithAppId(AppKey_ios);
#elif UNITY_ANDROID
        BuglyAgent.InitWithAppId (AppKey_Android);
#endif

		// 如果你确认已在对应的iOS工程或Android工程中初始化SDK，那么在脚本中只需启动C#异常捕获上报功能即可
		BuglyAgent.EnableExceptionHandler();

		//Application.logMessageReceivedThreaded += LogCallback;
	}

	public void SetUserId(string userID)
	{
		BuglyAgent.SetUserId(userID);
	}
    
	private void LogCallback(string condition, string stackTrace, LogType type)
	{
		if(type == LogType.Exception)
		{
			ExcepitonNode node = PageManager.Instance.CurrentPage.AddNode<ExcepitonNode>(true) as ExcepitonNode;
			node.SetException(condition + "\n" + stackTrace);
		}

	}

}
