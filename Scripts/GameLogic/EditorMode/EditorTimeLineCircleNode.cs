using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EditorTimeLineCircleNode : MonoBehaviour, IDragHandler, IEndDragHandler ,IBeginDragHandler
{
    private RectTransform m_RectTransform;
    private bool m_AlreadyGot;
    private Text m_Text;
    private bool m_Dragging;
    private int m_HOIndex;
    public int HOIndex
    {
        get { return m_HOIndex; }
    }
    void Start()
    {
        m_AlreadyGot = false;
        m_Dragging = false;
    }

    public void Init(int hoIndex)
    {
        m_HOIndex = hoIndex;

        GetComponents();
    }

    void GetComponents()
    {
        if (!m_AlreadyGot)
        {
            m_RectTransform = GetComponent<RectTransform>();
            m_Text = transform.Find("Text").GetComponent<Text>();
            m_AlreadyGot = true;
        }

        m_Text.text = m_HOIndex.ToString();
    }

    public Vector3 GetPosition()
    {
        return m_RectTransform.position;
    }

    public void OnBeginDrag(PointerEventData data)
    {
        m_RectTransform.position = new Vector3(Input.mousePosition.x, m_RectTransform.position.y, 0);
        EditorModeManager.Instance.Pause();
    }

    public void OnDrag(PointerEventData data)
    {
        m_RectTransform.position = new Vector3(Input.mousePosition.x, m_RectTransform.position.y, 0);
    }

    public void OnEndDrag(PointerEventData data)
    {
        m_RectTransform.position = new Vector3(Input.mousePosition.x, m_RectTransform.position.y, 0);
        float audioTime = EditorModeManager.Instance.Timer;
        float newTime = m_RectTransform.localPosition.x / EditorModeManager.Instance.TimeLineWidth * RuntimeConst.TimeRulerLength + audioTime;
        int newMS = (int)(newTime * RuntimeConst.SecondToMS);
    }

    public void UpdatePosition(float ratio)
    {
        m_RectTransform.localPosition = new Vector3(ratio * EditorModeManager.Instance.TimeLineWidth, 0, 0);
    }

    public void Delete()
    {
        ObjectPool.DeleteOne("EditorModeTimeLineNode", gameObject);
    }

}
