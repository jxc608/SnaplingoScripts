using UnityEngine;
using System.Collections.Generic;
using System;

public static class OtherPlayerLevelData
{
	#region Data
	public static LevelScroesObject LevelScroes { get; private set; }
	public static int Total { get; private set; }
	public static int PageNo { get; private set; }

	static RankObject selfRankObject;
	static List<RankObject> higherRanks = new List<RankObject>();
	static List<RankObject> lowerRanks = new List<RankObject>();
	static List<RankObject> pageRanks = new List<RankObject>();
	#endregion



	#region Get
	public static RankObject SelfRankObject
	{
		get { return selfRankObject; }
		set { selfRankObject = value; }
	}
	public static List<RankObject> Get_Higher_RankObjects(int num)
	{
		List<RankObject> list = new List<RankObject>();
		for (int i = 0; i < (Mathf.Min(num, higherRanks.Count)); i++)
		{
			list.Add(higherRanks[higherRanks.Count - 1 - i]);
		}
		return list;
	}
	public static List<RankObject> Get_Lower_RankObjects(int num)
	{
		List<RankObject> list = new List<RankObject>();
		for (int i = 0; i < (Mathf.Min(num, lowerRanks.Count)); i++)
		{
			list.Add(lowerRanks[i]);
		}
		return list;
	}
	public static List<RankObject> Get_Page_RankObjects()
	{
		return pageRanks;
	}
	#endregion



	#region Setup 
	public static void Reset()
	{
		selfRankObject = null;
		higherRanks.Clear();
		lowerRanks.Clear();
		pageRanks.Clear();
	}

	public static void Setup_Page(/**string jsonString**/LevelScroesObject scoresObj, int page, int countPerPage)
	{
		Reset();
		LevelScroes = scoresObj;//LevelScroesObject.CreateFromJSON(jsonString);
		Total = LevelScroes.data.total;
		PageNo = page;
		pageRanks = LevelScroes.data.rankingScores;
		for (int i = 0; i < LevelScroes.data.rankingScores.Count; i++)
		{
			LevelScroes.data.rankingScores[i].rank = countPerPage * page + i + 1;
		}
	}
	public static void Setup_10_Before_And_10_After(/**string jsonString**/LevelScroesObject scoresObj)
	{
		Reset();
		LevelScroes = scoresObj;//LevelScroesObject.CreateFromJSON(jsonString);

		Total = LevelScroes.data.total;

		bool isFoundSelf = false;
		int selfRank = 0;
		for (int i = 0; i < LevelScroes.data.rankingScores.Count; i++)
		{
			//print(LevelScroes.data.rankingScores[i].historyID);
			// 临时解决服务器rank少1
			//LevelScroes.data.rankingScores[i].rank += 1;

			if (isFoundSelf == false)
			{
				if (LevelScroes.data.rankingScores[i].uid == SelfPlayerData.Uuid)
				{
					selfRankObject = LevelScroes.data.rankingScores[i];
					isFoundSelf = true;
					selfRank = selfRankObject.rank;
				}
				else
				{
					higherRanks.Add(LevelScroes.data.rankingScores[i]);
				}
			}
			else
			{
				lowerRanks.Add(LevelScroes.data.rankingScores[i]);
			}
		}
		PageNo = selfRank / 10;
		for (int i = 0; i < LevelScroes.data.rankingScores.Count; i++)
		{
			if ((LevelScroes.data.rankingScores[i].rank - 1) / 10 == PageNo)
				pageRanks.Add(LevelScroes.data.rankingScores[i]);
		}
	}
	#endregion





}
//CacheRankData


