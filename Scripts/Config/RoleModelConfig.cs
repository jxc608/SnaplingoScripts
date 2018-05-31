using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Snaplingo.Config;
using LitJson;

public class RoleModelConfig : IConfig
{
	private static RoleModelConfig _instance;
	public static RoleModelConfig Instance { get { return _instance; } }
	private Dictionary<int, string> roleModelDic = new Dictionary<int, string>();
	public RoleModelConfig()
	{
		_instance = this;
	}
	public void Fill(string jsonstr)
	{
		JsonData data = JsonMapper.ToObject(jsonstr);
		for (int i = 0; i < data.Count;i++)
		{
			roleModelDic.Add(int.Parse(data[i].TryGetString("rolemodelid")), data[i].TryGetString("rolemodelname"));
		}
	}
    public string GetNameById(int modelId)
	{
		if(!roleModelDic.ContainsKey(modelId))
		{
			LogManager.LogError("[RoleModelConfig]can not find model " , modelId);
			return null;
		}
		return roleModelDic[modelId];
	}
    public int GetIdByName(string modelName)
	{
		foreach(KeyValuePair<int,string> key in roleModelDic)
		{
			if(key.Value.Equals(modelName))
			{
				return key.Key;
			}
		}
		return -1;
	}
}
