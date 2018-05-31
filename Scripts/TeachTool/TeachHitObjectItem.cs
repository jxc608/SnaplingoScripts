using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeachHitObjectItem : TeachSelectableItem 
{
    public int m_Index;
    public enum Type {LeftBox, Sentence, RightBox, ContentSentence, WordContent}
    private Type m_Type;
    private TeachSentenceEdit m_SentenceEdit;
    private TeachTool m_ContentEdit;

    public void Init(TeachSentenceEdit edit, Type type, int index)
    {
        m_SentenceEdit = edit;
        m_Type = type;
        m_Index = index;
    }

    public void ContentInit(TeachTool edit, Type type, int index)
    {
        m_ContentEdit = edit;
        m_Type = type;
        m_Index = index;
    }

    public override void OnPointerDown(UnityEngine.EventSystems.PointerEventData data)
    {
        SetSelect();
        switch(m_Type)
        {
            case Type.LeftBox:
                m_SentenceEdit.LeftBoxSingleSelect(this);
                break;
            case Type.RightBox:
                m_SentenceEdit.RightBoxSelect(this);
                break;
            case Type.Sentence:
                m_SentenceEdit.SelectOneSentence(this);
                break;
            case Type.ContentSentence:
                m_ContentEdit.SelectSentence(this);
                break;
            case Type.WordContent:
                m_ContentEdit.SelectWord(this);
                break;
        }
    }
}
