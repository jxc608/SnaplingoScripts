using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Purchasing;
using UnityEngine.Store;
using LitJson;
using UnityEngine.Purchasing.Security;

class IAPProxy: IStoreListener
{
	private static IAPProxy _Instance;
	public static IAPProxy Instance()
	{
		if (_Instance == null) {
			_Instance = new IAPProxy();
		}
		return _Instance;
	}

	/// <summary>
	/// 内购是否可用
	/// </summary>
	private bool IAPIsCanUse = true;

	/// <summary>
	/// 内购是否初始化完成
	/// </summary>
	private bool IAPIsInialize;

	/// <summary>
	/// 内购控制器
	/// </summary>
	private IStoreController Controller;

	private IAPNode Manager;

	public IAPProxy() { }

	public void Set(IAPNode manager)
	{
		Manager = manager;
	}

	/**--------------------- Pay Logic  ---------------------**/
	private Action<Product[]> mInitializeIAPCallback;
	public void InitializeIAP(List<ServerGoodsInfo> GoodsIDList, Action<Product[]> _callback)
	{
		if (IAPIsCanUse)
		{
			mInitializeIAPCallback = _callback;
			OnStartInitializeIAP(GoodsIDList);
		}
		else
		{
			LogManager.LogError("内购不可用!!!");
		}
	}

	/// <summary>
	/// 开始初始化
	/// </summary>
	private void OnStartInitializeIAP(List<ServerGoodsInfo> GoodsIDList)
	{
		var module = StandardPurchasingModule.Instance();
		module.useFakeStoreUIMode = FakeStoreUIMode.StandardUser;

		ConfigurationBuilder builder = ConfigurationBuilder.Instance(module);
		// 将其设置为true以启用Microsoft IAP模拟器进行本地测试.
		//builder.Configure<IMicrosoftConfiguration>().useMockBillingSystem = true;

		foreach (ServerGoodsInfo item in GoodsIDList)
		{
			builder.AddProduct(item.GoodsID, ProductType.Consumable);
		}
		UnityPurchasing.Initialize(this, builder);
	}


	/// <summary>
	/// 初始化完成
	/// </summary>
	/// <param name="controller"></param>
	/// <param name="extensions"></param>
	public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
	{
		IAPIsInialize = true;
		Controller = controller;
		var m_AppleExtensions = extensions.GetExtension<IAppleExtensions>();
		m_AppleExtensions.RegisterPurchaseDeferredListener(OnDeferred);

		string logStr = " IAP初始化完成 " + controller;
		Manager.log(logStr);
		mInitializeIAPCallback(Controller.products.all);
	}

	/// <summary>
	/// 初始化失败
	/// </summary>
	/// <param name="error"></param>
	public void OnInitializeFailed(InitializationFailureReason error)
	{
		IAPIsInialize = false;
		mInitializeIAPCallback(null);
	}

	/// <summary>
	/// 开始支付
	/// </summary>
	private Action<bool, string, string> mPayCallback;
	public void OnSnapStartPay(string productID, Action<bool, string, string> payCallback)
	{
		if (Controller == null || !IAPIsInialize)
		{
			LogManager.LogError("Purchasing is not initialized");
			payCallback(false, null, null);
			return;
		}

		mPayCallback = payCallback;
		var product = Controller.products.WithID(productID);
		Controller.InitiatePurchase(product);

		string logStr = " 开始支付 " + productID;
		Manager.log(logStr);
	}

	/// <summary>
	/// 支付结果
	/// </summary>
	/// <param name="e"></param>
	/// <returns></returns>
	public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs e)
	{
		//获取并解析你需要上传的数据。解析成string类型
		var wrapper = (Dictionary<string, object>)MiniJson.JsonDecode(e.purchasedProduct.receipt);
		if (null == wrapper)
		{
			return PurchaseProcessingResult.Complete;
		}

		string store = (string)wrapper["Store"];
		string transactionID = (string)wrapper["TransactionID"];
		string payload = (string)wrapper["Payload"];

		mPayCallback(true, transactionID, payload);
		//VerifyTransactionResult(transactionID, payload);

		//For GooglePlay payload contains more JSON
		//if (Application.platform == RuntimePlatform.Android)
		//{
		//	var gpDetails = (Dictionary<string, object>)MiniJson.JsonDecode(payload);
		//	var gpJson = (string)gpDetails["json"];
		//	var gpSig = (string)gpDetails["signature"];

		//	//Google验证商品信息的数据包含在gpJson里面还需要在服务端进行解析一下，对应的键是"purchaseToken"。
		//	StartCoroutine(PostRepict("http://www.xxxxxxxxxxxxx/purchase/Andverifytrade", e.purchasedProduct.definition.id, gpJson));
		//}

		string logStr = "支付成功:" + JsonMapper.ToJson(e);
		Manager.log(logStr);
		return PurchaseProcessingResult.Complete;
	}

	/// <summary>
	/// 支付失败
	/// </summary>
	/// <param name="i"></param>
	/// <param name="p"></param>
	public void OnPurchaseFailed(Product i, PurchaseFailureReason p)
	{
		mPayCallback(false, null, null);
		string logStr = "支付失败:" + p + " &&& " + JsonMapper.ToJson(i);
		Manager.log(logStr);
	}

	private void OnDeferred(Product item)
	{
		LogManager.Log("Purchase deferred: " , item.definition.id);
	}

}