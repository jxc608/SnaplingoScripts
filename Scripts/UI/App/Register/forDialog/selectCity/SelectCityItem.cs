using UnityEngine;
using UnityEngine.UI;

class SelectCityItem
{

	public bool isSelect;

	private SelectCityNode mManager;

	private Button parent;

	public string cityName;

	public int cityIndex;

	public SelectCityItem(Button _parent, SelectCityNode _manager)
	{
		parent = _parent;
		mManager = _manager;

		parent.onClick.AddListener(onClickSelectCityHandle);
	}

	public void refresh(string _cityName, int _cityIndex)
	{
		cityName = _cityName;
		cityIndex = _cityIndex;

		parent.GetComponent<Image>().sprite = Resources.Load<Sprite>("CityFlag/" + cityName + "");
		parent.GetComponent<Image>().SetNativeSize();
	}

	private void onClickSelectCityHandle()
	{
		mManager.selectCityHandle(this);
	}

	public void visible(bool visible)
	{
		parent.gameObject.SetActive(visible);
	}

	public Vector3 position()
	{
		return parent.transform.position;
	}

}
