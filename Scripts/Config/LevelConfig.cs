using System;
using System.Collections.Generic;
using UnityEngine;
using Snaplingo.Config;
using LitJson;


[Serializable]
public struct LevelData
{
	public int LevelDifficulty;
	public string LevelIcon;
	public int energy;
	public int exp;
	public string levelCityName;
	public int levelID;
	public string levelType;
	public int nextLevelID;
	public string songID;
	public int stageID;
	public int bosslife;
	public string boss_song_name;
	//public int auto_audio;
	public string scenePrefabPath;

}
//LevelInfo


// 场景的数据类
public class LevelConfig : IConfig
{
	public static LevelConfig Instance { get { return _instance; } }
	static LevelConfig _instance;
	public LevelConfig()
	{
		_instance = this;
	}

	static int _difficulty = 1;
	public static int Difficulty
	{
		get { return _difficulty; }
		set { _difficulty = Mathf.Clamp(value, 1, 2); }
	}


	Dictionary<int, LevelData> _allLevelDic;
	public static Dictionary<int, LevelData> AllLevelDic
	{
		get { return Instance._allLevelDic; }
	}


	// ConfigUtils.GetConfig 回调
	public void Fill(string jsonString)
	{
		JsonData json = JsonMapper.ToObject(jsonString);
		_allLevelDic = new Dictionary<int, LevelData>();
		for (int i = 0; i < json.Count; i++)
		{
			LevelData value = JsonUtility.FromJson<LevelData>(json[i].ToJson());
			value.scenePrefabPath = "ScenePrefabs/Scene_" + value.levelID.ToString();
			_allLevelDic.Add(value.levelID, value);
		}
	}

	public static List<LevelData> GetDatasByDifficulty()
	{
		List<LevelData> reslut = new List<LevelData>();
		foreach (LevelData item in AllLevelDic.Values)
		{
			if (item.LevelDifficulty == _difficulty)
			{
				reslut.Add(item);
			}
		}
		return reslut;
	}

    public static LevelData GetLevelDataByID(int id)
    {
        if(AllLevelDic.ContainsKey(id))
        {
            return AllLevelDic[id];
        }
        else
        {
            LevelData nullData = new LevelData();
            nullData.levelID = -1;
            return nullData;
        }
    }

	public static List<LevelData> GetDatasByBook(int id)
	{
		LogManager.Log(" 还没实现 ");
		return null;
	}

	public static bool CheckEnergy()
	{
		return true;

		//int energy = LevelConfig.AllLevelDic[StaticData.LevelID].energy;
		//if (DebugConfigController.Instance._Debug)
		//{
		//	LogManager.Log("测试状态下不扣除" , energy , "体力");
		//	return true;
		//}

		//if (PlayerInfo.Instance.Energy >= energy)
		//{
		//	PlayerInfo.Instance.Energy = PlayerInfo.Instance.Energy - energy;
		//	PlayerInfo.Instance.save();
		//	LogManager.Log("扣除" , energy , "体力");
		//	return true;
		//}
		//else
		//{
		//	LogManager.Log("体力不足~");
		//	PromptManager.Instance.MessageBox(PromptManager.Type.FloatingTip, "体力不足~");
		//	return false;
		//}
	}
}
//LevelConfig

