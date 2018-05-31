using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeachRhythmCreator  
{

    Dictionary<ClickObj, TeachRhythmTap> m_ClickObjs = new Dictionary<ClickObj, TeachRhythmTap>();
    private float m_EndTime;
    public  void CreateWords(SentenceObj sentenceObj, int sentenceIndex)
    { 
        Dictionary<ClickObj, TeachRhythmTap> temp = new Dictionary<ClickObj, TeachRhythmTap>();
        foreach (ClickObj kv in sentenceObj.ClickAndHOList.ClickObjs)
        {
            TeachRhythmTap tap = new TeachRhythmTap();
            temp.Add(kv,tap);
            tap.SetPosition(kv);
        }
        m_EndTime = sentenceObj.m_InOutTime.EndTime;
        m_ClickObjs = temp;
        StaticMonoBehaviour.Instance.StartCoroutine(SentenceLogicStream(temp));
    }

    public void HighLightWord(List<HitObject> hoList, int index, float preshowLength = 0)
    {
        List<ClickObj> keys = new List<ClickObj>(m_ClickObjs.Keys);
        int number = index + 1;
        for (int i = 0; i < keys.Count; i++)
        {
            if(number <= keys[i].m_ClickTimes)
            {
                m_ClickObjs[keys[i]].HighLight();
                return;
            }
            else
            {
                number -= keys[i].m_ClickTimes;
            }
        }
    }

    public void Restart()
    {
        if (m_ClickObjs != null)
        {
            foreach (KeyValuePair<ClickObj, TeachRhythmTap> kv in m_ClickObjs)
            {
                kv.Value.Delete();
            }
            m_ClickObjs.Clear();
        }
    }

    IEnumerator SentenceLogicStream(Dictionary<ClickObj, TeachRhythmTap> dic)
    {
        float waitTimeLength = m_EndTime / 1000f;
        while (AudioManager.Instance.GetBgmTime() < waitTimeLength)
        {
            yield return null;
        }
        foreach (KeyValuePair<ClickObj, TeachRhythmTap> kv in dic)
        {
            kv.Value.StartDim();
        }
        waitTimeLength += CorePlaySettings.Instance.m_StartFadeOutTimeLength / 1000f;
        while (AudioManager.Instance.GetBgmTime() < waitTimeLength)
        {
            yield return null;
        }
        foreach (KeyValuePair<ClickObj, TeachRhythmTap> kv in dic)
        {
            kv.Value.Delete();
        }

        dic.Clear();
    }
}
