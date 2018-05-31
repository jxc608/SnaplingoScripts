using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using UnityEngine;
using System.IO;
using System.Reflection;
using LitJson;

class SnapPutHttp
{
    public static HttpWebRequest PutRequest(SnapRequestVO vo,Action<SnapRpcDataVO> callback = null)
    {
        string request_url = GetHttpURL(vo);
        if (string.IsNullOrEmpty(request_url))
        {
            request_url = SnapHttpConfig.NET_APP_URL;
        }
        byte[] body = vo.toByte();
        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(request_url);
        request.Proxy = null;
		//LogManager.Log(request.RequestUri);
        request.Method = SnapHttpConfig.NET_REQUEST_PUT;
        request.Timeout = 600 * 1000;
        request.ReadWriteTimeout = 600 * 1000;
        request.ContentType = SnapAppUploadFile.OSS_ContentType;
        request.ContentLength = body.Length;
        if (vo.mp3byte != null && vo.mp3byte.Length > 0)
        {
            MethodInfo priMethod = request.Headers.GetType().GetMethod("AddWithoutValidate", BindingFlags.Instance | BindingFlags.NonPublic);
            priMethod.Invoke(request.Headers, new[] { "Date", SnapAppUploadFile.OSS_Date });
            priMethod.Invoke(request.Headers, new[] { "Content-Length", body.Length.ToString() });
            priMethod.Invoke(request.Headers, new[] { "Content-Type", SnapAppUploadFile.OSS_ContentType });
            request.Headers.Add("Authorization", SnapAppUploadFile.OSS_Authorization);
        }
        else
        {
			request.Headers.Add("Token", SnapHttpConfig.NOT_LOGINED_APP_TOKEN);
			request.Headers.Add("UserToken", SnapHttpConfig.LOGINED_APP_TOKEN);
        }
        
        try
        {
            if (body != null && body.Length > 0)
            {
                using (Stream stream = request.GetRequestStream())
                {
                    stream.Write(body, 0, body.Length);
                    stream.Flush();
                    stream.Close();
                }
            }

            if (callback != null)
            {
                //音频文件
                using(HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                        {
                            SnapRpcDataVO rpcData = new SnapRpcDataVO();
                            rpcData.code = 1;
                            rpcData.message = "request success";
                            rpcData.data = new JsonData();
                            rpcData.data["uploadUrl"] = response.ResponseUri.AbsolutePath;
                            //if (callback != null)
                            //{
                            //    callback.Invoke(rpcData);
                            //    callback = null;
                            //}
                            SnapHttpManager.getInstance().AddOneHttpCallBack(callback, rpcData);
                            reader.Close();
                        }
                    }
                    response.Close();
                    request.Abort();
                }
            }
        }
        catch (Exception ex)
        {
			LogManager.Log(ex.Message);
            throw;
        }
        return request;
    }

    private static string GetHttpURL(SnapRequestVO vo)
    {
        string request_url = SnapHttpConfig.NET_APP_URL + vo.requestAction;
        if (vo.thirdType == SnapAppApiThirdRequestFace.Third_Aliyun_OSS)
        {
            int index = 0;
            if (DebugConfigController.Instance.FormalData)
            {
                // 正式服
                index = 1;
            }            
            string bucketName = ConfigurationController.Instance.OSSListBucketNames[index];
            string endpoint = ConfigurationController.Instance.OSSListEndPoints[index];
            request_url = "http://" + bucketName + "." + endpoint + "/" + vo.requestAction;
        }

        return request_url;

    }
}
