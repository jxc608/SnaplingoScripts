using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEngine.UI;

public enum MissionType
{
	Main,
	Daily,
	Special,
}
//MissionType


[Serializable]
public class MissionObject
{
	public int ID;
	public MissionType missionType;
	public Image icon;
	public string iconName;
	public string title;
	public string description;
	public bool isDone;

	public List<RewordObject> rewords = new List<RewordObject>();
}
//MissionObject














