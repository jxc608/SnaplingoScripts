using UnityEngine;
using System.Collections;
using Snaplingo.SaveData;
using System;

public class BundleData : ISaveData
{

	public static string url = "http://qa-game.oss-cn-beijing.aliyuncs.com/static";
	public static int manifestVersion;

	public const string SceneBundleName = "level_scene_bundle";

	//public static BundleData Instance
	//{
	//	get { return SaveDataUtils.GetSaveData<BundleData>(); }
	//}










	#region Implementation
	public string SaveAsJson()
	{
		throw new NotImplementedException();
	}

	public void LoadFromJson(string json)
	{
		throw new NotImplementedException();
	}

	public string SaveTag()
	{
		return "BundleData";
	}
	#endregion
}
//BundleData














