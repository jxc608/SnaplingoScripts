using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Params 
{
    public object[] m_Params;

    public Params(params object[] param)
    {
        m_Params = param;
    }
}

public class SMEvent : UnityEvent<Params>{}

public class StatusMachine
{
    private Dictionary<string, SMEvent> m_StatusEvents = new Dictionary<string, SMEvent>();
    private string m_Status;
    public string Status
    {
        get { return m_Status; }
    }
    //private List<string> m_Default = new List<string>();
    public StatusMachine()
    {
        m_Status = "Idle";
    }

    public void RemoveListener(string triggerName, UnityAction<Params> listener)
    {
        if (m_StatusEvents.ContainsKey(triggerName))
        {
            m_StatusEvents[triggerName].RemoveListener(listener);
        }
    }

    public void RegisterEvent(string triggerName, UnityAction<Params> listener)
    {
        if(!m_StatusEvents.ContainsKey(triggerName))
        {
            SMEvent smEvent = new SMEvent();
            m_StatusEvents.Add(triggerName, smEvent);
        }

        m_StatusEvents[triggerName].AddListener(listener);
    }

    public void Trigger(List<string> foreStatus, string triggerName, Params param)
    {
        if(foreStatus.Contains(m_Status))
        {
            m_Status = triggerName;
            if(m_StatusEvents[triggerName] != null)
                m_StatusEvents[triggerName].Invoke(param);
        }
    }
}
