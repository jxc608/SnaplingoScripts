using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RhythmTapCreator
{
	#region [ --- Property --- ]
	Dictionary<string, RhythmTap> m_TapDic = new Dictionary<string, RhythmTap>();
	Dictionary<string, RhythmTap> m_CacheTapDic = new Dictionary<string, RhythmTap>();
	#endregion


	public RhythmTapCreator()
	{

	}

	private CorePlayManager m_Manager;
	public void SetManager(CorePlayManager manager)
	{
		m_Manager = manager;
	}

	#region [ --- Public --- ]
	public void CreateWords(SentenceObj sentenceObj, int sentenceIndex)
	{
		Dictionary<string, RhythmTap> tapDic = new Dictionary<string, RhythmTap>();
		float checkDuration = 0.001f * (sentenceObj.m_InOutTime.EndTime - sentenceObj.m_InOutTime.StartTime + 30);

		EffectInOutTiming inOut = sentenceObj.m_InOutTime.InOut;

		foreach (ClickObj co in sentenceObj.ClickAndHOList.ClickObjs)
		{
			if (!tapDic.ContainsKey(co.m_Word))
			{
				RhythmTap tap = CreateWord(m_Manager, co, inOut, sentenceIndex);
				tapDic.Add(co.m_Word, tap);
				if (co.m_ClickTimes > 1)
				{
					tap.SetTicWord();
				}
			}
			else
			{
				tapDic[co.m_Word].SetTicWord();
			}
		}
		m_TapDic = tapDic;

		//	LogManager.Log(" 1 计划时间 " , (Time.time + preShowDuration).ToString());
		StaticMonoBehaviour.Instance.StartCoroutine(SentenceLogicStream(checkDuration, tapDic, inOut));
	}

	public RhythmTap CreateWord(CorePlayManager manager, ClickObj clickWord, EffectInOutTiming inOut = EffectInOutTiming.Default, int sentenceIndex = 0)
	{
		RhythmTap tap = new RhythmTap();
		tap.Start(manager, clickWord, inOut, sentenceIndex);
		return tap;
	}

	public void HighLightWord(List<HitObject> hoList, int index, float preshowLength = 0)
	{
		//  
		// Temp Event
		//CorePlayTempHandler.highLightEvent.Invoke(hoList[index].Word);


		if (m_CacheTapDic == null)
			return;
		foreach (KeyValuePair<string, RhythmTap> kv in m_CacheTapDic)
		{
			if (kv.Value != null && kv.Value.contrller != null)
				kv.Value.contrller.On_HighLight(hoList[index].Word, preshowLength);
		}

	}

	public void Restart()
	{
		DeleteAll(m_TapDic);
		DeleteAll(m_CacheTapDic);
	}

	public void Close() { }
	#endregion

	#region [ --- Private --- ]
	IEnumerator SentenceLogicStream(float duration,
									Dictionary<string, RhythmTap> tapDic,
								   EffectInOutTiming inOut)
	{
		m_CacheTapDic = tapDic;

		yield return new WaitForSeconds(duration);

		// 得到语句结束判断   语句结束动画
		//foreach (KeyValuePair<string, RhythmTap> kv in tapDic)
		//{
		//	if (kv.Value != null)
		//	{
		//		kv.Value.OnSentenceEnd(CorePlayTempHandler.inputCheckCorrect);
		//	}
		//}

		// 语句渐隐
		//if (tapDic != null)
		//{
		//	foreach (KeyValuePair<string, RhythmTap> kv in tapDic)
		//	{
		//		kv.Value.StartFade();
		//	}
		//}
		//yield return new WaitForSeconds(0.5f);
		if (inOut == EffectInOutTiming.Both || inOut == EffectInOutTiming.DelayOut)
		{
			float delay = 0.001f * CorePlaySettings.Instance.m_PreShowTimeLength;
			yield return new WaitForSeconds(delay);
			//LogManager.LogWarning("  = 这句话推迟删除啦 " , Time.frameCount , " / " , delay);
		}

		DeleteAll(tapDic);
	}

	void DeleteAll(Dictionary<string, RhythmTap> tapDic)
	{
		if (tapDic == null)
			return;
		foreach (KeyValuePair<string, RhythmTap> kv in tapDic)
		{
			kv.Value.Delete();
		}
		tapDic = null;
	}
	#endregion
}
