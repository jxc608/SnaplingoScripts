using System;
using LitJson;
using Snaplingo.SaveData;
using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

public static class HttpHandler
{

	/// 存入 SelfPlayerLevelData
	public static void UploadScore(Action callback = null,
								   bool successNeedCallback = true,
								   bool failNeedCallback = true)
	{
		string uuid = SelfPlayerData.Uuid;
		if (SelfPlayerLevelData.MaxScore <= 0 || string.IsNullOrEmpty(uuid)) return;

		//string url = "api/game_app/v1/level_scores";
		JsonData data = new JsonData();
		int levelID = StaticData.LevelID;
		data["levelID"] = levelID;
		data["accuracy"] = SelfPlayerLevelData.TempAccuracy;
		data["score"] = SelfPlayerLevelData.TempScore;
		data["uid"] = uuid;
		data["maxCombo"] = SelfPlayerLevelData.TempMaxCombo;
		data["finalHP"] = SelfPlayerLevelData.TempLeftLife;
		data["readAccuracy"] = SelfPlayerLevelData.TempReadAccuracy;
		data["wordAccuracy"] = SelfPlayerLevelData.TempWordAccuracy;
		data["processData"] = SelfPlayerLevelData.TempSelfLevelProcessData;
		//byte[] Data = System.Text.Encoding.UTF8.GetBytes(data.ToJson());
		SelfPlayerData.PlayerLevelData = SelfPlayerLevelData.Instance;
		SaveDataUtils.Save<SelfPlayerData>();

		SelfPlayerLevelData.Reset();



		SnapAppApi.UploadGameScores(data, (SnapRpcDataVO vo) =>
		{
			//LogManager.Log("wangji",vo.ToString());
			//LogManager.Log(" UploadScore.收到 = " , jsonString);
			JsonData json = vo.data;//JsonMapper.ToObject(jsonString);
			LogManager.Log("上传分数成功之后：" , json.ToJson());
			SelfPlayerLevelData.Instance.AddLevelData(levelID);
			SelfPlayerLevelData.LevelDic[levelID].rank = int.Parse(json.TryGetString("rank", "0"));
			SelfPlayerLevelData.TempHistoryId = json["enemy"].TryGetString("historyID");
			SelfPlayerLevelData.TempOtherLevelProcessData = json["enemy"].TryGetString("TargetUsers");
			SaveDataUtils.Save<SelfPlayerData>();
			if (callback != null && successNeedCallback)
				callback.Invoke();
		});
	}

	/// 存入 SelfPlayerData
	public static void UploadUser(Action callback = null)
	{

		LoginRpcProxy.getInstance().UploadGameUserInfo(callback);
	}

	/// 存入 OtherPlayerLevelData
	public static void Request_Page(int page, Action callback = null, int countPerPage = 10)
	{
		JsonData data = new JsonData();
		data["levelID"] = StaticData.LevelID;
		data["page"] = page;
		data["per_page"] = countPerPage;
		data["from_index"] = 0;
		SnapAppApi.GetRankScoresByLevelID(data, (SnapRpcDataVO vo) =>
		{
			//LogManager.Log(vo.data.ToJson());
			LevelScroesObject scoresObject = levelObject(vo);
			if (scoresObject.data == null)
			{
				return;
			}
			OtherPlayerLevelData.Setup_Page(scoresObject, page, countPerPage);
			if (callback != null) callback.Invoke();
		});
	}

	/// 存入 OtherPlayerLevelData
	public static void Request_10_Before_And_10_After(Action callback = null, Action failCallback = null)
	{
		JsonData data = new JsonData();
		data["levelID"] = StaticData.LevelID;
		data["uid"] = SelfPlayerData.Uuid;
		SnapAppApi.GetRankOfTenByLevelAndUserID(data, (SnapRpcDataVO vo) =>
		{
			//string jsonString = JsonMapper.ToJson(vo);
			LogManager.Log(" Request_10_Before_And_10_After.收到 = " , vo.data);
			LevelScroesObject scoresObject = levelObject(vo);
			if (scoresObject.data == null)
			{
				if (failCallback != null) failCallback.Invoke();
				return;
			}
			else
			{
				OtherPlayerLevelData.Setup_10_Before_And_10_After(scoresObject);
				if (callback != null) callback.Invoke();
			}
		});
	}

	public static LevelScroesObject levelObject(SnapRpcDataVO vo)
	{
		LevelScroesObject scoresObj = new LevelScroesObject();
		scoresObj.status = vo.status;
		scoresObj.message = vo.message;
		if(vo.data!=null)
			LogManager.Log("获取排行榜时服务器返回数据：" , vo.data.ToJson());
		if (vo.data != null && ((IDictionary)vo.data).Keys.Count > 0)
		{
			string jsonString = JsonMapper.ToJson(vo.data);
			LevelScroesObject.Data scoreData = JsonUtility.FromJson<LevelScroesObject.Data>(jsonString);
			scoresObj.data = scoreData;
		}
		return scoresObj;
	}

}
//HttpHandler