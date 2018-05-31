using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Snaplingo.UI;

public class EditorModeSelectArea : MonoBehaviour ,IDragHandler, IEndDragHandler, IBeginDragHandler
{
    private Image m_SelectAreaImage;
    private Vector2 m_StartPos;
    private bool m_DraggingPoints;
	// Use this for initialization
	void Start () 
    {
        m_SelectAreaImage = PageManager.Instance.CurrentPage.GetNode<EditorModeNode>().transform.Find("EditPage/SelectResult").GetComponent<Image>();
        m_SelectAreaImage.rectTransform.sizeDelta = Vector2.zero;

        m_DraggingPoints = false;
	}
	
    private List<EditorTimeLineCircleNode> m_NodeList = new List<EditorTimeLineCircleNode>();
    private const float CanvasHeight = 1536f;
    public void SetSelectArea(Vector2 startPos, Vector2 endPos)
    {
        float ratio = PageManager.Instance.GetComponent<RectTransform>().sizeDelta.x / Screen.width;
        float width = Mathf.Abs(startPos.x - endPos.x) * ratio;
        float height = Mathf.Abs(startPos.y - endPos.y) * ratio;

        m_SelectAreaImage.rectTransform.sizeDelta = new Vector2(width, height);

        Vector2 canvasStartPos = UIUtils.GetCanvasPosFromTouchPosition(startPos);
        Vector2 canvasEndPos = UIUtils.GetCanvasPosFromTouchPosition(endPos);
        m_SelectAreaImage.rectTransform.localPosition = (canvasStartPos + canvasEndPos) / 2f;

        //FindAllCircleNode
        m_NodeList = EditorModeManager.Instance.GetCircleNodeList(new Vector2(m_SelectAreaImage.rectTransform.localPosition.x - width * 0.5f,
                                                                              m_SelectAreaImage.rectTransform.localPosition.y - height * 0.5f),
                                                                  new Vector2(m_SelectAreaImage.rectTransform.localPosition.x + width * 0.5f,
                                                                              m_SelectAreaImage.rectTransform.localPosition.y + height * 0.5f));
    }
   
    public void OnBeginDrag(PointerEventData data)
    {
        m_StartPos = m_SelectAreaImage.rectTransform.localPosition;
        if(m_NodeList.Count > 0)
        {
            m_DraggingPoints = true;
        }
    }

    public void OnDrag(PointerEventData data)
    {
        if(m_DraggingPoints)
        {
            Vector2 curPos = UIUtils.GetCanvasPosFromTouchPosition(InputUtils.GetTouchPosition());
            m_SelectAreaImage.rectTransform.localPosition = new Vector3(curPos.x, m_StartPos.y, 0);
        }
    }

    public void OnEndDrag(PointerEventData data)
    {
        //TODO
        if(m_DraggingPoints)
        {
            
        }
        m_DraggingPoints = false;
    }

    public void ClearNodeList()
    {
        m_NodeList.Clear();
        m_SelectAreaImage.rectTransform.sizeDelta = Vector2.zero;
    }

}
