using LitJson;
using Snaplingo.SaveData;
using System.Collections.Generic;
using UnityEngine;

public class SelfPlayerData : ISaveData
{
	public static SelfPlayerData Instance {
		get { return SaveDataUtils.GetSaveData<SelfPlayerData> (); }
	}

	SelfPlayerLevelData _playerLevelData;
	public static SelfPlayerLevelData PlayerLevelData {
		get { return Instance._playerLevelData; }
		set { Instance._playerLevelData = value; }
	}

	/**--------------------- 游戏用户数据  ---------------------**/
	string _deviceID = "";
	public static string DeviceID {
		get { return Instance._deviceID; }
		set { Instance._deviceID = value; }
	}

	string _uuid = "";
	public static string Uuid {
		get { return Instance._uuid; }
		set { Instance._uuid = value; }
	}

	string _nickname = "";
	public static string Nickname {
		get { return Instance._nickname; }
		set {
			Instance._nickname = value;
		}
	}
	string _modelId = "";
	public static string ModelId{
		get { return Instance._modelId; }
		set{
			Instance._modelId = value;
		}
	}
	string _emotionId = "";
	public static string EmotionId{
		get { return Instance._emotionId; }
		set{
			Instance._emotionId = value;
		}
	}
	string _userName = "";
	public static string UserName {
		get { return Instance._userName; }
		set { Instance._userName = value; }
	}

	string _password = "";
	public static string Password {
		get { return Instance._password; }
		set { Instance._password = value; }
	}

	string _telphoneNum = "";
	public static string TelphoneNum {
		get { return Instance._telphoneNum; }
		set { Instance._telphoneNum = value; }
	}

	//string _server = "";
	//public string Server { get { return _server; } set { _server = value; } }

	//string _sessionId = "";
	//public string SessionId { get { return _sessionId; } set { _sessionId = value; } }


	bool _newplayer = true;
	public static bool NewPlayer {
		get { return Instance._newplayer; }
		set { Instance._newplayer = value; }
	}

	int _level = int.Parse (PlayerInfoSetting.Instance.m_InitLevel);
	public static int Level {
		get { return Instance._level; }
		set { Instance._level = value; }
	}

	int _energy = int.Parse (PlayerInfoSetting.Instance.m_InitEnergy);
	public static int Energy {
		get { return Instance._energy; }
		set { Instance._energy = value; }
	}

	public int _hp = 0;
	public static int hp {
		get { return Instance._hp; }
		set { Instance._hp = value; }
	}

	int _experience = int.Parse (PlayerInfoSetting.Instance.m_InitExperience);
	public static int Experience {
		get { return Instance._experience; }
		set { Instance._experience = value; }
	}

	bool _firstTimeBossCG = false;
	public static bool FirstTimeBossCG {
		get { return Instance._firstTimeBossCG; }
		set { Instance._firstTimeBossCG = value; }
	}

