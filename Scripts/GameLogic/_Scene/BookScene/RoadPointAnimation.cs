using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadPointAnimation : MonoBehaviour {

    public float m_Delta;
    BezierLine m_BezierLine;
    bool m_AnimationStart;
    float m_AnimationTimer;
    int m_CurrentPointIndex;
    GameObject m_RoadPoint;
    List<GameObject> m_RoadPoints = new List<GameObject>();
	// Use this for initialization
	void Start () 
    {
        m_RoadPoint = Resources.Load("CorePlay/RoadPoint") as GameObject;
        m_BezierLine = GetComponent<BezierLine>();
	}
	
	// Update is called once per frame
	void Update () 
    {
        if(m_AnimationStart)
        {
            if(m_CurrentPointIndex < m_BezierLine.Points.Count)
            {
                m_AnimationTimer += Time.deltaTime;
                if (m_AnimationTimer >= m_Delta)
                {
                    m_AnimationTimer -= m_Delta;
                    CreatePoints();
                }
            }
            else
            {
                m_AnimationStart = false;
            }
        }
	}

    void CreatePoints()
    {
        GameObject obj = Instantiate(m_RoadPoint);
        m_RoadPoints.Add(obj);
        obj.transform.position = m_BezierLine.Points[m_CurrentPointIndex];
        obj.transform.LookAt(Camera.main.transform);
        obj.transform.SetParent(transform);
        m_CurrentPointIndex++;
    }

    void ClearAllRoadPoints()
    {
        for (int i = m_RoadPoints.Count - 1; i >= 0; i-- )
        {
            Destroy(m_RoadPoints[i]);
        }
        m_RoadPoints.Clear();
    }

    void StartAnimation()
    {
        ClearAllRoadPoints();
        m_AnimationStart = true;
        m_AnimationTimer = 0;
        m_CurrentPointIndex = 0;
    }
    private void OnGUI()
    {
        if(GUI.Button(new Rect(50,50,50,50),"points"))
        {
            StartAnimation();
        }
    }
}
