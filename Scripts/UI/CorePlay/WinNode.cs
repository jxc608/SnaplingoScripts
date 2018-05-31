using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Snaplingo.UI;
using System.Collections.Generic;
using Snaplingo.SaveData;

public class WinNode : Node
{
	#region [ --- Property --- ]
	float enterDelay = 1.2f;
	float addScoreTime = 2.4f;

	public PlayerInfoObject myInfo;
	public List<RankObject> higherRanks;
	public List<RankObject> lowerRanks;
	CommonPromptNode m_promptNode;
	#endregion
	#region [ --- Object References --- ]
	[Header (" --- Object References ---")]
	public Text text_rankUpValue;
	public GameObject obj_newRecord, obj_BG2, obj_rankUp;
	public Button btn_retry, btn_back, btn_next, btn_bigList;

	[Header (" --- PlayerInfoPanels References ---")]
	public PlayerInfoPanel[] playerInfoPanels;
	public GameObject obj_waitIcon;

	[Header (" --- DifficultyTip References ---")]
	public GameObject obj_difficultyPanel;
	public Button btn_ok, btn_cancel;

	//这个地方暂时先放贴图的引用，之后处理多语言的时候统一改，如果有那一天的话...
	[Header (" --- PromptTex ---")]
	[SerializeField]
	public Sprite TexSure;
	[SerializeField]
	public Sprite TexCancel;
	[SerializeField]
	public Sprite TexUnlock;
	[SerializeField]
	public Sprite TexSure_EN;
	[SerializeField]
	public Sprite TexCancel_EN;
	[SerializeField]
	public Sprite TexUnlock_EN;
	#endregion



	#region [ --- override --- ]
	public override void Init (params object[] args)
	{
		base.Init (args);
		btn_retry.onClick.AddListener (() => {
			AnalysisManager.Instance.OnEvent ("100005", null, "游戏中", "游戏成功，再来一遍：" + StaticData.LevelID);
			OnClickRetry ();
		});
		btn_back.onClick.AddListener (() => {
			AnalysisManager.Instance.OnEvent ("100005", null, "游戏中", "游戏成功，返回列表：" + StaticData.LevelID);
			OnClickBack ();
		});
		btn_next.onClick.AddListener (() => {
			AnalysisManager.Instance.OnEvent ("100005", null, "游戏中", "游戏成功，下一关卡：" + StaticData.LevelID);
			OnClickNext ();
		});
		btn_bigList.onClick.AddListener (() => {
			AnalysisManager.Instance.OnEvent ("100005", null, "游戏中", "游戏成功，打开排行榜：" + StaticData.LevelID);
			PageManager.Instance.CurrentPage.AddNode<ScoreListNode> (true);
		});

		btn_ok.onClick.AddListener (() => {
			AnalysisManager.Instance.OnEvent ("100005", null, "游戏中", "游戏成功，确认提高难度：" + StaticData.LevelID);
			DifficultyLevelUp ();
		});
		btn_cancel.onClick.AddListener (() => {
			AnalysisManager.Instance.OnEvent ("100005", null, "游戏中", "游戏成功，取消提高难度：" + StaticData.LevelID);
			DifficultyCancel ();
		});
		//btn_share.onClick.AddListener(() =>
		//{
		//	//PageManager.Instance.CurrentPage.GetNode<ShareActivityNode>().Open();
		//});
		Reset ();
	}

	public override void Open ()
	{

		base.Open ();
		//PageManager.Instance.CurrentPage.GetComponent<CorePlayPage> ().ShowPauseButton ();
		LevelStatus status = SelfPlayerData.Instance.GetLevelStatusByLevelId (StaticData.NowLevelData.nextLevelID);
		if (StaticData.NowLevelData.nextLevelID > 0 && status.isPay)
		{
			btn_next.interactable = true;
		}

		if (MiscUtils.IsOnline () && SelfPlayerLevelData.TempScore > 0)
		{
			HttpHandler.UploadScore (OnReceive_ScoreData, true, false);
		}

		if (CorePlayData.BossLife > 0)
		{
			//btn_share.gameObject.SetActive(false);
		}

	}
	public override void Close (bool destroy = false)
	{
		base.Close (destroy);
		Reset ();
	}

	void Reset ()
	{
		StopAllCoroutines ();
		foreach (var item in playerInfoPanels) { item.Reset (); }
		SelfPlayerLevelData.TempScore = 0;
		SelfPlayerLevelData.TempAccuracy = 0;
		btn_bigList.interactable = false;
		obj_waitIcon.SetActive (true);
		obj_newRecord.SetActive (false);
		obj_BG2.SetActive (false);
		obj_rankUp.SetActive (false);
		obj_rankUp.transform.SetParent (transform, false);
		obj_difficultyPanel.SetActive (false);
		gameObject.SetActive (false);
	}
	#endregion



