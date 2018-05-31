using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Snaplingo.Config;
using LitJson;
using System;

public class TaskConfig : IConfig
{
	public static TaskConfig Instance { get { return _instance; } }
	static TaskConfig _instance;
	public TaskConfig()
	{
		_instance = this;
	}

	public Dictionary<int, Dictionary<string, string>> m_items = new Dictionary<int, Dictionary<string, string>>();
	public void Fill(string jsonstr)
	{
		var json = JsonMapper.ToObject(jsonstr);
		List<string> keys = new List<string>(json.Keys);
		for (int i = 0; i < keys.Count; i++)
		{
			try
			{
				var value = json[keys[i]];
				Dictionary<string, string> items = new Dictionary<string, string>();
				int id = int.Parse(value.TryGetString("task_id"));
				items["group"] = value.TryGetString("group");
				items["icon"] = value.TryGetString("icon");
				items["next_id"] = value.TryGetString("next_id");
				items["task_condition_num_1"] = value.TryGetString("task_condition_num_1");
				items["task_condition_num_2"] = value.TryGetString("task_condition_num_2");
				items["task_condition_type_1"] = value.TryGetString("task_condition_type_1");
				items["task_condition_type_2"] = value.TryGetString("task_condition_type_2");
				items["task_desc"] = value.TryGetString("task_desc");
				items["task_name"] = value.TryGetString("task_name");
				items["task_reward_num_1"] = value.TryGetString("task_reward_num_2");
				items["task_reward_num_2"] = value.TryGetString("task_reward_num_2");
				items["task_reward_type_1"] = value.TryGetString("task_reward_type_1");
				items["task_reward_type_2"] = value.TryGetString("task_reward_type_2");
				items["task_type"] = value.TryGetString("task_type");
				m_items[id] = items;
			}
			catch (Exception e)
			{
				LogManager.Log("配表解析错误：" , e.ToString());
			}
		}
	}
	public string GetTaskConfigByIdAndKey(int id, string key)
	{
		return m_items[id][key];
	}
}
