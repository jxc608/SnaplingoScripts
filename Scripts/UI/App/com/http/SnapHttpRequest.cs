using System;
using System.IO;
using System.Net;
using LitJson;
using UnityEngine;
using System.Collections;
using System.Reflection;

public class SnapHttpRequest
{
    public SnapRequestVO httpRequestVO;
    //是否需要重新加载
    private Boolean needReload = false;
	//是否在请求中
	public Boolean isInRequesting = false;
	private HttpWebRequest request;
	private HttpWebResponse response;
    //加载器的id
    public int httpRequestId = 0;

    /**
	 *开始加载http协议，配置加载参数 
	 * @param vo
	 * 
	 */
    public void startLoader(SnapRequestVO vo, Boolean reload = false)
	{
		if (vo == null || isInRequesting)
		{
			Console.WriteLine(httpRequestId + "正在使用中...");
			return;
		}

		//System.GC.Collect();

		needReload = reload;
		isInRequesting = true;
                
        try
		{
            switch (vo.requestMethod)
            {
                case SnapHttpConfig.NET_REQUEST_GET:
                    request = GetRequest(vo);
                    break;
                case SnapHttpConfig.NET_REQUEST_POST:
                    request = PostRequest(vo);
                    break;
                case SnapHttpConfig.NET_REQUEST_PUT:
                    request = PutRequest(vo);
                    break;
                default:
                    break;
            }

            httpRequestVO = vo;
			LogManager.Log(" - Url:" , request.RequestUri);
            request.BeginGetResponse(new AsyncCallback(GetHttpResponse), request);

            //GetHttpResponse();
        }
		catch (WebException e)
		{
			LogManager.Log("http WebException:::" , e.Message);
            string find_str = "error: (";
            int index = e.Message.IndexOf(find_str);
            if (index != -1)
            {
                string errorCode = e.Message.Substring(index, find_str.Length + 3);
                errorCode = errorCode.Replace(find_str, "");
                //全局异常
                if (SnapAppApi.RequestErrorCodeBack != null)
                {
                    SnapAppApi.RequestErrorCodeBack.Invoke(int.Parse(errorCode));
                }
            }
            close();
		}
	}
    
