using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Text;

public class CleanSentence
{
    public int m_Index;
    public List<CO> m_COList = new List<CO>();
    public List<HO> m_HOList = new List<HO>();
    public Dictionary<string, string> m_WordMap = new Dictionary<string, string>();
    public int m_StartTime;
    public int m_EndTime;
    public string m_Content;

    public void RefreshContent()
    {
        m_Content = m_Index + " : 开始时间点 - " + m_StartTime + ", 结束时间点 - " + m_EndTime;
    }

    public override string ToString()
    {
        if (string.IsNullOrEmpty(m_Content))
        {
            m_Content = m_Index + " : 开始时间点 - " + m_StartTime + ", 结束时间点 - " + m_EndTime;
        }

        return m_Content;
    }
}

public class HO
{
    public int m_Index;
    public int m_StartTime;
    public Vector2 m_Position;
    public string m_Content;


    public override string ToString()
    {
        if(string.IsNullOrEmpty(m_Content))
        {
            m_Content = m_Index + " : 开始时间点 - " + m_StartTime + ", 位置 - X:" + m_Position.x + ", Y:" + m_Position.y; 
        }

        return m_Content;
    }
}

public class CO
{
    public int m_Index;
    public string m_ReadContent;
    public string m_ShowContent;
    public Vector2 m_Position;
    public int m_CliciTimes;
    public string m_Content;

    public void RefreshContent()
    {
        m_Content = m_Index + " :   鬼畜次数 : " + m_CliciTimes + ", 位置 - X:" + m_Position.x + ", Y:" + m_Position.y;
    }

    public override string ToString()
    {
        if (string.IsNullOrEmpty(m_Content))
        {
            m_Content = m_Index + " :   鬼畜次数 : " + m_CliciTimes + ", 位置 - X:" + m_Position.x + ", Y:" + m_Position.y;
        }

        return m_Content;
    }
}

public class ShowButtonObj
{
        
}

public class TeachSentenceEdit : TeachButtonControlBase
{

    private List<CleanSentence> m_Sentences = new List<CleanSentence>();
    private List<HO> m_HitObjects = new List<HO>();
    private List<HO> m_LeftBoxHO = new List<HO>();
    private GameObject m_HitObjectContent;
    private GameObject m_SentenceContent;
    private GameObject m_SentenceHOContent;
    private InputField m_SentenceStartTime;
    private InputField m_SentenceEndTime;

    // Use this for initialization
    protected override void Start()
    {
        base.Start();
        m_HitObjectContent = transform.Find("LeftBox/KeyEdit/HitObjectList/Viewport/Content").gameObject;
        m_SentenceContent = transform.Find("MiddleBox/KeyEdit/SentenceList/Viewport/Content").gameObject;
        m_SentenceHOContent = transform.Find("RightBox/KeyEdit/HitObjectList/Viewport/Content").gameObject;
        transform.Find("Load").GetComponent<Button>().onClick.AddListener(LoadFile);
        transform.Find("Save").GetComponent<Button>().onClick.AddListener(Save);
       
        transform.Find("LeftBox/CreateSentence").GetComponent<Button>().onClick.AddListener(CreateSentence);
        transform.Find("MiddleBox/DestroyLastSentence").GetComponent<Button>().onClick.AddListener(DestroyLastSentence);
        transform.Find("MiddleBox/DestroyAllSentence").GetComponent<Button>().onClick.AddListener(DestroyAllSentence);
        m_SentenceStartTime = transform.Find("MiddleBox/StartTime").GetComponent<InputField>();
        m_SentenceStartTime.onValueChanged.AddListener(StartTimeChanged);
        m_SentenceEndTime = transform.Find("MiddleBox/EndTime").GetComponent<InputField>();
        m_SentenceEndTime.onValueChanged.AddListener(EndTimeChanged);
        transform.Find("RightBox/CreateTic").GetComponent<Button>().onClick.AddListener(MergeTic);
        transform.Find("RightBox/DestroyTic").GetComponent<Button>().onClick.AddListener(DestroyTic);
    }

