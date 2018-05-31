using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System;

public class DynamicClickHandler : MonoBehaviour, IPointerDownHandler 
{
    Action m_ClickCallback;
    public void SetCallback(Action action)
    {
        m_ClickCallback = action;
    }

    public void OnPointerDown(PointerEventData data)
    {
        if(m_ClickCallback != null)
        {
            m_ClickCallback.Invoke();
        }
        else
        {
            LogManager.LogError("Dynamic Click Handler ");
        }
    }

    public static void AddListener(GameObject obj, Action action)
    {
        DynamicClickHandler handler = obj.GetComponent<DynamicClickHandler>();
        if(handler == null)
        {
            handler = obj.AddComponent<DynamicClickHandler>();
        }
        handler.SetCallback(action);
    }
}
