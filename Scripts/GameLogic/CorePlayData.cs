using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CorePlayData
{
	public static int SongID;
	public static string SongScript;
	public static int SongOffset;
	public static SongObject CurrentSong;
	public static SentenceObj CurrentSentence;
	public static int BossLife;
	public static string BossSongName;
	public static string EducationText;
	public static float BMPDelta;
	public static float TB;
	public static int FirstLevelMaxScore;
	// 每关的ID
	public static int ScenePrefabId;

	// Book
	public static bool isFirstTime = true;
	public static int pageID = 0;
    public static List<PlayerReadingData> PlayerReadingData = new List<PlayerReadingData>();

	public static void CalcFirstLevelScore ()
	{
		LevelData firstLevelData = LevelConfig.AllLevelDic[RuntimeConst.FirstLevelID];
		string scriptName = SongConfig.Instance.GetsongScriptBySongIDAndLevelDiffculty (LanguageManager.GetSongIdFromLanguage (firstLevelData.songID), firstLevelData.LevelDifficulty);

		SongObject firstSongObj = BeatmapParse.Parse (scriptName);

		FirstLevelMaxScore = BeatmapParse.GetSongObjectMaxScore (firstSongObj).MaxScore;

	}
}