	#region [--- Http Call Back ---]
	void OnReceive_ScoreData ()
	{
		if (MiscUtils.IsOnline ())
		{
			HttpHandler.Request_10_Before_And_10_After (OnReceive_10_10_RankData);
		}
	}

	void OnReceive_10_10_RankData ()
	{
		if (SelfPlayerLevelData.TempRankIncrement < 0)
		{
			SelfPlayerLevelData.TempRankIncrement = 0;
		}
		else
		{
			SelfPlayerLevelData.TempRankIncrement = SelfPlayerLevelData.TempRankIncrement - SelfPlayerLevelData.CurRank;
		}
		LogManager.LogWarning (" >>>> = ", SelfPlayerLevelData.TempRankIncrement);

		SetupRankPanel ();
	}
	#endregion


	public void SetupRankPanel ()
	{
		// 拿比我好的人的信息
		higherRanks = OtherPlayerLevelData.Get_Higher_RankObjects (1);
		// 拿比我差的人的信息
		lowerRanks = OtherPlayerLevelData.Get_Lower_RankObjects (2);
		// 拿我自己信息
		myInfo = SelfPlayerLevelData.CurInfo;

		if (myInfo == null || gameObject == null)
			return;

		StartCoroutine (_CorActiveButton ());
		obj_waitIcon.SetActive (false);
		// 临时
		var myRank = new RankObject ();
		myRank.uid = SelfPlayerData.Uuid;
		myRank.user = new UserObject ();
		myRank.user.avatar = SelfPlayerLevelData.CurInfo.avatar;
		myRank.user.nickname = SelfPlayerLevelData.CurInfo.nickName;
		myRank.rank = OtherPlayerLevelData.SelfRankObject.rank;
		myRank.score = OtherPlayerLevelData.SelfRankObject.score;
		myRank.accuracy = OtherPlayerLevelData.SelfRankObject.accuracy;


		if (SelfPlayerLevelData.TempRankIncrement > 0)
		{
			//LogManager.LogWarning("上升了");
			int index = 0;
			var selfPanel = playerInfoPanels[index];


			selfPanel.Setup (myRank, true)
					 .PlayMoveUp (enterDelay, addScoreTime);
			obj_newRecord.SetActive (SelfPlayerLevelData.ScoreIncrement > 0);
			obj_newRecord.transform.SetParent (selfPanel.transform, false);

			index++;
			foreach (var item in lowerRanks)
			{
				if (index < playerInfoPanels.Length)
					playerInfoPanels[index++].Setup (item)
											 .PlayMoveDown (enterDelay, addScoreTime);
			}

			text_rankUpValue.text = SelfPlayerLevelData.TempRankIncrement.ToString ();
			obj_rankUp.transform.SetParent (selfPanel.transform, false);
			obj_rankUp.transform.SetSiblingIndex (0);
			StartCoroutine (_CorShowRankUpAnim ());
		}
		else
		{
			//LogManager.LogWarning("排名未变化");
			obj_rankUp.SetActive (false);
			int index = 0;
			foreach (var item in higherRanks)
			{
				playerInfoPanels[index++].Setup (item);
			}

			// 自己
			var selfPanel = playerInfoPanels[index++];
			selfPanel.Setup (myRank, true)
					 .PlayMoveIn (enterDelay, addScoreTime);
			obj_newRecord.SetActive (SelfPlayerLevelData.ScoreIncrement > 0);
			obj_newRecord.transform.SetParent (selfPanel.transform, false);

			foreach (var item in lowerRanks)
			{
				if (index < playerInfoPanels.Length)
					playerInfoPanels[index++].Setup (item);
			}
		}
	}

	IEnumerator _CorShowRankUpAnim ()
	{
		obj_BG2.SetActive (true);
		yield return new WaitForSeconds (enterDelay);
		obj_rankUp.SetActive (true);
		yield return new WaitForSeconds (addScoreTime);
		obj_BG2.SetActive (false);
		obj_rankUp.SetActive (false);
	}
	IEnumerator _CorActiveButton ()
	{
		yield return new WaitForSeconds (enterDelay + addScoreTime);
		btn_bigList.interactable = true;
	}







