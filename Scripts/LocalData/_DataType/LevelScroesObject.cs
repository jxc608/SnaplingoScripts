using System;
using System.Collections.Generic;
using UnityEngine;

///API文档 http://wiki.snaplingo.net/pages/viewpage.action?pageId=2851295

[Serializable]
public class LevelScroesObject
{
	public bool status;
	public string message;
	public Data data;

	public static LevelScroesObject CreateFromJSON(string jsonString)
	{
		LogManager.Log(jsonString);
		return JsonUtility.FromJson<LevelScroesObject>(jsonString);
	}

	[Serializable]
	public class Data
	{
		public List<RankObject> rankingScores;
		public int total;
	}
	//Data
}
//LevelScroesObject

[Serializable]
public class RankObject
{
	public string id;
	public string uid;
	public UserObject user;
	public int levelID;
	public int score;
	public float accuracy;
	public int rank;
	public string historyID;
}
//RankObject

[Serializable]
public class UserObject
{
	public string id;
	public string nickname;
	public string avatar;
	public int level;
	public int energy;
	public int experience;
    public int country;

}
//UserObject