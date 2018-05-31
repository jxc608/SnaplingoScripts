using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LitJson;
using Snaplingo.SaveData;
using UnityEngine;

class GameUserService
{
	/// <summary>
	/// 保存游戏数据到本地
	/// </summary>
	/// <param name="user"></param>
	public static void SaveGameUser(JsonData user)
	{


		SelfPlayerData.PlayerLevelData = new SelfPlayerLevelData();
		//首先加载UserData，判断缓存里面是否有数据
		SaveDataUtils.Load<SelfPlayerData>();

		SelfPlayerData.DeviceID = user["deviceID"].ToString();
		SelfPlayerData.Uuid = user["id"].ToString();

		SelfPlayerData.AppUID = user["appUID"].ToString();
		SelfPlayerData.AvatarUrl = user["avatar"].ToString();
		SelfPlayerData.Sex = int.Parse(user["sex"].ToString());
		SelfPlayerData.Age = int.Parse(user["age"].ToString());
		SelfPlayerData.Country = int.Parse(user["country"].ToString());
		SelfPlayerData.Nickname = user["nickname"].ToString();
		SelfPlayerData.Profile = user["profile"].ToString();

		int server_lv = int.Parse(user["level"].ToString());
		int server_energy = int.Parse(user["energy"].ToString());
		int server_experience = int.Parse(user["experience"].ToString());

		//暂时以本地的数据为基准
		//SelfPlayerData.Level = server_lv;
		//SelfPlayerData.Energy = server_energy;
		//SelfPlayerData.Experience = server_experience;

		if (SelfPlayerData.Level <= server_lv)
		{
			SelfPlayerData.Level = server_lv;
		}
		if (SelfPlayerData.Energy <= server_energy)
		{
			SelfPlayerData.Energy = server_energy;
		}
		if (SelfPlayerData.Experience <= server_experience)
		{
			SelfPlayerData.Experience = server_experience;
		}

		SelfPlayerLevelData.Instance = SelfPlayerData.PlayerLevelData;
		SaveDataUtils.Save<SelfPlayerData>();
		GlobalConst.Player_IDTemp = SelfPlayerData.Uuid;

		InitBuyly();
		UploadUserScore();
	}

	public static void InitUser()
	{

	}

	public static void UploadUserScore()
	{
		HttpHandler.UploadScore(null, false, false);
	}

	/// <summary>
	/// 初始化bugly
	/// </summary>
	private static void InitBuyly()
	{
		DWBuglyAgent.Instance.SetUserId(SelfPlayerData.Uuid);

	}

}