	#region [--- Difficulty ---]
	void DifficultyLevelUp ()
	{
		// LogManager.Log("DifficultyLevelUp");
		if (SelfPlayerLevelData.LevelDic[StaticData.LevelID].difficulty < CorePlaySettings.Instance.m_Hard)
		{
			SelfPlayerLevelData.LevelDic[StaticData.LevelID].difficulty = CorePlaySettings.Instance.m_Hard;
		}
		StaticData.Difficulty++;
		TryAgain ();
	}

	void DifficultyCancel ()
	{
		// LogManager.Log("DifficultyCancel");
		TryAgain ();
	}
	#endregion




	void OnClickRetry ()
	{
		if (CorePlayData.BossLife > 0 && StaticData.Difficulty == CorePlaySettings.Instance.m_Normal)
		{
			obj_difficultyPanel.SetActive (true);
		}
		else
		{
			TryAgain ();
		}
	}
	void TryAgain ()
	{
		Close ();
		Time.timeScale = 1f;
		CorePlayManager.Instance.Retry ();
	}

	void OnClickBack ()
	{
		Close ();

		CorePlayPage.BackToMainProcess ();

		//if (CacheData.Instance.ExitCacheData(StaticData.LevelID))
		//{
		//	PromptManager.Instance.MessageBox(PromptManager.Type.WindowTip, "恭喜打破之前的记录，当前网络不畅，联网后会同步排行榜数据", (result) =>
		//	{
		//		Close();
		//		LoadSceneManager.Instance.LoadSceneAsync("BookScene");
		//	});
		//}
		//else
	}

	void OnClickNext ()
	{
		Time.timeScale = 1f;
		AudioController.StopMusic ();
		CorePlayManager.Instance.CleanAll ();
		StaticMonoBehaviour.Instance.StopAllCoroutines ();
		ObjectPool.ClearPool ("RhythmController");

		LevelData levelData = LevelConfig.AllLevelDic[StaticData.NowLevelData.nextLevelID];

		var levelStatus = SelfPlayerData.Instance.GetLevelStatusByLevelId (levelData.levelID);
		if (levelStatus.isPay || DebugConfigController.Instance._Debug)
		{
			if (levelStatus.isUnLock)
			{
				StaticData.LevelID = levelData.levelID;
				CorePlayData.BossLife = levelData.bosslife;
				CorePlayData.SongOffset = SongConfig.Instance.GetSongOffsetBySongIDAndLevelDiffculty (LanguageManager.GetSongIdFromLanguage (levelData.songID), levelData.LevelDifficulty);
				CorePlayData.SongID = LanguageManager.GetSongIdFromLanguage (levelData.songID);
				CorePlayData.EducationText = SongConfig.Instance.m_items[LanguageManager.GetSongIdFromLanguage (levelData.songID)]["educationText"];
				LoadSceneManager.Instance.LoadPlayScene (levelData);
				Close ();
			}
			else
			{
				CommonPromptNode node = PageManager.Instance.CurrentPage.AddNode<CommonPromptNode> (true) as CommonPromptNode;
				node.InitPrompt (false, SelfType.Prompt, SelfType.NeedUnlock,
															LanguageManager.languageType == LanguageType.Chinese ? TexSure : TexSure_EN,
															LanguageManager.languageType == LanguageType.Chinese ? TexCancel : TexCancel_EN,
															null, null);
			}
		}
		else
		{
			switch (StaticData.LevelID)
			{
				case 1004:
					AnalysisManager.Instance.OnEvent ("payNode_level4", null);
					break;
				case 1020:
					AnalysisManager.Instance.OnEvent ("payNode_level20", null);
					break;
				case 1032:
					AnalysisManager.Instance.OnEvent ("payNode_level32", null);
					break;
				default:
					break;
			}
			PageManager.Instance.CurrentPage.AddNode<CommonPromptNode> (true);
			m_promptNode = PageManager.Instance.CurrentPage.AddNode<CommonPromptNode> (true) as CommonPromptNode;
			m_promptNode.InitPrompt (false, SelfType.Prompt, SelfType.NeedPay,
												LanguageManager.languageType == LanguageType.Chinese ? TexUnlock : TexUnlock_EN,
												LanguageManager.languageType == LanguageType.Chinese ? TexCancel : TexCancel_EN,
												this.OpenPay, null);
		}


	}
	private void OpenPay ()
	{
		if (null != m_promptNode)
		{
			m_promptNode.Close (true);
			m_promptNode = null;
		}
		PageManager.Instance.CurrentPage.AddNode<IAPNode> (true);
	}





	public void TempReset ()
	{
		Close ();
		StageManager.Instance.Restart ();
	}

	public void TempPlayAllVoice ()
	{
		MicManager.Instance.PlayAllVoice (SelfPlayerData.Uuid, StaticData.LevelID);
	}



}
