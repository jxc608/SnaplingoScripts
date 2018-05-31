using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class UserAchievementData{
	
	//用户的ID
	//public string userId;
	public List<MissionObject> userAchievementList;
}
[Serializable]
public class AchievementItem
{
	//成就的获得状态
	public bool achievementIsGet;
	//成就名称
	public string achievementName;
	//成就获得条件
	public string achievementRequir;
	//成就奖励列表
	public List<string> achievementRewardList;
}
