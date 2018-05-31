using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Snaplingo.UI;
using UnityEngine.UI;

public class EditorModeNode : Node
{
    private GameObject m_BeginPage;
    private GameObject m_SelectSongPage;
    private GameObject m_SelectAudioPage;
    private GameObject m_EditPage;
    public override void Init(params object[] args)
    {
        m_BeginPage = transform.Find("BeginPage").gameObject;
        m_SelectSongPage = transform.Find("SelectSongPage").gameObject;
        m_SelectAudioPage = transform.Find("AddNewSongPage").gameObject;
        m_EditPage = transform.Find("EditPage").gameObject;

        m_BeginPage.SetActive(true);
        transform.Find("BeginPage/Add").GetComponent<Button>().onClick.AddListener(AddNewSong);
        transform.Find("BeginPage/Resave").GetComponent<Button>().onClick.AddListener(Resave);

        m_SelectSongPage.SetActive(false);
        m_SelectAudioPage.SetActive(false);
        m_EditPage.SetActive(false);
    }

    void AddNewSong()
    {
        m_BeginPage.SetActive(false);
        m_SelectAudioPage.SetActive(true);
    }

    void Resave()
    {
        m_BeginPage.SetActive(false);
        m_SelectSongPage.SetActive(true);
    }

    public void BackToBegin()
    {
        m_BeginPage.SetActive(true);
        m_SelectSongPage.SetActive(false);
        m_SelectAudioPage.SetActive(false);
        m_EditPage.SetActive(false);
    }

    public void OpenEditPage()
    {
        m_SelectAudioPage.SetActive(false);
        m_SelectSongPage.SetActive(false);
        m_EditPage.SetActive(true);
    }
}
