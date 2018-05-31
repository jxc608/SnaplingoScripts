using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using Snaplingo.UI;
using Loxodon.Framework.Contexts;
using Loxodon.Framework.Bundles;

public class LoadSceneManager : MonoBehaviour
{
	public const string BookSceneName = "BookScene";
	public const string PlaySceneName = "CorePlay";
	float fadeInDuration = .5f;
	float fadeOutDuration = .5f;
	bool m_LoadingPlayScene;
	bool m_isLoading = false;
	string m_NextScene = "";
	//Debug
	float m_startLoadingTime;

	ApplicationContext context;
	ApplicationContext m_selfContext {
		get {
			if (null == context)
			{
				context = Context.GetApplicationContext ();
			}
			return context;
		}
	}
	#region [ Mono --- ]
	public static LoadSceneManager Instance { get; private set; }
	void Awake ()
	{
		Instance = this;
		DontDestroyOnLoad (this.gameObject);
	}
	void Start ()
	{
		//context = Context.GetApplicationContext ();
	}
	#endregion



	#region public 加载 游戏场景
	public string GetNextScene ()
	{
		return m_NextScene;
	}
	//    0 --     其他Scene
	public void LoadNormalScene (string sceneName)
	{
		if (!_CheckInput (sceneName)) return;
		m_LoadingPlayScene = false;

		StartCoroutine (_LoadSceneAsyncInternal ());
	}
	//    0 --     PlayScene
	public void LoadPlayScene (LevelData levelData, string songScript = null)
	{
		if (!_CheckInput (PlaySceneName)) return;
		if (!LevelConfig.CheckEnergy ()) return;
		m_LoadingPlayScene = true;

		StaticData.LevelID = levelData.levelID;
		StaticData.NowLevelData = levelData;
		CorePlayData.SongID = LanguageManager.GetSongIdFromLanguage (levelData.songID);
		if (string.IsNullOrEmpty (songScript))
		{
			CorePlayData.SongScript = SongConfig.Instance.GetsongScriptBySongIDAndLevelDiffculty (LanguageManager.GetSongIdFromLanguage (levelData.songID), levelData.LevelDifficulty);
		}
		else
			CorePlayData.SongScript = songScript;
		CorePlayData.SongOffset = SongConfig.Instance.GetSongOffsetBySongIDAndLevelDiffculty (LanguageManager.GetSongIdFromLanguage (levelData.songID), levelData.LevelDifficulty);
		CorePlayData.CurrentSong = BeatmapParse.Parse (CorePlayData.SongScript);

		CorePlayData.BossLife = levelData.bosslife;
		CorePlayData.BossSongName = LevelConfig.AllLevelDic[levelData.levelID].boss_song_name;
		CorePlayData.EducationText = SongConfig.Instance.m_items[LanguageManager.GetSongIdFromLanguage (levelData.songID)]["educationText"];
		StartCoroutine (_LoadSceneAsyncInternal ());
	}

	bool _CheckInput (string sceneName)
	{
		if (m_isLoading) { return false; }
		else
		{
			m_isLoading = true;
			m_NextScene = sceneName;
			return true;
		}
	}
	#endregion

	#region _LoadSceneAsyncInternal
	IEnumerator _LoadSceneAsyncInternal ()
	{
		m_startLoadingTime = Time.realtimeSinceStartup;

		if (m_LoadingPlayScene)
			ShowLoading.Instance.TeachFadeIn (CorePlayData.EducationText, fadeInDuration);
		else
			ShowLoading.Instance.FadeIn (fadeInDuration);
		//yield return new WaitForSeconds(fadeInDuration);


		yield return StartCoroutine (LoadNextSceneAsync ());

		// 清理 bundle 没处理
		Resources.UnloadUnusedAssets ();
		System.GC.Collect ();

		LogManager.Log ("加载用时 ： " , ((float)(Time.realtimeSinceStartup - m_startLoadingTime)).ToString ());
		m_isLoading = false;
	}
	#endregion


	#region Load NextScene
	IEnumerator LoadNextSceneAsync ()
	{
		bool loadFail = false;
		var _resources = m_selfContext.GetService<IResources> ();
		AsyncOperation asyncLoadPlayScene;
		PageManager.Instance.DestroyCurrentPages ();
		yield return null;

		ObjectPool.RegisterPool ("RhythmController", 10, "Coreplay/RhythmController");
		if (m_LoadingPlayScene)
		{
			// Bundle 逻辑
			string sceneName = "AssetBundle/level_scene/Scene_" + StaticData.LevelID.ToString ();
			ISceneLoadingResult<Scene> result = _resources.LoadSceneAsync (sceneName + ".unity");
			while (!result.IsDone)
			{
				yield return null;
			}
			try
			{
				if (result.Exception != null)
					throw result.Exception;
				LogManager.Log (result.Result.name);
			}
			catch
			{
				loadFail = true;
			}

			if (loadFail)
			{
				int id = StaticData.LevelID % 4 == 0 ? 1004 : 1001;
				result = _resources.LoadSceneAsync ("AssetBundle/level_scene/Scene_" + id.ToString () + ".unity");
				while (!result.IsDone)
				{
					yield return null;
				}
			}

			asyncLoadPlayScene = SceneManager.LoadSceneAsync (PlaySceneName, LoadSceneMode.Additive);
			PageManager.Instance.OpenPage ("CorePlayPage", false);
		}
		else
		{
			asyncLoadPlayScene = SceneManager.LoadSceneAsync (m_NextScene);
		}
		while (!asyncLoadPlayScene.isDone)
		{
			yield return null;
		}

		//GeneralUIManager.RefreshSortingOrder();
		ShowLoading.Instance.FadeOut (null);
	}
	#endregion


}
//LoadSceneManager


