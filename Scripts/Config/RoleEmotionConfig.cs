using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Snaplingo.Config;
using LitJson;

public class RoleEmotionConfig : IConfig
{
	private static RoleEmotionConfig _instance;
	public static RoleEmotionConfig Instance { get { return _instance; } }
	public Dictionary<int, string> roleEmotionDic = new Dictionary<int, string>();
	public RoleEmotionConfig()
	{
		_instance = this;
	}
	public void Fill(string jsonstr)
	{
		JsonData data = JsonMapper.ToObject(jsonstr);
		for (int i = 0; i < data.Count;i++)
		{
			roleEmotionDic.Add(int.Parse(data[i].TryGetString("roleemotionid")), data[i].TryGetString("roleemotionname"));
		}
	}
    public string GetNameById(int emotionId)
	{
		if (!roleEmotionDic.ContainsKey(emotionId))
        {
			LogManager.LogError("[RoleEmotionConfig]can not find emotion " , emotionId);
            return null;
        }
		return roleEmotionDic[emotionId];
	}
    public int GetIdByName(string emotionName)
	{
		foreach(KeyValuePair<int,string> key in roleEmotionDic)
		{
			if(key.Value.Equals(emotionName))
			{
				return key.Key;
			}
		}
		return -1;
	}
}
