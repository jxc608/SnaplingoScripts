using System;

class GlobalConst {

	/** 用户ID **/
	public static string Player_ID = null;

	/** 用户临时ID **/
	public static string Player_IDTemp = null;

	/** 用户是否是中国 **/
	public static bool Player_IsChina = true;

    /** 当前服务器的时间 **/
    public static double HttpServerResponseTime = 0;

    /** 用户是否登陆了APP **/
    public static bool LoginToApp = false;

    /** app启动运行时间，单位秒 **/
    private static double AppLunchTime = 0;

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public static double CurrentServerTime
    {
        set
        {
            AppLunchTime = SystemTime.timeSinceLaunch;
            HttpServerResponseTime = (HttpServerResponseTime == 0) ? SnapHttpManager.GetTimeStamp() : value;
        }
        get
        {
            //转换成毫秒
            string cur_time_str = (SystemTime.timeSinceLaunch - AppLunchTime) + "";
            double cur_time = double.Parse(cur_time_str) + HttpServerResponseTime;
            return Math.Round(cur_time, 0) ;
        }
    }

}