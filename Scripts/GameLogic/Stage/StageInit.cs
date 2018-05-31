using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageInit : MonoBehaviour
{
	public StageManager m_StageManager;
	public AudioLoaderStage loader;
	// Use this for initialization
	void Start ()
	{
		StaticData.NowLevelData = LevelConfig.GetLevelDataByID (StaticData.LevelID);
		CorePlayData.SongID = LanguageManager.GetSongIdFromLanguage (StaticData.NowLevelData.songID);
		CorePlayData.SongScript = SongConfig.Instance.GetsongScriptBySongIDAndLevelDiffculty (LanguageManager.GetSongIdFromLanguage (StaticData.NowLevelData.songID), StaticData.NowLevelData.LevelDifficulty);
		CorePlayData.SongOffset = SongConfig.Instance.GetSongOffsetBySongIDAndLevelDiffculty (LanguageManager.GetSongIdFromLanguage (StaticData.NowLevelData.songID), StaticData.NowLevelData.LevelDifficulty);
		CorePlayData.CurrentSong = BeatmapParse.Parse (CorePlayData.SongScript);


		ChoreographerData cData = ChoreographerData.GetChoreographerDataFromJson (StaticData.ChoreographerData);
		loader.LoadAudio ();
		m_StageManager.Init (60f / CorePlayData.CurrentSong.BPM);
		m_StageManager.CreateDataFromWholeChoreographer (cData);
		m_StageManager.SetChampion (true);
		m_StageManager.ShowStage ();


		AnalysisManager.Instance.OnEvent ("enterStage", null);
	}


}
