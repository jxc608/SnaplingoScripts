using System.Collections;
using System.Collections.Generic;
using Snaplingo.Config;
using UnityEngine;
using System;
using LitJson;
/// <summary>
/// 描述称号的信息的枚举
/// </summary>
public enum RoleTitleEnum
{
	/// <summary>
    /// 称号所对应的UI的名字
    /// </summary>
    RoleTitleIcon,
    /// <summary>
    /// 称号的标题
    /// </summary>
    RoleTitleDesc,
    /// <summary>
    /// 称号的等级
    /// </summary>
    RoleTitleLevel,
    /// <summary>
    /// 称号是否正在穿戴
    /// </summary>
    RoleTitleIsWeared,
    /// <summary>
    /// 称号是否能穿
    /// </summary>
    RoleTitleIsCanWear,
    /// <summary>
    /// 称号的ID
    /// </summary>
    RoleTitleId,
    /// <summary>
    /// 称号的完成进度
    /// </summary>
    RoleTitleCount,
}
/// <summary>
/// 称号项的状态
/// </summary>
public enum RoleTitleStatus
{
	/// <summary>
    /// 已穿
    /// </summary>
    Weared,
    /// <summary>
    /// 能穿
    /// </summary>
    CanWear,
    /// <summary>
    /// 未完成
    /// </summary>
    UnFinished,
    /// <summary>
    /// 未穿
    /// </summary>
    NotWear
}
public class RoleTitleConfig : IConfig
{
    public static RoleTitleConfig Instance { get { return _instance; } }
    private static RoleTitleConfig _instance;
    public Dictionary<int, Dictionary<string, string>> roletitleDic = new Dictionary<int, Dictionary<string, string>>();
    public RoleTitleConfig()
    {
        _instance = this;
    }
    public void Fill(string jsonstr)
    {
        JsonData data = JsonMapper.ToObject(jsonstr);
        List<string> keys = new List<string>(data.Keys);
        foreach (string key in keys)
        {
            var value = data[key];
            Dictionary<string, string> dic = new Dictionary<string, string>();
            foreach (string k in value.Keys)
            {
                dic.Add(k, value[k].ToString());
            }
            roletitleDic.Add(int.Parse(key), dic);
        }
    }
    /// <summary>
    /// 根据服务端的返回数据去客户端获取数据
    /// </summary>
    /// <returns>用户的某一个称号的内容</returns>
    /// <param name="roleTitleItem">服务端返回的角色的一条称号数据</param>
    public Dictionary<string, string> GetRoleTitleItem(RoleTitleItemObject roleTitleItem)
    {
        int index = 0;

        //0 代表不可以穿戴 1 代表可以穿戴
        RoleTitleStatus roleTitleCanWear = RoleTitleStatus.UnFinished;
        Dictionary<string, string> roletitleItemDic = new Dictionary<string, string>();
        int count = roleTitleItem.roletitleCount;
        //获取某一类称号的所有梯度
        string[] levels = roletitleDic[roleTitleItem.roletitleId]["roletitlelevel"].Split('|');
        for (int i = 0; i < levels.Length; i++)
        {
            //如果当前称号的梯度值大于所有梯度中的某一梯度
            if (count >= int.Parse(levels[i]))
            {
                roleTitleCanWear = RoleTitleStatus.CanWear;
                //如果当前称号的梯度已经是所有梯度里面最大的之后相当于该称号的最高梯度已达到！
                if (i + 1 >= levels.Length)
                {
                    index = i;
                    count = int.Parse(levels[i]);
                }
                else
                {
                    //如果还有比当前称号的梯度更大的梯度则客户端要显示的梯度是离当前梯度最近的梯度
                    if (count < int.Parse(levels[i + 1]))
                    {
                        index = i + 1;
                    }
                }
            }
        }
		//当获得梯度之后则返回相应梯度的称号内容
		string tempRoleTitleDesc = LanguageManager.languageType == LanguageType.Chinese ? roletitleDic[roleTitleItem.roletitleId]["roletitledesc"].Split('|')[index].Split('$')[0] : roletitleDic[roleTitleItem.roletitleId]["roletitledesc"].Split('|')[index].Split('$')[1];
		roletitleItemDic.Add(RoleTitleEnum.RoleTitleDesc.ToString(), tempRoleTitleDesc);
        roletitleItemDic.Add(RoleTitleEnum.RoleTitleIcon.ToString(), roletitleDic[roleTitleItem.roletitleId]["roletitleicon"].Split('|')[index]);
        roletitleItemDic.Add(RoleTitleEnum.RoleTitleLevel.ToString(), roletitleDic[roleTitleItem.roletitleId]["roletitlelevel"].Split('|')[index]);
        roletitleItemDic.Add(RoleTitleEnum.RoleTitleIsWeared.ToString(), (roleTitleItem.roleTitleStatus).ToString());
        roletitleItemDic.Add(RoleTitleEnum.RoleTitleIsCanWear.ToString(), roleTitleCanWear.ToString());
        roletitleItemDic.Add(RoleTitleEnum.RoleTitleId.ToString(), roleTitleItem.roletitleId.ToString());
        roletitleItemDic.Add(RoleTitleEnum.RoleTitleCount.ToString(),count.ToString());
        return roletitleItemDic;
    }
}
/// <summary>
/// 存储服务端返回的称号状态信息
/// </summary>
[Serializable]
public class RoleTitleItemObject:IComparable<RoleTitleItemObject>
{
    public int roletitleId;
    public int roletitleCount;
    public RoleTitleStatus roleTitleStatus;
    /// <summary>
    /// 使称号能根据ID从小到大排序
    /// </summary>
    /// <returns>The to.</returns>
    /// <param name="r">The red component.</param>
	public int CompareTo(RoleTitleItemObject r)
    {   
		return this.roletitleId - r.roletitleId;
    }
}
