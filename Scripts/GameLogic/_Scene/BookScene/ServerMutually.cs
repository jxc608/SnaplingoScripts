using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum SendEventType
{
    //闯关类型
    CHUANGGUAN,
    //获得S评价类型
    GETS,
    //连续通关类型
    LIANXUTG,
    //击败大忘瓜类型
    JIBAIDAWANGGUA,
    //完美连击类型
    LIANJI,
    //绑定用户类型
    REGEDITUSER,
}

/// <summary>
/// 服务器交互的类
/// </summary>
public static class ServerMutually
{
    /// <summary>
    /// 向服务器发送的字典数据
    /// </summary>
    public static Dictionary<string, string> dataDic = new Dictionary<string, string>();
    /// <summary>
    /// 添加数据到字典
    /// </summary>
    /// <param name="key">Key.</param>
    /// <param name="value">Value.</param>
    public static void AddDataToDic(string key, string value)
    {
        dataDic.Add(key, value);
    }
    /// <summary>
    /// 发送数据到服务器
    /// </summary>
    public static void SendMessageToServer()
    {
        //1...给服务器发送数据
		LogManager.Log("给服务器发送数据");
        //2..发送成功数据之后清空字典
        dataDic.Clear();
    }
    /// <summary>
    /// 向服务器请求数据
    /// </summary>
    /// <returns>The message from server.</returns>
    /// <param name="taskType">Task type.</param>
    public static Dictionary<int, string> ResponseMessageFromServer(TaskType taskType)
    {
        //从服务器获取数据
		LogManager.Log("从服务器获取数据");
        Dictionary<int, string> taskResponse = new Dictionary<int, string>();
        if (taskType == TaskType.TASK)
        {
            //自己模拟的数据
            //数据格式：当前进度_总进度_是否领取奖励
            taskResponse.Add(4001, "1_1_0");
            taskResponse.Add(4002, "1_2_0");
            taskResponse.Add(4114, "1_1_1");
        }
        else
        {
            //数据格式：当前进度_总进度_是否领取奖励
            taskResponse.Add(4100, "0_1_0_1");
            taskResponse.Add(4101, "1_1_0_1");
            taskResponse.Add(4102, "1_1_1_1");
            taskResponse.Add(4103, "0_1_0_0");
        }
        return taskResponse;
    }

}
