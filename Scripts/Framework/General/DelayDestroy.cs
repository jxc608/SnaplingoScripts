using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DelayDestroy : MonoBehaviour 
{
    public float m_LifeTime;
    float m_Timer;

	// Use this for initialization
	void Start () 
    {
        m_Timer = 0;
	}
	
	// Update is called once per frame
	void Update () 
    {
        m_Timer += Time.deltaTime;

        if(m_Timer >= m_LifeTime)
        {
            Destroy(gameObject);
        }
	}
}
