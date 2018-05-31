using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

class UserVoBasic
{

	public static int GENDER_GIRL = 2;
	public static int GENDER_BOY = 1;

	/** 是否是注册用户 **/
	public bool UserIsRegister = false;

	/**--------------------- 注册过的用户数据  ---------------------**/

	/** 用户ID **/
	public string UserID;

	/** 用户名字 **/
	public string UserName;

	/** 用户年龄 **/
	public int UserAge;

	/** 用户性别 **/
	public int UserGender;

	/** 用户国家 **/
	public string UserCity;
	/** 用户是否修改过国家 **/
	public bool UserCityIsChange;

	/** 用户国家 Int **/
	public int UserCountry;

	/** 用户头像 **/
	public string UserIconUrl = "NoAvatar";

	/** 用户自我介绍 **/
	public string UserAboutMe;

	/** 用户游戏经验值 **/
	public int UserExp;

	/** 用户游戏等级 **/
	public int UserLevel;

	/** 用户游戏体力 **/
	public int UserEnergy;

	/** 用户游戏血量 **/
	public int UserHP = 99;

	///** 用户是否是老师 **/
	//public bool UserIsTeacher;

	///** 用户是否是双语明星 **/
	//public bool UserIsLanguageStar;

	///** 用户是否在线 **/
	//public bool UserIsOnline;

	///** 用户上课是否需要录制 **/
	//public bool UserTeachIsRecord;

	///** 用户上课教材卡是否需要看到中文 **/
	//public bool UserTeachCardIsShowCN;

	///** 用户是否是国内(服务器根据ip判断) **/
	//public bool UserIsChinaLocation;

	/**--------------------- 临时用户的用户数据  ---------------------**/
	/** 用户临时ID **/
	public string TempUserID;

	///** 用户名字 **/
	//public string TempUserName;

	///** 用户年龄 **/
	//public int TempUserAge;

	///** 用户性别 **/
	//public int TempUserGender;

	///** 用户国家 **/
	//public int TempUserCountry;

	///** 用户头像 **/
	//public string TempUserIconUrl;

}
