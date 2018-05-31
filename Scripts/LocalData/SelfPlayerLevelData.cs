using LitJson;
using Snaplingo.SaveData;
using System.Collections.Generic;
using UnityEngine.Events;
using System;
using UnityEngine;


[Serializable]
public class SelfPlayerLevelData : ISerializationCallbackReceiver
{
	[NonSerialized]
	public UnityEvent changeNameEvent = new UnityEvent ();
	private static SelfPlayerLevelData instance;

	public static SelfPlayerLevelData Instance {
		get { return instance = (instance == null) ? new SelfPlayerLevelData () : instance; }
		set { instance = value; }
	}

	/**--------------------- Json 游戏用户数据  ---------------------**/

	[SerializeField]
	List<int> keys = new List<int> ();
	[SerializeField]
	List<PlayerInfoObject> values = new List<PlayerInfoObject> ();
	public Dictionary<int, PlayerInfoObject> levelDic = new Dictionary<int, PlayerInfoObject> ();
	/**--------------------- Json 游戏用户数据  ---------------------**/


	public static int TempScore { get; set; }
	public static float TempAccuracy { get; set; }
	public static int TempMaxCombo { get; set; }
	public static int TempLeftLife { get; set; }
	public static float TempReadAccuracy { get; set; }
	public static float TempWordAccuracy { get; set; }
	public static int TempRankIncrement { get; set; }
    /// <summary>
    /// 自己的过程数据
    /// </summary>
    /// <value>The temp level danc data.</value>
	public static string TempSelfLevelProcessData { get; set; }
    /// <summary>
    /// 别人的过程数据
    /// </summary>
    /// <value>The temp other level operation danc data.</value>
	public static string TempOtherLevelProcessData { get; set; }
    /// <summary>
    /// 上传用户成绩成功后会返回一个historyID和匹配的玩家的信息 
	/// 当生成完用户的最终舞蹈数据以后传最终舞蹈数据到服务器时需要把这个字段回传给服务器
    /// </summary>
    /// <value>The temp history identifier.</value>
	public static string TempHistoryId { get; set; }

	public static Dictionary<int, PlayerInfoObject> LevelDic {
		get { return Instance.levelDic; }
	}
	public static PlayerInfoObject CurInfo {
		get {
			if (LevelDic.ContainsKey (StaticData.LevelID))
				return LevelDic[StaticData.LevelID];
			else
				return Instance.AddLevelData (StaticData.LevelID);
		}
	}
	public static int MaxScore {
		get {
			if (LevelDic.ContainsKey (StaticData.LevelID))
				return LevelDic[StaticData.LevelID].maxScore;
			else
				return 0;
		}
	}
	public static int ScoreIncrement { get; private set; }

	public static int CurRank {
		get {
			if (LevelDic.ContainsKey (StaticData.LevelID))
				return LevelDic[StaticData.LevelID].rank;
			else
				return 0;
		}
	}
	public static float CurAccuracy {
		get {
			if (LevelDic.ContainsKey (StaticData.LevelID))
				return LevelDic[StaticData.LevelID].accuracy;
			else
				return 0;
		}
	}
	public static int CurDifficulty {
		get {
			if (LevelDic.ContainsKey (StaticData.LevelID))
				return LevelDic[StaticData.LevelID].difficulty;
			else
				return 1;
		}
	}
	public static int Cur_maxCombo {
		get { return LevelDic[StaticData.LevelID].maxCombo; }
		set { LevelDic[StaticData.LevelID].maxCombo = value; }
	}
	public static int Cur_leftLife {
		get { return LevelDic[StaticData.LevelID].leftLife; }
		set { LevelDic[StaticData.LevelID].leftLife = value; }
	}
	public static float Cur_readAccuracy {
		get { return LevelDic[StaticData.LevelID].readAccuracy; }
		set { LevelDic[StaticData.LevelID].readAccuracy = value; }
	}
	public static float Cur_wordAccuracy {
		get { return LevelDic[StaticData.LevelID].wordAccuracy; }
		set { LevelDic[StaticData.LevelID].wordAccuracy = value; }
	}


	#region Implementation
	public void OnBeforeSerialize ()
	{
		keys = new List<int> (levelDic.Keys);
		values = new List<PlayerInfoObject> (levelDic.Values);
	}

	public void OnAfterDeserialize ()
	{
		var count = Math.Min (keys.Count, values.Count);
		levelDic = new Dictionary<int, PlayerInfoObject> (count);
		for (var i = 0; i < count; ++i) {
			levelDic.Add (keys[i], values[i]);
		}
	}
	#endregion

	public static void Reset ()
	{
		TempScore = 0;
		TempAccuracy = 0;
		TempMaxCombo = 0;
		TempLeftLife = 0;
		TempReadAccuracy = 0;
		TempWordAccuracy = 0;
	}


	public PlayerInfoObject AddLevelData (int levelId)
	{
		if (levelDic.ContainsKey (levelId)) {
			return levelDic[levelId];
		}
		else {
			string nickName = SelfPlayerData.Nickname;
			var playerInfo = new PlayerInfoObject (nickName);
			levelDic.Add (levelId, playerInfo);
			return playerInfo;
		}
	}

	public static bool HasLocalData (int levelId)
	{
		return (LevelDic.ContainsKey (levelId) && LevelDic[levelId].rank > 0);
	}

	public static void SaveToLocal (int new_score, float accuracy, int combo, int life, float readAccuracy, float wordAccuracy, string processData)
	{
		new_score = Mathf.Clamp (new_score, 0, 999999999);
		ScoreIncrement = new_score - MaxScore;
		TempScore = new_score;
		TempAccuracy = accuracy;
		TempMaxCombo = combo;
		TempLeftLife = life;
		TempReadAccuracy = readAccuracy;
		TempWordAccuracy = wordAccuracy;
		TempSelfLevelProcessData = processData;
		if (ScoreIncrement <= 0 || life == -1) {
			ScoreIncrement = 0;
			//return;
		}

		// 解锁关卡
		if(SelfPlayerData.Instance.GetLevelStatusByLevelId(StaticData.LevelID+1).isPay&&!SelfPlayerData.Instance.GetLevelStatusByLevelId(StaticData.LevelID + 1).isUnLock)
		{
			SelfPlayerData.Instance.SetLevelStatus(StaticData.LevelID);
			LogManager.Log(" 解锁关卡 = " , StaticData.LevelID);
		}      
		CurInfo.maxScore = new_score;
		CurInfo.accuracy = accuracy;
		CurInfo.maxCombo = combo;
		CurInfo.leftLife = life;
		CurInfo.readAccuracy = readAccuracy;
		CurInfo.wordAccuracy = wordAccuracy;
		CurInfo.rank = -1;
		SaveDataUtils.Save<SelfPlayerData> ();

	}


}
public class LevelStatus
{
	public bool isPay;
	public bool isUnLock;
}
//SelfPlayerInfo


