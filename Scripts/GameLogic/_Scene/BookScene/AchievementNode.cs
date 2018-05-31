using UnityEngine.EventSystems;
using Snaplingo.UI;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using DG.Tweening;

public class AchievementNode : Node
{
    /// <summary>
    /// 已完成成就的总数
    /// </summary>
    public int isFinishAchieveCount;
    /// <summary>
    /// 关闭成就界面的按钮
    /// </summary>
    public Button but_Back;
    /// <summary>
    /// 显示成就完成进度的ui
    /// </summary>
    public Text achieveCount;
    /// <summary>
    /// 一个成就项
    /// </summary>
    public GameObject achievementItem;
    /// <summary>
    /// 一个成就的信息相当于策划的配表数据
    /// </summary>
    public MissionObject missionObject;
    public Transform parentTrans;
    /// <summary>
    /// 所有成就的信息
    /// </summary>
    private UserAchievementData userAchievementData;

    private List<GameObject> achievementList;

    private void Start()
    {
        //初始化完成先隐藏
        //gameObject.SetActive(false);
    }
    public override void Open()
    {
        base.Open();
        //每次打开都把已完成的成就数置0
        isFinishAchieveCount = 0;
        ResetAchievement();
    }
    private void ResetAchievement()
    {
        for (int i = 0; i < achievementList.Count; i++)
        {
            achievementList[i].GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);
        }
    }
    public override void Init(params object[] args)
    {
        base.Init();
        //给关闭成就界面的按钮绑定事件
        but_Back.onClick.AddListener(() =>
        {
            //base.Close();
            PageManager.Instance.CurrentPage.GetNode<AchievementNode>().GetComponent<DOTweenAnimation>().DOPlayBackwards();
            if (PageManager.Instance.CurrentPage.GetNode<AchievementItemNode>().gameObject.activeSelf)
            {
                PageManager.Instance.CurrentPage.GetNode<AchievementItemNode>().gameObject.SetActive(false);
            }
        });
        //给成就界面绑定事件
        gameObject.GetComponent<Button>().onClick.AddListener(() =>
        {
            if (PageManager.Instance.CurrentPage.GetNode<AchievementItemNode>().gameObject.activeInHierarchy)
            {
                PageManager.Instance.CurrentPage.GetNode<AchievementItemNode>().gameObject.SetActive(false);
            }
        });
        CreateAchievementItem();
    }
    /// <summary>
    /// 创建所有的成就项
    /// </summary>
    private void CreateAchievementItem()
    {
        #region 创建json文件
        /*UserAchievementData data = new UserAchievementData();
        data.userAchievementList = new List<MissionObject>();
        MissionObject mission = new MissionObject();
        mission.ID = 1;
        mission.icon = null;
        mission.iconName = "chengjiu1.png";
        mission.title = "成就一";
        mission.description = "完成第八关即可完成此成就！";
        mission.isDone = false;
        mission.rewords = new List<RewordObject>();
        RewordObject rewordObject = new RewordObject();
        rewordObject.ID = 1;
        rewordObject.icon = null;
        rewordObject.iconName = "reword.png";
        rewordObject.title = "奇怪的人头";
        rewordObject.description = "xxxxxxxxxxx";
        RewordObject rewordObject1 = new RewordObject();
        rewordObject1.ID = 2;
        rewordObject1.icon = null;
        rewordObject1.iconName = "reword.png";
        rewordObject1.title = "奇怪的人头";
        rewordObject1.description = "xxxxxxxxxxx";
        RewordObject rewordObject3 = new RewordObject();
        rewordObject3.ID = 3;
        rewordObject3.icon = null;
        rewordObject3.iconName = "reword.png";
        rewordObject3.title = "奇怪的人头";
        rewordObject3.description = "xxxxxxxxxxx";
        mission.rewords.Add(rewordObject);
        mission.rewords.Add(rewordObject1);
        mission.rewords.Add(rewordObject3);
        data.userAchievementList.Add(mission);
        //print(JsonUtility.ToJson(data));*/
        #endregion
        achievementList = new List<GameObject>();
        //读取临时的json文件相当于配表数据
        string json = File.ReadAllText(Application.dataPath + "/Resources/Config/UserAchievement.json");
        userAchievementData = JsonUtility.FromJson<UserAchievementData>(json);
        //获取要创建成就项的父节点
        //根据临时数据依次创建成就项
        foreach (MissionObject item in userAchievementData.userAchievementList)
        {
            GameObject achieve = Instantiate(achievementItem, Vector3.zero, Quaternion.identity, parentTrans);
            //SetAchievementName(achieve, item.title);
            SetAchievementState(achieve, item, CheckUserAchievementIsFinished(item.ID));
            SetEventToButton(achieve, item);
            achievementList.Add(achieve);
        }
        achieveCount.text = isFinishAchieveCount.ToString() + "/" + userAchievementData.userAchievementList.Count;
    }
    private void TEST()
    {
        //JsonUtils.
    }
    //设置成就项的状态
    private void SetAchievementState(GameObject achieve, MissionObject item, bool isLight)
    {
        item.isDone = isLight;
        if (isLight)
        {
            //累计已完成的成就数
            isFinishAchieveCount++;
            //将已完成的成就项的icon改变
            achieve.GetComponent<Image>().sprite = Resources.Load<Sprite>("Achievement/kx4X");
            achieve.transform.GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite>("Achievement/ih8swr");
        }
        else
        {
            print("此成就还未获得！！！");
            achieve.GetComponent<Image>().sprite = Resources.Load<Sprite>("Achievement/uTkx4X");
            achieve.transform.GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite>("Achievement/fw658");
        }
    }
    //设置成就的名字
    private void SetAchievementName(GameObject achieve, string achieveName)
    {
        achieve.transform.GetChild(0).GetComponent<Text>().text = achieveName;
    }
    //设置每个按钮的事件
    private void SetEventToButton(GameObject achieve, MissionObject item)
    {
        //给按钮添加默认的点击音频
        AudioManager.Instance.InitBtnClick(achieve);
        achieve.GetComponent<Button>().onClick.AddListener(() =>
        {
            print("OK");
            SetAchievementItemSelectStatus(achieve);
            PageManager.Instance.CurrentPage.GetNode<AchievementItemNode>().SetAchievementItemInfor(item);
        });
    }
    /// <summary>
    /// 设置成就项的选择状态 是否被选中！
    /// </summary>
    /// <param name="achieve">Achieve.</param>
    private void SetAchievementItemSelectStatus(GameObject achieve)
    {
        for (int i = 0; i < achievementList.Count; i++)
        {
            if (achieve.Equals(achievementList[i]))
            {
                achievementList[i].GetComponent<Image>().color = new Color(125f / 255, 125f / 255, 125f / 255, 1f);
            }
            else
            {
                achievementList[i].GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);
            }
        }
    }
    /// <summary>
    /// 检测用户是否达成某个成就
    /// </summary>
    /// <returns><c>true</c>, if user achievement is finished was checked, <c>false</c> otherwise.</returns>
    /// <param name="achievementId">成就Id</param>
    private bool CheckUserAchievementIsFinished(int achievementId)
    {
        return Random.Range(0, 2) != 0 ? true : false;
    }
}
