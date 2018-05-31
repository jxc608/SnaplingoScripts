using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

/// <summary>
/// 语言类型枚举，新增语言继续配在里面就行。
/// </summary>
public enum LanguageType
{
    /// <summary>
    /// 中文
    /// </summary>
    Chinese = 0,
    /// <summary>
    /// 英文
    /// </summary>
    English = 1,
}

public class LanguageManager
{
	private static LanguageManager _instance = null;
    public static LanguageManager Instance
	{
		get
		{
			if(null == _instance)
			{
				_instance = new LanguageManager();
			}
			return _instance;
		}
	}

	private GameManager m_gameManger;
    public static LanguageType languageType = LanguageType.Chinese;
    /// <summary>
    /// 存放配表数据的字典
    /// </summary>
    public Dictionary<string, List<string>> currentLanguageDic;
    /// <summary>
    /// 根据设备的语言设置App初始语言
    /// </summary>
    /// <returns>The current language.</returns>
    /// <param name="language">Language.</param>
    private LanguageType GetCurrentLanguage(SystemLanguage language)
    {
        switch(language)
        {
            case SystemLanguage.English:
                return LanguageType.English;
            case SystemLanguage.Chinese:
                return LanguageType.Chinese;
            case SystemLanguage.ChineseSimplified:
                return LanguageType.Chinese;
            case SystemLanguage.ChineseTraditional:
                return LanguageType.Chinese;
            default:
                return LanguageType.English;
        }
    }

	//public void Awake()
 //   {
	//	//Instance = this;

 //       //DontDestroyOnLoad(gameObject);
 //   }
	//public IEnumerator Start()
	//{
		
	//}

	public IEnumerator Init(GameManager gameManager)
	{
		m_gameManger = gameManager;
        languageType = GetCurrentLanguage(Application.systemLanguage);
		yield return m_gameManger.StartCoroutine(LoadLocalanguageConfig());
	}
	/// <summary>
	/// 加载本地多语言配表
	/// </summary>
	/// <returns>The localanguage config.</returns>
	public IEnumerator LoadLocalanguageConfig()
    {
        currentLanguageDic = new Dictionary<string, List<string>>();
        string path = Application.streamingAssetsPath + "/language/" + "language.txt";
        string wwwPath = null;
#if UNITY_ANDROID
        wwwPath = path;
#elif UNITY_IOS || UNITY_EDITOR
        wwwPath = "file://" + path;
#endif
        WWW www = new WWW(wwwPath);
        yield return www;
        if (string.IsNullOrEmpty(www.error))
        {
            CreateCurrentLanguageDic(www.text);
        }
    }
    /// <summary>
    /// 把配表数据转化成字典存到内存中
    /// </summary>
    /// <param name="data">Data.</param>
    private void CreateCurrentLanguageDic(string data)
    {
        currentLanguageDic = new Dictionary<string, List<string>>();
        string[] lines = data.Split('\n');
        for (int i = 0; i < lines.Length; i++)
        {
            string[] cols = lines[i].Split('$');
            List<string> temp = new List<string>();
            for (int j = 1; j < cols.Length; j++)
            {
                temp.Add(cols[j]);
            }
            currentLanguageDic.Add(cols[0], temp);
        }
    }
    /// <summary>
    /// 通过对象标记和语言的种类返回具体的value
    /// </summary>
    /// <returns>The value by key.</returns>
    /// <param name="key">Key.</param>
    /// <param name="type">Type.</param>
    public string GetValueByKey(string key,LanguageType type)
    {
        return currentLanguageDic[key][(int)type];
    }
    /// <summary>
    /// 根据对象返回该对象的Value
    /// </summary>
    /// <returns>The enum value.</returns>
    /// <param name="obj">Object.</param>
    public string GetEnumValue(GameObject obj)
    {
        return GetValueByKey(obj.GetComponent<LanguageFlagClass>().selfType.ToString(),languageType);
    }
    /// <summary>
	/// 根据歌曲的ID返回当前的关卡应该播放的歌曲(中文或者英文)
    /// </summary>
    /// <returns>The song identifier from language.</returns>
    /// <param name="songId">Song identifier.</param>
    public static int GetSongIdFromLanguage(string songId)
    {
        if(languageType == LanguageType.Chinese)
        {
            return int.Parse(songId.Split('|')[0]);
        }
        else
        {
            if(songId.Split('|').Length == 1)
            {
                return int.Parse(songId.Split('|')[0]);
            }
            return int.Parse(songId.Split('|')[1]);
        }
    }
}
