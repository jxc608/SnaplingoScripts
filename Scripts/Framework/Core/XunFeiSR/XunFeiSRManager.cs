using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using System;

public class XunFeiSRManager : Manager
{
	public static XunFeiSRManager Instance = GetManager<XunFeiSRManager>();
	private enum Status { None, Listening }
	private Status m_Status = Status.None;
#if UNITY_IPHONE && !UNITY_EDITOR
    [DllImport("__Internal")]
	private static extern void _StartUp(string id,int languageType);

    [DllImport("__Internal")]
	private static extern void _StartRecognize();

    [DllImport("__Internal")]
	private static extern void _StopRecognize();

    [DllImport("__Internal")]
	private static extern void _CancelRecognize();

    void GetResult(string content)
    {
	//LogManager.Log("unity get content is:" , content);
        if (m_Callback != null && !string.IsNullOrEmpty(content))
        {
            m_Callback(content);
        }
    }
	#endif
	//[RuntimeInitializeOnLoadMethod]
    public static void OnRuntimeMethodLoad(LanguageType type)
    {
		LogManager.Log("XunFeiSRManager.OnRuntimeMethodLoad: " , type);
		#if UNITY_IPHONE && !UNITY_EDITOR
        _StartUp(ConfigurationController.Instance._XunfeiSRID, (int)type);
		#endif
    }

	private Action<string> m_Callback;
	public void StartListen(Action<string> callback)
	{
		if (Application.internetReachability == NetworkReachability.NotReachable)
		{
			LogManager.LogError("not internet reachable");
			return;
		}

		if (m_Status == Status.Listening) return;

		m_Status = Status.Listening;
#if UNITY_IPHONE && !UNITY_EDITOR
        _StartRecognize();
        m_Callback = callback;
#endif
	}

	public void StopListen()
	{
		m_Status = Status.None;
#if UNITY_IPHONE && !UNITY_EDITOR
        _StopRecognize();
        m_Callback = null;
#endif
	}


}
