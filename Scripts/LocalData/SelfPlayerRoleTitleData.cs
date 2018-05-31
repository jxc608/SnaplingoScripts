using System;
using System.Collections;
using System.Collections.Generic;
using LitJson;
using Snaplingo.SaveData;
using UnityEngine;

/// <summary>
/// 本地存储称号数据的类
/// </summary>
public class SelfPlayerRoleTitleData : ISaveData
{
    public static SelfPlayerRoleTitleData Instance
    {
        get { return SaveDataUtils.GetSaveData<SelfPlayerRoleTitleData>(); }
    }
    /// <summary>
    /// 用户所有称号的集合
    /// </summary>
	private List<RoleTitleItemObject> roleTitleList = new List<RoleTitleItemObject>();
    public static List<RoleTitleItemObject> RoleTitleList
    {
        get
        {
			return Instance.roleTitleList;
        }
        set
        {
            Instance.roleTitleList = value;
        }
    }
    /// <summary>
    /// 给从服务器拿到的数据按id从小到大排序保证每次拿到的数据的顺序一致。
    /// </summary>
    /// <returns>排序完的数据</returns>
    /// <param name="list">从服务器拿到的数据</param>
	private static List<RoleTitleItemObject> SortRoleTitleObjectList(List<RoleTitleItemObject> list)
	{
		list.Sort();
		LogManager.Log("排序");
		return list;
	}
    /// <summary>
    /// 每次Load之后都会清空原先的称号集合重新再刷新一次称号集合
    /// </summary>
    /// <param name="json">Json.</param>
    public void LoadFromJson(string json)
    {
		LogManager.Log(json);
        if (string.IsNullOrEmpty(json))
            return;
        if (roleTitleList != null)
        {
            roleTitleList.Clear();
        }
        else
        {
            roleTitleList = new List<RoleTitleItemObject>();
        }
        JsonData data = JsonMapper.ToObject(json);
        foreach (JsonData tempData in data)
        {
            RoleTitleItemObject item = new RoleTitleItemObject();
            item.roletitleId = int.Parse(tempData.TryGetString("roletitleId"));
            item.roletitleCount = int.Parse(tempData.TryGetString("roletitleCount"));
            item.roleTitleStatus = (RoleTitleStatus)Enum.Parse(typeof(RoleTitleStatus), tempData.TryGetString("roletitleStatus"));
            roleTitleList.Add(item);
        }
		//LogManager.Log(roleTitleList.Count);
    }
    /// <summary>
    /// 每次Save都会把当前最新的称号集合做持久化存储
    /// </summary>
    /// <returns>The as json.</returns>
    public string SaveAsJson()
    {
        JsonData data = new JsonData();
        foreach (RoleTitleItemObject item in roleTitleList)
        {
            JsonData tempData = new JsonData();
            tempData["roletitleId"] = item.roletitleId;
            tempData["roletitleCount"] = item.roletitleCount;
            tempData["roletitleStatus"] = item.roleTitleStatus.ToString();
            data.Add(tempData);
        }
        return data.ToJson();
    }

    public string SaveTag()
    {
        return GetType().ToString();
    }
    /// <summary>
    /// 更新称号的进度
    /// </summary>
    /// <param name="roleTitleID">Role title identifier.</param>
    public void UpdateRoleTitleCount(int roleTitleID)
    {
		if (roleTitleList == null) return;
        for (int i = 0; i < roleTitleList.Count;i++)
        {
            if(roleTitleList[i].roletitleId == roleTitleID)
            {
                roleTitleList[i].roletitleCount++;
                return;
            }
        }
    }
    /// <summary>
    /// 更新称号的穿戴状态
    /// </summary>
    /// <param name="roleTitleID">Role title identifier.</param>
    /// <param name="roleTitleStatus">Role title status.</param>
    public void UpdateRoleTitleStatus(int roleTitleID,RoleTitleStatus roleTitleStatus)
    {
        for (int i = 0; i < roleTitleList.Count;i++)
        {
            if(roleTitleList[i].roletitleId==roleTitleID)
            {
                roleTitleList[i].roleTitleStatus = roleTitleStatus;
                return;
            }
        }
    }
}
