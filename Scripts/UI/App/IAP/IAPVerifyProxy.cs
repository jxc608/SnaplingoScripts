using Snaplingo.SaveData;
using LitJson;
using System;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Purchasing;
using System.Timers;

class IAPVerifyProxy : ISaveData
{
	public static IAPVerifyProxy Instance
	{
		get { return SaveDataUtils.GetSaveData<IAPVerifyProxy>(); }
	}

	/// <summary>
	/// 订单记录表
	/// </summary>
	private List<IAPVerifyTempItem> TransactionTemp;

	/// <summary>
	/// 当前正在验证的交易
	/// </summary>
	private IAPVerifyTempItem CurrentVerifyTransaction;

	/// <summary>
	/// 立即验证的超时回调
	/// </summary>
	private Action VerifyTransactionTimeout;
	/// <summary>
	/// 立即验证的超时回调
	/// </summary>
	private Timer VerifyTransactionTimeoutTimer;

	/// <summary>
	/// 巡查计时器
	/// </summary>
	private Timer RetryTimer;
	private int RetryTimerDelay = 1;

	private IAPNode Manager;

	//检测漏单是否正在运行中
	private bool IsRuning;

	public IAPVerifyProxy()
	{
		TransactionTemp = new List<IAPVerifyTempItem>();
	}

	/// <summary>
	/// 开始巡查漏单处理
	/// </summary>
	/// <param name="isImmediately">是否立即查询</param>
	/// <param name="manager">可以不用传</param>
	/// <param name="timeoutCallback">如果立即查询, 超时回调</param>
	public void StartRetry(bool isImmediately, IAPNode manager = null, Action timeoutCallback = null)
	{
		Manager = manager;
		StopRetryTimer(true);
		if (isImmediately)
		{
			string logStr = "立即验证漏单, 10秒后进行超时处理";
			//if (Manager) Manager.log(logStr);
			LogManager.Log(logStr);
			VerifyTransactionTimeout = timeoutCallback;
			VerifyItemTransaction();
		}
		else
		{
			if (TransactionTemp.Count > 0)
			{
				StartRetryTimer();
			}
		}
	}

	private void VerifyItemTransaction(object source = null, ElapsedEventArgs e = null)
	{
		if (TransactionTemp.Count > 0)
		{
			CurrentVerifyTransaction = TransactionTemp[0];

			StartTimeoutTimer();

			JsonData paramObj = new JsonData();
			paramObj["userID"] = CurrentVerifyTransaction.userID;
			paramObj["receipt"] = CurrentVerifyTransaction.payLoad;
			paramObj["transactionID"] = CurrentVerifyTransaction.transactionID;
			paramObj["price"] = CurrentVerifyTransaction.price;

			IAPRpcProxy.VerifyTransaction(paramObj, VerifyTransactionResultCallback);
		}
		else
		{
			//!!! 全部漏单处理完毕

			string logStr = "全部漏单处理完毕";
			//if (Manager) Manager.log(logStr);
			LogManager.Log(logStr);

			IsRuning = false;

			StopRetryTimer(true);
		}
	}
	private void VerifyItemTransactionTimeoutCallback(object source = null, ElapsedEventArgs e = null)
	{
		string logStr = "订单(漏单)处理超时,停止处理:" + "用户ID:" + CurrentVerifyTransaction.userID + ", 订单号:" + CurrentVerifyTransaction.transactionID;
		//if (Manager) Manager.log(logStr);
		LogManager.Log(logStr);

		StopTimeoutTimer();

		StartRetryTimer();

		if (VerifyTransactionTimeout != null)
		{
			VerifyTransactionTimeout();
			VerifyTransactionTimeout = null;
		}
	}


	/// <summary>
	/// 验证结果
	/// </summary>
	/// <param name="rpcResultObj"></param>
	private void VerifyTransactionResultCallback(SnapRpcDataVO rpcResultObj)
	{
		StopTimeoutTimer();
		JsonData data = rpcResultObj.data;
		if (rpcResultObj.code == RpcDataCode.RequestFinish)
		{
			string logStr = "验证完成, 有效交易:" + rpcResultObj.code + "(" + rpcResultObj.message + ")";
			if (Manager) Manager.log(logStr);
			LogManager.Log(logStr);

			if (Manager) Manager.BuyFinishGoToGameLoic();
		}
		else
		{
			string logStr = "验证失败, 无效交易:" + rpcResultObj.code + "(" + rpcResultObj.message + ")";
			if (Manager) Manager.log(logStr);
			LogManager.Log(logStr);
		}

		//!!! Item漏单处理完毕

		//收据验证完成, 清除本地
		RemoveItemIAPTransaction(CurrentVerifyTransaction.userID, CurrentVerifyTransaction.transactionID);
		CurrentVerifyTransaction = null;
		//继续巡查
		VerifyItemTransaction();
		if (Manager) Manager.clear();
	}

	/// <summary>
	/// 开始计时器
	/// </summary>
	private void StartRetryTimer()
	{
		StopRetryTimer();
		RetryTimerDelay = RetryTimerDelay * 4;
		if (RetryTimerDelay >= 1200)
		{
			RetryTimerDelay = 1;
		}
		RetryTimer = new Timer(RetryTimerDelay * 1000);
		RetryTimer.Elapsed += new ElapsedEventHandler(VerifyItemTransaction);
		RetryTimer.AutoReset = false;
		RetryTimer.Enabled = true;
		RetryTimer.Start();

		string logStr = "漏单队列启动," + (RetryTimerDelay) + "秒后开始处理漏单";
		//if (Manager) Manager.log(logStr);
		LogManager.Log(logStr);
	}

