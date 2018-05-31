using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(BezierLine)), CanEditMultipleObjects]
public class BezierLineEditor : Editor 
{
    Transform m_Root;
    BezierLine m_Self;
    void OnEnable()
    {
        m_Self = target as BezierLine;
        m_Root = m_Self.transform;

        InitAnchor();
    }

    void InitAnchor()
    {
       
    }

    private void OnDisable()
    {
        
    }

    void OnInspectorGUI()
    {
        DrawDefaultInspector();
    }

    void OnSceneGUI()
    {
        if (m_Self == null)
            return;

        EditorGUI.BeginChangeCheck();
        Vector3[] tempPos ={Vector3.zero, Vector3.zero, Vector3.zero} ;

        for (int i = 0; i < BezierLine.AnchorNumber; i++)
        {
            tempPos[i] = Handles.PositionHandle(m_Self.m_AnchorPos[i], m_Root.rotation);
        }
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(m_Self, "move Anchor");
          
            for (int i = 0; i < BezierLine.AnchorNumber; i++)
            {
                m_Self.m_AnchorPos[i] = tempPos[i];
            }
        }

        GUIStyle style = new GUIStyle();
        style.normal.textColor = Color.green;
        for (int i = 0; i < BezierLine.AnchorNumber; i++)
        {
            string s = (i + 1).ToString();
            Handles.Label(tempPos[i] , s, style);
        }

        CalcBezier();
        DrawBezeir();
    }

    void CalcBezier()
    {
        m_Self.m_BezierPoints.Clear();
        m_Self.m_BezierPoints = BezierLine.GetEqualSpaceBezier(m_Root.position,
                                                               m_Self.m_AnchorPos[BezierLine.FirstAnchor],
                                                               m_Self.m_AnchorPos[BezierLine.SecondAnchor],
                                                               m_Self.m_AnchorPos[BezierLine.ThirdAnchor], m_Self.m_PointNumber);
    }

    void DrawBezeir()
    {
        for (int i = 0; i < m_Self.m_PointNumber - 1; i++)
        {
            Handles.DrawLine(m_Self.m_BezierPoints[i], m_Self.m_BezierPoints[i + 1]);
        }
    }
}
