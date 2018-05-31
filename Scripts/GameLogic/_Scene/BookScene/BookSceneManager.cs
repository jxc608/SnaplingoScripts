using UnityEngine;
using Snaplingo.UI;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.Video;
using System.Collections.Generic;
using System.Collections;

using Loxodon.Framework.Contexts;
using Loxodon.Framework.Asynchronous;
using Loxodon.Framework.Bundles;
using Snaplingo.Config;
using System;
using Snaplingo.SaveData;
using LitJson;

namespace TA
{
	public class LevelDataEvent : UnityEvent<LevelData> { }

	public class BookSceneManager : MonoBehaviour
	{
		#region [ --- Event --- ]
		// LevelEntry 触发 , UI 监听
		public LevelDataEvent onReceiveLevelData = new LevelDataEvent ();
		#endregion


		#region [ --- Property --- ]
		public static BookSceneManager instance;
		//public static bool isFirstTime = true;
		//public static int pageID = 0;

		List<StagePage> allStagePages = new List<StagePage> ();
		public StagePage stagePage;
		#endregion


		#region [ --- Object References --- ]
		[Header (" --- Object References ---")]
		[SerializeField]
		Actor_Book actorBook;
		[SerializeField]
		StageManager stageDisk;

		[HideInInspector]
		public GameObject[] prefab_BookPages;
		#endregion



		#region [ --- Mono --- ]
		void Awake ()
		{
			instance = this;
			onReceiveLevelData.RemoveAllListeners ();
			prefab_BookPages = new GameObject[3];
			prefab_BookPages[0] = Resources.Load ("BookScene/Stage_Page_1") as GameObject;
			prefab_BookPages[1] = Resources.Load ("BookScene/Stage_Page_2") as GameObject;
			prefab_BookPages[2] = Resources.Load ("BookScene/Stage_Page_3") as GameObject;
		}
		void Start ()
		{
			if (debug)
				LoadBundle ();
			else
				GameStart ();
			//GetCourseFormServer ();
			AudioController.PlayMusic("BookMusic");
		}
		/// <summary>
		/// 获取当前用户的所有关卡的状态信息
		/// </summary>
		public static void GetCourseFormServer ()
		{
			SaveDataUtils.Load<SelfPlayerRoleTitleData> ();
			JsonData data = new JsonData ();
			data["userID"] = SelfPlayerData.Uuid;
			DancingWordAPI.Instance.RequestCourseFromServer (data, (string result) => {
				LogManager.Log (result);
				JsonData resultData = JsonMapper.ToObject (result);
				JsonData courseData = resultData["data"];
				List<CourseData> courseDatas = new List<CourseData> ();
				for (int i = 0; i < courseData.Count; i++)
				{
					courseDatas.Add (new CourseData () {
						m_CourseID = int.Parse (courseData[i].TryGetString ("courseID")),
						m_Progress = int.Parse (courseData[i].TryGetString ("progress")),
						m_IsPay = bool.Parse (courseData[i].TryGetString ("isPay"))
					});
				}
				SelfPlayerData.CourseData = courseDatas;
				SaveDataUtils.Save<SelfPlayerData> ();
				SelfPlayerData.LevelStatus = SelfPlayerData.Instance.CreateLevelStatusDic ();
				//测试
				//LevelStatus dic = SelfPlayerData.Instance.GetLevelStatusByLevelId(1003);
				//print(dic.isPay + "=====" + dic.isLock);
				//SelfPlayerData.Instance.SetLevelStatus(1002);
			}, () => {
				///print("GetCourse");
				SaveDataUtils.Load<SelfPlayerData> ();
				SelfPlayerData.LevelStatus = SelfPlayerData.Instance.CreateLevelStatusDic ();
			});
		}

