using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Snaplingo.UI;

public class LineSpace
{
    public List<KeyValuePair<float, float>> Space = new List<KeyValuePair<float, float>>();
}

public struct PossibleRange
{
    public int LineIndex;
    public KeyValuePair<float, float> LeftToRightRange;
}

public class CorePlayBossTapCreator
{
    private int m_RandomSeed;
    private float[] m_LineHeight = { 2.8f, 0, -2.8f };
    private const float RectWidth = 11f;
    //private float[] m_LineHeight = { 0 };
    private LineSpace[] m_Lines ;
    public CorePlayBossTapCreator()
    {
        m_RandomSeed = (int)(Time.realtimeSinceStartup * 1000);
        m_Lines = new LineSpace[m_LineHeight.Length];
        for (int i = 0; i < m_LineHeight.Length; i++)
        {
            m_Lines[i] = new LineSpace();
        }
    }

    List<BossTap> m_TapDic = new List<BossTap>();
    public void CreateTap(List<string> sentence, List<string> showWords, float sentenceDuration, int sentenceIndex)
    {
        List<BossTap> tapDic = new List<BossTap>();
        List<string> keys = new List<string>();
        int number = sentence.Count;
        for (int i = 0; i < number; i++)
        {
            if(!keys.Contains(sentence[i]))
            {
                BossTap tap = new BossTap();
                tap.Init(showWords[i], sentence[i], Vector3.zero, sentenceIndex);
                tapDic.Add(tap); 
                keys.Add(sentence[i]);
            }
        }
        m_TapDic = tapDic;
        StaticMonoBehaviour.Instance.StartCoroutine(ResetTapPosition());
    }

    const float TopEdge = 0;
    const float BottomEdge = -HalfScreenHeight;
    IEnumerator ResetTapPosition()
    {
        yield return new WaitForEndOfFrame();
        bool done = SetButtonPositions();
        while(!done)
        {
            done = SetButtonPositions();
        }
    }

    private const float MinWidth = 4f;
    private float m_RectCenter;
    bool SetButtonPositions()
    {
        InitLineSpace();

        List<BossTap> tempTapList = new List<BossTap>();
        List<float> tempWidthList = new List<float>();

        for (int i = 0; i < m_TapDic.Count; i++)
        {
            float width = m_TapDic[i].GetScaler().x;
            if (width < MinWidth) width = MinWidth;
            if(tempWidthList.Count == 0)
            {
                tempWidthList.Add(width);
                tempTapList.Add(m_TapDic[i]);
            }
            else
            {
                for (int j = 0; j < tempWidthList.Count; j++)
                {
                    if(width > tempWidthList[j])
                    {
                        tempWidthList.Insert(j, width);
                        tempTapList.Insert(j, m_TapDic[i]);
                        break;
                    }
                    else if(j == tempWidthList.Count - 1)
                    {
                        tempWidthList.Add( width);
                        tempTapList.Add(m_TapDic[i]);
                        break;
                    }
                }
            }
        }

        for (int i = 0; i < tempTapList.Count; i++)
        {
            List<PossibleRange> possibleRanges = GetPossibleRange(tempTapList[i]);
            if (possibleRanges.Count == 0)
            {
                return false;
            }
            else
            {
                RefreshRandomSeed();
                int index = Random.Range(0, possibleRanges.Count);
                float leftEdge = possibleRanges[index].LeftToRightRange.Key + tempWidthList[i] * 0.5f;
                float rightEdge = possibleRanges[index].LeftToRightRange.Value - tempWidthList[i] * 0.5f;
                RefreshRandomSeed();
                float posX = Random.Range(leftEdge, rightEdge);

                tempTapList[i].SetPosition(new Vector3(posX, m_LineHeight[possibleRanges[index].LineIndex]));
                PossibleRange possibleRange = new PossibleRange();
                possibleRange.LineIndex = possibleRanges[index].LineIndex;
                possibleRange.LeftToRightRange = new KeyValuePair<float, float>(posX - tempWidthList[i] * 0.5f, posX + tempWidthList[i] * 0.5f);

                if (m_Lines[possibleRange.LineIndex].Space.Count == 0)
                {
                    m_Lines[possibleRange.LineIndex].Space.Add(possibleRange.LeftToRightRange);
                }
                else
                {
                    bool added = false;
                    for (int j = 0; j < m_Lines[possibleRange.LineIndex].Space.Count; j++)
                    {
                        if (possibleRange.LeftToRightRange.Key < m_Lines[possibleRange.LineIndex].Space[j].Key)
                        {
                            m_Lines[possibleRange.LineIndex].Space.Insert(j, possibleRange.LeftToRightRange);
                            added = true;
                            break;
                        }
                    }
                    if (!added)
                    {
                        m_Lines[possibleRange.LineIndex].Space.Add(possibleRange.LeftToRightRange);
                    }
                }
            }
        }

        return true;
    }