	string _avatarUrl = "NoAvatar";
	public static string AvatarUrl {
		get { return Instance._avatarUrl; }
		set { Instance._avatarUrl = value; }
	}
    //0=女
	public int _sex = 0;
	public static int Sex {
		get { return Instance._sex; }
		set { Instance._sex = value; }
	}
	public int _age = 0;
	public static int Age {
		get { return Instance._age; }
		set { Instance._age = value; }
	}
	public string _appUID = "";
	public static string AppUID {
		get { return Instance._appUID; }
		set { Instance._appUID = value; }
	}
    //具体国家代码从I18NConfig查
	public int _country = 0;
	public static int Country {
		get { return Instance._country; }
		set { Instance._country = value; }
	}
	public string _profile = "";
	public static string Profile {
		get { return Instance._profile; }
		set { Instance._profile = value; }
	}
    /// <summary>
	/// 玩家地区(省份)
    /// </summary>
	public string _province = "";
	public static string Province{
		get { return Instance._province; }
		set { Instance._province = value; }
	}
    /// <summary>
	/// 玩家地区(城市)
    /// </summary>
	public string _city = "";
	public static string City{
		get { return Instance._city; }
		set { Instance._city = value; }
	}
	/// <summary>
	/// 本地课程数据结构
	/// </summary>
	List<CourseData> _courseData = new List<CourseData> ();
	public static List<CourseData> CourseData {
		get { return Instance._courseData; }
		set { Instance._courseData = value; }
	}
	/// <summary>
	/// 所有关卡的玩过未玩过 和 解锁未解锁的状态
	/// </summary>
	private Dictionary<int, LevelStatus> _levelStatus = new Dictionary<int, LevelStatus> ();
	public static Dictionary<int, LevelStatus> LevelStatus {
		get { return Instance._levelStatus; }
		set { Instance._levelStatus = value; }
	}
	/// <summary>
	/// 通过关卡的Id返回关卡的状态
	/// </summary>
	/// <returns>The level status by level identifier.</returns>
	/// <param name="levelId">Level identifier.</param>
	public LevelStatus GetLevelStatusByLevelId (int levelId)
	{
		//LevelStatus status = new LevelStatus();
		foreach (KeyValuePair<int, LevelStatus> key in LevelStatus)
		{
			if (key.Key == levelId)
			{
				return key.Value;
			}
		}
		return new LevelStatus {
			isPay = false,
			isUnLock = false
		};
	}
	/// <summary>
	/// 创建当前用户的所有关卡的状态信息的字典集合
	/// </summary>
	/// <returns>The level status dic.</returns>
	public Dictionary<int, LevelStatus> CreateLevelStatusDic ()
	{
		List<CourseData> courses = SelfPlayerData.CourseData;
		Dictionary<int, LevelStatus> resultDic = new Dictionary<int, LevelStatus> ();
		for (int i = 0; i < courses.Count; i++)
		{
			List<int> levels = CourseConfig.Instance.GetLevels (courses[i].m_CourseID);
			for (int j = 0; j < levels.Count; j++)
			{
				LevelStatus levelStatus = new LevelStatus ();
				levelStatus.isPay = courses[i].m_IsPay;
				levelStatus.isUnLock = IsLevelCanPlay (j, courses[i].m_Progress);
				resultDic.Add (levels[j], levelStatus);
			}
		}
		return resultDic;
	}
	/// <summary>
	/// 判断当前关卡是否可以玩
	/// </summary>
	/// <returns><c>true</c>, if level can play was ised, <c>false</c> otherwise.</returns>
	/// <param name="startLevelIndex">Start level index.</param>
	/// <param name="endLevelIndex">End level index.</param>
	private bool IsLevelCanPlay (int startLevelIndex, int endLevelIndex)
	{
		return startLevelIndex <= endLevelIndex ? true : false;
	}
	/// <summary>
	/// 更新课程中关卡的状态
	/// </summary>
	/// <param name="levelId">Level identifier.</param>
	public void SetLevelStatus (int levelId)
	{
		LogManager.Log ("更新关卡的状态：" , levelId , CourseConfig.Instance);
		int courseId = CourseConfig.Instance.GetCourseIdFromLevelId (levelId);
		List<CourseData> courses = SelfPlayerData.CourseData;
		for (int i = 0; i < courses.Count; i++)
		{
			if (courses[i].m_CourseID == courseId)
			{
				courses[i].m_Progress++;
				SaveDataUtils.Save<SelfPlayerData> ();
				UpdateCourse (courses[i]);
				break;
			}
		}
	}
	public int GetMaxLevelId ()
	{
		int maxlevelId = -999999;
		for (int i = 0; i < CourseData.Count; i++)
		{
			if (CourseData[i].m_IsPay)
			{
				maxlevelId = Mathf.Max (maxlevelId, CourseConfig.Instance.GetLevels (CourseData[i].m_CourseID)[0] + CourseData[i].m_Progress);
			}
		}
		return maxlevelId;
	}
	/// <summary>
	/// 更新课程中的关卡的
	/// </summary>
	/// <param name="course">Course.</param>
	private void UpdateCourse (CourseData course)
	{
		JsonData data = new JsonData ();
		data["userID"] = SelfPlayerData.Uuid;
		data["courseID"] = course.m_CourseID;
		data["progress"] = course.m_Progress;
		DancingWordAPI.Instance.SubmitCourseToServer (data, (string result) => {
			LevelStatus = CreateLevelStatusDic ();
		}, null);
	}
	//public int _levelStatus = 1000;
	//public static int LevelStatus
	//{
	//	get { return Instance._levelStatus; }
	//	set { Instance._levelStatus = value; }
	//}
	/**--------------------- Json 游戏用户数据  ---------------------**/

