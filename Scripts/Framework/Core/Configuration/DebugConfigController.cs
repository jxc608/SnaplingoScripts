using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public enum PrintLogLevel
{
	None = 0,
	Error,
	Warring,
	Log
    
    
    
}

public class DebugConfigController : ScriptableObject
{
	public static string DebugConfigControllerPath = "Settings/DebugConfigController";

	private static DebugConfigController _instance = null;

	public static DebugConfigController Instance {
		get {
			if (_instance == null)
			{
				_instance = Resources.Load<DebugConfigController> (DebugConfigControllerPath);
#if UNITY_EDITOR
				if (_instance == null)
				{
					EditorUtils.CreateAsset<DebugConfigController> (DebugConfigController.DebugConfigControllerPath);
					_instance = Resources.Load<DebugConfigController> (DebugConfigControllerPath);
				}
#endif
			}
			return _instance;
		}
	}

#if UNITY_EDITOR
	public Dictionary<string, object> _tempFields = null;

	public void Pack ()
	{
		_tempFields = new Dictionary<string, object> ();

		var fields = GetType ().GetFields ();
		foreach (var field in fields)
		{
			if (field.Name == "DebugConfigControllerPath" || field.Name == "_tempFields" || field.Name == "_instance")
				continue;

			_tempFields.Add (field.Name, field.GetValue (Instance));
		}
	}

	public void Unpack ()
	{
		var fields = GetType ().GetFields ();
		foreach (var field in fields)
		{
			if (field.Name == "DebugConfigControllerPath" || field.Name == "_tempFields" || field.Name == "_instance")
				continue;

			if (_tempFields != null && _tempFields.ContainsKey (field.Name))
				field.SetValue (Instance, _tempFields[field.Name]);
		}
	}
#endif

	[Space (10)]
	[Header (" 打包 修改 !!!   正式服 false")]
	public bool _Debug = true;
	[HideInInspector]
	public int HttpHostIndex = 0;
	[Header (" 打包 修改 !!!   正式服 true")]
	public bool _formalData = false;
	[Header (" 打包 修改 !!!   正式服 false")]
	public bool _debugLanguage = false;

	public bool _isChinese = false;


	public bool FormalData {
		get { return _formalData; }
	}
	[Header(" 打包 修改 !!!   正式服 None")]
	public PrintLogLevel _PrintLogLevel = PrintLogLevel.Log;

	public void ForceProductionServer ()
	{
		HttpHostIndex = 1;
	}


	[Space (20)]
	public bool _TestBundleInEditor = false;

	public bool TestBundleInEditor {
		get { return _Debug && _TestBundleInEditor; }
	}

	[Space (10)]
	public bool _CheckBundleInStreamingAssets = false;

	public bool CheckBundleInStreamingAssets {
		get { return _Debug && _CheckBundleInStreamingAssets; }
	}

	[Space (10)]
	public bool _autoTest = false;
	public bool AutoTest {
		get { return _autoTest; }
	}

	[Space (10)]
	public bool _RunTimeEditor = false;
	public bool RunTimeEditor {
		get { return _RunTimeEditor; }
	}


}
