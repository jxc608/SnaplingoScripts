using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Spine.Unity;
using LitJson;
using Snaplingo.SaveData;
using Snaplingo.UI;

public class RoleManager : MonoBehaviour
{
	/// <summary>
	/// 角色身体标签
	/// </summary>
	public Button label_Body;
	/// <summary>
	/// 角色表情标签
	/// </summary>
	public Button label_Face;
	/// <summary>
	/// 角色姓名的输入框
	/// </summary>
	public InputField InputFieldName;
	/// <summary>
	/// 随机名字的按钮
	/// </summary>
	public Button randomName;
	/// <summary>
	/// 进入游戏的按钮
	/// </summary>
	public Button playGame;
	/// <summary>
	/// 角色身体拖动区域
	/// </summary>
	public GameObject roleBodyFrame;
	/// <summary>
	/// 角色表情拖动区域
	/// </summary>
	public GameObject roleFaceFrame;
	/// <summary>
	/// 角色模型父物体
	/// </summary>
	public Transform roleParent;
	/// <summary>
	/// spine
	/// </summary>
	private SkeletonAnimation skeletonAnimation;
	/// <summary>
	/// 用户选择的上一个角色
	/// </summary>
	private GameObject lastRoleGameObject;
	/// <summary>
	/// 角色模型的集合
	/// </summary>
	public List<GameObject> roleGameObjects = new List<GameObject> ();
	/// <summary>
	/// 当前角色的模型名字
	/// </summary>
	public string currentRoleModelName;
	/// <summary>
	/// 当前角色的表情名字
	/// </summary>
	private string currentRoleEmotionName;
	/// <summary>
	/// 角色备选头像的集合
	/// </summary>
	public List<Sprite> roleAvatarList;

	public Button back;

	static bool clickedPlayGame = false;

