using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Snaplingo.UI;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

/// <summary>
/// 几乎复制粘贴的WinNode.cs 不过以后留给张宏来重构吧吧 by balana
/// </summary>
public class StageOverNode : Node 
{

    public RectTransform m_MiddleGroup;
    public RectTransform m_LeftGroup;
    public RectTransform m_RightGroup;
    public GameObject m_DiffcultyPanel;

    CommonPromptNode m_promptNode;

    //这个地方暂时先放贴图的引用，之后处理多语言的时候统一改，如果有那一天的话...
    [Header(" --- PromptTex ---")]
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


    private string m_MiddleID;
    private string m_LeftID;
    private string m_RightID;
	private void Start()
	{
		transform.Find("MiddleDancerGroup/Like").GetComponent<Button>().onClick.AddListener(() => {
            LogManager.Log(m_MiddleID);
			ClickSupport(SelfPlayerData.Uuid, transform.Find("MiddleDancerGroup/ClickSupportEffect").gameObject);
        });
		transform.Find("MiddleDancerGroup/Listen").GetComponent<Button>().onClick.AddListener(() => {
            LogManager.Log(m_MiddleID);
            PlayVoice(SelfPlayerData.Uuid);
        });

        transform.Find("LeftDancerGroup/Like").GetComponent<Button>().onClick.AddListener(() => {
            LogManager.Log(m_LeftID);
			ClickSupport(m_LeftID, transform.Find("LeftDancerGroup/ClickSupportEffect").gameObject);
        });

        transform.Find("LeftDancerGroup/Listen").GetComponent<Button>().onClick.AddListener(() => {
            LogManager.Log(m_LeftID);
            PlayVoice(m_LeftID);
        });

        transform.Find("RightDancerGroup/Like").GetComponent<Button>().onClick.AddListener(() => {
            LogManager.Log(m_RightID);
			ClickSupport(m_RightID, transform.Find("RightDancerGroup/ClickSupportEffect").gameObject);
        });

        transform.Find("RightDancerGroup/Listen").GetComponent<Button>().onClick.AddListener(() => {
            LogManager.Log(m_RightID);
            PlayVoice(m_RightID);
        });
	}
	public override void Init(params object[] args)
    {
		LoginScene.isFirstGoingGame = string.IsNullOrEmpty(PlayerPrefs.GetString("isFirst")) ? true : false;
        transform.Find("Share").GetComponent<Button>().onClick.AddListener(()=>{
            
        });

        transform.Find("Ranking").GetComponent<Button>().onClick.AddListener(() => {
            AnalysisManager.Instance.OnEvent("100005", null, "游戏中", "游戏成功，打开排行榜：" + StaticData.LevelID);
            PageManager.Instance.CurrentPage.AddNode<ScoreListNode>(true);
        });

        transform.Find("Menu").GetComponent<Button>().onClick.AddListener(() => {
            AnalysisManager.Instance.OnEvent("100005", null, "游戏中", "游戏成功，返回列表：" + StaticData.LevelID);
            OnClickBack();
        });

        transform.Find("TryAgain").GetComponent<Button>().onClick.AddListener(()=>{
            AnalysisManager.Instance.OnEvent("100005", null, "游戏中", "游戏成功，再来一遍：" + StaticData.LevelID);
            OnClickRetry();
        });

        transform.Find("NextSong").GetComponent<Button>().onClick.AddListener(()=>{
            AnalysisManager.Instance.OnEvent("100005", null, "游戏中", "游戏成功，下一关卡：" + StaticData.LevelID);
            OnClickNext();
        });

        transform.Find("difficultyPanel/btn_ok").GetComponent<Button>().onClick.AddListener(() =>{
            AnalysisManager.Instance.OnEvent("100005", null, "游戏中", "游戏成功，确认提高难度：" + StaticData.LevelID);
            DifficultyLevelUp();
        });

        transform.Find("difficultyPanel/btn_cancel").GetComponent<Button>().onClick.AddListener(() => {
            AnalysisManager.Instance.OnEvent("100005", null, "游戏中", "游戏成功，取消提高难度：" + StaticData.LevelID);
            DifficultyCancel();
        });

        ///////////////////// 点赞 和 听语音 ////////////////////////
        

        
    }
    private void PlayVoice(string uid)
	{
		if(uid.Equals("npc"))
		{
			
		}
		else
		{
			MicManager.Instance.PlayAllVoice(uid, StaticData.LevelID, null);
		}
	}
	private void ClickSupport(string uid,GameObject supportEffectParent)
	{
		supportEffectParent.transform.parent.GetChild(0).GetComponent<Button>().interactable = false;
		supportEffectParent.transform.parent.GetChild(2).gameObject.SetActive(true);
		supportEffectParent.transform.GetChild(0).gameObject.SetActive(true);
		SupportManager.Instance.SetUserSupportToServer(SelfPlayerData.Uuid, uid, StaticData.LevelID, supportEffectParent);
	}
	private void HideClickSupportEffect(bool isShow)
	{
		transform.Find("MiddleDancerGroup/ClickSupportEffect/SupportEffect").gameObject.SetActive(isShow);
		transform.Find("MiddleDancerGroup/Like").gameObject.GetComponent<Button>().interactable = true;
		transform.Find("MiddleDancerGroup/LikeCover").gameObject.SetActive(false);

		transform.Find("LeftDancerGroup/ClickSupportEffect/SupportEffect").gameObject.SetActive(isShow);
		transform.Find("LeftDancerGroup/Like").gameObject.GetComponent<Button>().interactable = true;
		transform.Find("LeftDancerGroup/LikeCover").gameObject.SetActive(false);

		transform.Find("RightDancerGroup/ClickSupportEffect/SupportEffect").gameObject.SetActive(isShow);
		transform.Find("RightDancerGroup/Like").gameObject.GetComponent<Button>().interactable = true;
		transform.Find("RightDancerGroup/LikeCover").gameObject.SetActive(false);
	}
    public override void Open()
    {
        base.Open();
        m_DiffcultyPanel.SetActive(false);
		HideClickSupportEffect(false);

        LevelStatus status = SelfPlayerData.Instance.GetLevelStatusByLevelId(StaticData.NowLevelData.nextLevelID);
		LogManager.Log("next level:" , StaticData.NowLevelData.nextLevelID);
		LogManager.Log("status:" , status.isPay);
        if (StaticData.NowLevelData.nextLevelID > 0 && status.isPay)
        {
            transform.Find("NextSong").GetComponent<Button>().interactable = true;
        }
        else
        {
            transform.Find("NextSong").GetComponent<Button>().interactable = false;
        }

        if(SceneManager.GetActiveScene().name == "StageScene")
        {
            transform.Find("TryAgain").gameObject.SetActive(false);
        }
    }


