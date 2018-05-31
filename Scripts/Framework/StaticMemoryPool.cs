using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class StaticMemoryPool  
{
    public static Dictionary<string, UnityEngine.Object> m_Pool = new Dictionary<string, UnityEngine.Object>();

    public static void AddIntoPool(string key, UnityEngine.Object value)
    {
        if(m_Pool.ContainsKey(key))
        {
            m_Pool[key] = value;
            Resources.UnloadUnusedAssets();
        }
        else
        {
            m_Pool.Add(key, value);
        }
    }

    public static void ClearItem(string key)
    {
        if(m_Pool.ContainsKey(key))
        {
            m_Pool.Remove(key);
            Resources.UnloadUnusedAssets();
        }
    }

    public static T GetItem<T>(string key) where T : UnityEngine.Object
    {
        if(m_Pool.ContainsKey(key))
        {
            return m_Pool[key] as T;
        }
        else
        {
            return null;
        }
    }
}
