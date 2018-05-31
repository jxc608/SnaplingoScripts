using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.UI;
using Snaplingo.UI;


public class GetSliderTime: MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
	private bool m_ChangeValue;

	public void Start()
	{
		m_ChangeValue = false;
	}

	private void Update()
	{
		if (m_ChangeValue == false)
		{
			EiderSong.Instance.transform.Find("PlayAudio").Find("PlayTime").GetComponent<Slider>().value = CorePlayMusicPlayer.Instance.GetBgmTime();
		}
		else
		{
			float audioTime = EiderSong.Instance.transform.Find("PlayAudio").Find("PlayTime").GetComponent<Slider>().value;
			CorePlayMusicPlayer.Instance.ResetSongTime(audioTime);
		}
	}

	public void OnPointerDown(PointerEventData data)
	{
		m_ChangeValue = true;
	}

	public void OnPointerUp(PointerEventData data)
	{
		m_ChangeValue = false;
	}
}
