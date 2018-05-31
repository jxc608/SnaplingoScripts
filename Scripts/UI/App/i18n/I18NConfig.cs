using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

class I18NConfig
{
	/**--------------------- Key值要和Excel中的i18n下面的Country相同  ---------------------**/
	/**--------------------- 在服务器端存放的Country为Int值,这里记录下来,在i18n中支持直接使用服务器端的int值查找  -------------**/

	/** 中国  */
	public const string Country_local_CN = "zh_CN";
	public const int Country_Server_CN = 0;

	/** 美国  */
	public const string Country_local_US = "en_US";
	public const int Country_Server_US = 1;

	/** 加拿大  */
	public const string Country_local_CA = "en_CA";
	public const int Country_Server_CA = 2;

	/** 英国  */
	public const string Country_local_GB = "en_GB";
	public const int Country_Server_GB = 3;

	/** 澳大利亚  */
	public const string Country_local_AU = "en_AU";
	public const int Country_Server_AU = 4;

	/** 新加坡  */
	public const string Country_local_SG = "en_SG";
	public const int Country_Server_SG = 5;

	/** 香港  */
	public const string Country_local_HK = "en_HK";
	public const int Country_Server_HK = 6;

	/** 爱尔兰  */
	public const string Country_local_IE = "en_IE";
	public const int Country_Server_IE = 7;

	/** 以色列  */
	public const string Country_local_IL = "en_IL";
	public const int Country_Server_IL = 8;

	/** 牙买加  */
	public const string Country_local_JM = "en_JM";
	public const int Country_Server_JM = 9;

	/** 新西兰  */
	public const string Country_local_NZ = "en_NZ";
	public const int Country_Server_NZ = 10;

	/** 南非  */
	public const string Country_local_ZA = "en_ZA";
	public const int Country_Server_ZA = 11;

	/** 英联邦  */
	public const string Country_local_CR = "en_CR";
	public const int Country_Server_CR = 12;

	/** 欧盟  */
	public const string Country_local_EU = "en_EU";
	public const int Country_Server_EU = 13;

	/** 默认  */
	public const String Country_local_Default = Country_local_US;

	///** 根据Country Int值 获取 Country String值 */
	//public static Dictionary<int, string> CityDictionary = new Dictionary<int, string>{
	//	{ I18NConfig.Country_Server_CN, I18NConfig.Country_local_CN },
	//	{ I18NConfig.Country_Server_US, I18NConfig.Country_local_US },
	//	{ I18NConfig.Country_Server_CA, I18NConfig.Country_local_CA },
	//	{ I18NConfig.Country_Server_GB, I18NConfig.Country_local_GB },
	//	{ I18NConfig.Country_Server_AU, I18NConfig.Country_local_AU },
	//	{ I18NConfig.Country_Server_SG, I18NConfig.Country_local_SG },
	//	{ I18NConfig.Country_Server_HK, I18NConfig.Country_local_HK },
	//	{ I18NConfig.Country_Server_IE, I18NConfig.Country_local_IE },
	//	{ I18NConfig.Country_Server_IL, I18NConfig.Country_local_IL },
	//	{ I18NConfig.Country_Server_NZ, I18NConfig.Country_local_NZ },
	//	{ I18NConfig.Country_Server_ZA, I18NConfig.Country_local_ZA },
	//	{ I18NConfig.Country_Server_CR, I18NConfig.Country_local_CR },
	//	{ I18NConfig.Country_Server_EU, I18NConfig.Country_local_EU },
	//	};

	public static string getCityFlagByInt(int _cityInt)
	{
		switch (_cityInt)
		{
			case Country_Server_CN:
				return Country_local_CN;
			case Country_Server_US:
				return Country_local_US;
			case Country_Server_CA:
				return Country_local_CA;
			case Country_Server_GB:
				return Country_local_GB;
			case Country_Server_AU:
				return Country_local_AU;
			case Country_Server_SG:
				return Country_local_SG;
			case Country_Server_HK:
				return Country_local_HK;
			case Country_Server_IE:
				return Country_local_IE;
			case Country_Server_IL:
				return Country_local_IL;
			case Country_Server_NZ:
				return Country_local_NZ;
			case Country_Server_ZA:
				return Country_local_ZA;
			case Country_Server_CR:
				return Country_local_CR;
			case Country_Server_EU:
				return Country_local_EU;
		}

		return Country_local_CN;
	}

}
