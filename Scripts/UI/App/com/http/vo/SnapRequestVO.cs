using System;
using LitJson;
using System.Net;
using System.Collections;

public class SnapRequestVO
{

	//当前处于某个loader
	public int loaderId = 0;
	//http请求的索引
	public int requestId = 0;
	//http请求的action索引
	public SnapAppApiInterface requestActionId;
	//http请求的action
	public string requestAction = "";
	//http请求发送的参数
	public JsonData requestObj = null;
	//http请求的优先级，数字越高优先级越大，其实就是个先发与后发的支撑，暂时不用
	public int requestPriority = 0;
	//http请求的回调事件名称
	public Action<SnapRpcDataVO> requestBackAction = null;
	//加载失败的次数
	public int requestErrCount = SnapHttpConfig.NET_RETRY_NUM;

	//加载方式,默认为post
	public string requestMethod = SnapHttpConfig.NET_REQUEST_POST;

    //开始请求的时间
    public int requestStartTime = 0;

    //是否是第三方请求
    public SnapAppApiThirdRequestFace thirdType = SnapAppApiThirdRequestFace.No_Third;

    //是否是第三方请求
    public string urlTypeExt = "?";

    public byte[] mp3byte;

    public SnapRequestVO(JsonData dataVO = null, Action<SnapRpcDataVO> callbackAction = null, string requestAction = "")
	{
		this.requestObj = dataVO;
		this.requestBackAction = callbackAction;
		this.requestAction = requestAction;     
    }

	//转换成json字符串
	public string toJSONString()
	{
		if (requestObj == null)
		{
			return "";
		}
		if (requestMethod == SnapHttpConfig.NET_REQUEST_POST || requestMethod == SnapHttpConfig.NET_REQUEST_PUT)
		{
			return requestObj.ToJson();
		}
        else if (requestMethod == SnapHttpConfig.NET_REQUEST_GET)
        {
            string paramStr = urlTypeExt;
            if (requestObj != null)
            {
                if (((IDictionary)requestObj).Contains("urlTypeExt"))
                {
                    ((IDictionary)requestObj).Remove("urlTypeExt");
                }
                
                foreach (var item in requestObj.Keys)
                {
                    if (urlTypeExt.Equals("/"))
                    {
                        paramStr += requestObj[item] + "/";
                    }
                    else
                    {
                        paramStr += (item + "=" + requestObj[item] + "&");
                    }                    
                }
            }
            paramStr = paramStr.Substring(0, paramStr.Length - 1);
            return paramStr;
        }
        return "";
	}

	//转换成二进制
	public Byte[] toByte()
	{
        if (mp3byte != null)
        {
            return mp3byte;
        }
        if (((IDictionary)requestObj).Contains("byte_data"))
        {
            string str = requestObj["byte_data"].ToString();
            return System.Text.Encoding.Default.GetBytes(str);
        }
        return System.Text.Encoding.Default.GetBytes(requestObj.ToJson());
	}


}
