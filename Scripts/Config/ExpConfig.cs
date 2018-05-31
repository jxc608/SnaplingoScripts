using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Snaplingo.Config;
using LitJson;
using System;

public class ExpConfig : IConfig
{
	public static ExpConfig Instance { get { return _instance; } }
	static ExpConfig _instance;
	public ExpConfig()
	{
		_instance = this;
	}

	public List<Dictionary<string, string>> dataList = new List<Dictionary<string, string>>();

	public List<ExpBasic> expData = new List<ExpBasic>();

	public void Fill(string jsonstr)
	{
		var json = JsonMapper.ToObject(jsonstr);
		List<string> keys = new List<string>(json.Keys);
		for (int i = 0; i < keys.Count; i++)
		{
			try
			{
				var value = json[keys[i]];
				ExpBasic item = new ExpBasic();
				item.level = (int)value["level"];
				item.name = (string)value["name"];
				//item.decoIcon = (string)value["decoIcon"];
				item.energy = (int)value["energy"];
				item.hp = (int)value["hp"];
				item.expStart = (int)value["exp"];
				item.expNeed = (int)value["need"];

				if (item.expNeed != 0)
				{
					item.expEnd = item.expStart + item.expNeed - 1;
				}
				else
				{
					item.expEnd = item.expStart + item.expNeed;
				}

				expData.Add(item);
			}
			catch (Exception e)
			{
				LogManager.LogError("Exp Config row " , i , " parse Error! " , e);
			}
		}
	}

	private void sortA()
	{

	}

	public ExpBasic getExpInfo(int exp)
	{
		ExpBasic returnInfo = new ExpBasic();

		for (int i = 0; i < expData.Count; i++)
		{
			ExpBasic item = expData[i];
			if (exp >= item.expStart && exp < item.expEnd)
			{
				returnInfo = item;
				return returnInfo;
			}
			//最后一级
			else if (exp >= item.expStart && item.expNeed == 0)
			{
				returnInfo = item;
				return returnInfo;
			}
		}

		return returnInfo;
	}

}

[Serializable]
public struct ExpBasic
{

	/// <summary>
	/// 当前等级
	/// </summary>
	public int level;

	/// <summary>
	/// 当前等级描述(一阶圣手)
	/// </summary>
	public string name;

	/// <summary>
	/// 当前等级描述(一阶圣手)对应图标名字
	/// </summary>
	//public string decoIcon;

	/// <summary>
	/// 当前等级对应体力
	/// </summary>
	public int energy;

	/// <summary>
	/// 当前等级对应血量
	/// </summary>
	public int hp;

	/// <summary>
	/// 当前等级起始经验值
	/// </summary>
	public int expStart;

	/// <summary>
	/// 当前等级目标经验值
	/// </summary>
	public int expEnd;

	/// <summary>
	/// 当前等级升级所需经验
	/// </summary>
	public int expNeed;

}