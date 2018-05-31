using UnityEngine;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using System;
using UnityEngine.UI;
using System.Collections;
using Loxodon.Framework.Contexts;
using Snaplingo.Config;
using Loxodon.Framework.Asynchronous;
using Loxodon.Framework.Bundles;

public class LevelDebug : MonoBehaviour
{
	#region [ Property --- ]
	LevelData levelData;
	int levelID;
	#endregion


	#region [ --- Object References --- ]
	[Header (" --- Object References ---")]
	public InputField input_levelID;
	public Button btn_Go, btn_Go2;

	#endregion

	ApplicationContext context;
	Dictionary<IConfig, string> configDic = new Dictionary<IConfig, string> ();


	void Start ()
	{
		configDic.Add (new ExpConfig (), "AssetBundle/Config/ExpConfig.json");
		configDic.Add (new FontConfig (), "AssetBundle/Config/FontConfig.json");
		configDic.Add (new LevelConfig (), "AssetBundle/Config/LevelConfig.json");
		configDic.Add (new SongConfig (), "AssetBundle/Config/SongConfig.json");
		configDic.Add (new StageConfig (), "AssetBundle/Config/StageConfig.json");
		configDic.Add (new TaskConfig (), "AssetBundle/Config/TaskConfig.json");
		configDic.Add (new RoleModelConfig (), "AssetBundle/Config/RoleModelConfig.json");
		configDic.Add (new RoleEmotionConfig (), "AssetBundle/Config/RoleEmotionConfig.json");

		context = Context.GetApplicationContext ();
		LoadBundleManager.instance.Init (LoadBundleCallback);
	}

	void LoadBundleCallback ()
	{
		StartCoroutine (LoadConfigs ());
	}

	IEnumerator LoadConfigs ()
	{
		var _resources = context.GetService<IResources> ();
		foreach (var item in configDic)
		{
			IProgressResult<float, TextAsset> result = _resources.LoadAssetAsync<TextAsset> (item.Value);
			while (!result.IsDone)
			{
				//LogManager.Log("Progress:{0}%", result.Progress * 100);
				yield return null;
			}
			try
			{
				if (result.Exception != null)
					throw result.Exception;
				item.Key.Fill (result.Result.text);
				LogManager.Log ("Load : " , result.Result.name);
			}
			catch (Exception e)
			{
				LogManager.LogError ("Load failure.Error:{0}", e);
			}
		}
		GameStart ();
	}

	void GameStart ()
	{
		CorePlayData.CalcFirstLevelScore ();

		levelID = PlayerPrefs.GetInt ("debugLevelID", RuntimeConst.FirstLevelID);
		input_levelID.text = levelID.ToString ();
		btn_Go.onClick.AddListener (OnClick);
		btn_Go2.onClick.AddListener (OnClick2);
	}



	void OnClick ()
	{
		if (!int.TryParse (input_levelID.text, out levelID))
		{
			LogManager.LogError (" /// 错误  levelID !");
			return;
		}
		else if (!LevelConfig.AllLevelDic.ContainsKey (levelID))
		{
			levelID = levelID % 4 == 0 ? 1004 : 1001;
			LogManager.LogWarning (" /// LevelConfig 中没有场景，  使用默认场景 " , levelID);
		}

		PlayerPrefs.SetInt ("debugLevelID", levelID);
		levelData = LevelConfig.AllLevelDic[levelID];

		LoadSceneManager.Instance.LoadPlayScene (levelData);
	}

	void OnClick2 ()
	{
		levelData = LevelConfig.AllLevelDic[1002];
		LoadSceneManager.Instance.LoadPlayScene (levelData, "test");
	}














}
//LevelDebug













