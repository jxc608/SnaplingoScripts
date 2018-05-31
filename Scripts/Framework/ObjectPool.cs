using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ObjectPool 
{
    private static Dictionary<string, List<GameObject>> m_ObjectPool = new Dictionary<string, List<GameObject>>();
    private static Dictionary<string, string> m_ObjectPathDic = new Dictionary<string, string>();

    private static GameObject m_Pool;
    public static void RegisterPool(string type, int startCount ,string path)
    {
        if(!m_ObjectPool.ContainsKey(type))
        {
            m_ObjectPathDic.Add(type, path);
            m_ObjectPool[type] = new List<GameObject>();
            for (int i = 0; i < startCount; i++)
            {
                GameObject obj = GameObject.Instantiate(ResourceLoadUtils.Load<GameObject>(path));
                GameObject.DontDestroyOnLoad(obj);
                obj.transform.SetParent(GetPool().transform);
                obj.SetActive(false);
                obj.name = type;
                m_ObjectPool[type].Add(obj);
            }
        }
        else
        {
            int number = m_ObjectPool[type].Count;
            if (number < startCount)
            {
                for (int i = 0; i < startCount - number; i++)
                {
                    GameObject obj = GameObject.Instantiate(ResourceLoadUtils.Load<GameObject>(path));
                    GameObject.DontDestroyOnLoad(obj);
                    obj.transform.SetParent(GetPool().transform);
                    obj.SetActive(false);
                    obj.name = type;
                    m_ObjectPool[type].Add(obj);
                }
            }
        }
    }

    private static GameObject GetPool()
    {
        if(m_Pool == null)
        {
            m_Pool = new GameObject();
            GameObject.DontDestroyOnLoad(m_Pool);
            m_Pool.name = "ObjectPool";
        }
        return m_Pool;
    }

    public static GameObject GetOne(string type)
    {
        GameObject obj = null;
        if(m_ObjectPool.ContainsKey(type))
        {
            if(m_ObjectPool[type].Count > 0)
            {
                obj = m_ObjectPool[type][0];
                obj.SetActive(true);
                m_ObjectPool[type].RemoveAt(0);
            }
            else
            {
                obj = GameObject.Instantiate(ResourceLoadUtils.Load<GameObject>(m_ObjectPathDic[type]));
                GameObject.DontDestroyOnLoad(obj);
                obj.name = type;
            }
        }
        else
        {
            LogManager.LogError("Did not register before asking for GameObject!!");
        }

        return obj;
    }

    public static void DeleteOne(string type, GameObject obj)
    {
        if (obj == null)
            return;
        if(m_ObjectPool.ContainsKey(type))
        {
            obj.transform.SetParent(GetPool().transform);
            obj.SetActive(false);
            if(!m_ObjectPool[type].Contains(obj))
                m_ObjectPool[type].Add(obj);
        }
    }

    public static void ClearPool(string type)
    {
        if(m_ObjectPool.ContainsKey(type))
        {
            for (int i = m_ObjectPool[type].Count - 1; i >= 0; i--)
            {
                GameObject.Destroy(m_ObjectPool[type][i]);
            }
            m_ObjectPool[type].Clear();
        }
    }

    public static void ClearAll()
    {
        
    }
}