    private string m_CurrentSongName;
    void LoadFile()
    {
        m_CurrentSongName = m_DropDown.options[m_DropDown.value].text;
        string path = "Songs/" + m_CurrentSongName + "OSU";
        TextAsset osu = ResourceLoadUtils.Load<TextAsset>(path);
        string[] contents = null;
        if (osu == null)
        {
            string filePath = GetPathString(m_CurrentSongName + "OSU.txt");
            if(File.Exists(filePath))
            {
                contents = File.ReadAllLines(filePath);
            }
            else
            {
                PromptManager.Instance.MessageBox( PromptManager.Type.FloatingTip, m_CurrentSongName + ".txt 文件没有找到！！");
                return;
            }
        }
        else
        {
            contents = osu.text.Split('\n');
        }

        m_HitObjects.Clear();
        ClearAllContent();

        bool hitObjStart = false;
        for (int i = 0; i < contents.Length; i++)
        {
            string parseContent = contents[i].Trim();

            if (string.IsNullOrEmpty(parseContent) || parseContent.StartsWith("//"))
            {//空行和注释行 不解析
                continue;
            }

            if (parseContent.Contains("[HitObjects]"))
            {
                hitObjStart = true;
                continue;
            }

            if (hitObjStart)
            {
                if (parseContent.Contains("C") || parseContent.Contains("P") || parseContent.Contains("L") || parseContent.Contains("B"))
                {//2017年10月28日版本先不做这些功能
                    continue;
                }
                else
                {
                    string[] param = parseContent.Split(',');
                    if (param.Length != 6)
                    {
						LogManager.LogError("data parse error: the data file is  --" , path);
                    }
                    else
                    {
                        Vector2 pos = new Vector2(int.Parse(param[0]), int.Parse(param[1]));
                        int startTime = int.Parse(param[2]);

                        HO ho = new HO();
                        ho.m_Index = m_HitObjects.Count + 1;
                        ho.m_StartTime = startTime;
                        ho.m_Position = pos;
                        m_HitObjects.Add(ho);
                    }
                }
            }
        }

        InitLeftBoxHOList();
        CreateLeftBoxScrollViewContent();
    }

    void ClearAllContent()
    {
        ClearChildContent(m_HitObjectContent.transform);
        ClearChildContent(m_SentenceContent.transform);
        ClearChildContent(m_SentenceHOContent.transform);
        m_CurrentSelectSentence = null;
        m_CurrentSelectItemsInRightBox.Clear();
        m_LeftBoxCurrentItem = null;
        m_LeftBoxHO.Clear();
        m_Sentences.Clear();
    }

    protected override void ListenMusic()
    {
        if (!m_GameStart)
        {
            string songName = m_DropDown.options[m_DropDown.value].text;
            CorePlayData.CurrentSong = BeatmapParse.Parse(songName);
            CorePlayMusicPlayer.Instance.LoadSong(Path.GetFileNameWithoutExtension(CorePlayData.CurrentSong.AudioFileName.Trim()));

            TeachCreateEvents createEvents = new TeachCreateEvents();
            m_TimerEvents = createEvents.GetTimerEventQueue();

            m_GameStart = true;
        }
    }

    protected override void Stop()
    {
        m_CurrentSong = null;
        m_GameStart = false;
        CorePlayMusicPlayer.Instance.StopMusic();
    }


    void Save()
    {
        if(m_LeftBoxHO.Count > 0)
        {
            PromptManager.Instance.MessageBox( PromptManager.Type.WindowTip, "还有hit Object 没有编辑成句子，确定要保存吗？",(result) => {
                if(result == PromptManager.Result.OK)
                {
                    DoSave();
                }
            });
        }
        else
        {
            DoSave();
        }
    }

    void DoSave()
    {
        List<string> content = new List<string>();
        int startIndex = 1;
        for (int i = 0; i < m_Sentences.Count;i++)
        {
            StringBuilder oneSentence = new StringBuilder();
            oneSentence.Append(m_Sentences[i].m_StartTime.ToString() + "," + m_Sentences[i].m_EndTime.ToString() + ",");
            StringBuilder coList = new StringBuilder();
            for (int j = 0; j < m_Sentences[i].m_COList.Count; j++)
            {
                string tail = "";
                if (m_Sentences[i].m_COList[j].m_CliciTimes > 1)
                {
                    tail = "|" + m_Sentences[i].m_COList[j].m_CliciTimes.ToString();
                }
                if (j == m_Sentences[i].m_COList.Count - 1)
                {//last one
                    coList.Append(startIndex.ToString() + tail);
                }
                else
                {
                    coList.Append(startIndex.ToString() + tail + "$");
                }
            }
            oneSentence.Append(coList.ToString() + "," + coList.ToString());
            content.Add(oneSentence.ToString());
        }
        string filePath = GetPathString(m_CurrentSongName + "_temp.txt");
        File.WriteAllLines(filePath, content.ToArray());
    }

