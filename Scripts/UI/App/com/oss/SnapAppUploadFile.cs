using System;

using UnityEngine;
using LitJson;
using System.IO;
using System.Threading;
using NAudio.Lame;
using NAudio.Wave.WZT;

class SnapAppUploadFile
{
    public static string OSS_Authorization = "";
    public static string OSS_ContentType = "";
    public static string OSS_Date = "";
    public static string fileUploadKey = "";
    private string UploadOSSPathHeader = "";

    string _filePath;
    //0代表txt,1代表音频
    int _fileExtType = 0;
    Action<string> _resultAction;
    private Thread thread;

    //private static SnapAppUploadFile instance;
    //static public SnapAppUploadFile getInstance()
    //{
    //    if (instance == null)
    //    {
    //        instance = new SnapAppUploadFile();
    //    }
    //    return instance;


    //}

    public SnapAppUploadFile()
    {

    }

    /// <summary>
    /// 把本地文件上传到oss,默认为根据头文件进行上传
    /// </summary>
    /// <param name="filePath"></param>
    /// <param name="uploadType"></param>
    /// <param name="resultAction"></param>
    public void PutObjectFileToOSS(string filePath, Action<string> resultAction = null)
    {
        _resultAction = resultAction;
        string[] arr = filePath.Split('.');
        string[] arr2 = arr[0].ToString().Split('/');
        string fileExtType = arr[1];
        //game / upload /{ YYYYMMdd}/{ user_uuid}/{ timestamp}.mp3

        int index = 0;
        if (DebugConfigController.Instance.FormalData)
        {
            // 正式服
            index = 1;
        }
        UploadOSSPathHeader = ConfigurationController.Instance.OSSListGamePaths[index] + "/";
        UploadOSSPathHeader += DateTime.UtcNow.ToString("yyyyMMdd") + "/" + SelfPlayerData.Uuid + "/" + SnapHttpManager.GetTimeStamp();

        if (fileExtType == "txt" || fileExtType == "json" || fileExtType == "xml")
        {
            _filePath = filePath;
            fileUploadKey = UploadOSSPathHeader + "." + arr[1];
            _fileExtType = 0;
            GetFileKey(() =>
            {
                PutObjectWithHeader(_resultAction);
            });
        }
        else if (fileExtType == "mp3" || fileExtType == "wav")
        {
            fileUploadKey = UploadOSSPathHeader + ".mp3";
            _fileExtType = 1;
            //wav to mp3
            string mp3FileName = arr2[arr2.Length - 1] + ".mp3";
            string mp3FilePath = "";
            for (int i = 0; i < arr2.Length - 1; i++)
            {
                mp3FilePath += arr2[i] + "/";
            }
            mp3FilePath = mp3FilePath.Substring(0, mp3FilePath.Length - 1);
            _filePath = Path.Combine(mp3FilePath, mp3FileName);
            WaveToMP3(filePath, _filePath, LAMEPreset.ABR_128);
        }
    }

    private void WaveToMP3(string waveFileName, string mp3FileName, LAMEPreset bitRate = LAMEPreset.ABR_128)
    {
        using (var reader = new WaveFileReader(waveFileName))
        using (var writer = new LameMP3FileWriter(mp3FileName, reader.WaveFormat, bitRate))
            reader.CopyTo(writer);

        GetFileKey(() =>
        {
            PutObjectWithHeader(_resultAction);
        });
    }

    /// <summary>
    /// 根据头上传文件
    /// </summary>
    /// <param name="bucketName"></param>
    private void PutObjectWithHeader(Action<string> resultAction = null)
    {
        thread = new Thread(new ParameterizedThreadStart(startUploadFile));
        thread.Name = "startUploadFile";
        thread.Start(thread.Name);
    }

    private void startUploadFile(object o)
    {
        try
        {
            if (File.Exists(_filePath))
            {
                using (FileStream fs = File.Open(_filePath, FileMode.Open, FileAccess.Read))
                {
                    fs.Seek(0, SeekOrigin.Begin);
                    if (_fileExtType == 0)
                    {
                        PutTextFile(fs);
                    }
                    else if(_fileExtType == 1)
                    {
                        PutAudioFile(fs);
                    }
                }
            }
            else
            {
				LogManager.LogError(_filePath , "is not exits");
            }
        }
        catch (IOException ex)
        {
			LogManager.LogError("Exception:::" , ex.Message);
        }
    }

    private void PutTextFile(FileStream fs)
    {
        Byte[] bytes = new byte[fs.Length];
        fs.Read(bytes, 0, (int)fs.Length);
        JsonData json_data = new JsonData();
        json_data["byte_data"] = System.Text.Encoding.Default.GetString(bytes);
        json_data["apiName"] = fileUploadKey;

        SnapAppApi.Request_SnapAppApi(SnapAppApiInterface.Request_ThirdWebToApp, SnapHttpConfig.NET_REQUEST_PUT,
            json_data, uploadFileSuccess, 0, SnapAppApiThirdRequestFace.Third_Aliyun_OSS);
    }

    private void PutAudioFile(FileStream fs)
    {
        BinaryReader br = new BinaryReader(fs);
        int lenth = Convert.ToInt32(br.BaseStream.Length);
        byte[] bytes = br.ReadBytes(lenth);
        br.Close();
        fs.Close();


        //开始上传到Oss
        SnapRequestVO vo = new SnapRequestVO();
        vo.mp3byte = bytes;
        vo.thirdType = SnapAppApiThirdRequestFace.Third_Aliyun_OSS;
        vo.requestAction = fileUploadKey;
        SnapPutHttp.PutRequest(vo, uploadFileSuccess);

        thread.Abort();

    }

    private void uploadFileSuccess(SnapRpcDataVO resultVo)
    {
        string result = resultVo.data["uploadUrl"].ToString();
        if (_resultAction != null)
        {
            _resultAction.Invoke(result);
        }
    }

    private void GetFileKey(Action callback = null)
    {
        int index = DebugConfigController.Instance.HttpHostIndex;
        string bucketName = ConfigurationController.Instance.OSSListBucketNames[index];
        
        JsonData data = new JsonData();
        data["osskey"] = fileUploadKey;
        data["bucket"] = bucketName;

		LogManager.Log("UploadOSSFile key=" , fileUploadKey);

        SnapAppApi.Request_SnapAppApi(SnapAppApiInterface.Request_AliyunOSSApp, SnapHttpConfig.NET_REQUEST_GET, data, (SnapRpcDataVO rpcData) =>
        {
            if (rpcData.code == 1)
            {
                JsonData header = rpcData.data["header"];
                SnapAppUploadFile.OSS_Authorization = header["Authorization"].ToString();
                SnapAppUploadFile.OSS_ContentType = header["Content-Type"].ToString();
                SnapAppUploadFile.OSS_Date = header["Date"].ToString();

                if (callback != null)
                {
                    callback.Invoke();
                }
            }
        });
    }

}
