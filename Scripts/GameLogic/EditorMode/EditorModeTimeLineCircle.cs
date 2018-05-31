using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Snaplingo.UI;
using UnityEngine.UI;

public class EditorModeTimeLineCircle : MonoBehaviour 
{
    private RectTransform m_Ruler;
    private GameObject m_Ruler1;
    private GameObject m_Ruler2;
    public Color[] m_ColorGroup;

    private float m_ScreenWidth;
    public float ScreenWidth 
    {
        get { return m_ScreenWidth; }
    }
    public void Init()
    {
        m_Ruler = transform.Find("MovingRuler").GetComponent<RectTransform>();
        m_Ruler1 = transform.Find("MovingRuler/Ruler1").gameObject;
        m_Ruler2 = transform.Find("MovingRuler/Ruler2").gameObject;
        SetRulerSize();
    }

    void SetRulerSize()
    {
        m_ScreenWidth = PageManager.Instance.CurrentPage.GetComponent<RectTransform>().sizeDelta.x;
        m_Ruler1.transform.Find("Cibiao1").SetRectWidth(m_ScreenWidth);
        m_Ruler1.transform.Find("Jizhun").SetRectWidth(m_ScreenWidth);
        m_Ruler2.transform.Find("Cibiao1").SetRectWidth(m_ScreenWidth);
        m_Ruler2.transform.Find("Jizhun").SetRectWidth(m_ScreenWidth);
        RectTransform rt = m_Ruler2.GetComponent<RectTransform>();
        rt.localPosition = new Vector3(m_ScreenWidth, rt.localPosition.y, 0);
    }

    public void ChangeRulerNumber(int scale)
    {
        int num = m_Ruler1.transform.childCount - 1;
        if (num < scale)
        {
            for (int i = num; i < scale; i++)
            {
                GameObject child = Instantiate(m_Ruler1.transform.Find("Cibiao1").gameObject);
                child.name = "Cibiao" + (i + 1).ToString();
                child.GetComponent<Image>().color = m_ColorGroup[i];
                child.transform.SetParent(m_Ruler1.transform);
                child.GetComponent<RectTransform>().localScale = Vector3.one;
            }
        }
        else if (num > scale)
        {
            for (int i = scale; i < num; i++)
            {
                Destroy(m_Ruler1.transform.Find("Cibiao" + (i + 1).ToString()).gameObject);
            }
        }
        else
            return;

        m_Ruler1.transform.Find("Jizhun").SetAsLastSibling();

        RectTransform rectTransfrom = m_Ruler1.transform.Find("Cibiao1").GetComponent<RectTransform>();
        Vector3 position = rectTransfrom.localPosition;
        float width = rectTransfrom.sizeDelta.x;
        float Delta = rectTransfrom.sizeDelta.x / RuntimeConst.TimeRulerLength / scale;
        for (int i = 2; i <= scale; i++)
        {
            Vector3 local = position + new Vector3(Delta * (i - 1), rectTransfrom.localPosition.y, 0);
            m_Ruler1.transform.Find("Cibiao" + i).GetComponent<RectTransform>().localPosition = local;
        }

        Destroy(m_Ruler2);
        m_Ruler2 = Instantiate(m_Ruler1);
        m_Ruler2.name = "Cibiao2";
        m_Ruler2.transform.SetParent(m_Ruler1.transform.parent);
        m_Ruler2.GetComponent<RectTransform>().localScale = Vector3.one;
        m_Ruler2.GetComponent<RectTransform>().localPosition = m_Ruler1.GetComponent<RectTransform>().localPosition + new Vector3(width, 0, 0);
    }

    public void GetHOList()
    {
        m_HOList.Clear();
        if(CorePlayData.CurrentSong != null)
        {
            int counter = 0;
            for (int i = 0; i < CorePlayData.CurrentSong.SentenceObjs.Count; i++)
            {
                for (int j = 0; j < CorePlayData.CurrentSong.SentenceObjs[i].ClickAndHOList.HitObjects.Count;j++)
                {
                    m_HOList.Add(counter,CorePlayData.CurrentSong.SentenceObjs[i].ClickAndHOList.HitObjects[j]);
                    counter++;
                }
            }
        }
    }

    private Dictionary<int, HitObject> m_HOList = new Dictionary<int, HitObject>();
    private Dictionary<int, EditorTimeLineCircleNode> m_NodeMap = new Dictionary<int, EditorTimeLineCircleNode>();
    public void UpdateRuler(float timer)
    {
        MoveRuler(timer);
        MoveNodes(timer);
    }

    private void MoveRuler(float timer)
    {
        float param = timer % RuntimeConst.TimeRulerLength / RuntimeConst.TimeRulerLength;

        m_Ruler.localPosition = new Vector3(-param * ScreenWidth, m_Ruler.localPosition.y, 0);
    }

    private void MoveNodes(float timer)
    {
        foreach(KeyValuePair<int, EditorTimeLineCircleNode> kv in m_NodeMap)
        {
            kv.Value.Delete();
        }
        m_NodeMap.Clear();


        int leftMS = 0;
        if(timer - 6 >= 0)
        {
            leftMS = (int)((timer - RuntimeConst.TimeRulerHalfLength) * RuntimeConst.SecondToMS);
        }

        int rightMS = (int)((timer + RuntimeConst.TimeRulerHalfLength) * RuntimeConst.SecondToMS);

        List<int> keyList = new List<int>(m_HOList.Keys);
        for (int i = 0; i < keyList.Count; i++)
        {
            if(m_HOList[i].StartMilliSecond >= leftMS && m_HOList[i].StartMilliSecond <= rightMS)
            {
                GameObject obj = ObjectPool.GetOne("EditorModeTimeLineNode");
                obj.transform.SetParent(PageManager.Instance.CurrentPage.transform);
                obj.transform.localScale = Vector3.one;
                EditorTimeLineCircleNode circle = obj.GetComponent<EditorTimeLineCircleNode>();
                circle.Init(i + 1);
                m_NodeMap.Add(i, circle);
                float ratio = (m_HOList[i].StartMilliSecond - timer * RuntimeConst.SecondToMS) / (RuntimeConst.SecondToMS * RuntimeConst.TimeRulerLength);
                circle.UpdatePosition(ratio);
            }
        }
    }

    public List<EditorTimeLineCircleNode> GetCircleNodeList(Vector2 startPos, Vector2 endPos)
    {
        List<EditorTimeLineCircleNode> list = new List<EditorTimeLineCircleNode>();
        foreach(KeyValuePair<int, EditorTimeLineCircleNode> kv in m_NodeMap)
        {
            Vector3 pos = kv.Value.GetPosition();
            if(pos.x >= startPos.x && pos.x <= endPos.x && pos.y >= startPos.y && pos.y <= endPos.y)
            {
                list.Add(kv.Value);
            }
        }

        return list;
    }
}
