using UnityEngine;
using UnityEngine.UI;
using Snaplingo.UI;
using LitJson;
using System;
using System.Collections.Generic;
using UnityEngine.Purchasing;
using LitJson;
using System.Collections;

public class IAPNode : Node
{

	//一页能显示几个商品
	private static int UIItemPageGoodsNum = 6;

	//log文本
	public Text LogText;

	//关闭按钮
	public Button CloseBtn, LastBtn, NextBtn;

	//loading遮挡物
	public Image LoadingBG;

	//确认购买
	public GameObject IsBuyParent;
	private IAPIsBuy IsBuy;

	//服务器存储上架哪些商品
	private List<ServerGoodsInfo> GoodsPublisServer;
	//根据服务器商品从苹果初始化成功的商品
	private List<Product> GoodsAppleInfo;

	//商品列表
	public List<GameObject> GoodsIconList;
	private int GoodsListMaxPage;
	private int GoodsListCurrentPage;

	// 当前进行内购的商品ID
	private string CurrentIAPGoodsID;
	// 当前进行内购的商品价钱
	private decimal CurrentIAPGoodsMoney;
	// 当前进行内购的交易ID
	private string CurrentIAPTransactionID;
	// 当前进行内购的订单收据
	private string CurrentIAPPayLoad;

	//当前是否被关闭
	private bool CurrentIsDispose;

	//当前是否成功进行过购买
	private bool CurrentIsSuccessBuy;

	public override void Init (params object[] args)
	{
		base.Init (args);
	}

	public override void Open ()
	{
		IAPProxy.Instance ().Set (this);
		IsBuy = new IAPIsBuy (IsBuyParent);

		LastBtn.gameObject.SetActive (false);
		NextBtn.gameObject.SetActive (false);
		LoadingBG.gameObject.SetActive (false);

		JsonData paramObj = new JsonData ();
		//销售渠道id  0:apple
		paramObj["channel"] = 0;
		paramObj["uid"] = SelfPlayerData.Uuid;
		IAPRpcProxy.RequestGoodsInfo (paramObj, RequestGoodsInfoCallback);
		string logStr = "开始请求商品信息"; ;
		log (logStr);

		AnalysisManager.Instance.OnEvent ("shopNode", null);

	}

	/// <summary>
	/// 请求商品信息
	/// </summary>
	/// <param name="rpcResultObj"></param>
	private void RequestGoodsInfoCallback (SnapRpcDataVO rpcResultObj)
	{
		if (CurrentIsDispose) return;

		JsonData data = rpcResultObj.data;
		if (rpcResultObj.code == RpcDataCode.RequestFinish)
		{
			GoodsPublisServer = new List<ServerGoodsInfo> ();
			for (int i = 0; i < data.Count; i++)
			{
				ServerGoodsInfo item = new ServerGoodsInfo ();
				item.GoodsID = (string)data[i]["appleGoodsId"];
				item.GoodsSnapID = (string)data[i]["id"];
				item.GoodsIconUrl = (string)data[i]["icon"];
				item.GoodsLvInt = (int)data[i]["courseNum"];
				item.GoodsSnapType = (int)data[i]["itemType"];
				item.GoodsIsBuyed = (Boolean)data[i]["isBuy"];
				GoodsPublisServer.Add (item);
			}

			IAPProxy.Instance ().InitializeIAP (GoodsPublisServer, InitializeUI);

			string logStr = "请求商品信息成功:" + JsonMapper.ToJson (data);
			log (logStr);



		}
		else
		{
			PromptManager.Instance.MessageBox (PromptManager.Type.FloatingTip, "支付通信失败，请稍后重试");
			string logStr = "请求商品信息失败:" + JsonMapper.ToJson (data);
			log (logStr);
		}
	}

