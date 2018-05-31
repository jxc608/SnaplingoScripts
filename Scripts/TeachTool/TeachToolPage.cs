using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TeachToolPage : MonoBehaviour {

    public static TeachToolPage Instance;
    private GameObject m_ChoosePage;
    private GameObject m_SentenceStructureEditPage;
    private GameObject m_ContentEditPage;

    private void Awake()
    {
        Instance = this;
    }

    void Start () 
    {
        m_ChoosePage = transform.Find("ChoosePage").gameObject;
        m_SentenceStructureEditPage = transform.Find("StructureEdit").gameObject;
        m_ContentEditPage = transform.Find("ContentEdit").gameObject;

        transform.Find("ChoosePage/EditSentence").GetComponent<Button>().onClick.AddListener(EditSentence);
        transform.Find("ChoosePage/EditContent").GetComponent<Button>().onClick.AddListener(EditContent);

        m_SentenceStructureEditPage.SetActive(false);
        m_ContentEditPage.SetActive(false);
	}
	
    void EditSentence()
    {
        m_ChoosePage.SetActive(false);
        m_SentenceStructureEditPage.SetActive(true);

    }

    void EditContent()
    {
        m_ChoosePage.SetActive(false);
        m_ContentEditPage.SetActive(true);
    }

    public void BackToChoosePage()
    {
        m_ChoosePage.SetActive(true);
        m_SentenceStructureEditPage.SetActive(false);
        m_ContentEditPage.SetActive(false);
    }
}
