using UnityEngine;
using UnityEngine.UI;
using Snaplingo.UI;
using Snaplingo.SaveData;
using System.Collections;
using LitJson;

public class LevelEntryPanel : MonoBehaviour
{
	#region [ --- Property --- ]
	LevelData levelData;
	//string lyricName;
	bool loadAgain;
	#endregion


	#region [ --- Object References --- ]
	[Header(" --- Object References ---")]
	// 临时
	public Sprite[] sp_ranks;
	// 临时
	public Sprite[] sp_LevelDifficultys;

	// panel 信息
	public Text text_SongName, text_childName, text_describe;
	public Image image_authorIcon;
	public Image image_Difficulty;
	public PlayerInfoPanel[] playerInfoPanels;
	public Button btn_BG, btn_BigList, btn_more, btn_Close, btn_Go;
	public Image image_Gear1, image_Gear2, image_Gear3, image_title;
	#endregion



	#region [ --- Mono --- ]
	void Awake()
	{
		btn_Go.onClick.AddListener(LoadPlayScene);
		btn_BigList.onClick.AddListener(() =>
		{
			AnalysisManager.Instance.OnEvent("100005", null, "关卡信息", "点击个人信息，进入排行榜");
			PageManager.Instance.CurrentPage.AddNode<ScoreListNode>(true);
		});
		btn_more.onClick.AddListener(() =>
		{
			AnalysisManager.Instance.OnEvent("100005", null, "关卡信息", "点击进入排行榜");
			PageManager.Instance.CurrentPage.AddNode<ScoreListNode>(true);
		});
		btn_BG.onClick.AddListener(() =>
		{
			AnalysisManager.Instance.OnEvent("100005", null, "关卡信息", "点击空白处，关闭关卡：" + levelData.levelID);
			Close();
		});
		btn_Close.onClick.AddListener(() =>
		{
			AnalysisManager.Instance.OnEvent("100005", null, "关卡信息", "点击关闭按钮，关闭关卡：" + levelData.levelID);
			Close();
		});
		text_describe.text = string.Empty;
	}
	#endregion



	#region [ --- Public --- ]
	public void Init()
	{
		gameObject.SetActive(false);
	}
	public void Open(LevelData data)
	{
		levelData = data;
		gameObject.SetActive(true);

		StartCoroutine(_CorOpen());
	}

	IEnumerator _CorOpen()
	{
		OtherPlayerLevelData.Reset();
		yield return null;

		foreach (var item in playerInfoPanels) { item.Reset(); }
		loadAgain = false;
		yield return null;
		// Song
		StaticData.LevelID = levelData.levelID;
        var _songDic = SongConfig.Instance.GetsongInfoBysongID(LanguageManager.GetSongIdFromLanguage( levelData.songID));
        print("多语言"+LanguageManager.GetSongIdFromLanguage(levelData.songID));
		text_SongName.text = _songDic["songName"];
		ChangeTitleWidth();
		// image_authorIcon
		string authorIconPath = "UI/Avatar/" + _songDic["songAuthorIconUrl"];
		image_authorIcon.sprite = Resources.Load<Sprite>(authorIconPath);
		text_childName.text = _songDic["songAuthorName"];
		yield return null;
        text_describe.text =  LanguageManager.Instance.GetValueByKey(SelfType.NotScore.ToString(),LanguageManager.languageType);

		if (MiscUtils.IsOnline())
		{
			HttpHandler.Request_10_Before_And_10_After(Receive_10_10_RankData, Receive_None);
			AnalysisManager.Instance.OnEvent("100005", null, "关卡信息", "打开关卡：" + levelData.levelID);
			yield return null;
		}
		else
		{
            text_describe.text = LanguageManager.Instance.GetValueByKey(SelfType.NotNetWork.ToString(),LanguageManager.languageType); 
		}

	}


