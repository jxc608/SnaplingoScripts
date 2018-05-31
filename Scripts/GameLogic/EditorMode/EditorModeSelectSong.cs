using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Snaplingo.UI;

public class EditorModeSelectSong : MonoBehaviour 
{
    private Dropdown m_SongDown;
    private string m_SongPath = "Assets/Resources/Songs";
    private string m_RunTimeSongPath = "/Resources/Songs";

    private Dropdown m_AudioDown;
    private string m_AudioPath = "Assets/Resources/Audio/Songs";
    private string m_RunTimeAudioPath = "/Resources/Audio/Songs";
    private string m_SongName;
    private string m_AudioName;
	// Use this for initialization
	void Start () 
    {
        m_SongName = null;
        m_SongDown = transform.Find("Songdown").GetComponent<Dropdown>();
        m_SongDown.ClearOptions();
        m_SongDown.onValueChanged.AddListener(UpdateFileName);

        m_AudioDown = transform.Find("Audiodown").GetComponent<Dropdown>();
        m_AudioDown.ClearOptions();

        List<string> options = null;
#if !UNITY_EDITOR
        options = EditorModeFileMisc.GetFileName("*.txt", Application.dataPath + m_RunTimeSongPath);
#else
        options = EditorModeFileMisc.GetFileName("*.txt", m_SongPath);
#endif
        if (options != null)
        {
            m_SongDown.AddOptions(options);
        }


        options = EditorModeFileMisc.GetAudioFileName();
        if (options != null)
        {
            m_AudioDown.AddOptions(options);
        }

        transform.Find("Enter").GetComponent<Button>().onClick.AddListener(Enter);
        transform.Find("Back").GetComponent<Button>().onClick.AddListener(Back);
	}
	
    private void UpdateFileName(int optionIndex)
    {
        m_SongName = m_SongDown.options[optionIndex].text;
        m_AudioName = BeatmapParse.GetAudioFileName(m_SongName).Replace(".mp3","");
        if (m_AudioName != null)
        {
            for (int i = 0; i < m_AudioDown.options.Count; i++)
            {
                if(string.Equals(m_AudioName, m_AudioDown.options[i].text))
                {
                    m_AudioDown.value = i;
                    break;
                }
            }
        }
    }

    private void Back()
    {
        PageManager.Instance.CurrentPage.GetNode<EditorModeNode>().BackToBegin();
        m_SongName = null;
    }

    private void Enter()
    {
        m_SongName = m_SongDown.options[m_SongDown.value].text;
        m_AudioName = m_AudioDown.options[m_AudioDown.value].text;

        CorePlayData.CurrentSong = BeatmapParse.Parse(m_SongName);
        if(string.Equals(CorePlayData.CurrentSong.AudioFileName, m_AudioName))
        {
            EditorModeManager.Instance.GetBPMFromData = true;
        }
        else
        {
            EditorModeManager.Instance.GetBPMFromData = false;
            CorePlayData.CurrentSong.AudioFileName = m_AudioName;
        }

        PageManager.Instance.CurrentPage.GetNode<EditorModeNode>().OpenEditPage();
        EditorModeManager.Instance.SetData();
    }
}