    #region 选取hitObject编辑句子
    void InitLeftBoxHOList()
    {
        m_LeftBoxHO.Clear();
        for (int i = 0; i < m_HitObjects.Count; i++)
        {
            m_LeftBoxHO.Add(m_HitObjects[i]);
        }
    }

    private const float StartPosY = 91;
    private const float ItemHeight = 100;
    private const float StartWidth = 2000;
    private const float ItemXPos = 1000f;
    private const float Interval = 10;
    void CreateLeftBoxScrollViewContent()
    {
        ClearChildContent(m_HitObjectContent.transform);

        int startIndex = 0;
        m_HitObjectContent.GetComponent<RectTransform>().sizeDelta = new Vector2(StartWidth, StartPosY + (Interval + ItemHeight) * m_HitObjects.Count);
        foreach (HO ho in m_LeftBoxHO)
        {
            GameObject obj = Instantiate(ResourceLoadUtils.Load<GameObject>("TeachTool/HitObjectItem"));
            obj.transform.SetParent(m_HitObjectContent.transform);
            obj.transform.localScale = Vector2.one;
            obj.GetComponent<RectTransform>().localPosition = new Vector3(ItemXPos, -StartPosY - (ItemHeight + Interval) * startIndex, 0);
            TeachHitObjectItem item = obj.GetComponent<TeachHitObjectItem>();
            item.Init(this, TeachHitObjectItem.Type.LeftBox, ho.m_Index);
            item.SetString(ho.ToString());
            startIndex++;
        }
    }

    TeachHitObjectItem m_LeftBoxCurrentItem;
    public void LeftBoxSingleSelect(TeachHitObjectItem item)
    {
        if (m_LeftBoxCurrentItem != null)
            m_LeftBoxCurrentItem.SetUnselect();
        m_LeftBoxCurrentItem = item;
    }

    public void CreateSentence()
    {
        List<HO> temp = new List<HO>();
        while (m_LeftBoxHO.Count > 0 && m_LeftBoxHO[0].m_Index <= m_LeftBoxCurrentItem.m_Index)
        {
            temp.Add(m_LeftBoxHO[0]);
            m_LeftBoxHO.RemoveAt(0);
        }

        if (temp.Count > 0)
        {
            CleanSentence cs = new CleanSentence();
            cs.m_Index = m_Sentences.Count + 1;
            cs.m_StartTime = temp[0].m_StartTime;
            cs.m_EndTime = temp[temp.Count - 1].m_StartTime;
            cs.m_HOList = temp;

            List<CO> coList = new List<CO>();
            for (int i = 0; i < cs.m_HOList.Count; i++)
            {
                CO co = new CO();
                co.m_Index = i + 1;
                co.m_CliciTimes = 1;
                co.m_Position = cs.m_HOList[i].m_Position;
                coList.Add(co);
            }

            cs.m_COList = coList;
            m_Sentences.Add(cs);
        }

        CreateLeftBoxScrollViewContent();
        CreateSentenceBoxContent();
    }

    void CreateSentenceBoxContent()
    {
        ClearChildContent(m_SentenceContent.transform);

        int startIndex = 0;
        m_SentenceContent.GetComponent<RectTransform>().sizeDelta = new Vector2(StartWidth, StartPosY + (Interval + ItemHeight) * m_HitObjects.Count);
        foreach (CleanSentence cs in m_Sentences)
        {
            GameObject obj = Instantiate(ResourceLoadUtils.Load<GameObject>("TeachTool/HitObjectItem"));
            obj.transform.SetParent(m_SentenceContent.transform);
            obj.transform.localScale = Vector2.one;
            obj.GetComponent<RectTransform>().localPosition = new Vector3(ItemXPos, -StartPosY - (ItemHeight + Interval) * startIndex, 0);
            TeachHitObjectItem item = obj.GetComponent<TeachHitObjectItem>();
            item.Init(this, TeachHitObjectItem.Type.Sentence, cs.m_Index);
            item.SetString(cs.ToString());
            startIndex++;
        }
    }