	private const float EdgeWidth = 200f;
	private void ChangeTitleWidth()
	{
		float canvasWidth = PageManager.Instance.GetComponent<RectTransform>().sizeDelta.x;
		float textWidth = text_SongName.preferredWidth;
		float panelWidth = textWidth + EdgeWidth;
		image_title.rectTransform.sizeDelta = new Vector2(panelWidth, image_title.rectTransform.sizeDelta.y);
		image_Gear1.rectTransform.localPosition = new Vector3(-panelWidth * 0.5f - image_Gear1.rectTransform.sizeDelta.x * 0.5f, image_Gear1.rectTransform.localPosition.y, 0);
		image_Gear3.rectTransform.localPosition = new Vector3(-panelWidth * 0.5f - image_Gear1.rectTransform.sizeDelta.x * 0.5f - image_Gear3.rectTransform.sizeDelta.x * 0.5f,
															  image_Gear3.rectTransform.localPosition.y, 0);
		image_Gear2.rectTransform.localPosition = new Vector3(panelWidth * 0.5f + image_Gear2.rectTransform.sizeDelta.x * 0.5f, image_Gear2.rectTransform.localPosition.y, 0);
	}

	public void Close()
	{
		StopAllCoroutines();
		gameObject.SetActive(false);
	}
	#endregion

	#region [--- Display Difficulty --- ]
	private GameObject obj_difficulty;
	private GameObject obj_difficultyList;
	void ShowDifficultyList()
	{
		if (levelData.bosslife > 0)
			obj_difficultyList.SetActive(true);
	}

	void ChangeToNormalDifficulty()
	{
		obj_difficultyList.SetActive(false);
		image_Difficulty.sprite = sp_LevelDifficultys[0];
		StaticData.Difficulty = CorePlaySettings.Instance.m_Normal;
	}

	void ChangeToHardDifficulty()
	{
		obj_difficultyList.SetActive(false);
		int oldDifficulty = SelfPlayerLevelData.LevelDic[StaticData.LevelID].difficulty;
		if (oldDifficulty > CorePlaySettings.Instance.m_Normal)
		{
			image_Difficulty.sprite = sp_LevelDifficultys[1];
			StaticData.Difficulty = CorePlaySettings.Instance.m_Hard;
		}
		else
		{
			image_Difficulty.sprite = sp_LevelDifficultys[0];
			PromptManager.Instance.MessageBox(PromptManager.Type.FloatingTip, "需要上一关等级至少为S");
		}
	}
	#endregion

	#region [ --- Http Call Back --- ]
	void Receive_None()
	{
		HttpHandler.Request_Page(0);
	}

	void Receive_10_10_RankData()
	{
		if (this.gameObject == null)
			return;
		if (OtherPlayerLevelData.SelfRankObject == null)
		{
			return;
		}

		text_describe.text = string.Empty;
		SelfPlayerLevelData.CurInfo.maxScore = OtherPlayerLevelData.SelfRankObject.score;
		SelfPlayerLevelData.CurInfo.rank = OtherPlayerLevelData.SelfRankObject.rank;

		int index = 0;
		playerInfoPanels[index++].Setup(OtherPlayerLevelData.SelfRankObject);

		var lowerRanks = OtherPlayerLevelData.Get_Lower_RankObjects(3);
		foreach (var item in lowerRanks)
		{
			if (index < playerInfoPanels.Length)
				playerInfoPanels[index++].Setup(item);
		}
	}


	void OnReceive_UploadScore()
	{
		HttpHandler.Request_10_Before_And_10_After(Receive_10_10_RankData);
	}
	#endregion


	void SendScoreAndLoadAgain()
	{
		loadAgain = true;
		SelfPlayerLevelData.TempScore = SelfPlayerLevelData.MaxScore;
		SelfPlayerLevelData.TempAccuracy = SelfPlayerLevelData.CurAccuracy;
		SelfPlayerLevelData.TempMaxCombo = SelfPlayerLevelData.Cur_maxCombo;
		SelfPlayerLevelData.TempLeftLife = SelfPlayerLevelData.Cur_leftLife;
		SelfPlayerLevelData.TempReadAccuracy = SelfPlayerLevelData.Cur_readAccuracy;
		SelfPlayerLevelData.TempWordAccuracy = SelfPlayerLevelData.Cur_wordAccuracy;
		HttpHandler.UploadScore(
			OnReceive_UploadScore,
			true,
			false);
	}

	void LoadPlayScene()
	{
		LogManager.Log("进入游戏关卡：" , levelData.levelID);
		AnalysisManager.Instance.OnEvent("100005", null, "关卡信息", "进入游戏关卡：" + levelData.levelID);

		LoadSceneManager.Instance.LoadPlayScene(levelData);

		Close();
	}


}
//LevelEntryPanel