	private static RoleManager _instance;
	public static RoleManager Instance {
		get { return _instance; }
		set { _instance = value; }
	}
	public void Awake ()
	{
		_instance = this;
	}
	private void Start ()
	{
		//初始化角色的昵称
		SetUserName ();
		roleFaceFrame.SetActive (false);
		//随机一个角色模型
		currentRoleEmotionName = "emotion_1";
		int index = Random.Range (0, roleGameObjects.Count);
		PageManager.Instance.gameObject.GetComponent<Canvas> ().worldCamera = Camera.main;
		SetUserRoleModel (index);
		//绑定事件
		label_Body.onClick.AddListener (OpenBodyFrame);
		label_Face.onClick.AddListener (OpenFaceFrame);
		randomName.onClick.AddListener (() => {
			InputFieldName.text = RandomUserNickName (LanguageManager.languageType);
		});
		//创建完角色之后跳转到注册界面
		playGame.onClick.AddListener (() => {
			if (clickedPlayGame == false)
			{
				AnalysisManager.Instance.OnEvent ("createAvatar", null);
				clickedPlayGame = true;
			}
			//弹出注册界面
			print (PageManager.Instance.CurrentPage.Name);
			PageManager.Instance.OpenPage<MainPage> (false);
			PageManager.Instance.CurrentPage.AddNode<RegisterNode> (true);
		});
		AudioController.PlayMusic ("BookMusic");
	}
	public void SbmitRoleInfoToServer ()
	{
		JsonData data = new JsonData ();
		data["uid"] = SelfPlayerData.Uuid;
		int modelId = RoleModelConfig.Instance.GetIdByName (currentRoleModelName);
		if (modelId != -1)
			data["modelId"] = modelId;
		int emotionId = RoleEmotionConfig.Instance.GetIdByName (currentRoleEmotionName);
		if (emotionId != -1)
			data["emotionId"] = emotionId;
		//data["nickName"] = InputFieldName.text; 
		data["nickName"] = PlayerPrefs.GetString ("user_cache_name");
		LogManager.Log (PlayerPrefs.GetString ("user_cache_name"));
		DancingWordAPI.Instance.SubmitRoleDataToServer (data, (string result) => {
			SaveRoleInforToLocal (PlayerPrefs.GetString ("user_cache_name"), modelId.ToString (), emotionId.ToString ());
			LogManager.Log ("上传用户角色信息成功！", result);
		}, () => {
			SaveRoleInforToLocal (PlayerPrefs.GetString ("user_cache_name"), modelId.ToString (), emotionId.ToString ());
		});
	}
	private void SaveRoleInforToLocal (string nickName, string modelId, string emotionId)
	{
		SelfPlayerData.Nickname = nickName;
		SelfPlayerData.ModelId = modelId;
		SelfPlayerData.EmotionId = emotionId;
		SaveDataUtils.Save<SelfPlayerData> ();
		//这时结束玩家第一次进游戏流程
		PlayerPrefs.SetString ("isFirst", "True");

		AnalysisManager.Instance.OnEvent ("enterTasteLevel", null);
		LoadSceneManager.Instance.LoadPlayScene (LevelConfig.AllLevelDic[1001]);
	}
	/// <summary>
	/// 初始化角色昵称
	/// 如果用户有昵称则使用之前的昵称 如果没有昵称则随机一个昵称
	/// </summary>
	private void SetUserName ()
	{
		if (!string.IsNullOrEmpty (SelfPlayerData.Nickname))
		{
			//print("OK");
			InputFieldName.text = SelfPlayerData.Nickname;
		}
		else
		{
			InputFieldName.text = RandomUserNickName (LanguageManager.languageType);
		}
	}
	/// <summary>
	/// 打开角色模型选择界面
	/// </summary>
	private void OpenBodyFrame ()
	{
		roleFaceFrame.SetActive (false);
		roleBodyFrame.SetActive (true);
		label_Body.GetComponent<Image> ().sprite = Resources.Load<Sprite> ("RoleCreate/UI-roleinterface-0025");
		label_Face.GetComponent<Image> ().sprite = Resources.Load<Sprite> ("RoleCreate/UI-roleinterface-0029");
	}
	/// <summary>
	/// 打开表情选择界面
	/// </summary>
	private void OpenFaceFrame ()
	{
		roleBodyFrame.SetActive (false);
		roleFaceFrame.SetActive (true);
		label_Body.GetComponent<Image> ().sprite = Resources.Load<Sprite> ("RoleCreate/UI-roleinterface-0029");
		label_Face.GetComponent<Image> ().sprite = Resources.Load<Sprite> ("RoleCreate/UI-roleinterface-0025");
	}
	/// <summary>
	/// 更换表情
	/// </summary>
	/// <param name="emotionName">Emotion name.</param>
	public void ChangeUserRoleEmotion (string emotionName)
	{
		LogManager.Log ("更换角色表情");
		currentRoleEmotionName = emotionName;
		skeletonAnimation.skeleton.SetSkin (emotionName);
	}
	/// <summary>
	/// 更换角色
	/// </summary>
	/// <param name="modelName">Model name.</param>
	public void ChangeUserRoleModel (string modelName)
	{
		LogManager.Log ("更换角色", modelName);
		int index = int.Parse (modelName.Split ('_')[1]) - 1;
		SetUserRoleModel (index);
	}
	/// <summary>
	/// 控制用户点选角色时创建不同的角色模型
	/// </summary>
	/// <param name="index">Index.</param>
	private void SetUserRoleModel (int index)
	{
		if (lastRoleGameObject != null) Destroy (lastRoleGameObject);
		GameObject role = Instantiate (roleGameObjects[index], roleParent);
		currentRoleModelName = roleGameObjects[index].name;
		lastRoleGameObject = role;
		skeletonAnimation = role.GetComponent<SkeletonAnimation> ();
		//if (!string.IsNullOrEmpty(currentRoleEmotionName))
		skeletonAnimation.skeleton.SetSkin (currentRoleEmotionName);
	}
	/// <summary>
	/// 从本地昵称库随机一个昵称
	/// </summary>
	/// <returns>The user nick name.</returns>
	public static string RandomUserNickName (LanguageType type)
	{
		if (type == LanguageType.Chinese)
		{
			string[] names = Resources.Load<TextAsset> ("Names/chinesename").text.Split ('\n');
			int index = Random.Range (0, names.Length);
			char[] TrimChar = { ' ', '-', '~', '!', '@', '#', '$', '%', '^', '&', '*', '~', '！', ',', '，', '.', '。', '\'', '、' };

			return names[index].Replace (" ", "").Replace ("-", "").Replace ("~", "").Replace ("!", "").Replace ("@", "").Replace ("#", "").Replace ("$", "").Replace ("%", "")
							   .Replace ("^", "").Replace ("&", "").Replace ("*", "").Replace ("(", "").Replace (")", "").Replace ("+", "").Replace ("=", "").Replace (";", "")
							   .Replace (":", "").Replace ("\\", "").Replace ("|", "").Replace (",", "").Replace ("\"", "").Replace ("\'", "").Replace ("/", "").Replace ("?", "")
							   .Replace ("<", "").Replace (">", "").Replace ("！", "").Replace ("￥", "").Replace ("……", "").Replace ("（", "").Replace ("）", "").Replace ("{", "").Replace ("}", "")
							   .Replace ("【", "").Replace ("】", "").Replace ("[", "").Replace ("]", "").Replace ("“", "").Replace ("‘", "").Replace ("：", "").Replace ("；", "").Replace ("，", "")
							   .Replace ("。", "").Replace ("、", "").Replace ("？", "");
		}
		else
		{
			string[] name1 = Resources.Load<TextAsset> ("Names/postNames").text.Split ('\n');
			int index1 = Random.Range (0, name1.Length);
			string[] name2 = Resources.Load<TextAsset> ("Names/preNames").text.Split ('\n');
			int index2 = Random.Range (0, name2.Length);
			return name1[index1] + " " + name2[index2];
		}
	}
}