	/// <summary>
	/// 停止计时器
	/// </summary>
	private void StopRetryTimer(bool isResetDelay = false)
	{
		if (RetryTimer != null)
		{
			RetryTimer.Enabled = false;
			RetryTimer.Stop();
		}
		RetryTimer = null;
		if (isResetDelay)
		{
			RetryTimerDelay = 1;
		}
	}

	/// <summary>
	/// 启动超时计时器
	/// </summary>
	private void StartTimeoutTimer()
	{
		if (VerifyTransactionTimeoutTimer == null)
		{
			VerifyTransactionTimeoutTimer = new Timer(10000);
			VerifyTransactionTimeoutTimer.Elapsed += new ElapsedEventHandler(VerifyItemTransactionTimeoutCallback);
			VerifyTransactionTimeoutTimer.AutoReset = false;
			VerifyTransactionTimeoutTimer.Enabled = true;
			VerifyTransactionTimeoutTimer.Start();
		}
		else
		{
			VerifyTransactionTimeoutTimer.Start();
		}
	}

	/// <summary>
	/// 停止超时计时器
	/// </summary>
	private void StopTimeoutTimer()
	{
		if (VerifyTransactionTimeoutTimer != null)
		{
			VerifyTransactionTimeoutTimer.Stop();
		}
	}

	/// <summary>
	/// 本地添加一条订单记录, 防止漏单
	/// </summary>
	/// <param name="userID"></param>
	/// <param name="payLoad"></param>
	public void AddItemIAPTransaction(string userID, string transactionID, string payLoad, string price)
	{
		string logStr = "";
		if (userID == null || userID == "" || transactionID == null || transactionID == "" || payLoad == null || payLoad == "" || price == null || price == "")
		{
			logStr = "数据错误,无法添加订单记录: 用户ID:" + userID + ", 订单ID:" + transactionID + ", 收据:" + payLoad + ", 价钱:" + price;
			//if (Manager) Manager.log(logStr);
			LogManager.Log(logStr);
			return;
		}
		IAPVerifyTempItem Transaction = new IAPVerifyTempItem();
		Transaction.userID = userID;
		Transaction.transactionID = transactionID;
		Transaction.payLoad = payLoad;
		Transaction.price = price;
		TransactionTemp.Add(Transaction);

		SaveDataUtils.Save<IAPVerifyProxy>();

		logStr = "本地添加一条订单记录, 防止漏单: 用户ID:" + userID + ", 订单ID:" + transactionID + ", 当前有" + TransactionTemp.Count + "条待处理订单(漏单)";
		//if (Manager) Manager.log(logStr);
		LogManager.Log(logStr);
	}

	/// <summary>
	/// 订单(漏单)处理完成, 清除记录
	/// </summary>
	/// <param name="userID"></param>
	/// <param name="payLoad"></param>
	public void RemoveItemIAPTransaction(string userID, string transactionID)
	{
		for (int i = 0; i < TransactionTemp.Count; i++)
		{
			if (TransactionTemp[i].userID == userID && TransactionTemp[i].transactionID == transactionID)
			{
				TransactionTemp.RemoveAt(i);
			}
		}

		SaveDataUtils.Save<IAPVerifyProxy>();

		string logStr = "单条订单(漏单)处理完成, 清除记录: 用户ID:" + userID + ", 订单ID:" + transactionID;
		//if (Manager) Manager.log(logStr);
		LogManager.Log(logStr);
	}


	/// <summary>
	/// 保存为json文件
	/// </summary>
	/// <returns></returns>
	public string SaveAsJson()
	{
		JsonData data = new JsonData();
		foreach (IAPVerifyTempItem item in TransactionTemp)
		{
			JsonData tempData = new JsonData();
			tempData["UserID"] = item.userID;
			tempData["TransactionID"] = item.transactionID;
			tempData["PayLoad"] = item.payLoad;
			tempData["price"] = item.price;
			data.Add(tempData);
		}
		return data.ToJson();
	}


	/// <summary>
	/// 从本地取出
	/// </summary>
	/// <param name="json"></param>
	public void LoadFromJson(string json)
	{
		if (string.IsNullOrEmpty(json) || IsRuning)
			return;
		if (TransactionTemp != null)
		{
			TransactionTemp.Clear();
		}
		else
		{
			TransactionTemp = new List<IAPVerifyTempItem>();
		}
		JsonData data = JsonMapper.ToObject(json);
		foreach (JsonData tempData in data)
		{
			IAPVerifyTempItem item = new IAPVerifyTempItem();
			item.userID = tempData.TryGetString("UserID");
			item.transactionID = tempData.TryGetString("TransactionID");
			item.payLoad = tempData.TryGetString("PayLoad");
			item.price = tempData.TryGetString("price");
			TransactionTemp.Add(item);
		}
		if (TransactionTemp.Count > 0)
		{
			string logStr = "开始处理本地" + TransactionTemp.Count + "条漏单:" + json;
			//if (Manager) Manager.log(logStr);
			LogManager.Log(logStr);

			IsRuning = true;
			StartRetry(false);
		}
	}

	public string SaveTag()
	{
		return GetType().ToString();
	}
}

[Serializable]
class IAPVerifyTempItem
{
	//用户ID
	public string userID;

	//payLoad收据
	public string payLoad;

	//交易ID
	public string transactionID;

	//商品价钱
	public string price;
}