using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Snaplingo.UI;
using UnityEngine.UI;


public class AchievementItemNode : Node
{

    /// <summary>
    /// 成就的icon
    /// </summary>
	public Image achieveImage;
    /// <summary>
    /// 成就的名称
    /// </summary>
	public Text achieveName;
    /// <summary>
    /// 成就的描述
    /// </summary>
	public Text achieveDesc;
    /// <summary>
    /// 获得此成就之后的奖励
    /// </summary>
	public Text achieveRewords;
    /// <summary>
    /// 成就的获得时间
    /// </summary>
    public Text achieveGetTime;

    private void Start()
    {
        //初始化完成之后先隐藏
        gameObject.SetActive(false);
    }
    public override void Open()
    {
        base.Open();
    }
    public override void Init(params object[] args)
    {
        base.Init();
    }
    /// <summary>
    /// 设置一个成就的详细信息
    /// </summary>
    /// <param name="item">一个成就的信息</param>
	public void SetAchievementItemInfor(MissionObject item)
    {
        achieveName.text = item.title;
        achieveDesc.text = item.description;
        string rewordStr = null;
        foreach (RewordObject reword in item.rewords)
        {
            rewordStr += "[" + reword.title + "] ";
        }
        if (item.isDone)
        {
            achieveGetTime.text = " 获得于：" + GetAchievementGetTime(item.ID);
        }
        else
        {
            achieveGetTime.text = "未获得";
        }
        achieveRewords.text = rewordStr;
        //设置完成之后显示此成就的信息界面
        gameObject.SetActive(true);
    }
    /// <summary>
    /// 获取成就的获得时间
    /// </summary>
    /// <returns>The achievement get time.</returns>
    private string GetAchievementGetTime(int achievenId)
    {
        string str = System.DateTime.Now.ToString("yyyy年MM月dd日");
        print(str);
        return str;
    }
}
