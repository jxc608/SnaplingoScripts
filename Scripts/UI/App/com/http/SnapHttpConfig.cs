using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public enum HttpErrorCode
{
	Success = 0,

    LoginTokenError = 102,

}

public class SnapHttpConfig
{
	//请求的url
	public static string NET_APP_URL = "http://qa-api.game.snaplingo.com/api/game_app/v2/";
	//设置请求内容二进制类型
	public static string NET_CONTENT_TYPE_URLENCODED = "application/x-www-form-urlencoded; charset=UTF-8";
	//设置请求内容json类型
	public static string NET_CONTENT_TYPE_JSON = "application/json";
	//设置请求内容类型
	public static string NET_CONTENT_TYPE = NET_CONTENT_TYPE_JSON;
	//设置请求GET
	public const string NET_REQUEST_GET = "GET";
	//设置请求POST
	public const string NET_REQUEST_POST = "POST";
    //设置请求PUT
    public const string NET_REQUEST_PUT = "PUT";
    //设置请求方式
    public static string NET_REQUEST_METHOD = NET_REQUEST_POST;

	//请求错误，重试的次数
	public static int NET_RETRY_NUM = 3;
	//设置超时时间
	public static int NET_TIMEOUT = 10000;

	//服务token
	public static string NOT_LOGINED_APP_TOKEN
	{
		get 
		{
			if(string.IsNullOrEmpty(m_notLogindAppToken))
			{
				m_notLogindAppToken = ConfigurationController.Instance.HttpToken;
			}
			return m_notLogindAppToken;
		}
	}
	private static string m_notLogindAppToken = "";


    //登陆成功后的token
    public static string LOGINED_APP_TOKEN = "";


    //登陆成功后的token
    //public static string REQUEST_APP_API_TOKEN = "";

}
