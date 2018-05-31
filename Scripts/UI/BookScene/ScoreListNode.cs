using UnityEngine;
using UnityEngine.UI;
using Snaplingo.UI;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.Collections;

public class ScoreListNode : Node
{
	#region [ Property --- ]
	int pageNo;
	int maxPage;
	int cur_rank;
	int m_SelectIndex;
	int m_PlayerPosition = 0;
	PlayerInfoPanel[] playerInfoPanels;
	#endregion

	#region [ Object References --- ]
	[Header (" --- Object References ---")]
	public Sprite[] sp_ranks;
	public Button btn_Close, btn_NamePanel, btn_prePage, btn_nextPage;
	public ChangeSelfInfoNode node_ChangeSelfInfoNode;

	[Header (" --- MyInfo ---")]
	public Image image_rank;
	public Text text_rank, text_record, text_playerName;
	public Transform _playerInfoRoot;
	#endregion
	#region
	//public Text rankTitle, scoreTitle;
	#endregion
	#region [ Mono --- ]
	void Awake ()
	{
		btn_Close.onClick.AddListener (() => {
			AnalysisManager.Instance.OnEvent ("100005", null, "排行榜", "点击关闭按钮，关闭排行榜");
			Close (true);
		});
		btn_NamePanel.onClick.AddListener (() => {
			PageManager.Instance.CurrentPage.AddNode<UserInfoNode> (true, SelfPlayerData.Uuid);
			//AnalysisManager.Instance.OnEvent("100005", null, "排行榜", "点击修改信息按钮，打开修改信息界面");
			//OnCkickNamePanel();
		});
		btn_prePage.onClick.AddListener (() => {
			AnalysisManager.Instance.OnEvent ("100005", null, "排行榜", "获取上一页排行榜");
			OnPreviousPage ();
		});
		btn_nextPage.onClick.AddListener (() => {
			AnalysisManager.Instance.OnEvent ("100005", null, "排行榜", "获取下一页排行榜");
			OnNextPage ();
		});
		playerInfoPanels = new PlayerInfoPanel[_playerInfoRoot.childCount];
		for (int i = 0; i < playerInfoPanels.Length; i++)
		{
			playerInfoPanels[i] = _playerInfoRoot.GetChild (i).GetComponent<PlayerInfoPanel> ();
		}
		SelfPlayerLevelData.Instance.changeNameEvent.AddListener (OnChangeName);
	}
	void OnDestroy ()
	{
		SelfPlayerLevelData.Instance.changeNameEvent.RemoveListener (OnChangeName);
	}
	#endregion


	#region [ override --- ]
	public override void Init (params object[] args)
	{
		base.Init ();
		pageNo = 0;
	}

	public override void Open ()
	{
		//LogManager.Log("打开排行榜");
		StartCoroutine (_CorOpen ());
		//rankTitle.text = LanguageManager.Instance.GetValueByKey(rankTitle.GetComponent<LanguageFlagClass>().selfType.ToString(), LanguageManager.languageType);
		//scoreTitle.text = LanguageManager.Instance.GetValueByKey(scoreTitle.GetComponent<LanguageFlagClass>().selfType.ToString(), LanguageManager.languageType);

	}
	#endregion
	IEnumerator _CorOpen ()
	{
		text_playerName.text = SelfPlayerData.Nickname;
		btn_prePage.gameObject.SetActive (false);
		btn_nextPage.gameObject.SetActive (false);
		foreach (var item in playerInfoPanels) { item.Reset (); }
		yield return null;
		SetupSelfInfo ();
		yield return null;
		OnReceive_Page_RankData ();
		yield return new WaitForSeconds (.5f);
		AnalysisManager.Instance.OnEvent ("100005", null, "排行榜", "打开排行榜");
		yield return null;
	}



	void SetupSelfInfo ()
	{
		if (!SelfPlayerLevelData.HasLocalData (StaticData.LevelID))
		{
			cur_rank = 0;
			text_rank.text = "-";
			image_rank.sprite = null;
			image_rank.color = Color.clear;
			text_record.text = "0";
		}
		else
		{
			cur_rank = SelfPlayerLevelData.CurRank;
			text_rank.text = cur_rank.ToString ();
			int _accuracy = SetLevel.setLevel (SelfPlayerLevelData.CurAccuracy);
			image_rank.sprite = sp_ranks[_accuracy];
			image_rank.color = Color.white;
			text_record.text = SelfPlayerLevelData.MaxScore.ToString ();
		}
	}

	public void OnReceive_Page_RankData ()
	{
		if (this.gameObject == null) return;

		var rankObjs = OtherPlayerLevelData.Get_Page_RankObjects ();
		for (int i = 0; i < rankObjs.Count; i++)
		{
			if (rankObjs[i].rank > 0)
				playerInfoPanels[i].Setup (rankObjs[i]);
		}

		pageNo = OtherPlayerLevelData.PageNo;
		maxPage = Mathf.Max (0, (OtherPlayerLevelData.Total - 1)) / playerInfoPanels.Length;

		//LogManager.Log("OnReceive_Page_RankData() pageNo = " , pageNo , "  maxPage = " , maxPage);
		if (OtherPlayerLevelData.Total <= playerInfoPanels.Length)
		{
			btn_prePage.gameObject.SetActive (false);
			btn_nextPage.gameObject.SetActive (false);
		}
		else if (pageNo <= 0)
		{
			btn_prePage.gameObject.SetActive (false);
			btn_nextPage.gameObject.SetActive (true);
		}
		else if (pageNo >= maxPage)
		{
			btn_prePage.gameObject.SetActive (true);
			btn_nextPage.gameObject.SetActive (false);
		}
		else
		{
			btn_prePage.gameObject.SetActive (true);
			btn_nextPage.gameObject.SetActive (true);
		}
	}
	void OnChangeName ()
	{
		text_playerName.text = SelfPlayerData.Nickname;
	}


	#region [ Button Call Back --- ]
	void OnPreviousPage ()
	{
		if (MiscUtils.IsOnline ())
		{
			btn_prePage.gameObject.SetActive (false);
			foreach (var item in playerInfoPanels) { item.Reset (); }

			pageNo = Mathf.Max (0, pageNo - 1);
			//LogManager.Log(" 翻开 : " , pageNo);
			HttpHandler.Request_Page (pageNo, OnReceive_Page_RankData, playerInfoPanels.Length);
		}
	}

	void OnNextPage ()
	{
		if (MiscUtils.IsOnline ())
		{
			btn_nextPage.gameObject.SetActive (false);
			foreach (var item in playerInfoPanels) { item.Reset (); }

			pageNo = Mathf.Min (maxPage, pageNo + 1);
			//LogManager.Log(" 翻开 : " , pageNo);
			HttpHandler.Request_Page (pageNo, OnReceive_Page_RankData, playerInfoPanels.Length);
		}
	}

	void OnCkickNamePanel ()
	{
		ChangeSelfInfoNode node = Instantiate (node_ChangeSelfInfoNode);
		node.transform.SetParent (transform, false);
	}
	#endregion











}


