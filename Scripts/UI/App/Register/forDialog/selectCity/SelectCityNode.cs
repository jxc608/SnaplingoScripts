using UnityEngine;
using UnityEngine.UI;
using Snaplingo.UI;
using System.Collections.Generic;

class SelectCityNode : Node
{
	private int mCurrentSelectCityIndex;
	private string mCurrentSelectCityName;

	private List<List<CityConfigItem>> mCityConfig;

	private int mCurrentPageIndex = 0;

	public List<Button> mCityUIList;
	private List<SelectCityItem> mCityItemList;

	public Image mSelectFlagMc;
	public Button mLastBtn, mNextBtn, mSubmitBtn, mCancleBtn;

	public override void Open()
	{
		base.Open();

		mCityItemList = new List<SelectCityItem>();
		for (int i = 0; i < mCityUIList.Count; i++)
		{
			SelectCityItem cityItem = new SelectCityItem(mCityUIList[i], this);
			mCityItemList.Add(cityItem);
		}
		mCurrentPageIndex = 0;
		onClickNextPageHandle();

		mLastBtn.onClick.AddListener(onClickLastPageHandle);
		mNextBtn.onClick.AddListener(onClickNextPageHandle);
		mSubmitBtn.onClick.AddListener(onClickSubmitHandle);
		mCancleBtn.onClick.AddListener(onClickCancleHandle);
	}

	/// <summary>
	/// 选择国家
	/// </summary>
	public void selectCityHandle(SelectCityItem _item)
	{
		if (_item.isSelect) return;
		LogManager.Log("选择国家:" , _item.cityName , ", " , _item.cityIndex);
		mCurrentSelectCityIndex = _item.cityIndex;
		mCurrentSelectCityName = _item.cityName;

		foreach (SelectCityItem item in mCityItemList)
		{
			item.isSelect = false;
		}
		_item.isSelect = true;
		refreshDisplay();
	}

	private void refreshDisplay()
	{
		mSelectFlagMc.gameObject.SetActive(false);
		//刷新国旗显示 & 刷新选中显示
		foreach (SelectCityItem item in mCityItemList)
		{
			item.visible(false);
		}
		for (int i = 0; i < mCityItemList.Count; i++)
		{
			if (mCurrentPageIndex >= mCityConfig.Count)
			{
				if (i >= 2) break;
			}
			mCityItemList[i].visible(true);
			mCityItemList[i].refresh(mCityConfig[mCurrentPageIndex - 1][i].city, mCityConfig[mCurrentPageIndex - 1][i].index);
			if (mCityItemList[i].isSelect && mCityItemList[i].cityName == mCurrentSelectCityName)
			{
				mSelectFlagMc.gameObject.SetActive(true);
				mSelectFlagMc.transform.position = mCityItemList[i].position();
			}
		}

		//刷新按钮显示
		if (mCurrentPageIndex - 1 <= 0)
		{
			mLastBtn.gameObject.SetActive(false);
			mNextBtn.gameObject.SetActive(true);
		}
		else if (mCurrentPageIndex + 1 > mCityConfig.Count)
		{
			mNextBtn.gameObject.SetActive(false);
			mLastBtn.gameObject.SetActive(true);
		}
		else
		{
			mNextBtn.gameObject.SetActive(true);
			mLastBtn.gameObject.SetActive(true);
		}
	}

	/// <summary>
	/// 点击上一页
	/// </summary>
	private void onClickLastPageHandle()
	{
		mCurrentPageIndex--;
		refreshDisplay();
	}

	/// <summary>
	/// 点击下一页
	/// </summary>
	private void onClickNextPageHandle()
	{
		mCurrentPageIndex++;
		refreshDisplay();
	}

	/// <summary>
	/// 点击提交
	/// </summary>
	private void onClickSubmitHandle()
	{
		RegisterConfig.Temp_userCity = mCurrentSelectCityIndex;
		GlobalConst.Player_IsChina = mCurrentSelectCityIndex == I18NConfig.Country_Server_CN;
		PageManager.Instance.CurrentPage.AddNode<RegisterNode>(true);
		Close(true);
	}

	/// <summary>
	/// 点击关闭
	/// </summary>
	private void onClickCancleHandle()
	{
		Close(true);
	}

	public override void Init(params object[] args)
	{
		base.Init(args);

		mCityConfig = new List<List<CityConfigItem>>();
		List<CityConfigItem> page1JsonData = new List<CityConfigItem>();
		page1JsonData.Add(new CityConfigItem() { city = I18NConfig.Country_local_CN, index = I18NConfig.Country_Server_CN });
		page1JsonData.Add(new CityConfigItem() { city = I18NConfig.Country_local_US, index = I18NConfig.Country_Server_US });
		page1JsonData.Add(new CityConfigItem() { city = I18NConfig.Country_local_GB, index = I18NConfig.Country_Server_GB });
		page1JsonData.Add(new CityConfigItem() { city = I18NConfig.Country_local_CA, index = I18NConfig.Country_Server_CA });
		page1JsonData.Add(new CityConfigItem() { city = I18NConfig.Country_local_HK, index = I18NConfig.Country_Server_HK });
		page1JsonData.Add(new CityConfigItem() { city = I18NConfig.Country_local_EU, index = I18NConfig.Country_Server_EU });
		mCityConfig.Add(page1JsonData);

		List<CityConfigItem> page2JsonData = new List<CityConfigItem>();
		page2JsonData.Add(new CityConfigItem() { city = I18NConfig.Country_local_CR, index = I18NConfig.Country_Server_CR });
		page2JsonData.Add(new CityConfigItem() { city = I18NConfig.Country_local_AU, index = I18NConfig.Country_Server_AU });
		page2JsonData.Add(new CityConfigItem() { city = I18NConfig.Country_local_NZ, index = I18NConfig.Country_Server_NZ });
		page2JsonData.Add(new CityConfigItem() { city = I18NConfig.Country_local_IE, index = I18NConfig.Country_Server_IE });
		page2JsonData.Add(new CityConfigItem() { city = I18NConfig.Country_local_IL, index = I18NConfig.Country_Server_IL });
		page2JsonData.Add(new CityConfigItem() { city = I18NConfig.Country_local_ZA, index = I18NConfig.Country_Server_ZA });
		mCityConfig.Add(page2JsonData);

		List<CityConfigItem> page3JsonData = new List<CityConfigItem>();
		page3JsonData.Add(new CityConfigItem() { city = I18NConfig.Country_local_JM, index = I18NConfig.Country_Server_JM });
		page3JsonData.Add(new CityConfigItem() { city = I18NConfig.Country_local_SG, index = I18NConfig.Country_Server_SG });
		mCityConfig.Add(page3JsonData);
	}
}

class CityConfigItem
{
	public string city;
	public int index;
}
