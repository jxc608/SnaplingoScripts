using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.UI;

class GetBpm : MonoBehaviour, IDeselectHandler
{
	public void OnDeselect(BaseEventData eventData)
	{
		GetComponent<InputField>().OnDeselect(eventData);
		int Bpm = int.Parse(EiderToolPage.Instance.EiderPage.transform.Find("Bpm").Find("Text").GetComponent<Text>().text);
		if (Bpm == 0)
			EiderToolPage.Instance.EiderPage.transform.Find("Bpm").GetComponent<InputField>().text = EiderToolPage.Instance.SongObject.SongInfos.BPM.ToString();
		else
			EiderToolPage.Instance.SongObject.SongInfos.BPM = Bpm;
	}
}