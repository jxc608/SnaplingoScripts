using System.Collections.Generic;
using UnityEngine;
using Snaplingo.UI;
using UnityEngine.EventSystems;
using DG.Tweening;
using TMPro;

namespace TA
{
	public class LevelEntry : StageActor, IPointerClickHandler
	{
		#region [ --- Property --- ]
		public LevelData levelData;
		public Color color_Dark = Color.gray;
		public Color color_Dark2 = new Color (.7f, .7f, .7f, 1f);

		public Color color_Unlock;
		public float fadeDuration = 0.2f;
		BookSceneManager _bookSceneManager;
		List<Actor_Building> builds = new List<Actor_Building> ();
		bool isShown;
		SpriteRenderer sr_lock;
		SpriteRenderer sr_highlight;
		#endregion


		#region [ --- Object References --- ]
		[Header (" --- Object References ---")]
		public SpriteRenderer sp_level;
		public SpriteRenderer[] sp_fades;
		public Sprite sp_lock;
		public Sprite sp_highlight;
		public Sprite sp_highlight_boss;

		public TMP_Text text;
		public Transform root_builds;

		Collider coll;
		#endregion

		public StagePage Page {
			set {
				m_page = value;
			}
			get {
				return m_page;
			}
		}
		private StagePage m_page;
		private CommonPromptNode m_promptNode;

		//当前index (当前是第几个Node)
		//private int mCurrentIndex;
		//用户成绩
		//星星列表
		//private List<GameObject> mStarList;
		//当前数据
		//private Dictionary<string, string> mItemLevelData;
		//bool mCurrentIsCanPlay = true;
		//bool mCurrentLevelIsFinish = false;


		#region [ --- Mono --- ]
		void Awake ()
		{
			coll = GetComponent<Collider> ();

			if (root_builds != null)
			{
				for (int i = 0; i < root_builds.childCount; i++)
				{
					if (null != root_builds.GetChild (i).GetComponent<Actor_Building> ())
						builds.Add (root_builds.GetChild (i).GetComponent<Actor_Building> ());
				}
			}
			sr_highlight = new GameObject ("sr_highlight", typeof (SpriteRenderer)).GetComponent<SpriteRenderer> ();
			sr_highlight.transform.SetParent (transform, false);
			sr_highlight.gameObject.layer = LayerMask.NameToLayer ("Scenery3D");
			sr_highlight.sortingLayerName = "UI";
			sr_highlight.sortingOrder = 2;
			sr_lock = new GameObject ("sr_lock", typeof (SpriteRenderer)).GetComponent<SpriteRenderer> ();
			sr_lock.transform.SetParent (transform, false);
			sr_lock.gameObject.layer = LayerMask.NameToLayer ("Scenery3D");
			sr_lock.sortingLayerName = "UI";
			sr_lock.sortingOrder = 3;
			sr_lock.sprite = sp_lock;
		}
		public void Reset ()
		{
			foreach (var item in sp_fades)
			{
				DOTween.Kill (item);
				item.color = new Color (1, 1, 1, 0);
			}
			sr_highlight.gameObject.SetActive (false);
			sr_lock.gameObject.SetActive (false);
			text.color = new Color32 (87, 134, 185, 0);
			coll.enabled = false;
			isShown = false;
		}
		#endregion


		#region [ --- Implement --- ]
		public void OnPointerClick (PointerEventData eventData)
		{
			if (_bookSceneManager != null)
			{
				var levelStatus = SelfPlayerData.Instance.GetLevelStatusByLevelId (levelData.levelID);
				//bool pay = levelData.levelID % 100 > 8 ? true : false;
				if (levelStatus.isPay || DebugConfigController.Instance._Debug)
				{
					if (levelStatus.isUnLock || DebugConfigController.Instance._Debug)
					{
						if (SelfPlayerData.Instance.GetMaxLevelId () == levelData.levelID)
						{
							LoadSceneManager.Instance.LoadPlayScene (levelData);
						}
						else
						{
							_bookSceneManager.onReceiveLevelData.Invoke (levelData);
						}
					}
					else
					{
						CommonPromptNode node = PageManager.Instance.CurrentPage.AddNode<CommonPromptNode> (true) as CommonPromptNode;
						node.InitPrompt (false, SelfType.Prompt, SelfType.NeedUnlock,
											LanguageManager.languageType == LanguageType.Chinese ? m_page.TexSure : m_page.TexSure_EN,
											LanguageManager.languageType == LanguageType.Chinese ? m_page.TexCancel : m_page.TexCancel_EN,
											null, null);
					}

				}
				else
				{
					AnalysisManager.Instance.OnEvent ("payNode_other", null);
					PageManager.Instance.CurrentPage.AddNode<CommonPromptNode> (true);
					m_promptNode = PageManager.Instance.CurrentPage.AddNode<CommonPromptNode> (true) as CommonPromptNode;
					m_promptNode.InitPrompt (false, SelfType.Prompt, SelfType.NeedPay,
											LanguageManager.languageType == LanguageType.Chinese ? m_page.TexUnlock : m_page.TexUnlock_EN,
											LanguageManager.languageType == LanguageType.Chinese ? m_page.TexCancel : m_page.TexCancel_EN,
											this.OpenPay, null);
				}
			}

		}