	/// <summary>
	/// 初始化UI
	/// </summary>
	/// <param name="_goodsList"></param>
	private void InitializeUI (Product[] _goodsList)
	{
		if (CurrentIsDispose) return;
		if (_goodsList == null)
		{
			PromptManager.Instance.MessageBox (PromptManager.Type.FloatingTip, "支付启动失败，请稍后重试");
			log ("IAP 初始化失败");
			return;
		}
		List<Product> goodsList = new List<Product> ();
		foreach (Product item in _goodsList)
		{
			goodsList.Add (item);
		}

		//// ========================
		//foreach (Product item in _goodsList)
		//{
		//	goodsList.Add(item);
		//}
		//foreach (Product item in _goodsList)
		//{
		//	goodsList.Add(item);
		//}
		//foreach (Product item in _goodsList)
		//{
		//	goodsList.Add(item);
		//}

		//// ========================

		GoodsAppleInfo = goodsList;

		GoodsListMaxPage = (int)Math.Ceiling ((float)goodsList.Count / UIItemPageGoodsNum);
		GoodsListCurrentPage = 1;
		refreshPageBtn ();
		refreshUIForPage ();

		string logStr = "IAP初始化商品成功:" + JsonMapper.ToJson (_goodsList);
		log (logStr);
	}

	/// <summary>
	/// 根据当前页刷新UI
	/// </summary>
	private void refreshUIForPage ()
	{
		// 先全隐藏
		foreach (GameObject item in GoodsIconList) item.GetComponent<IAPGoodsItem> ().visible (false);

		//拿到当前页的数据
		ArrayList pageData = new ArrayList ();
		int pageStartIndex = (GoodsListCurrentPage - 1) * UIItemPageGoodsNum;
		int pageEndIndex = (pageStartIndex + UIItemPageGoodsNum);
		for (int startIndex = pageStartIndex; startIndex < pageEndIndex; startIndex++)
		{
			if (GoodsAppleInfo.Count >= startIndex + 1)
			{
				ArrayList itemPageData = new ArrayList ();
				string goodsID = GoodsAppleInfo[startIndex].definition.id;
				ServerGoodsInfo serverInfo = null;
				foreach (ServerGoodsInfo item in GoodsPublisServer) { if (goodsID == item.GoodsID) { serverInfo = item; break; } }
				itemPageData.Add (GoodsAppleInfo[startIndex]);
				itemPageData.Add (serverInfo);
				pageData.Add (itemPageData);
			}
		}

		// 刷新显示
		for (int refreshI = 0; refreshI < pageData.Count; refreshI++)
		{
			ArrayList itemPageData = pageData[refreshI] as ArrayList;
			Product _appleData; ServerGoodsInfo _snapData;
			_appleData = itemPageData[0] as Product;
			_snapData = itemPageData[1] as ServerGoodsInfo;
			GoodsIconList[refreshI].GetComponent<IAPGoodsItem> ().refresh (_appleData, _snapData, this);
		}
		pageData = null;
	}

	/// <summary>
	/// 点击一个商品弹出是否购买
	/// </summary>
	/// <param name="goodsID"></param>
	/// <param name="title"></param>
	/// <param name="info"></param>
	/// <param name="money"></param>
	public void ClickItemGoodsIsBuy (string goodsID, decimal money, string title, string info)
	{
		CurrentIAPGoodsID = goodsID;
		CurrentIAPGoodsMoney = money;

		IsBuy.StartIsBuy (title, info, ClickIsBuyOK, clear);
	}
	private void ClickIsBuyOK ()
	{
#if UNITY_EDITOR
		PromptManager.Instance.MessageBox (PromptManager.Type.FloatingTip, "Winods环境无法进行内购");
		return;
#endif

		if (CurrentIAPGoodsID != null && CurrentIAPGoodsMoney > 0)
		{
			IAPProxy.Instance ().OnSnapStartPay (CurrentIAPGoodsID, BuyCallback);
			AnalysisManager.Instance.OnEvent ("pay_apple", null);
		}
		LoadingBG.gameObject.SetActive (true);
	}

	/// <summary>
	/// 支付结果
	/// </summary>
	/// <param name="isSuccess"></param>
	/// <param name="transactionID"></param>
	/// <param name="payLoad"></param>
	private void BuyCallback (bool isSuccess, string transactionID, string payLoad)
	{
		if (isSuccess)
		{
			//先把收据保存到本地
			CurrentIAPTransactionID = transactionID;
			CurrentIAPPayLoad = payLoad;
			IAPVerifyProxy.Instance.AddItemIAPTransaction (SelfPlayerData.Uuid, CurrentIAPTransactionID, CurrentIAPPayLoad, CurrentIAPGoodsMoney.ToString ());
			IAPVerifyProxy.Instance.StartRetry (true, this, BuyVerifyTimeoutCallback);
		}
		else
		{
			//支付失败
			clear ();
		}
	}

