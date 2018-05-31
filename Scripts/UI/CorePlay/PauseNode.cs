using UnityEngine;
using System.Collections;
using Snaplingo.UI;
using UnityEngine.UI;

public class PauseNode : Node
{
	#region [ --- Object References --- ]
	[Header(" --- Object References ---")]
	public Button btn_play;
	#endregion


	//private Image m_BG;
	private Text m_TeachText;
	public override void Init(params object[] args)
	{
		base.Init(args);
		//m_BG = transform.Find("BG").GetComponent<Image>();
		m_TeachText = transform.Find("TeachText").GetComponent<Text>();
        int songID = LanguageManager.GetSongIdFromLanguage(LevelConfig.AllLevelDic[StaticData.LevelID].songID);

		string sprName = SongConfig.Instance.GetsongInfoBysongID(songID)["teachSceneBG"];
		//Sprite spr = ResourceLoadUtils.Load<Sprite>("UI/TeachSceneBG/" + sprName);
		//m_BG.sprite = spr;
		m_TeachText.text = SongConfig.Instance.GetsongInfoBysongID(songID)["educationText"];
		btn_play.onClick.AddListener(()=>
		{
			AnalysisManager.Instance.OnEvent("100005", null, "游戏中", "游戏继续：" + StaticData.LevelID);
			Continue();
		});
		UIUtils.RegisterButton("Retry", Retry, transform);
		UIUtils.RegisterButton("BackToMain", BackToMain, transform);
	}

	public override void Open()
	{
		base.Open();
		transform.SetAsLastSibling();
	}

	void Continue()
	{
		Close();
		CorePlayManager.Instance.Continue();
	}

	void Retry()
	{
		AnalysisManager.Instance.OnEvent("100005", null, "游戏中", "游戏重新开始：" + StaticData.LevelID);
		Close();
		Time.timeScale = 1f;
		CorePlayManager.Instance.Retry();
	}

	void BackToMain()
	{
		AnalysisManager.Instance.OnEvent("100005", null, "游戏中", "游戏中返回列表：" + StaticData.LevelID);
		Close();
        CorePlayPage.BackToMainProcess();
	}
}
