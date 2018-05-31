using UnityEngine;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class PlayerInfoSetting : ScriptableObject
{
	public static string PlayerInfoSettingPath = "Settings/PlayerInfoSetting";
    #if UNITY_EDITOR
	[MenuItem("自定义/工具/导出脚本对象/PlayerInfo数据", false, 1)]
    static void CreateDebugController()
    {
		EditorUtils.CreateAsset<PlayerInfoSetting>(PlayerInfoSettingPath);
    }
    #endif

	private static PlayerInfoSetting _instance = null;

	public static PlayerInfoSetting Instance
    {
        get
        {
            if (_instance == null)
            {
				_instance = ResourceLoadUtils.Load<PlayerInfoSetting>(PlayerInfoSettingPath);
            }
            return _instance;
        }
    }

    [Header("默认用户level")]
    public string m_InitLevel = "1";
	[Header("默认用户energy")]
	public string m_InitEnergy = "6";
	[Header("默认用户experience")]
	public string m_InitExperience = "0";
	[Header("默认用户HP")]
	public string m_InitHP = "0";
}