		///Debug
		/// 
		[Header (" --- Debug ---")]
		public bool debug;
		public GameObject[] debugObjs;
		ApplicationContext context;
		Dictionary<IConfig, string> configDic = new Dictionary<IConfig, string> ();
		void LoadBundle ()
		{
			foreach (var item in debugObjs)
			{
				item.SetActive (true);
			}
			configDic = new Dictionary<IConfig, string> ();
			configDic.Add (new LevelConfig (), "AssetBundle/Config/LevelConfig.json");
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
					LogManager.Log (result.Result.name);
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
			// 0 设置 UI
			PageManager.Instance.OpenPage<BookSceneUIPage> (false);
			var bookPage = PageManager.Instance.CurrentPage as BookSceneUIPage;
			bookPage.Setup (CorePlayData.isFirstTime);
			//PageManager.Instance.CurrentPage.AddNode<LableTaskNode>(true);
			//PageManager.Instance.CurrentPage.AddNode<AchievementNode>(true);
			//PageManager.Instance.CurrentPage.AddNode<AchievementItemNode>(true);

			for (int i = 0; i < prefab_BookPages.Length; i++)
			{
				var obj = Instantiate (prefab_BookPages[i]);
				obj.transform.SetParent (transform, false);
				var page = obj.GetComponent<StagePage> ();
				page.SetupPage ();
				allStagePages.Add (page);
				obj.SetActive (false);
			}
			stagePage = allStagePages[CorePlayData.pageID];
			stagePage.gameObject.SetActive (true);

			if (CorePlayData.isFirstTime)
			{
				//stageDisk.SetDefault ();
				stagePage.SetDefault ();
				actorBook.SetState (State.Opening);
				//actorBook.SetState (State.Default);
				//PlayVideo ();
			}
			else
			{
				//stageDisk.Enter_Playing ();
				stagePage.Enter_Playing ();
				actorBook.SetState (State.Playing);
			}

		}
		#endregion






		#region [ --- Public --- ]
		public void ReLoadScene ()
		{
			PageManager.Instance.DestroyCurrentPages ();
			SceneManager.LoadScene (SceneManager.GetActiveScene ().name);
		}
		// Book
		public void OpenActorBook ()
		{
			actorBook.SetState (State.Opening);
		}
		public void CloseActorBook ()
		{
			actorBook.SetState (State.Closure);
		}
		public void ActorBookPageTurning (bool isPageUp)
		{
			if (isPageUp)
			{
				actorBook.PageUp ();
			}
			else
			{
				actorBook.PageDown ();
			}
		}
		// Book_Disk
		public void OpenStageDisk ()
		{
			//stageDisk.Enter_Opening ();
			CorePlayData.isFirstTime = false;
		}
		public void CloseStageDisk ()
		{
			//stageDisk.Enter_Closure ();
		}
		// Page
		public void OpenStagePage ()
		{
			stagePage.Enter_Opening ();
		}
		public void CloseStagePage ()
		{
			stagePage.Enter_Closure ();
		}

		public void SetupNewPage (bool pageUp)
		{
			if (pageUp)
			{
				CorePlayData.pageID = (CorePlayData.pageID + 1) % prefab_BookPages.Length;
				stagePage.gameObject.SetActive (false);
				stagePage = allStagePages[CorePlayData.pageID];
				stagePage.gameObject.SetActive (true);
			}
			else
			{
				CorePlayData.pageID = (CorePlayData.pageID - 1 + prefab_BookPages.Length) % prefab_BookPages.Length;
				stagePage.gameObject.SetActive (false);
				stagePage = allStagePages[CorePlayData.pageID];
				stagePage.gameObject.SetActive (true);
			}
		}


		#endregion





		#region [ --- Private --- ]

		#endregion

		#region [ --- Play Video --- ]
		private void PlayVideo ()
		{
			if (StaticData.ApplicationStart)
			{
				if (CorePlaySettings.Instance.m_UseMemoryPool)
				{
					VideoPlay.Instance.PlayVideo (StaticMemoryPool.GetItem<VideoClip> ("startAnimation"), PlayEnd);
				}
				else
				{
					VideoClip vc = ResourceLoadUtils.Load<VideoClip> ("animation");
					VideoPlay.Instance.PlayVideo (vc, PlayEnd);
				}
			}
			else
			{
				PlayBG ();
			}
		}

		private void PlayEnd (VideoPlayer source)
		{
			StaticData.ApplicationStart = false;
			StaticMemoryPool.ClearItem ("startAnimation");
			((BookSceneUIPage)PageManager.Instance.CurrentPage).CloseSkip ();

			VideoPlay.Instance.FadeOut (() => { PlayBG (); });
		}

		public void PlayBG ()
		{
			AudioController.Play ("BookMusic");
		}
		#endregion









	}
	//BookSceneManager
}













