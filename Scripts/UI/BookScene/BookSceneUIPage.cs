using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Snaplingo.UI;
using DG.Tweening;
using Snaplingo.SaveData;

public class BookSceneUIPage : Page
{
	#region [ --- Property --- ]
	TA.BookSceneManager _bookSceneManager;
	// Json 中的 关卡数据
	List<Dictionary<string, string>> levelDataList;

	float mUserExpProgressBarMaxWidth;
	float mUserPowerProgressBarMaxWidth;
	//bool mCurrentIsCanChangeDifficulty = true;
	#endregion


	#region [ --- Object References --- ]
	[Header(" --- Object References ---")]

	// 经验
	//public Image bar_Exp;
	//public Text text_Exp;

	// 体力
	//public Image bar_Energy;
	//public Text text_Energy;
	//public Button btn_DebugUnlock;
	//public Button btn_Difficulty;
	//public Button btn_LastStage;
	//public Button but_NextStage;

	// sub Panel
	public LevelEntryPanel levelEntryPanel;
	#endregion

	public GameObject UINode;
	public GameObject gameObject_Skip;
	//private GameObject gameObject_Top;


	//public Button taskButton;
	//public Button achievementButton;
	//void Awake()
	//{
	//	taskButton.onClick.AddListener(() =>
	//	{
	//		PageManager.Instance.CurrentPage.GetNode<LableTaskNode>().gameObject.SetActive(true);
	//	});
	//	achievementButton.onClick.AddListener(() =>
	//	{
	//		PageManager.Instance.CurrentPage.GetNode<AchievementNode>().gameObject.SetActive(true);
	//	});
	//}

	private void Start()
	{
		PageManager.Instance.CurrentPage.AddNode<RoleTitleNode>(true);
	}

	public override void Open()
	{
		base.Open();
		//UINode.SetActive(false);



		// 初始化 
		_bookSceneManager = TA.BookSceneManager.instance;
		//mUserExpProgressBarMaxWidth = bar_Exp.rectTransform.sizeDelta.x;
		//mUserPowerProgressBarMaxWidth = bar_Energy.rectTransform.sizeDelta.x;

		//btn_DebugUnlock.onClick.AddListener(() =>
		//{
		//	//一键解锁所有关卡
		//	LevelIDInfo.Instance.LevelStatus = 9999;
		//	_bookSceneManager.RefreshStage();
		//});
		//btn_Difficulty.onClick.AddListener(OnClickDifficulty);
		//btn_LastStage.onClick.AddListener(OnClickLastStage);
		//but_NextStage.onClick.AddListener(OnClickNextStage);
		//gameObject_Skip = transform.Find("Skip").gameObject;
		gameObject_Skip.transform.Find("Skip").GetComponent<Button>().onClick.AddListener(Skip);
		//gameObject_Top = transform.Find("_top").gameObject;
		// 初始化 Sub Panel
		levelEntryPanel.Init();
		_bookSceneManager.onReceiveLevelData.AddListener(data =>
		{
			levelEntryPanel.Open(data);
		});

		if (!StaticData.ApplicationStart)
		{
			gameObject_Skip.SetActive(false);
		}
		//Setup(TA.BookSceneManager.isFirstTime);
		RefreshUIPage();
	}

	public void Setup(bool isFirstTime)
	{
		if (isFirstTime)
		{
			//UINode.SetActive(false);
		}
		else
		{
			gameObject_Skip.SetActive(false);
			UINode.SetActive(true);
		}
		RefreshUIPage();
	}



	#region [ --- Event Call Back --- ]

	// 
	void OnClickDifficulty()
	{
		_bookSceneManager.ReLoadScene();

		//LogManager.Log(" OnClickLevelNodeOpenLevelInfoDialog");
		//if (mCurrentIsCanChangeDifficulty)
		//{
		//	LevelConfig.Difficulty++;
		//	//SetupLevelData();
		//	RefreshUIPage();
		//}
		//else
		//{
		//	LogManager.Log("还不可以切换难度, 因为当前还有没有完成的~");
		//	PromptManager.Instance.MessageBox(PromptManager.Type.FloatingTip, "还不可以切换难度\n 因为当前还有没有完成的");
		//}
	}

	void OnClickLastStage()
	{
		LogManager.Log(" 重新实现 OnClickLastStage");
	}
	void OnClickNextStage()
	{
		LogManager.Log(" 重新实现 OnClickNextStage");
	}
	#endregion



	#region [ --- Private --- ]
	//刷新UI方法
	void RefreshUIPage()
	{
	}
	#endregion





	public void CloseSkip()
	{
		gameObject_Skip.SetActive(false);
		AnalysisManager.Instance.OnEvent("100005", null, "片头", "未跳过片头视频");
		// 视频结束
		// Mp4 方式
		//_bookSceneManager.OpenBook();

		videoEnd();
	}

	public void Skip()
	{
		AnalysisManager.Instance.OnEvent("100005", null, "片头", "跳过片头视频");
		StaticData.ApplicationStart = false;
		VideoPlay.Instance.FadeOut(() => { _bookSceneManager.PlayBG(); });
		StaticMemoryPool.ClearItem("startAnimation");
		CloseSkip();
	}

	private void videoEnd()
	{
		UINode.SetActive(true);
	}


}
//BookSceneUIPage