    void DestroyLastSentenceProcess()
    {
        if (m_Sentences.Count > 0)
        {
            CleanSentence cs = m_Sentences[m_Sentences.Count - 1];
            for (int i = cs.m_HOList.Count - 1; i >= 0; i--)
            {
                m_LeftBoxHO.Insert(0, cs.m_HOList[i]);
            }
            m_Sentences.Remove(cs);
            if(cs.m_Index == m_CurrentSelectSentence.m_Index)
            {
                m_CurrentSelectSentence = null;
                ClearSentenceContentBox();
            }
        }
    }

    void DestroyLastSentence()
    {
        DestroyLastSentenceProcess();
        CreateLeftBoxScrollViewContent();
        CreateSentenceBoxContent();
    }

    void DestroyAllSentence()
    {
        while(m_Sentences.Count > 0)
        {
            DestroyLastSentenceProcess();
        }
        ClearSentenceContentBox();
        CreateLeftBoxScrollViewContent();
        CreateSentenceBoxContent();
    }
    #endregion

    #region 编辑单个句子


    void StartTimeChanged(string content)
    {
        if(m_CurrentSelectSentence != null)
        {
            m_Sentences[m_CurrentSelectSentence.m_Index - 1].m_StartTime = int.Parse(content);
            m_Sentences[m_CurrentSelectSentence.m_Index - 1].RefreshContent();
            m_CurrentSelectSentence.SetString(m_Sentences[m_CurrentSelectSentence.m_Index - 1].ToString());
        }
    }

    void EndTimeChanged(string content)
    {
        if (m_CurrentSelectSentence != null)
        {
            m_Sentences[m_CurrentSelectSentence.m_Index - 1].m_EndTime = int.Parse(content);
            m_Sentences[m_CurrentSelectSentence.m_Index - 1].RefreshContent();
            m_CurrentSelectSentence.SetString(m_Sentences[m_CurrentSelectSentence.m_Index - 1].ToString());
        }
    }

    void ClearSentenceContentBox()
    {
        m_CurrentSelectItemsInRightBox.Clear();
        ClearChildContent(m_SentenceHOContent.transform);
    }
    TeachHitObjectItem m_CurrentSelectSentence;
    public void SelectOneSentence(TeachHitObjectItem item)
    {
        if (m_CurrentSelectSentence != null)
        {
            m_CurrentSelectSentence.SetUnselect();
            ClearSentenceContentBox();
            if(m_CurrentSelectSentence.m_Index == item.m_Index)
            {
                return;
            }
        }
        m_CurrentSelectSentence = item;
        m_SentenceStartTime.text = m_Sentences[m_CurrentSelectSentence.m_Index - 1].m_StartTime.ToString();
        m_SentenceEndTime.text = m_Sentences[m_CurrentSelectSentence.m_Index - 1].m_EndTime.ToString();
        ShowOneSentence();
    }

    void ShowOneSentence()
    {
        ClearChildContent(m_SentenceHOContent.transform);

        int startIndex = 0;
        m_SentenceHOContent.GetComponent<RectTransform>().sizeDelta = new Vector2(StartWidth, StartPosY + (Interval + ItemHeight) * m_HitObjects.Count);
        foreach (CO co in m_Sentences[m_CurrentSelectSentence.m_Index - 1].m_COList)
        {
            GameObject obj = Instantiate(ResourceLoadUtils.Load<GameObject>("TeachTool/HitObjectItem"));
            obj.transform.SetParent(m_SentenceHOContent.transform);
            obj.transform.localScale = Vector2.one;
            obj.GetComponent<RectTransform>().localPosition = new Vector3(ItemXPos, -StartPosY - (ItemHeight + Interval) * startIndex, 0);
            TeachHitObjectItem tho = obj.GetComponent<TeachHitObjectItem>();
            tho.Init(this, TeachHitObjectItem.Type.RightBox, co.m_Index);
            tho.SetString(co.ToString());
            startIndex++;
        }
    }