    private const int BottomLine = 2;
    void InitLineSpace()
    {
        foreach (LineSpace line in m_Lines)
        {
            line.Space.Clear();
        }

        //初始化随机区域
        RefreshRandomSeed();
        m_RectCenter = Random.Range(-HalfScreenWidth + RectWidth * 0.5f, HalfScreenWidth - RectWidth * 0.5f);

        //添加迷你大忘瓜所在的位置
        //RectTransform rectTrans = PageManager.Instance.CurrentPage.GetNode<BossWarNode>().transform.Find("UIMaskProgressBar/Point").GetComponent<RectTransform>();
        //float screenWidth = PageManager.Instance.CurrentPage.GetComponent<RectTransform>().sizeDelta.x;
        //float posX = rectTrans.localPosition.x / screenWidth * 2f * HalfScreenWidth;
        //if(posX - MinWidth * 0.6f < m_RectCenter + RectWidth*0.5f)
        //{
        //    float rightEdge;
        //    if(posX + MinWidth * 0.6f < m_RectCenter + RectWidth * 0.5f)
        //    {
        //        rightEdge = posX + MinWidth * 0.6f;
        //    }
        //    else
        //    {
        //        rightEdge = m_RectCenter + RectWidth * 0.5f;
        //    }

        //    m_Lines[BottomLine].Space.Add(new KeyValuePair<float, float>(posX - MinWidth * 0.6f, rightEdge));
        //}
    }

    void RefreshRandomSeed()
    {
        m_RandomSeed++;
        Random.InitState(m_RandomSeed);
    }

    private const float WidthExtra = 1f; 
    List<PossibleRange> GetPossibleRange(BossTap tap)
    {
        List<PossibleRange> result = new List<PossibleRange>();
        float width = tap.GetScaler().x + WidthExtra; 
        if (width < MinWidth) width = MinWidth;
        for (int i = 0; i < m_LineHeight.Length; i++)
        {
            if (m_Lines[i].Space.Count == 0)
            {
                PossibleRange posRan = new PossibleRange();
                posRan.LineIndex = i;
                posRan.LeftToRightRange = new KeyValuePair<float, float>(m_RectCenter - RectWidth * 0.5f, m_RectCenter + RectWidth * 0.5f);
                result.Add(posRan);
                continue;
            }
            else
            {
                for (int j = 0; j < m_Lines[i].Space.Count; j++)
                {
                    float leftEdge, rightEdge;
                    if (j == 0)
                    {
                        leftEdge = m_RectCenter - RectWidth * 0.5f;
                    }
                    else
                    {
                        leftEdge = m_Lines[i].Space[j - 1].Value;
                    }

                    if (j == m_Lines[i].Space.Count - 1)
                    {
                        rightEdge = m_RectCenter + RectWidth * 0.5f;
                    }
                    else
                    {
                        rightEdge = m_Lines[i].Space[j + 1].Key;
                    }


                    if(m_Lines[i].Space[j].Key - leftEdge > width)
                    {
                        PossibleRange posRan = new PossibleRange();
                        posRan.LineIndex = i;
                        posRan.LeftToRightRange = new KeyValuePair<float, float>(leftEdge, m_Lines[i].Space[j].Key);
                        result.Add(posRan);
                    }

                    if(j == m_Lines[i].Space.Count - 1)
                    {
                        if (rightEdge - m_Lines[i].Space[j].Value > width)
                        {
                            PossibleRange posRan = new PossibleRange();
                            posRan.LineIndex = i;
                            posRan.LeftToRightRange = new KeyValuePair<float, float>(m_Lines[i].Space[j].Value, rightEdge);
                            result.Add(posRan);
                        }
                    }
                }
            }
        }
        return result;
    }

    const float OSU_WIDTH = 512f;
    const float OSU_HEIGHT = 384f;
    const float ScreenWidth = 20.54f;
    const float ScreenHeight = 15.3f;
    const float HalfScreenWidth = 9.27f;
    const float HalfScreenHeight = 6.65f;
    Vector3 GetOneVector3()
    {
        return GetOneRandomVector3(HalfScreenWidth, -HalfScreenHeight, 0, 0);
    }
    Vector3 GetOneRandomVector3(float width, float heightMin, float heightMax, float deep)
    {
        Random.InitState(m_RandomSeed);
        m_RandomSeed++;
        return new Vector3(Random.Range(-width, width), Random.Range(heightMin, heightMax), Random.Range(-deep, deep));
    }

    void DeleteAll(List<BossTap> tapDic)
    {
        if (tapDic == null)
            return;
        foreach (BossTap tap in tapDic)
        {
            tap.Delete();
        }
        tapDic = null;
    }

    public void Delete()
    {
        DeleteAll(m_TapDic);
    }

    public void Restart()
    {
        DeleteAll(m_TapDic);
    }

    public void ShowWord()
    {
        if (m_TapDic == null)
            return;
        foreach (BossTap tap in m_TapDic)
        {
            tap.ShowWord();
        }
    }

    public void SetClickable()
    {
        if (m_TapDic == null)
            return;
        foreach (BossTap tap in m_TapDic)
        {
            tap.SetClickable();
        }
    }

    public void ResetButtonPos()
    {
        RefreshRandomSeed();
        float newX = Random.Range(-HalfScreenWidth + RectWidth * 0.5f, HalfScreenWidth - RectWidth * 0.5f);
        float newY = Random.Range(-1.4f, 1.4f);

        foreach(BossTap tap in m_TapDic)
        {
            tap.SetPosition(tap.GetPosition() + new Vector3(newX, newY, 0));
        }
    }
}
