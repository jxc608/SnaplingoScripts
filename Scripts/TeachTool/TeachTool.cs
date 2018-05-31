using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class TeachTool : TeachButtonControlBase 
{
    private GameObject m_SentenceContent;
    private GameObject m_WordContent;
    private TeachHitObjectItem m_CurrentSelectSentence;
    private TeachHitObjectItem m_CurrentSelectWord;
    private InputField m_ReadContent;
    private InputField m_ShowContent;

    protected override void Start()
    {
        base.Start();
        m_SentenceContent = transform.Find("LeftBox/KeyEdit/HitObjectList/Viewport/Content").gameObject;
        m_WordContent = transform.Find("MiddleBox/KeyEdit/SentenceList/Viewport/Content").gameObject;
        transform.Find("Load").GetComponent<Button>().onClick.AddListener(LoadFile);
        transform.Find("Save").GetComponent<Button>().onClick.AddListener(Save);
        m_ReadContent = transform.Find("MiddleBox/ReadContent").GetComponent<InputField>();
        m_ReadContent.onValueChanged.AddListener(ReadContentValueChange);
        m_ShowContent = transform.Find("MiddleBox/ShowContent").GetComponent<InputField>();
        m_ShowContent.onValueChanged.AddListener(ShowContentValueChange);
    }

    private List<CleanSentence> m_Sentences = new List<CleanSentence>();
    private string m_CurrentSongName;
    void LoadFile()
    {
        m_CurrentSongName = m_DropDown.options[m_DropDown.value].text;
        TextAsset sentences = null;
        string filePath = GetPathString(m_CurrentSongName + "_temp.txt");
        if (File.Exists(filePath))
        {
            AssetBundle bundle = AssetBundle.LoadFromFile(filePath);
            sentences = bundle.LoadAsset<TextAsset>(m_CurrentSongName + "OSU");
        }
        else
        {
			LogManager.LogError(m_CurrentSongName , ".txt 文件没有找到！!");
            return;
        }


        m_Sentences.Clear();
        ClearAllContent();

        string[] contents = sentences.text.Split('\n');
        for (int i = 0; i < contents.Length; i++)
        {
            string parseContent = contents[i].Trim();

            string[] firstSplit = parseContent.Split(',');

            CleanSentence cs = new CleanSentence();
            cs.m_StartTime = int.Parse(firstSplit[0]);
            cs.m_EndTime = int.Parse(firstSplit[1]);
            cs.m_Index = i + 1;

            string[] readWords = firstSplit[2].Split('$');
            string[] showWords = firstSplit[3].Split('$');
            Dictionary<string, string> wordMap = new Dictionary<string, string>();
            for (int j = 0; j < readWords.Length; j++)
            {
                string originWord = readWords[j];
                string originShowWord = showWords[j];
                string word = "";
                string showWord = "";
                if (originWord.Contains("|"))
                {//tic word
                    string[] splits = originWord.Split('|');
                    word = splits[0];
                    showWord = originShowWord.Split('|')[0];
                }
                else
                {
                    word = originWord;
                    showWord = originShowWord;
                }
                if (!wordMap.ContainsKey(word))
                {
                    wordMap.Add(word, showWord);
                }

                cs.m_WordMap = wordMap;
            }
        }

        CreateSentenceContent();
    }

    void Save()
    {
        
    }

    void ClearAllContent()
    {
        
    }


    private const float StartPosY = 91;
    private const float ItemHeight = 100;
    private const float StartWidth = 2000;
    private const float ItemXPos = 1000f;
    private const float Interval = 10;
    void CreateSentenceContent()
    {
        ClearChildContent(m_SentenceContent.transform);

        int startIndex = 0;
        m_SentenceContent.GetComponent<RectTransform>().sizeDelta = new Vector2(StartWidth, StartPosY + (Interval + ItemHeight) * m_Sentences.Count);
        foreach (CleanSentence cs in m_Sentences)
        {
            GameObject obj = Instantiate(ResourceLoadUtils.Load<GameObject>("TeachTool/HitObjectItem"));
            obj.transform.SetParent(m_SentenceContent.transform);
            obj.transform.localScale = Vector2.one;
            obj.GetComponent<RectTransform>().localPosition = new Vector3(ItemXPos, -StartPosY - (ItemHeight + Interval) * startIndex, 0);
            TeachHitObjectItem item = obj.GetComponent<TeachHitObjectItem>();
            item.ContentInit(this, TeachHitObjectItem.Type.ContentSentence, cs.m_Index);
            item.SetString(cs.ToString());
            startIndex++;
        }
    }


    public void SelectSentence(TeachHitObjectItem item)
    {
        if(m_CurrentSelectSentence != null)
        {
            m_CurrentSelectSentence.SetUnselect();
            ClearChildContent(m_WordContent.transform);
            if(m_CurrentSelectSentence.m_Index == item.m_Index)
            {
                m_CurrentSelectSentence = null;
                return;
            }
        }

        m_CurrentSelectSentence = item;
        CreateWordContent();
    }

    void CreateWordContent()
    {
        ClearChildContent(m_WordContent.transform);

        //int startIndex = 0;
        m_WordContent.GetComponent<RectTransform>().sizeDelta = new Vector2(StartWidth, StartPosY + (Interval + ItemHeight) * m_Sentences.Count);
        //foreach (CO co in m_Sentences[m_CurrentSelectSentence.m_Index - 1].m_WordMap)
        //{
        //    GameObject obj = Instantiate(ResourceLoadUtils.Load<GameObject>("TeachTool/HitObjectItem"));
        //    obj.transform.SetParent(m_WordContent.transform);
        //    obj.transform.localScale = Vector2.one;
        //    obj.GetComponent<RectTransform>().localPosition = new Vector3(ItemXPos, -StartPosY - (ItemHeight + Interval) * startIndex, 0);
        //    TeachHitObjectItem item = obj.GetComponent<TeachHitObjectItem>();
        //    item.ContentInit(this, TeachHitObjectItem.Type.WordContent, cs.m_Index);
        //    item.SetString(cs.ToString());
        //    startIndex++;
        //}
    }

    public void SelectWord(TeachHitObjectItem item)
    {
        if (m_CurrentSelectWord != null)
        {
            m_CurrentSelectWord.SetUnselect();
            if (m_CurrentSelectWord.m_Index == item.m_Index)
            {
                m_CurrentSelectWord = null;
                return;
            }
        }

        m_CurrentSelectWord = item;
    }

    void ReadContentValueChange(string content)
    {
        if(m_CurrentSelectWord != null)
        {


        }
    }

    void ShowContentValueChange(string content)
    {

    }
}
