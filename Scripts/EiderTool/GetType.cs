using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

class GetType : MonoBehaviour, IDeselectHandler
{
	public void OnDeselect(BaseEventData eventData)
	{
		GetComponent<Dropdown>().OnDeselect(eventData);
		int start = int.Parse(EiderSong.Instance.m_StartToEnd.Find("Start").Find("Text").GetComponent<Text>().text) - 1;
		int end = int.Parse(EiderSong.Instance.m_StartToEnd.Find("End").Find("Text").GetComponent<Text>().text) - 1;
		List<HitInfo> temp = EiderToolPage.Instance.SongObject.HitInfos;
		string type = EiderSong.Instance.m_StartToEnd.Find("Type").Find("Label").GetComponent<Text>().text;
		SentenceInfo.SentenceType sentenceType = SentenceInfo.SentenceType.ClickNode;
		if (type == "Click")
			sentenceType = SentenceInfo.SentenceType.ClickNode;
		else
		{
			sentenceType = SentenceInfo.SentenceType.SoundNode;
		}
		for (int i = start; i <= end; i++)
		{
			temp[i].StartHitTime = temp[start].HitTime;
			temp[i].EndHitTime = temp[end].EndHitTime;
			temp[i].Type = sentenceType;
		}
	}
}