using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeachCreateEvents : MonoBehaviour {
    public TeachCreateEvents()
    {
        
    }

    public Queue<CorePlayTimerEvent> GetTimerEventQueue()
    {//按照事件发生的顺序来写入timer event 队列。  
        
        Queue<CorePlayTimerEvent> queue = new Queue<CorePlayTimerEvent>();

        int bpmDelta = (int)(CorePlayData.CurrentSong.BPM / 60f / 2f * 1000);
        #region CalcIntrada

        CorePlayTimerEvent playMusic = new CorePlayTimerEvent(0, CorePlayTimerEvent.EventType.StartPlayMusic, 0, 0);
        queue.Enqueue(playMusic);
      
        #endregion

        int preTime = bpmDelta > CorePlaySettings.Instance.m_PreShowTimeLength ? CorePlaySettings.Instance.m_PreShowTimeLength : bpmDelta;

        #region Sentence And HighLight

        for (int i = 0; i < CorePlayData.CurrentSong.SentenceObjs.Count; i++)
        {
            SentenceObj so = CorePlayData.CurrentSong.SentenceObjs[i];

            switch (so.Type)
            {
                case SentenceType.Common:
                    int preShowTime = so.m_InOutTime.StartTime - preTime;

                    CorePlayTimerEvent startTapSentence = new CorePlayTimerEvent(preShowTime, CorePlayTimerEvent.EventType.CreateTapSentence, i, 0);
                    queue.Enqueue(startTapSentence);
   
                    for (int j = 0; j < so.ClickAndHOList.HitObjects.Count; j++)
                    {
                        CorePlayTimerEvent highLightTap = new CorePlayTimerEvent(so.ClickAndHOList.HitObjects[j].StartMilliSecond, CorePlayTimerEvent.EventType.HighLightTap, i, j);
                        queue.Enqueue(highLightTap);
                    }
                    break;
            }
        }
        #endregion

        return queue;
    }
}