	/// <summary>
	/// 是否登陆到app
	/// </summary>
	/// <returns></returns>
	public static bool loginToApp ()
	{
		return GlobalConst.LoginToApp;
	}

	public string SaveTag ()
	{
		//return "PlayerInfo";
		return GetUserFileName ();
	}

	public string GetUserFileName ()
	{
		string fileName = "UserData_NotLogin";
		if (GlobalConst.LoginToApp)
		{
			//fileName = _deviceID + "_" + GlobalConst.Player_ID;
			fileName = GlobalConst.Player_ID;
		}
		else
		{
			if (_deviceID != "")
			{
				fileName = _deviceID + "_NotLogin";
			}
			else
			{
				string did = PlayerPrefs.GetString ("DeviceID", "");
				if (!string.IsNullOrEmpty (did))
				{
					fileName = did + "_NotLogin";
				}
			}
		}
		LogManager.Log ("GetUserFileName:::" , fileName);
		return fileName;
	}

	public string SaveAsJson ()
	{
		JsonData data = new JsonData ();
		data["uuid"] = string.IsNullOrEmpty (Uuid) ? "" : Uuid;
		data["nickname"] = string.IsNullOrEmpty (Nickname) ? "" : Nickname;
		data["level"] = Level;
		data["energy"] = Energy;
		data["hp"] = hp;
		data["experience"] = Experience;
		data["firstTimeBossCG"] = _firstTimeBossCG ? 1 : 0;
		data["newplayer"] = NewPlayer;
		data["avatarUrl"] = string.IsNullOrEmpty (AvatarUrl) ? "NoAvatar" : AvatarUrl;

		data["userName"] = UserName;
		data["password"] = Password;
		data["telphoneNum"] = TelphoneNum;
		data["deviceID"] = DeviceID;

		data["sex"] = Sex;
		data["age"] = Age;
		data["appUID"] = AppUID;
		data["country"] = Country;
		data["profile"] = Profile;

		//data["levelStatus"] = LevelStatus;

		data["playerLevelData"] = JsonUtility.ToJson (SelfPlayerLevelData.Instance);

		//corseData
		JsonData courseData = new JsonData ();
		if (_courseData.Count > 0)
		{
			for (int i = 0; i < _courseData.Count; i++)
			{
				JsonData newCourse = new JsonData ();
				newCourse["courseID"] = _courseData[i].m_CourseID;
				newCourse["progress"] = _courseData[i].m_Progress;
				newCourse["isPay"] = _courseData[i].m_IsPay;
				courseData.Add (newCourse);
			}
			data["courseData"] = courseData;
		}
		//coreseDataEnd

		LogManager.LogWarning (" >> SaveAsJson = " , data.ToJson ());

		return data.ToJson ();
	}

