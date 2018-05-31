using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using LitJson;
using UnityEngine;
using UnityEngine.UI;

public class SupportManager : MonoBehaviour
{
	//点赞测试uid："336696f6-9415-4bf4-8b1c-3cf30306cd25"
	private static SupportManager _instance = null;
	public static SupportManager Instance
	{
		get
		{
			if (_instance == null)
			{
				GameObject obj = new GameObject();
				_instance = obj.AddComponent<SupportManager>();
				obj.name = _instance.GetType().FullName;
			}
			return _instance;
		}
	}
	/// <summary>
	/// 获取用户获得的所有赞以及新增赞的数量
	/// </summary>
	/// <param name="uid">用户id.</param>
	/// <param name="paras">其他参数.</param>
	public void GetUserSupportCountFromUid(string uid, params GameObject[] paras)
	{
		JsonData data = new JsonData();
		data["uid"] = uid;
		DancingWordAPI.Instance.RequestLevelSupportFromServer(data, (string result) =>
		{
			JsonData resultData = JsonMapper.ToObject(result);
			if (resultData.TryGetString("code").Equals("1"))
			{
				int count = int.Parse(resultData["data"]["count"].ToJson());
				int newCount = int.Parse(resultData["data"]["newCount"].ToJson());
				LogManager.Log("一共有：" , count , " 人给你点赞啦！\n");
				LogManager.Log("今日新增：" , newCount , "人！");
				//设置UI上的变化
				paras[1].GetComponent<Text>().text = "+"+count.ToString();
				StartCoroutine(StartPlaySupportEffect(paras[0], paras[0].GetChildren().Length));
			}
		}, () =>
		{
			LogManager.Log("获取服务器数据失败，从本地拿数据！");
		});
	}
	/// <summary>
	/// 点赞之后上传数据到服务器
	/// </summary>
	/// <param name="fromUid">点赞者uid.</param>
	/// <param name="ToUid">获赞者uid.</param>
	/// <param name="levelId">关卡id.</param>
	/// <param name="paras">其他参数.</param>
	public void SetUserSupportToServer(string fromUid, string ToUid, int levelId, params GameObject[] paras)
	{
		LogManager.Log("自己的ID：" , fromUid);
		LogManager.Log("玩家的ID：" , ToUid);
		if(ToUid.Equals("npc")||string.IsNullOrEmpty(ToUid))
		{
			StartCoroutine(StartPlaySupportEffect(paras[0], 1));
			return;
		}
		JsonData data = new JsonData();
		data["fromUid"] = fromUid;
		data["ToUid"] = ToUid;
		data["levelId"] = levelId;
		DancingWordAPI.Instance.SubmitLevelSupportToServer(data, (string result) =>
		{
			LogManager.Log(result);
			JsonData resultData = JsonMapper.ToObject(result);
			if (resultData.TryGetString("code").Equals("1"))
			{
				LogManager.Log("点赞成功啦。");
				//设置UI上的变化+
				//paras[1].GetComponent<Image>().sprite = null;
				StartCoroutine(StartPlaySupportEffect(paras[0], 1));
			}
			else
			{
				LogManager.Log("点赞失败啦。");
			}
		}, () =>
		{
			LogManager.Log("数据存本地。");
		});
	}

	//public bool isOne;
	//public GameObject supportObjects;
	//public void PlaySupportEffect()
	//{
	//	if (isOne)
	//		StartCoroutine(StartPlaySupportEffect(supportObjects, 1));
	//	else
	//		StartCoroutine(StartPlaySupportEffect(supportObjects, supportObjects.GetChildren().Length));
	//}
	public IEnumerator StartPlaySupportEffect(GameObject supports, int count)
	{
		GameObject[] objs = supports.GetChildren();
		for (int i = objs.Length - 1; i >= objs.Length - count; i--)
		{
			//LogManager.Log("OK");
			objs[i].GetComponent<DOTweenAnimation>().DORestart();
			objs[i].GetComponent<DOTweenAnimation>().DOPlayForward();
			yield return new WaitForSeconds(0.3f);
		}
	}
}