		private void OpenPay ()
		{
			if (null != m_promptNode)
			{
				m_promptNode.Close (true);
				m_promptNode = null;
			}
			PageManager.Instance.CurrentPage.AddNode<IAPNode> (true);

		}
		#endregion



		#region [ --- Override --- ]
		protected override void SetDefault ()
		{
			Reset ();
		}

		// Opening
		protected override void Enter_Opening ()
		{
		}

		// Playing
		protected override void Enter_Playing ()
		{
			Show ();
		}
		protected override void Update_Playing ()
		{
		}

		// Closure
		protected override void Enter_Closure ()
		{
			Reset ();
		}
		#endregion




		#region [ --- Public --- ]
		public void Init (LevelData data)
		{
			_bookSceneManager = BookSceneManager.instance;
			levelData = data;
			sr_highlight.sprite = data.levelID % 4 == 0 ? sp_highlight_boss : sp_highlight;
			text.text = GetShowText ((data.levelID - 1000).ToString ());
			Reset ();
		}
		private string GetShowText (string levelIndex)
		{
			string result = null;
			for (int i = 0; i < levelIndex.Length; i++)
			{
				result += "<sprite=" + levelIndex[i] + ">";
			}
			return result;
		}
		public void Show ()
		{
			if (isShown) return;
			isShown = true;


			// 测试环境，解锁全部关卡
			if (DebugConfigController.Instance._Debug)
			{
				SetDisplay (true, Color.white);
				return;
			}


			var levelStatus = SelfPlayerData.Instance.GetLevelStatusByLevelId (levelData.levelID);
			//LogManager.Log (levelData.levelID , " : " , levelStatus.isPay , "=====" , levelStatus.isUnLock);
			//bool pay = levelData.levelID % 100 > 8 ? true : false;
			if (levelStatus.isPay)
			{
				if (levelStatus.isUnLock)
				{
					// 解锁关卡 + 当前关
					//if (levelData.levelID == SelfPlayerData.Instance.GetMaxLevelId ())
					//{
					//}
					SetDisplay (true, Color.white);
				}
				// 被锁定关卡
				else
				{
					SetDisplay (false, color_Dark2);
				}
			}
			else
			{
				// 未购买
				sr_lock.gameObject.SetActive (true);
				SetDisplay (false, color_Dark);
			}

		}
		#endregion


		void SetDisplay (bool isUnLock, Color color)
		{
			foreach (var item in sp_fades)
			{
				item.color = new Color (1, 1, 1, 0);
				item.DOColor (color, fadeDuration);
			}
			coll.enabled = true;
			text.gameObject.SetActive (true);
			if (color == Color.white)
			{
				text.color = new Color32 (87, 134, 185, 255);
			}
			else
				text.color = color;


			if (isUnLock)
			{
				// 显示建筑
				foreach (var item in builds)
				{
					item.Show ();
				}

				sr_highlight.gameObject.SetActive (true);
				sr_highlight.color = new Color (1, 1, 1, 0);
				sr_highlight.DOComplete ();
				sr_highlight.DOFade (1, 1.5f)
							.SetLoops (-1, LoopType.Restart);
				transform.DOJump (transform.position, .1f, 2, 1.5f)
						 .SetLoops (-1, LoopType.Restart);
			}
		}



	}
	//LevelEntry
}













