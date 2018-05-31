using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.UI;

class GetSongName : MonoBehaviour, IDeselectHandler
{
	public void OnDeselect(BaseEventData eventData)
	{
		GetComponent<InputField>().OnDeselect(eventData);
		string SongName = EiderToolPage.Instance.SelectAudioPage.transform.Find("InputField").Find("Text").GetComponent<Text>().text;
		EiderToolPage.Instance.SongObject.SongInfos.SongName = SongName;
        SongTxtObject ho = ReadSongEider.GetSongInfo(SongName);
		if (ho != null)
		{
			EiderToolPage.Instance.SongObject.SongInfos.BPM = ho.SongInfos.BPM;
			EiderToolPage.Instance.SongObject.HitInfos = ho.HitInfos;
			EiderToolPage.Instance.SongObject.BossInfos = ho.BossInfos;
		}
		else
		{
			EiderToolPage.Instance.SongObject.HitInfos.Clear();
			EiderToolPage.Instance.SongObject.SongInfos.BPM = 100;
		}
	}
}