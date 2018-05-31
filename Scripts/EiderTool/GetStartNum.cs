using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

class GetStartNum : MonoBehaviour, IDeselectHandler
{
	public void OnDeselect(BaseEventData eventData)
	{
		GetComponent<InputField>().OnDeselect(eventData);
		if (string.IsNullOrEmpty(EiderSong.Instance.m_StartToEnd.Find("Start").Find("Text").GetComponent<Text>().text) || string.IsNullOrEmpty(EiderSong.Instance.m_StartToEnd.Find("End").Find("Text").GetComponent<Text>().text))
			return;
		int start = int.Parse(EiderSong.Instance.m_StartToEnd.Find("Start").Find("Text").GetComponent<Text>().text) - 1;
		int end = int.Parse(EiderSong.Instance.m_StartToEnd.Find("End").Find("Text").GetComponent<Text>().text) - 1;
		if (end < start)
			return;
		List<HitInfo> temp = EiderToolPage.Instance.SongObject.HitInfos;
		string type = EiderSong.Instance.m_StartToEnd.Find("Type").Find("Label").GetComponent<Text>().text;
		SentenceInfo.SentenceType sentenceType = SentenceInfo.SentenceType.ClickNode;
		int ddd = 100;
		if (type == "Click")
			sentenceType = SentenceInfo.SentenceType.ClickNode;
		else
		{
			sentenceType = SentenceInfo.SentenceType.SoundNode;
			ddd = 1000;
		}

		if (start != 0)
		{
			for (int i = start - 1; i >= 0; i--)
			{
				if (temp[i].StartHitTime == temp[start - 1].StartHitTime)
					temp[i].EndHitTime = temp[start].HitTime - ddd;
				else
					break;
			}
		}

		if (end != temp.Count - 1)
		{
			for (int i = end + 1; i < temp.Count; i++)
			{
				if (temp[i].EndHitTime == temp[end + 1].EndHitTime)
					temp[i].StartHitTime = temp[end + 1].HitTime;
				else
					break;
			}
			if (temp[end + 1].Type == SentenceInfo.SentenceType.SoundNode)
				ddd = 1000;
			else
				ddd = 100;
			temp[end].EndHitTime = temp[end + 1].HitTime - ddd;
		}	
		else
		{
			temp[end].EndHitTime = temp[end].HitTime + 2000;
		}

		for (int i = start; i <= end; i++)
		{
			temp[i].StartHitTime = temp[start].HitTime;
			temp[i].EndHitTime = temp[end].EndHitTime;
			temp[i].Type = sentenceType;
		}
	}
}