    /// <summary>
    /// 获取http请求返回的数据
    /// </summary>
    private void GetHttpResponse(IAsyncResult result)
    {
        HttpWebRequest req = (HttpWebRequest)result.AsyncState;
        using (response = (HttpWebResponse)req.GetResponse())
        {
            GetServerTime(response.Headers);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                {
                    JsonData jsonData = JsonMapper.ToObject(new JsonReader(reader.ReadToEnd()));
                    SnapRpcDataVO rpcData = new SnapRpcDataVO();
                    if (jsonData != null)
                    {
                        rpcData.action = httpRequestVO.requestAction;
                        rpcData.data = jsonData["data"];
                        rpcData.status = (bool)jsonData["status"];
                        if (rpcData.status == true)
                        {
                            rpcData.code = 1;
                        }
                        else
                        {
                            rpcData.code = int.Parse(jsonData.TryGetString("code"));
                        }
                        rpcData.message = jsonData["message"].ToString();
                    }
                    close();
					LogManager.LogWarning(" - Result:" , JsonMapper.ToJson(jsonData));
					CheckResponseError(rpcData);
                    //if (httpRequestVO.requestBackAction != null)
                    //{
                    //    httpRequestVO.requestBackAction.Invoke(rpcData);
                    //    httpRequestVO.requestBackAction = null;
                    //}
                    
                }
            }
            else
            {
				LogManager.Log(response.StatusCode);
                close();
                if (response.StatusCode == HttpStatusCode.RequestTimeout
                    || response.StatusCode == HttpStatusCode.GatewayTimeout)
                {
                    //需要重新请求的错误异常
                    if (needReload)
                    {
						LogManager.Log("reconnect");
                        if (httpRequestVO.requestErrCount < SnapHttpConfig.NET_RETRY_NUM)
                        {
                            httpRequestVO.requestErrCount++;
                            SnapHttpManager.getInstance().Re_Request_SnapAppApi_Inner(httpRequestVO);
                        }
                    }
                }
                else
                {
                    //全局异常
                    if (SnapAppApi.RequestErrorCodeBack != null)
                    {
                        SnapAppApi.RequestErrorCodeBack.Invoke((int)response.StatusCode);
                    }
                }
            }
        }
    }

	private void CheckResponseError(SnapRpcDataVO rpcData)
	{
		if(!rpcData.status)
		{
			LogManager.LogError("[HttpRequest]Check Response Error! error code: "
			               , rpcData.code , ", err message: " , rpcData.message);

			if(rpcData.code == (int)HttpErrorCode.LoginTokenError)
			{
				GameManager.LoginOnOtherDivices(false);
			}

		}

		SnapHttpManager.getInstance().AddOneHttpCallBack(httpRequestVO.requestBackAction, rpcData);
	}

    /// <summary>
    /// 获取服务器时间
    /// </summary>
    /// <param name="headers"></param>
    private void GetServerTime(WebHeaderCollection headers)
    {
        foreach (string h in headers.Keys)
        {
            if (h.Equals("Date"))
            {
                DateTime NettDateTime = Convert.ToDateTime(headers[h]);
                DateTime startTime = new DateTime(1970, 1, 1, 0, 0, 0, 0);
                long t = (NettDateTime.Ticks - startTime.Ticks) / 10000;   //除10000调整为13位      
                GlobalConst.CurrentServerTime = t / 1000;
				//LogManager.Log("GetServerTime:::" , GlobalConst.CurrentServerTime);
                break;
            }
        }
    }

    /// <summary>
    /// Put请求协议，app内部不会使用，一般都是第三方
    /// </summary>
    /// <param name="vo"></param>
    /// <returns></returns>
    public HttpWebRequest PutRequest(SnapRequestVO vo)
    {
        return SnapPutHttp.PutRequest(vo);
    }

    /// <summary>
    /// Post请求方式
    /// </summary>
    /// <param name="vo"></param>
    /// <returns></returns>
    public HttpWebRequest PostRequest(SnapRequestVO vo)
    {
        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(SnapHttpConfig.NET_APP_URL + vo.requestAction);
        request.ServicePoint.Expect100Continue = false;
		//LogManager.Log((SnapHttpConfig.NET_APP_URL , vo.requestAction , vo.toJSONString()));
        GetRequestHeader(request, vo);
        request.ContentLength = vo.toJSONString().Length;
        using (StreamWriter streamWriter = new StreamWriter(request.GetRequestStream()))
        {
            streamWriter.Write(vo.toJSONString());
            streamWriter.Flush();
            streamWriter.Close();
        }
        return request;
    }

    /// <summary>
    /// Get请求方式
    /// </summary>
    /// <param name="vo"></param>
    /// <returns></returns>
    public HttpWebRequest GetRequest(SnapRequestVO vo)
    {
        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(SnapHttpConfig.NET_APP_URL + vo.requestAction + vo.toJSONString());
		//LogManager.Log((SnapHttpConfig.NET_APP_URL , vo.requestAction , vo.toJSONString()));
        GetRequestHeader(request,vo);
        return request;
    }

    private WebHeaderCollection GetRequestHeader(HttpWebRequest request, SnapRequestVO vo)
    {
        request.ServicePoint.Expect100Continue = false;
        request.ServicePoint.UseNagleAlgorithm = false;
        request.ServicePoint.ConnectionLimit = 50;
        request.AllowWriteStreamBuffering = false;
        request.Method = vo.requestMethod;        
        request.KeepAlive = false;
        request.ContentType = SnapHttpConfig.NET_CONTENT_TYPE;
        request.Timeout = SnapHttpConfig.NET_TIMEOUT;
        request.ReadWriteTimeout = SnapHttpConfig.NET_TIMEOUT;
        request.Accept = SnapHttpConfig.NET_CONTENT_TYPE;
		request.Headers.Add("Token", SnapHttpConfig.NOT_LOGINED_APP_TOKEN);
		request.Headers.Add("UserToken", SnapHttpConfig.LOGINED_APP_TOKEN);
        return request.Headers;
    }

    public void close()
	{
		isInRequesting = false;
		if (response != null)
		{
			response.Close();
			response = null;
		}
		if (request != null)
		{
			request.Abort();
            request = null;

        }
	}

}