	/// <summary>
	/// 验证超时，启动巡查
	/// </summary>
	private void BuyVerifyTimeoutCallback ()
	{
		LoadingBG.gameObject.SetActive (false);
		IAPVerifyProxy.Instance.StartRetry (false, this);
	}

	/// <summary>
	/// 购买完成, 继续游戏逻辑 
	/// </summary>
	public void BuyFinishGoToGameLoic ()
	{
		LoadingBG.gameObject.SetActive (false);
		foreach (ServerGoodsInfo item in GoodsPublisServer)
		{
			if (CurrentIAPGoodsID == item.GoodsID)
			{
				item.GoodsIsBuyed = true;
				break;
			}
		}
		// 刷新显示
		foreach (GameObject item in GoodsIconList) item.GetComponent<IAPGoodsItem> ().buyOK ();

		CurrentIsSuccessBuy = true;

		string logStr = CurrentIAPGoodsID + " 购买完成, 继续游戏逻辑";
		log (logStr);
		AnalysisManager.Instance.OnEvent ("unlockLevel", null);
	}

	/// <summary>
	/// 清除缓存
	/// </summary>
	public void clear ()
	{
		CurrentIAPGoodsID = null;
		CurrentIAPGoodsMoney = 0;
		CurrentIAPTransactionID = null;
		CurrentIAPTransactionID = null;
	}

	#region Page显示 & Log
	/// <summary>
	/// 点击上一页
	/// </summary>
	public void ClickLastPageHandle ()
	{
		GoodsListCurrentPage--;
		refreshPageBtn ();
		refreshUIForPage ();
	}
	/// <summary>
	/// 点击下一页
	/// </summary>
	public void ClickNextPageHandle ()
	{
		GoodsListCurrentPage++;
		refreshPageBtn ();
		refreshUIForPage ();
	}
	/// <summary>
	/// 刷新Page显示
	/// </summary>
	private void refreshPageBtn ()
	{
		if (GoodsListCurrentPage <= 1)
		{
			if (GoodsListCurrentPage == GoodsListMaxPage)
			{
				LastBtn.gameObject.SetActive (false);
				NextBtn.gameObject.SetActive (false);
			}
			else
			{
				LastBtn.gameObject.SetActive (false);
				NextBtn.gameObject.SetActive (true);
			}
		}
		else if (GoodsListCurrentPage >= GoodsListMaxPage)
		{
			LastBtn.gameObject.SetActive (true);
			NextBtn.gameObject.SetActive (false);
		}
		else
		{
			LastBtn.gameObject.SetActive (true);
			NextBtn.gameObject.SetActive (true);
		}

	}

	public void log (string info)
	{
		if (CurrentIsDispose) return;
		DateTime date = DateTime.Now;
		string logStr = "\n" + date.ToLongTimeString ().ToString () + " - " + info + "\n";
		LogText.text = logStr + LogText.text;

		LogText.transform.position = new Vector3 (LogText.transform.position.x, -100000, LogText.transform.position.z);

		TextEditor te = new TextEditor ();
		te.text = LogText.text;
		te.OnFocus ();
		te.Copy ();
	}
	#endregion

	public void CloseIAPDialog ()
	{
		if (CurrentIsSuccessBuy)
		{
			if (UnityEngine.SceneManagement.SceneManager.GetActiveScene ().name == LoadSceneManager.BookSceneName)
				TA.BookSceneManager.instance.ReLoadScene ();
		}

		CurrentIsDispose = true;
		Close (true);
	}

}

[Serializable]
class ServerGoodsInfo
{
	//商品ID
	public string GoodsID;

	//商品服务器ID
	public string GoodsSnapID;

	//商品图标地址
	public string GoodsIconUrl = "";

	//商品Lv等级 1 2 3 4 5
	public int GoodsLvInt = 0;

	//商品类型
	public int GoodsSnapType = 0;

	//商品是否购买过
	public bool GoodsIsBuyed = false;
}