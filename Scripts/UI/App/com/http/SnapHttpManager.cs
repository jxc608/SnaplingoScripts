using System;
using System.Collections;
using System.Collections.Generic;
using System.Timers;
using LitJson;
using UnityEngine;

public class SnapHttpManager
{

	//请求的索引ID
	private int requestIndex = 0;
	//初始化几个loader
	const int initLoaderNum = 4;
	//加载器的数组
	private List<SnapHttpRequest> requestItemList = null;
	//正在请求VO的字典
	private Dictionary<int, SnapRequestVO> requestingDic = null;
	//协议的VO数组
	public List<SnapRequestVO> requestVOList = null;
	//timer计时器
	private Timer timer;

    private static List<SnapHttpCallBack> callBackList = null;

	private static SnapHttpManager instance = null;

	public static SnapHttpManager getInstance()
	{
		if (instance == null)
		{
			instance = new SnapHttpManager();
		}
		return instance;
	}

	public SnapHttpManager()
	{
		if (instance != null)
		{
			//new 了一次，扔个出始化异常
			throw new Exception("SnapHttpManager被重复出始化");
		}
        initRequestURL();
		initParams();
		initHttpRequests();
	}

    double last_time;
    public void update()
    {
        double current_time = SystemTime.timeSinceLaunch;
        if (current_time - last_time < 0.15)
        {
            return;
        }
        last_time = current_time;
        if (callBackList.Count > 0)
        {
            SnapHttpCallBack callBack = callBackList[0];
            callBackList.RemoveAt(0);
			if(callBack!=null)
            callBack.SendAction();
        }
    }
    
    public void AddOneHttpCallBack(Action<SnapRpcDataVO> action, SnapRpcDataVO data)
    {
        SnapHttpCallBack http = new SnapHttpCallBack();
        http.actionCallBack = action;
        http.dataVO = data;
        callBackList.Add(http);
    }

    private void initRequestURL()
    {
        int index = 0;
        if (DebugConfigController.Instance.FormalData)
        {
            // 正式服
            index = 1;
        }
        string[] httpHostURLS = ConfigurationController.Instance.HttpHostList;
        string urlHeader = index == 0 ? "http" : "https";
        SnapHttpConfig.NET_APP_URL = urlHeader + "://" + httpHostURLS[index] + "/api/game_app/v2/";
    }

	public void Request_SnapAppApi(SnapAppApiInterface requestInterType, string apiName, 
        string requestMethod, JsonData jsonData = null, 
        Action<SnapRpcDataVO> callBackAction = null, int priority = 0,
        SnapAppApiThirdRequestFace third = SnapAppApiThirdRequestFace.No_Third)
	{
		createRequestVO(requestInterType, apiName, requestMethod, jsonData, callBackAction, priority, third);
		sendHttpRequest();
	}

	/// <summary>
	/// 重新加载，外部请不要调用这个方法，以防出错
	/// </summary>
	public void Re_Request_SnapAppApi_Inner(SnapRequestVO vo)
	{
		if (vo == null)
		{
			return;
		}
		if (vo.requestPriority == int.MaxValue)
		{
			requestVOList.Remove(vo);
			requestVOList.Insert(0, vo);
        }
        else
        {
            requestVOList.Add(vo);
        }
		//sendHttpRequest();
	}

	//检测http是否有未用的通道
	private void sendHttpRequest()
	{
		for (int i = 0; i < requestItemList.Count; i++)
		{
			if (!requestItemList[i].isInRequesting)
			{
				loadHttpRequest(i);
				break;
			}
		}
	}

	//开始进行加载处理
	private void loadHttpRequest(int id)
	{
		if (requestVOList.Count > 0 && id < requestItemList.Count && (requestItemList[id] != null))
		{
			if (requestItemList[id] != null)
			{
				requestItemList[id].close();
			}
			SnapRequestVO vo = requestVOList[0];
			requestVOList.RemoveAt(0);
			vo.requestStartTime = GetTimeStamp();
			vo.loaderId = id;
			requestItemList[id].startLoader(vo);
		}
	}

	//创建请求vo
	public SnapRequestVO createRequestVO(SnapAppApiInterface requestInterTypestring, string apiName, 
        string requestMethod, JsonData jsonData = null, Action<SnapRpcDataVO> callBackAction = null, 
        int priority = 0, SnapAppApiThirdRequestFace third = SnapAppApiThirdRequestFace.No_Third)
	{
		SnapRequestVO requestVO = new SnapRequestVO(jsonData, callBackAction);
		requestVO.requestActionId = requestInterTypestring;
		requestVO.requestAction = apiName;
		requestVO.requestMethod = requestMethod;
		requestVO.requestId = ++requestIndex;
		requestVO.requestPriority = priority;
        requestVO.thirdType = third;

        if (jsonData != null)
        {
            requestVO.urlTypeExt = jsonData.TryGetString("urlTypeExt", "?");
        }

        if (priority == int.MaxValue)
		{
			requestVOList.Insert(0, requestVO);
		}
		else
		{
			requestVOList.Add(requestVO);
		}
		return requestVO;
	}

	//初始化必须参数
	private void initParams()
	{
        callBackList = new List<SnapHttpCallBack>();
        requestVOList = new List<SnapRequestVO>();
		requestingDic = new Dictionary<int, SnapRequestVO>();
		requestItemList = new List<SnapHttpRequest>();
		//定时器先这么设置，后期有需要再次修改
		startTimer();
	}

	//开启定时器
	private void startTimer()
	{
		timer = new Timer(1000);
		timer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
		timer.AutoReset = true;
		timer.Enabled = true;
	}

	//指定Timer触发的事件
	private void OnTimedEvent(object source, ElapsedEventArgs e)
	{
		//表示还有需要发送请求的vo
		if (requestVOList.Count > 0)
		{
			sendHttpRequest();
		}
	}

	//初始化http请求
	private void initHttpRequests()
	{
		for (int i = 0; i < initLoaderNum; i++)
		{
			SnapHttpRequest requestItem = new SnapHttpRequest();
			requestItem.httpRequestId = i;
			requestItemList.Add(requestItem);
		}
	}

	//获取unix时间戳
	public static int GetTimeStamp()
	{
		TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
		return Convert.ToInt32(ts.TotalSeconds);
	}

}