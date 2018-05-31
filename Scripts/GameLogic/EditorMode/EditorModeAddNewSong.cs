using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using Snaplingo.UI;

public class EditorModeAddNewSong : MonoBehaviour 
{

    private Dropdown m_DropDown;
    private string m_AudioPath = "Assets/Resources/Audio/Songs";
    private string m_RunTimeAudioPath = "/Resources/Audio/Songs";

    void Start()
    {
        m_DropDown = transform.Find("Dropdown").GetComponent<Dropdown>();
        m_DropDown.ClearOptions();
        List<string> options = EditorModeFileMisc.GetAudioFileName();

        if (options != null)
        {
            m_DropDown.AddOptions(options);
        }

        transform.Find("Enter").GetComponent<Button>().onClick.AddListener(Enter);
        transform.Find("Back").GetComponent<Button>().onClick.AddListener(Back);
    }

    private void Back()
    {
        PageManager.Instance.CurrentPage.GetNode<EditorModeNode>().BackToBegin();
    }

    private void Enter()
    {
        PageManager.Instance.CurrentPage.GetNode<EditorModeNode>().OpenEditPage();
        EditorModeManager.Instance.SetData();
        string audioName = m_DropDown.transform.Find("Label").GetComponent<Text>().text;
        SongObject so = new SongObject();
        so.AudioFileName = audioName;
        CorePlayData.CurrentSong = so;
    }
}
