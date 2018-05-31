using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Loxodon.Framework.Contexts;
using Loxodon.Framework.Asynchronous;
using Loxodon.Framework.Bundles;
using Snaplingo.Config;

public class ResourcesManager 
{

	private static ResourcesManager _instance;
	public static ResourcesManager Instance
	{
		get
		{
			if(null == _instance)
			{
				_instance = new ResourcesManager();
			}
			return _instance;
		}
	}

	private ResourcesManager()
	{
		
	}

	private GameManager m_gameManager;

	ApplicationContext context;
    Dictionary<IConfig, string> configDic = new Dictionary<IConfig, string>();

	private Action m_onConfigLoadCallback = null;
	private Action m_onInitCallback = null;
	private Action m_onProgressCallback = null;
	private float m_fConfigProgress = 0.0f;
	public float ConfigProgress
	{
		get
		{
			return m_fConfigProgress;
		}
	}

	public void Init(GameManager gameManager, Action onInitCallback)
	{
		m_gameManager = gameManager;
		context = Context.GetApplicationContext();
		LoadBundleManager.instance.Init(onInitCallback);
	}


	public void LoadAllConfig(Action onloadCallback = null)
	{
		this.m_onConfigLoadCallback = onloadCallback;
		m_fConfigProgress = 0.0f;

		configDic.Add(new ExpConfig(), "AssetBundle/Config/ExpConfig.json");
        configDic.Add(new FontConfig(), "AssetBundle/Config/FontConfig.json");
        configDic.Add(new LevelConfig(), "AssetBundle/Config/LevelConfig.json");
        configDic.Add(new SongConfig(), "AssetBundle/Config/SongConfig.json");
        configDic.Add(new StageConfig(), "AssetBundle/Config/StageConfig.json");
        configDic.Add(new TaskConfig(), "AssetBundle/Config/TaskConfig.json");
        configDic.Add(new RoleTitleConfig(), "AssetBundle/Config/RoleTitleConfig.json");
        configDic.Add(new CourseConfig(), "AssetBundle/Config/CourseConfig.json");
        configDic.Add(new RoleModelConfig(), "AssetBundle/Config/RoleModelConfig.json");
        configDic.Add(new RoleEmotionConfig(), "AssetBundle/Config/RoleEmotionConfig.json");

		m_gameManager.StartCoroutine(LoadConfigs());
	}

	//void LoadBundleCallback()
    //{
		
    //}

    IEnumerator LoadConfigs()
    {
        var _resources = context.GetService<IResources>();
		int i = 0;
        foreach (var item in configDic)
        {
            IProgressResult<float, TextAsset> result = _resources.LoadAssetAsync<TextAsset>(item.Value);
            while (!result.IsDone)
            {
				//LogManager.Log("Progress:{0}%", result.Progress * 100);
                yield return null;
            }
            try
            {
				++i;
				m_fConfigProgress = (float)((float)i / (float)configDic.Count);
                if (result.Exception != null)
                    throw result.Exception;
                item.Key.Fill(result.Result.text);
				LogManager.Log(result.Result.name);
            }
            catch (Exception e)
            {
				LogManager.LogError("Load failure.Error:{0}", e);
            }
        }
        yield return null;
		if(null != m_onConfigLoadCallback)
		{
			m_onConfigLoadCallback.Invoke();
			m_onConfigLoadCallback = null;
		}
        //GameStart();
    }

}
