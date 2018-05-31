using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StreamTap : RhythmObject 
{
    public void Init(string beforeWord, string afterWord, int sentenceIndex)
    {
        m_HitWord = beforeWord;
        m_SentenceIndex = sentenceIndex;

        var tran = ObjectPool.GetOne("RhythmController").transform;

        m_RhythmController = tran.GetComponent<RhythmController>();
        m_RhythmController.Init(this,
                                beforeWord,
                                afterWord, Vector3.zero, EffectInOutTiming.Default);
        m_ClickAble = true;
    }

    public void ShowWord()
    {
        if (m_RhythmController != null)
        {
            //m_RhythmController.gameObject.SetActive(true);
            m_RhythmController.On_Show();
        }
    }
}
