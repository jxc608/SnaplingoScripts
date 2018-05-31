using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TeachSelectableItem : MonoBehaviour, IPointerDownHandler 
{
    protected Image m_BG;
    protected Text m_Text;
    protected virtual void OnEnable()
    {
        m_BG = GetComponent<Image>();
        m_Text = transform.Find("Text").GetComponent<Text>();
        SetUnselect();
    }

    public virtual void OnPointerDown(PointerEventData data)
    {
       
    }

    public virtual void SetSelect()
    {
        m_BG.color = Color.blue;
    }

    public virtual void SetUnselect()
    {
        m_BG.color = Color.white;
    }

    public virtual void SetString(string content)
    {
        m_Text.text = content;
    }
}
