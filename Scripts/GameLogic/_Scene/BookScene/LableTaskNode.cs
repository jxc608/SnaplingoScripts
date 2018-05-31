using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Snaplingo.UI;
using UnityEngine.UI;
using System.IO;

/// <summary>
/// 任务的类型：彩蛋或者任务
/// </summary>
public enum TaskType
{
    TASK,
    EGG,
}

public class LableTaskNode : Node
{
    /// <summary>
    /// 任务界面的标签按钮集合
    /// </summary>
    public List<Button> lableButtonList;
    /// <summary>
    /// 标签任务项
    /// </summary>
    public GameObject lableTaskItem;
    /// <summary>
    /// 关闭任务页面的按钮
    /// </summary>
    public Button closeLableTask;
    /// <summary>
    /// 任务项的父节点
    /// </summary>
    public Transform parentTrans;
    /// <summary>
    /// 任务项的集合
    /// </summary>
    private List<GameObject> taskItemList;
    /// <summary>
    /// 任务框
    /// </summary>
    public Image taskMainImage;
    /// <summary>
    /// 任务数据源
    /// </summary>
    private UserAchievementData userAchievementData;
    /// <summary>
    /// 任务ID数组
    /// </summary>
    private int[] taskIdArray;
    private void Start()
    {
        PageManager.Instance.CurrentPage.GetNode<LableTaskNode>().gameObject.SetActive(false);
        print(ServerMutually.ResponseMessageFromServer(TaskType.EGG).Count);
    }
    public override void Open()
    {
        base.Open();
    }
    public override void Init(params object[] args)
    {
        base.Init(args);
        AddEventToLableButton();
        CreateLableTaskItem(TaskType.TASK);
        ChangeLableButton(lableButtonList[0].name);
    }
    private void CreateLableTaskItem(TaskType taskType)
    {
        ClearTaskItemList();
        //读取临时的json文件相当于配表数据
        //string json = File.ReadAllText(Application.dataPath + "/_SnadBox/UserAchievement.json");
        //userAchievementData = JsonUtility.FromJson<UserAchievementData>(json);
        InitUserDataClass(taskType);
        //根据临时数据依次创建任务项
        foreach (MissionObject item in userAchievementData.userAchievementList)
        {
            GameObject lableTaskObj = Instantiate(lableTaskItem, Vector3.zero, Quaternion.identity, parentTrans);
            if (taskType == TaskType.EGG)
            {
                //创建彩蛋任务项
                CreateEggTaskItem(taskType, lableTaskObj, item);
            }
            else
            {
                //创建普通任务项
                CreateDailyTaskItem(taskType, lableTaskObj, item);
            }
        }
    }
    /// <summary>
    /// 初始化任务数据源
    /// </summary>
    /// <param name="taskType">任务类型</param>
    private void InitUserDataClass(TaskType taskType)
    {
        string json = File.ReadAllText(Application.dataPath + "/Resources/Config/TaskConfig.json");
        TaskConfig.Instance.Fill(json);
        userAchievementData = new UserAchievementData();
        userAchievementData.userAchievementList = new List<MissionObject>();
        taskIdArray = new int[ServerMutually.ResponseMessageFromServer(taskType).Keys.Count];
        ServerMutually.ResponseMessageFromServer(taskType).Keys.CopyTo(taskIdArray, 0);
        for (int i = 0; i < taskIdArray.Length; i++)
        {
            MissionObject missionObject = new MissionObject();
            missionObject.ID = taskIdArray[i];
            missionObject.missionType = GetTaskType(int.Parse(TaskConfig.Instance.GetTaskConfigByIdAndKey(taskIdArray[i], "task_type")));
            missionObject.title = TaskConfig.Instance.GetTaskConfigByIdAndKey(taskIdArray[i], "task_name");
            missionObject.description = TaskConfig.Instance.GetTaskConfigByIdAndKey(taskIdArray[i], "task_desc");
            userAchievementData.userAchievementList.Add(missionObject);
        }
    }
    /// <summary>
    /// 从配表里面获取任务类型
    /// </summary>
    /// <returns>The task type.</returns>
    /// <param name="type">Type.</param>
    private MissionType GetTaskType(int type)
    {
        switch (type)
        {
            case 0: return MissionType.Main;
            case 2: return MissionType.Daily;
            case 3: return MissionType.Special;
        }
        return default(MissionType);
    }
    /// <summary>
    /// 创建彩蛋任务项
    /// </summary>
    /// <param name="type">任务的类型</param>
    /// <param name="obj">任务项</param>
    /// <param name="item">任务项的数据</param>
    private void CreateEggTaskItem(TaskType type, GameObject obj, MissionObject item)
    {
        if (GetTaskValue(type, item, 3) == "1")
        {
            //如果彩蛋已经被砸开咋直接初始化任务项
            CreateDailyTaskItem(type, obj, item);
            //设置任务的icon为蛋碎的图片
            //TODO
            obj.transform.GetChild(1).GetComponent<Image>().sprite = Resources.Load<Sprite>("Task/Egg_6_Part");
        }
        else
        {
            //如果彩蛋没有被砸开则只显示去主界面砸彩蛋去接任务
            obj.transform.GetChild(2).gameObject.SetActive(false);
            obj.transform.GetChild(3).gameObject.SetActive(true);
            //设置任务的icon为彩蛋的图片
            //TODO
            obj.transform.GetChild(1).GetComponent<Image>().sprite = Resources.Load<Sprite>("Task/Egg_6");
            taskItemList.Add(obj);
        }
        //关闭任务类型标志
        obj.transform.GetChild(0).gameObject.SetActive(false);
        obj.transform.GetChild(1).gameObject.SetActive(true);
    }
    /// <summary>
    /// 从服务器返回信息里面获取相关的数值
    /// </summary>
    /// <returns>The task value.</returns>
    /// <param name="type">Type.</param>
    /// <param name="item">Item.</param>
    /// <param name="index">Index.</param>
    private string GetTaskValue(TaskType type, MissionObject item, int index)
    {
        string str = ServerMutually.ResponseMessageFromServer(type)[item.ID].Split('_')[index];
        return str;
    }
    /// <summary>
    /// 创建正常的任务项
    /// </summary>
    /// <param name="type">Type.</param>
    /// <param name="obj">Object.</param>
    /// <param name="item">Item.</param>
    private void CreateDailyTaskItem(TaskType type, GameObject obj, MissionObject item)
    {
        SetTaskType(obj, item, item.missionType);
        obj.transform.GetChild(2).gameObject.SetActive(true);
        obj.transform.GetChild(3).gameObject.SetActive(false);
        obj.transform.GetChild(0).gameObject.SetActive(true);
        obj.transform.GetChild(1).gameObject.SetActive(false);
        //设置任务的icon为正常任务对应的icon
        //TODO
        obj.transform.GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite>("Task/TaskIcon");
        obj.transform.GetChild(2).GetChild(0).GetChild(0).GetComponent<Text>().text = item.title;
        obj.transform.GetChild(2).GetChild(1).GetComponent<Text>().text = item.description;
        SetTaskSilderAndStatus(type, obj, item);
        taskItemList.Add(obj);
    }
    /// <summary>
    /// 设置任务的完成状态和奖励领取状态
    /// </summary>
    /// <param name="type">Type.</param>
    /// <param name="obj">Object.</param>
    /// <param name="item">Item.</param>
    private void SetTaskSilderAndStatus(TaskType type, GameObject obj, MissionObject item)
    {
        obj.transform.GetChild(2).GetChild(4).GetChild(0).GetComponent<Text>().text = GetTaskValue(type, item, 0) + "/" + GetTaskValue(type, item, 1);
        obj.transform.GetChild(2).GetChild(5).gameObject.SetActive(IsFinishTask(type, item));
        obj.transform.GetChild(2).GetChild(2).GetComponent<Image>().sprite = Resources.Load<Sprite>("Task/" + GetTaskRewordStatus(type, item));
        if (IsFinishTask(type, item) && GetTaskValue(type, item, 2) == "0")
        {
            //如果奖励没有被领取
            obj.transform.GetChild(2).GetChild(2).gameObject.AddComponent<Button>().onClick.AddListener(() =>
            {
                print(item.ID);
                RewordResult(item);
            });
        }

    }
    /// <summary>
    /// 当用户点击领取任务奖励按钮之后的事件
    /// </summary>
    /// <param name="item">一个任务条目</param>
    private void RewordResult(MissionObject item)
    {
        print("任务的ID"+item.ID);
        print("获得奖励个数：" + item.rewords.Count);
        print("改变奖励的领取状态");
    }
    /// <summary>
    /// 获取任务奖励的领取状态对应的图片资源的名称
    /// </summary>
    /// <returns>The task reword status.</returns>
    /// <param name="type">Type.</param>
    /// <param name="item">Item.</param>
    private string GetTaskRewordStatus(TaskType type, MissionObject item)
    {
        if (!IsFinishTask(type, item))
        {
            return "weiwancheng";
        }
        else if (IsFinishTask(type, item) && GetTaskValue(type, item, 2) == "1")
        {
            return "yilingqu";
        }
        else if (IsFinishTask(type, item) && GetTaskValue(type, item, 2) == "0")
        {
            return "weilingqu";
        }
        return null;
    }
    /// <summary>
    /// 检测当前任务项是否完成
    /// </summary>
    /// <returns><c>true</c>, if finish task was ised, <c>false</c> otherwise.</returns>
    /// <param name="type">Type.</param>
    /// <param name="item">Item.</param>
    private bool IsFinishTask(TaskType type, MissionObject item)
    {
        if (int.Parse(GetTaskValue(type, item, 0)) - int.Parse(GetTaskValue(type, item, 1)) == 0)
        {
            return true;
        }
        else
        {
            return false;
        }

    }
    /// <summary>
    /// 设置正常任务项的种类（主线或者特殊）
    /// </summary>
    /// <param name="obj">Object.</param>
    /// <param name="item">Item.</param>
    /// <param name="type">Type.</param>
    private void SetTaskType(GameObject obj, MissionObject item, MissionType type)
    {
        switch (type)
        {
            case MissionType.Main:
                obj.transform.GetChild(0).GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite>("Task/" + MissionType.Main.ToString());
                break;
            case MissionType.Special:
                obj.transform.GetChild(0).GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite>("Task/" + MissionType.Special.ToString());
                break;
        }
    }
    /// <summary>
    /// 清除已经创建的任务项（更新）
    /// </summary>
    private void ClearTaskItemList()
    {
        if (taskItemList == null)
        {
            taskItemList = new List<GameObject>();
        }
        else
        {
            for (int i = taskItemList.Count - 1; i >= 0; i--)
            {
                Destroy(taskItemList[i]);
            }
            taskItemList = new List<GameObject>();
        }
    }
    /// <summary>
    /// 给标签绑定点击事件
    /// </summary>
    private void AddEventToLableButton()
    {
        lableButtonList[0].onClick.AddListener(() =>
        {
            CreateLableTaskItem(TaskType.TASK);
            ChangeLableButton(lableButtonList[0].name);
        });
        lableButtonList[1].onClick.AddListener(() =>
        {
            CreateLableTaskItem(TaskType.EGG);
            ChangeLableButton(lableButtonList[1].name);
        });
        closeLableTask.onClick.AddListener(() =>
        {
            PageManager.Instance.CurrentPage.GetNode<LableTaskNode>().gameObject.SetActive(false);
        });
    }
    /// <summary>
    /// 更换标签按钮的状态
    /// </summary>
    /// <param name="buttonName">Button name.</param>
    private void ChangeLableButton(string buttonName)
    {
        for (int i = 0; i < lableButtonList.Count; i++)
        {
            if (lableButtonList[i].name.Equals(buttonName))
            {
                lableButtonList[i].gameObject.SetActive(false);
                taskMainImage.sprite = Resources.Load<Sprite>("Task/" + buttonName);
            }
            else
            {
                lableButtonList[i].gameObject.SetActive(true);
            }
        }
    }
}
