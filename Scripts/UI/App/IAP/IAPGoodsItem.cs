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

class IAPGoodsItem : MonoBehaviour
{
	//父级
	public GameObject Parant;

	//有商品
	public GameObject GoodsTure;
	//商品图标
	public Image GoodsIconTrue;
	//商品钱
	public Text GoodsMoneyTF;
	//商品等级
	public Text GoodsLvTF;
	//是否购买过
	public GameObject GoodsIsBuyed;

	//无商品
	public GameObject GoodsFasle;
	//商品图标
	public Image GoodsIconFalse;

	//商品ID
	private string GoodsID;

	//商品价钱
	private decimal GoodsMoney;

	//商品名字
	private string GoodsName;

	//商品信息
	private string GoodsInfo;

	private IAPNode Manager;

	//当前是否包含有效商品
	private bool CurrentIsHaveGoods;

	//服务器商品数据
	private ServerGoodsInfo GoodsServerData;

	public IAPGoodsItem()
	{
		//Parant = parent;
	}

	private void Start()
	{
		GoodsIconTrue.GetComponent<Button>().onClick.AddListener(ClickGoods);
	}


	/// <summary>
	/// 注入
	/// </summary>
	/// <param name="_appleData"></param>
	/// <param name="buyCallback"></param>
	public void refresh(Product _appleData, ServerGoodsInfo _snapData, IAPNode _manaegr)
	{
		if (_appleData != null && _appleData.definition != null && _appleData.definition.id != null)
		{
			GoodsServerData = _snapData;

			visible(true);
			GoodsID = _appleData.definition.id;
			GoodsMoney = _appleData.metadata.localizedPrice;
			GoodsName = _appleData.metadata.localizedTitle;
			GoodsInfo = _appleData.metadata.localizedDescription;
			Manager = _manaegr;

			GoodsMoneyTF.text = "" + GoodsMoney;

			GoodsLvTF.text = "" + GoodsServerData.GoodsLvInt;

			AsyncImageDownload.GetInstance().SetAsyncImage(GoodsServerData.GoodsIconUrl, GoodsIconTrue);

			GoodsIsBuyed.SetActive(GoodsServerData.GoodsIsBuyed);
			GoodsIconTrue.GetComponent<Button>().enabled = !GoodsServerData.GoodsIsBuyed;

		}
		else
		{
			visible(false);
		}
	}


	/// <summary>
	/// 购买完成
	/// </summary>
	/// <param name="visible"></param>
	public void buyOK()
	{
		if (CurrentIsHaveGoods)
		{
			GoodsIsBuyed.SetActive(GoodsServerData.GoodsIsBuyed);
		}
	}

	/// <summary>
	/// 设置是否有商品
	/// </summary>
	/// <param name="visible"></param>
	public void visible(bool visible)
	{
		CurrentIsHaveGoods = visible;
		GoodsTure.SetActive(visible);
		GoodsFasle.SetActive(!visible);
	}

	/// <summary>
	/// 点击一个商品s
	/// </summary>
	public void ClickGoods()
	{
		if (GoodsServerData.GoodsIsBuyed)
		{
			LogManager.Log("买过咯！！");
			return;
		}
		Manager.ClickItemGoodsIsBuy(GoodsID, GoodsMoney, GoodsName, GoodsInfo);
	}
}