	public void LoadFromJson (string json)
	{
		LogManager.LogWarning ("LoadFromJson = " , json);

		JsonData data = JsonMapper.ToObject (json);
		_uuid = data.TryGetString ("uuid");
		_nickname = data.TryGetString ("nickname");
		_level = int.Parse (data.TryGetString ("level", PlayerInfoSetting.Instance.m_InitLevel));
		_energy = int.Parse (data.TryGetString ("energy", PlayerInfoSetting.Instance.m_InitEnergy));
		_hp = int.Parse (data.TryGetString ("hp", PlayerInfoSetting.Instance.m_InitHP));
		_experience = int.Parse (data.TryGetString ("experience", PlayerInfoSetting.Instance.m_InitExperience));
		_firstTimeBossCG = int.Parse (data.TryGetString ("firstTimeBossCG", "0")) == 1 ? true : false;
		_avatarUrl = data.TryGetString ("avatarUrl");
		string newPlayer = data.TryGetString ("newplayer", "true");
		if (newPlayer == "False" || newPlayer == "false")
			_newplayer = false;
		else
			_newplayer = true;

		_userName = data.TryGetString ("userName");
		_password = data.TryGetString ("password");
		_telphoneNum = data.TryGetString ("telphoneNum");
		_deviceID = data.TryGetString ("deviceID");

		_sex = int.Parse (data.TryGetString ("sex", "0"));
		_age = int.Parse (data.TryGetString ("age", "0"));
		_appUID = data.TryGetString ("age", "0");
		_country = int.Parse (data.TryGetString ("country", "0"));
		_profile = data.TryGetString ("profile", "0");
		//_levelStatus = int.Parse(data.TryGetString("levelStatus", "1000"));


		_playerLevelData = JsonUtility.FromJson<SelfPlayerLevelData> (data["playerLevelData"].ToString ());

		//corseData
		if (data.Keys.Contains ("courseData"))
		{
			JsonData corseData = data["courseData"];
			_courseData.Clear ();
			for (int i = 0; i < corseData.Count; i++)
			{
				CourseData newCorse = new CourseData ();
				newCorse.m_CourseID = int.Parse (corseData[i].TryGetString ("courseID"));
				newCorse.m_Progress = int.Parse (corseData[i].TryGetString ("progress"));
				newCorse.m_IsPay = bool.Parse (corseData[i].TryGetString ("isPay"));
				_courseData.Add (newCorse);
			}
			//LogManager.Log(corseData.Count);
		}
		//else
		//{//初始化
		//    //临时假数据
		//    //CorseData corse1 = new CorseData();
		//    //corse1.m_CorseID = 1;
		//    //corse1.m_LevelNumber = 20;
		//    //corse1.m_CurrentPayLevel = 1004;
		//    //corse1.m_CurrentUnlockLevel = 1004;

		//    //CorseData corse2 = new CorseData();
		//    //corse2.m_CorseID = 2;
		//    //corse2.m_LevelNumber = 12;
		//    //corse2.m_CurrentPayLevel = 0;
		//    //corse2.m_CurrentUnlockLevel = 0;

		//    //_corseData.Add(corse1);
		//    //_corseData.Add(corse2);
		//}

		//corseData End

		//LogManager.Log(data["playerLevelData"].ToJson());

	}
	public void Refresh (string uuid, string nickname, int level = 0, int energy = 0, int experience = 0)
	{
		if (!string.IsNullOrEmpty (uuid))
		{
			_uuid = uuid;
		}
		if (!string.IsNullOrEmpty (nickname))
		{
			_nickname = nickname;
		}
		_level = level;
		_energy = energy;
		_experience = experience;

		SaveDataUtils.Save<SelfPlayerData> ();
	}

	public void save ()
	{
		SaveDataUtils.Save<SelfPlayerData> ();
	}

	//游戏结束时加经验, 传进来对应LevelGroup的下标
	public void AddExpAndSaveToLocal (int levelIndex)
	{
		int expBase = LevelConfig.AllLevelDic[StaticData.LevelID].exp;
		int exp = (int)Mathf.Floor (expBase * StaticData.LevelExpBase[levelIndex]);

		_experience = _experience + exp;
		LogManager.Log ("给用户加" , exp , "经验");


		ExpBasic expItem = ExpConfig.Instance.getExpInfo (_experience);

		if (expItem.level > _level)
		{
			_level = expItem.level;
			PromptManager.Instance.MessageBox (PromptManager.Type.FloatingTip, "升到" + _level + "级啦~~~");
			LogManager.Log ("升到" , _level , "级啦~~~");
		}
		save ();

	}
}