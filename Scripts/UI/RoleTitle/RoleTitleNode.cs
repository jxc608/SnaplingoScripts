using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using LitJson;
using Snaplingo.SaveData;
using Snaplingo.UI;
using UnityEngine;
using UnityEngine.UI;

public class RoleTitleNode : Node
{
	/// <summary>
	/// 关闭称号界面的按钮
	/// </summary>
	public Button closeButton;
	/// <summary>
	/// 称号UI项的父物体
	/// </summary>
	public Transform parentTrans;
	/// <summary>
	/// 称号项
	/// </summary>
	public GameObject roleTitleItem;
	/// <summary>
	/// 所有称号游戏物体的集合
	/// </summary>
	private List<GameObject> roleTitleList;
	/// <summary>
	/// 可穿戴的称号的集合
	/// </summary>
	private List<RoleTitleItem> roleTitleItemList;
	/// <summary>
	/// 所有称号项的数据集合
	/// </summary>
	private List<RoleTitleItemObject> roleTitleObjectList;

	private void Start()
	{
		roleTitleList = new List<GameObject>();
		roleTitleItemList = new List<RoleTitleItem>();
	}
	public override void Init(params object[] args)
	{
		base.Init(args);
		closeButton.onClick.AddListener(OnClickCloseCallBack);
	}
	private void OnClickCloseCallBack()
	{
		//print("OK");
		GetComponent<DOTweenAnimation>().DOPlayBackwards();
		SaveDataUtils.Save<SelfPlayerRoleTitleData>();
		DancingWordAPI.Instance.UpDateServerRoleTitleInfo(SelfPlayerRoleTitleData.RoleTitleList);
	}
	public override void Open()
	{
		base.Open();
		ClearRoleTitleList();
		ClearRoleTitleItemList();
		//从服务器请求数据并存本地
		GetRoleTitleDataFromServer();
	}
	/// <summary>
	/// 从服务器获取当前用户的所有称号数据并保存到本地
	/// </summary>
	private void GetRoleTitleDataFromServer()
	{
		JsonData data = new JsonData();
		data["userID"] = SelfPlayerData.Uuid;
		DancingWordAPI.Instance.RequestRoleTitleFromServer(data, (string result) =>
		{
			//print(result);
			JsonData jsonResult = JsonMapper.ToObject(result)["data"];
			roleTitleObjectList = new List<RoleTitleItemObject>();
			for (int i = 0; i < jsonResult.Count; i++)
			{
				RoleTitleItemObject roleTitleItemObject = new RoleTitleItemObject();
				roleTitleItemObject.roletitleId = int.Parse(jsonResult[i].TryGetString("titleID"));
				roleTitleItemObject.roletitleCount = int.Parse(jsonResult[i].TryGetString("progress"));
				roleTitleItemObject.roleTitleStatus = bool.Parse(jsonResult[i].TryGetString("isUsed")) != true ? RoleTitleStatus.NotWear : RoleTitleStatus.Weared;
				roleTitleObjectList.Add(roleTitleItemObject);
			}
			roleTitleObjectList.Sort();
			SelfPlayerRoleTitleData.RoleTitleList = roleTitleObjectList;
			SaveDataUtils.Save<SelfPlayerRoleTitleData>();
			CreateRoleTitleItem();
		}, () =>
		{
			SaveDataUtils.Load<SelfPlayerRoleTitleData>();
			SelfPlayerRoleTitleData.RoleTitleList.Sort();
			roleTitleObjectList = SelfPlayerRoleTitleData.RoleTitleList;
			if (roleTitleObjectList != null)
			{
				CreateRoleTitleItem();
			}
		});
	}
	/// <summary>
	/// 清除角色称号项
	/// </summary>
	private void ClearRoleTitleList()
	{
		if (roleTitleList == null) return;
		for (int i = roleTitleList.Count - 1; i >= 0; i--)
		{
			//print("OK");
			Destroy(roleTitleList[i]);
			roleTitleList.RemoveAt(i);
		}
	}
	/// <summary>
	/// 清除已经完成的角色称号项
	/// </summary>
	private void ClearRoleTitleItemList()
	{
		if (roleTitleItemList == null) return;
		for (int i = roleTitleItemList.Count - 1; i >= 0; i--)
		{
			roleTitleItemList.RemoveAt(i);
		}
	}
	/// <summary>
	/// 创建当前所有的角色称号项
	/// </summary>
	private void CreateRoleTitleItem()
	{
		if (roleTitleList == null) return;

		foreach (RoleTitleItemObject item in roleTitleObjectList)
		{
			Dictionary<string, string> tempDic = RoleTitleConfig.Instance.GetRoleTitleItem(item);
			GameObject obj = Instantiate(roleTitleItem, parentTrans);
			obj.name = tempDic[RoleTitleEnum.RoleTitleId.ToString()];
			obj.GetComponent<RoleTitleItem>().roleTitleText.text = tempDic[RoleTitleEnum.RoleTitleDesc.ToString()];
			obj.GetComponent<RoleTitleItem>().roleTitleSliderValue.text = tempDic[RoleTitleEnum.RoleTitleCount.ToString()] + "/" + tempDic[RoleTitleEnum.RoleTitleLevel.ToString()];
			obj.GetComponent<RoleTitleItem>().roleTitleSlider.value = int.Parse(tempDic[RoleTitleEnum.RoleTitleCount.ToString()]) * 1.0f / int.Parse(tempDic[RoleTitleEnum.RoleTitleLevel.ToString()]);
			RoleTitleResourcesEnum roleTitleResourcesEnum = RoleTitleResourcesEnum.unfinish;
			if (tempDic[RoleTitleEnum.RoleTitleIsCanWear.ToString()].Equals(RoleTitleStatus.CanWear.ToString()))
			{
				obj.GetComponent<RoleTitleItem>().roleTitleButton.interactable = true;
				roleTitleResourcesEnum = RoleTitleResourcesEnum.canweared;
				if (tempDic[RoleTitleEnum.RoleTitleIsWeared.ToString()].Equals(RoleTitleStatus.Weared.ToString()))
				{
					roleTitleResourcesEnum = RoleTitleResourcesEnum.weared;
				}
				roleTitleItemList.Add(obj.GetComponent<RoleTitleItem>());
			}
			else
			{
				roleTitleResourcesEnum = RoleTitleResourcesEnum.unfinish;
				obj.GetComponent<RoleTitleItem>().roleTitleButton.interactable = false;
			}
			obj.GetComponent<RoleTitleItem>().roleTitleResourcesEnum = roleTitleResourcesEnum;
			obj.GetComponent<RoleTitleItem>().roleTitleButton.GetComponent<Image>().sprite = Resources.Load<Sprite>("RoleTitle/" + roleTitleResourcesEnum.ToString());
			roleTitleList.Add(obj);
		}
	}
	/// <summary>
	/// 更新角色称号的穿戴状态
	/// </summary>
	/// <param name="item">Item.</param>
	public void UpdateRoleTitleStatus(RoleTitleItem item)
	{
		if (item.roleTitleResourcesEnum == RoleTitleResourcesEnum.weared)
		{
			item.roleTitleButton.interactable = true;
			SelfPlayerRoleTitleData.Instance.UpdateRoleTitleStatus(int.Parse(item.gameObject.name), RoleTitleStatus.NotWear);
			item.roleTitleResourcesEnum = RoleTitleResourcesEnum.canweared;
			item.roleTitleButton.GetComponent<Image>().sprite = Resources.Load<Sprite>("RoleTitle/" + RoleTitleResourcesEnum.canweared.ToString());
			return;
		}
		item.roleTitleResourcesEnum = RoleTitleResourcesEnum.weared;
		SelfPlayerRoleTitleData.Instance.UpdateRoleTitleStatus(int.Parse(item.gameObject.name), RoleTitleStatus.Weared);
		item.roleTitleButton.GetComponent<Image>().sprite = Resources.Load<Sprite>("RoleTitle/" + RoleTitleResourcesEnum.weared.ToString());
		print(roleTitleItemList.Count);
		for (int i = 0; i < roleTitleItemList.Count; i++)
		{
			if (roleTitleItemList[i] != item)
			{
				roleTitleItemList[i].roleTitleButton.interactable = true;
				roleTitleItemList[i].roleTitleResourcesEnum = RoleTitleResourcesEnum.canweared;
				SelfPlayerRoleTitleData.Instance.UpdateRoleTitleStatus(int.Parse(roleTitleItemList[i].gameObject.name), RoleTitleStatus.NotWear);
				roleTitleItemList[i].roleTitleButton.GetComponent<Image>().sprite = Resources.Load<Sprite>("RoleTitle/" + RoleTitleResourcesEnum.canweared.ToString());
			}
		}
	}
}
