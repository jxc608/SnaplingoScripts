using UnityEngine;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using Snaplingo.UI;

public class LogManager : GeneralUIManager
{
	static bool _showLog = false;

	public static void Log (params object[] info)
	{
		//CheckDisplayUI ();
		if (DebugConfigController.Instance._PrintLogLevel >= PrintLogLevel.Log)
			Debug.Log (MakeString(info));
	}

	public static void LogWarning (params object[] info)
	{
		//CheckDisplayUI ();
		if (DebugConfigController.Instance._PrintLogLevel >= PrintLogLevel.Warring)
			Debug.LogWarning (MakeString(info));
	}

	public static void LogError (params object[] info)
	{
		//CheckDisplayUI ();
		if (DebugConfigController.Instance._PrintLogLevel >= PrintLogLevel.Error)
			Debug.LogError (MakeString(info));
	}

	private static string MakeString(params object[] info)
	{
		int nLength = 0;
		List<string> logs = new List<string>();
		for (int i = 0; i < info.Length; ++i)
		{
			if(null == info[i])
			{
				logs.Add("null");
			}
			else
			{
				logs.Add(info[i].ToString());
			}

			nLength += logs[i].Length;
		}
		StringBuilder stringBuilder = new StringBuilder(nLength);
		for (int i = 0; i < logs.Count; ++i)
		{
			stringBuilder.Append(logs[i]);
		}

		return stringBuilder.ToString();
	}
}