    public void SetBinding(GameObject middle, GameObject left, GameObject right)
    {
        m_MiddleGroup.position = Camera.main.WorldToScreenPoint(middle.transform.position - new Vector3(0, 2f, 0)) ;
        m_LeftGroup.position = Camera.main.WorldToScreenPoint(left.transform.position - new Vector3(0, 2f, 0)) ;
        m_RightGroup.position = Camera.main.WorldToScreenPoint(right.transform.position - new Vector3(0, 2f, 0)) ;
    }

    public void SetID(string middle, string left, string right)
    {
        m_MiddleID = middle;
        m_LeftID = left;
        m_RightID = right;
    }

    #region [--- Difficulty ---]
    void DifficultyLevelUp()
    {
        if (SelfPlayerLevelData.LevelDic[StaticData.LevelID].difficulty < CorePlaySettings.Instance.m_Hard)
        {
            SelfPlayerLevelData.LevelDic[StaticData.LevelID].difficulty = CorePlaySettings.Instance.m_Hard;
        }
        StaticData.Difficulty++;
        TryAgain();
    }

    void DifficultyCancel()
    {
        TryAgain();
    }
    #endregion

    void OnClickRetry()
    {
        if (CorePlayData.BossLife > 0 && StaticData.Difficulty == CorePlaySettings.Instance.m_Normal)
        {
            m_DiffcultyPanel.SetActive(true);
        }
        else
        {
            TryAgain();
        }
    }

    void TryAgain()
    {
        Close();
        Time.timeScale = 1f;
        CorePlayManager.Instance.Retry();
    }

    void OnClickBack()
    {
        Close();

        CorePlayPage.BackToMainProcess();

        //if (CacheData.Instance.ExitCacheData(StaticData.LevelID))
        //{
        //  PromptManager.Instance.MessageBox(PromptManager.Type.WindowTip, "恭喜打破之前的记录，当前网络不畅，联网后会同步排行榜数据", (result) =>
        //  {
        //      Close();
        //      LoadSceneManager.Instance.LoadSceneAsync("BookScene");
        //  });
        //}
        //else
    }

    void OnClickNext()
    {
        Time.timeScale = 1f;
        AudioController.StopMusic();
        CorePlayManager.Instance.CleanAll();
        StaticMonoBehaviour.Instance.StopAllCoroutines();
        ObjectPool.ClearPool("RhythmController");

        LevelData levelData = LevelConfig.AllLevelDic[StaticData.NowLevelData.nextLevelID];

        var levelStatus = SelfPlayerData.Instance.GetLevelStatusByLevelId(levelData.levelID);
        if (levelStatus.isPay || DebugConfigController.Instance._Debug)
        {
            if (levelStatus.isUnLock)
            {
                StaticData.LevelID = levelData.levelID;
                CorePlayData.BossLife = levelData.bosslife;
                CorePlayData.SongOffset = SongConfig.Instance.GetSongOffsetBySongIDAndLevelDiffculty(LanguageManager.GetSongIdFromLanguage(levelData.songID), levelData.LevelDifficulty);
                CorePlayData.SongID = LanguageManager.GetSongIdFromLanguage(levelData.songID);
                CorePlayData.EducationText = SongConfig.Instance.m_items[LanguageManager.GetSongIdFromLanguage(levelData.songID)]["educationText"];
                LoadSceneManager.Instance.LoadPlayScene(levelData);
                Close();
            }
            else
            {
                CommonPromptNode node = PageManager.Instance.CurrentPage.AddNode<CommonPromptNode>(true) as CommonPromptNode;
                node.InitPrompt(false, SelfType.Prompt, SelfType.NeedUnlock,
                                                            LanguageManager.languageType == LanguageType.Chinese ? TexSure : TexSure_EN,
                                                            LanguageManager.languageType == LanguageType.Chinese ? TexCancel : TexCancel_EN,
                                                            null, null);
            }
        }
        else
        {
            PageManager.Instance.CurrentPage.AddNode<CommonPromptNode>(true);
            m_promptNode = PageManager.Instance.CurrentPage.AddNode<CommonPromptNode>(true) as CommonPromptNode;
            m_promptNode.InitPrompt(false, SelfType.Prompt, SelfType.NeedPay,
                                                LanguageManager.languageType == LanguageType.Chinese ? TexUnlock : TexUnlock_EN,
                                                LanguageManager.languageType == LanguageType.Chinese ? TexCancel : TexCancel_EN,
                                                this.OpenPay, null);
        }
    }

    private void OpenPay()
    {
        if (null != m_promptNode)
        {
            m_promptNode.Close(true);
            m_promptNode = null;
        }
        PageManager.Instance.CurrentPage.AddNode<IAPNode>(true);
    }


}
