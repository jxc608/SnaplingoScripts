using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

[Serializable]
public class RewordObject
{
	public int ID;
	public Image icon;
	public string iconName;
	public string title;
	public string description;

	public string SaveToString()
	{
		return JsonUtility.ToJson(this);
	}
}
//RewordObject














