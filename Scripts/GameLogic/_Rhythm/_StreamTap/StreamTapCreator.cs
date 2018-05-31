using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StreamTapCreator 
{
    private StreamTap m_StreamTap;
    public void CreateWord(string beforeClick, string afterClick, int sentenceIndex)
    {
        m_StreamTap = new StreamTap();
        m_StreamTap.Init(beforeClick, afterClick, sentenceIndex);
    }

    public void Delete()
    {
		if(m_StreamTap != null)
			m_StreamTap.Delete();
    }
}
