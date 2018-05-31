using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CorePlayCreateEvents
{
	public CorePlayCreateEvents()
	{

	}

	private const int AlmostEndValue = 2;
	public Queue<CorePlayTimerEvent> GetTimerEventQueue()
	{//按照事件发生的顺序来写入timer event 队列。  
		Queue<CorePlayTimerEvent> queue = new Queue<CorePlayTimerEvent>();

		int bpmDelta = (int)(CorePlayData.CurrentSong.BPM / 60f / 2f * 1000);
		#region CalcIntrada
		if (CorePlayData.CurrentSong.SentenceObjs[0].Type != SentenceType.BossStart)
		{
			CorePlayTimerEvent playMusic = new CorePlayTimerEvent(0, CorePlayTimerEvent.EventType.StartPlayMusic, 0, 0);
			queue.Enqueue(playMusic);
            int startCountTime = CorePlayData.CurrentSong.SentenceObjs[0].m_InOutTime.StartTime - (bpmDelta * 4);
			CorePlayTimerEvent startCount = new CorePlayTimerEvent(startCountTime, CorePlayTimerEvent.EventType.StartCount, 0, 0);
			queue.Enqueue(startCount);
		}
		#endregion

		#region Sentence And HighLight
		bool voiceShowing = false;
        //int beatMS = (int)(60000 - CorePlayData.CurrentSong.BPM);
		for (int i = 0; i < CorePlayData.CurrentSong.SentenceObjs.Count; i++)
		{
			SentenceObj so = CorePlayData.CurrentSong.SentenceObjs[i];

			switch (so.Type)
			{
                case SentenceType.Common:
                    int preShowTime = so.m_InOutTime.StartTime - CorePlaySettings.Instance.m_PreShowTimeLength;
					if (voiceShowing)
					{
						CorePlayTimerEvent endVoiceBG = new CorePlayTimerEvent(preShowTime, CorePlayTimerEvent.EventType.EndVoiceUI, 0, 0);
						queue.Enqueue(endVoiceBG);
						CorePlayTimerEvent endVoiceCheck = new CorePlayTimerEvent(preShowTime, CorePlayTimerEvent.EventType.EndVoiceChecker, 0, 0);
						queue.Enqueue(endVoiceCheck);
						voiceShowing = false;
					}

					// 1 创建 句子 事件
					CorePlayTimerEvent startTapSentence = new CorePlayTimerEvent(preShowTime, CorePlayTimerEvent.EventType.CreateTapSentence, i, 0);
					queue.Enqueue(startTapSentence);

					// 2 检查 点击 事件
					CorePlayTimerEvent startTapSentenceCheck = new CorePlayTimerEvent(preShowTime, CorePlayTimerEvent.EventType.StartTapChecker, i, 0);
					queue.Enqueue(startTapSentenceCheck);

                    //float higlLightTime = 0;
                    for (int j = 0; j < so.ClickAndHOList.HitObjects.Count; j++)
					{
  
                        //higlLightTime = temp * 0.001f;
                        int highLightTimer = so.ClickAndHOList.HitObjects[j].StartMilliSecond - CorePlaySettings.Instance.m_PreShowTimeLength;
						// 3 读!! 单词 事件
                        CorePlayTimerEvent highLightTap = new CorePlayTimerEvent(highLightTimer, CorePlayTimerEvent.EventType.HighLightTap, i, j, 0.4f);
						queue.Enqueue(highLightTap);
					}

					break;

				case SentenceType.BossStart:
                    CorePlayTimerEvent bossStart = new CorePlayTimerEvent(so.m_InOutTime.StartTime, CorePlayTimerEvent.EventType.StartBoss, 0, 0);
					queue.Enqueue(bossStart);
					break;

				case SentenceType.Voice:
                    int preVoiceShowTime = so.m_InOutTime.StartTime - CorePlaySettings.Instance.m_PreShowTimeLength;
					if (!voiceShowing)
					{
						// 00 显示 UI
						CorePlayTimerEvent startVoiceBG = new CorePlayTimerEvent(preVoiceShowTime, CorePlayTimerEvent.EventType.StartVoiceUI, 0, 0);
						queue.Enqueue(startVoiceBG);
						voiceShowing = true;
					}

					// 1 创建 句子 事件
					CorePlayTimerEvent startVoiceSentence = new CorePlayTimerEvent(preVoiceShowTime, CorePlayTimerEvent.EventType.CreateVoiceSentence, i, 0);
					queue.Enqueue(startVoiceSentence);

					// 2 检查 "语音" 事件  （ 普通关 每句 ）
                    CorePlayTimerEvent startVoiceSentenceCheck = new CorePlayTimerEvent(so.m_InOutTime.StartTime, CorePlayTimerEvent.EventType.StartVoiceChecker, i, 0);
					queue.Enqueue(startVoiceSentenceCheck);

                    for (int j = 0; j < so.ClickAndHOList.HitObjects.Count; j++)
					{
						int ms = (int)(CorePlayData.TB * CorePlaySettings.Instance.m_VoiceTBParam * 1000);
                        int highLightVoiceTime = so.ClickAndHOList.HitObjects[j].StartMilliSecond - ms;
						//LogManager.Log("  读!! 单词 事件 = " , j , "  / " , so.HitObjects[j].StartMilliSecond , "/ " , ms);
						// 3 读!! 单词 事件
						CorePlayTimerEvent highLightVoice = new CorePlayTimerEvent(highLightVoiceTime, CorePlayTimerEvent.EventType.HighLightVoice, i, j);
						queue.Enqueue(highLightVoice);
					}
					break;

                case SentenceType.KeyVoiceStart:
                    CorePlayTimerEvent keyVoice = new CorePlayTimerEvent(so.m_InOutTime.StartTime, CorePlayTimerEvent.EventType.StartKeyVoice, 0, 0);
                    queue.Enqueue(keyVoice);
                    break;
			}
		}
		#endregion

		return queue;
	}
}
