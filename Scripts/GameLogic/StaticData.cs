using System;
using LitJson;
using UnityEngine;
using Snaplingo.SaveData;
using UnityEngine.Events;

public static class StaticData
{
	public static int LevelID;
	public static bool ApplicationStart;
	public static int Difficulty;
	//评价系数 Exp = [Exp]_基数 x A_评价系数
	public static float[] LevelExpBase = { 2, 1.5f, 1.25f, 1.1f, 1, 0.95f };

	public static LevelData NowLevelData;
    public static string ChoreographerData;


}

