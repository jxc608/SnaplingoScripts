using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Snaplingo.UI;
using UnityEngine.UI;

public class EditorModePointerControl : MonoBehaviour
{
    private enum PointerStatus { Idle, Down, Dragging }
    private PointerStatus m_PointerStatus;
    private Vector2 m_StartPos;
    private Vector2 m_CurPos;
    private EditorModeSelectArea m_SelectArea;
    // Use this for initialization
    void Start()
    {
        m_PointerStatus = PointerStatus.Idle;
        m_SelectArea = FindObjectOfType<EditorModeSelectArea>();
    }

    // Update is called once per frame
    void Update()
    {
        switch (m_PointerStatus)
        {
            case PointerStatus.Idle:
                PointerIdle();
                break;
            case PointerStatus.Down:
                PointerDown();
                break;
            case PointerStatus.Dragging:
                PointerDragging();
                break;
        }
    }

    void PointerIdle()
    {
        if(InputUtils.OnPressed())
        {
            if(!UIUtils.IsClickUI())
            {
                m_PointerStatus = PointerStatus.Down;
                m_StartPos = InputUtils.GetTouchPosition();
            }
        }
    }

    private const float StartDragValve = 0.05f;
    void PointerDown()
    {
        if(InputUtils.OnReleased())
        {
            m_PointerStatus = PointerStatus.Idle;
            m_SelectArea.ClearNodeList();
        }

        Vector2 vec = InputUtils.GetTouchPosition() - m_StartPos;
        if(vec.magnitude > StartDragValve)
        {
            m_PointerStatus = PointerStatus.Dragging;
        }
    }

    void PointerDragging()
    {
        if (InputUtils.OnReleased())
        {
            m_PointerStatus = PointerStatus.Idle;
        }

        m_SelectArea.SetSelectArea(m_StartPos, InputUtils.GetTouchPosition());
    }
}