    List<TeachHitObjectItem> m_CurrentSelectItemsInRightBox = new List<TeachHitObjectItem>();
    //list中始终保证最多有两个元素
    public void RightBoxSelect(TeachHitObjectItem item)
    {
        if(m_CurrentSelectItemsInRightBox.Contains(item))
        {
            item.SetUnselect();
            m_CurrentSelectItemsInRightBox.Remove(item);
        }
        else if(m_CurrentSelectItemsInRightBox.Count == 0)
        {
            m_CurrentSelectItemsInRightBox.Add(item);
        }
        else if(m_CurrentSelectItemsInRightBox.Count == 1)
        {
            if(m_CurrentSelectItemsInRightBox[0].m_Index > item.m_Index)
            {
                m_CurrentSelectItemsInRightBox.Insert(0, item);
            }
            else
            {
                m_CurrentSelectItemsInRightBox.Add(item);
            }
        }
        else
        {
            if(m_CurrentSelectItemsInRightBox[0].m_Index > item.m_Index)
            {
                m_CurrentSelectItemsInRightBox[0].SetUnselect();
                m_CurrentSelectItemsInRightBox.RemoveAt(0);
                m_CurrentSelectItemsInRightBox.Insert(0,item);
            }
            else
            {
                m_CurrentSelectItemsInRightBox[1].SetUnselect();
                m_CurrentSelectItemsInRightBox.RemoveAt(1);
                m_CurrentSelectItemsInRightBox.Add(item);
            }
        }
    }

    void MergeTic()
    {
        if(m_CurrentSelectItemsInRightBox.Count == 2)
        {
            int clickTimes = 0;
            int startIndex = m_CurrentSelectItemsInRightBox[0].m_Index;
            int endIndex = m_CurrentSelectItemsInRightBox[1].m_Index;
            for (int i = startIndex; i <= endIndex; i++)
            {
                clickTimes += m_Sentences[m_CurrentSelectSentence.m_Index - 1].m_COList[i - 1].m_CliciTimes;
            }
            m_Sentences[m_CurrentSelectSentence.m_Index - 1].m_COList[startIndex - 1].m_CliciTimes = clickTimes;
            for (int i = endIndex ; i > startIndex ; i--)
            {
                m_Sentences[m_CurrentSelectSentence.m_Index - 1].m_COList.RemoveAt(i - 1);
            }
            for (int i = 0 ; i < m_Sentences[m_CurrentSelectSentence.m_Index - 1].m_COList.Count; i++)
            {
                m_Sentences[m_CurrentSelectSentence.m_Index - 1].m_COList[i].m_Index = i + 1;
                m_Sentences[m_CurrentSelectSentence.m_Index - 1].m_COList[i].RefreshContent();
            }

            m_CurrentSelectItemsInRightBox.Clear();
            ShowOneSentence();
        }
        else
        {
            PromptManager.Instance.MessageBox( PromptManager.Type.FloatingTip, "选择两个点才能合并鬼畜");
        }
    }

    void DestroyTic()
    {
        if(m_CurrentSelectItemsInRightBox.Count > 0)
        {
            foreach (TeachHitObjectItem item in m_CurrentSelectItemsInRightBox)
            {
                int clickTimes = m_Sentences[m_CurrentSelectSentence.m_Index - 1].m_COList[item.m_Index - 1].m_CliciTimes;
                m_Sentences[m_CurrentSelectSentence.m_Index - 1].m_COList[item.m_Index - 1].m_CliciTimes = 1;
                if (clickTimes > 1)
                {
                    for (int i = 1; i < clickTimes; i++)
                    {
                        CO co = new CO();
                        co.m_CliciTimes = 1;
                        co.m_Index = item.m_Index + i;
                        co.m_Position = m_Sentences[m_CurrentSelectSentence.m_Index - 1].m_HOList[item.m_Index + i - 1].m_Position;
                        m_Sentences[m_CurrentSelectSentence.m_Index - 1].m_COList.Insert(item.m_Index + i - 1, co);
                    }
                }
            }


            for (int i = 0; i < m_Sentences[m_CurrentSelectSentence.m_Index - 1].m_COList.Count; i++)
            {
                m_Sentences[m_CurrentSelectSentence.m_Index - 1].m_COList[i].m_Index = i + 1;
                m_Sentences[m_CurrentSelectSentence.m_Index - 1].m_COList[i].RefreshContent();
            }

            m_CurrentSelectItemsInRightBox.Clear();
            ShowOneSentence();
        }
    }

    #endregion
}
