using System;
using System.Collections.Generic;
using Snaplingo.UI;
using UnityEngine.UI;
using UnityEngine;

public class EditBossNode : Node
{
	private Transform m_BossNode;
	public GameObject m_LineItem;
	public ScrollRect m_Scroll;
	public Button m_AverageTime;
	private InputField m_StartTime;
	private InputField m_DelayTime;

	private ContentBossNode m_ContentBossNode;
	private BossLineInfo m_BossInfo;
	private List<GameObject> m_BossLine = new List<GameObject>();

	public override void Init(params object[] args)
	{
		base.Init(args);
		m_BossNode = transform.Find("BossNode");
		
		m_StartTime = m_BossNode.Find("GetStartTime").GetComponent<InputField>();
		m_DelayTime = m_BossNode.Find("GetDelayTime").GetComponent<InputField>();

		m_BossNode.Find("Close").GetComponent<Button>().onClick.AddListener(OnClose);
		m_StartTime.onValueChanged.AddListener(StartTime);
		m_DelayTime.onValueChanged.AddListener(DelayTime);
		m_AverageTime.onClick.AddListener(PrinteResult);
		m_Scroll.content.Find("Add").Find("Add").GetComponent<Button>().onClick.AddListener(Add);

		m_ContentBossNode = transform.Find("ContentEditBossNode").GetComponent<ContentBossNode>();
		m_ContentBossNode.Init();
		m_ContentBossNode.Close();
	}

	private void PrinteResult()
	{
		int num = EiderToolPage.Instance.SongObject.BossInfos.BossLineObject.Count;
		float sumTime = 0;
		for (int i = 0; i < num; i++)
		{
			BossLineInfo temp = EiderToolPage.Instance.SongObject.BossInfos.BossLineObject[i];
			sumTime += temp.KeepTimeLength * 0.001f / temp.SoundText.Split('$').Length;
		}
		LogManager.Log("平均时间为：" , sumTime/num);

	}

	public override void Open()
	{
		base.Open();
		DisPlay();
	}

	public void DisPlay()
	{
		BossInfo temp = EiderToolPage.Instance.SongObject.BossInfos;
		m_StartTime.text = temp.StartTime.ToString();
		m_DelayTime.text = temp.DelayTime.ToString();
		int tempCount = temp.BossLineObject.Count;
		m_Scroll.content.GetComponent<RectTransform>().sizeDelta = new Vector2(m_Scroll.content.GetComponent<RectTransform>().sizeDelta.x, 110 * (tempCount + 1));
		if (m_BossLine.Count > temp.BossLineObject.Count)
		{
			for (int i = m_BossLine.Count - 1; i >= temp.BossLineObject.Count; i--)
			{
				Destroy(m_BossLine[i]);
				m_BossLine.RemoveAt(i);
			}
		}
		else
		{
			for (int i = m_BossLine.Count; i < temp.BossLineObject.Count; i++)
			{
				GameObject newLine = Instantiate(m_LineItem) as GameObject;
				newLine.GetComponentInChildren<Text>().text = (i + 1).ToString();
				newLine.transform.SetParent(m_Scroll.content);
				newLine.SetActive(true);
				newLine.transform.localScale = Vector3.one;
				newLine.transform.localPosition = new Vector3(750, 50 - 110 * (i + 2), 0);
				m_BossLine.Add(newLine);
			}
		}
	}

	private void OnClose()
	{
		EiderSong.Instance.m_EiderStatus = EiderSong.EiderStatus.Nomal;
		Close();
		
	}

	private void StartTime(string content)
	{
		EiderToolPage.Instance.SongObject.BossInfos.StartTime = int.Parse(content);
	}

	private void DelayTime(string content)
	{
		EiderToolPage.Instance.SongObject.BossInfos.DelayTime = int.Parse(content);
	}

	private void Add()
	{
		m_BossNode.gameObject.SetActive(false);
		m_ContentBossNode.Open();
		m_ContentBossNode.AddNew();
	}
}
