using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Snaplingo.UI;
using DG.Tweening;
using UnityEngine.UI;
using System;

public class RhythmVoiceCreator
{
	public RhythmVoiceCreator()
	{
	}

	public void CreateWords(SentenceObj sentenceObj, int sentenceIndex)
	{
		CorePlayManager.Instance.m_VoiceNum += 1;
		AnalysisManager.Instance.OnEvent("100003", null, StaticData.LevelID.ToString(), "创建语音数量");
		float duration = (sentenceObj.m_InOutTime.EndTime - sentenceObj.m_InOutTime.StartTime) * 0.001f;
		VoiceController.instance.CreateWord(sentenceObj.ClickAndHOList.HitObjects[0].Word, duration);
	}

	public void CreateWords(string sentence, float duration)
	{
		CorePlayManager.Instance.m_VoiceNum += 1;
		AnalysisManager.Instance.OnEvent("100003", null, StaticData.LevelID.ToString(), "创建语音数量");
		VoiceController.instance.CreateWord(sentence, duration);
	}

	public void HighLightWord(List<HitObject> hoList, int index, float preshowLength = 0)
	{
		VoiceController.HighLightWord(hoList, index, preshowLength);
	}

	public void BGFadeIn(Action callback = null)
	{
		VoiceController.Show(callback);
	}

	public void BGFadeOut(Action callback = null)
	{
		VoiceController.Hide(callback);
	}

	public void Restart()
	{
		if (VoiceController.instance == null) return;
		VoiceController.instance.Restart();
	}
}
