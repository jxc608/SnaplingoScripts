using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StageNameTitle : MonoBehaviour 
{
    private GameObject m_BindingGameObject;
    private Text m_Name;
    private Image m_Country;
    private RectTransform m_RectTrans;
    private bool m_StartBinding;

    private void Awake()
    {
        m_Name = GetComponentInChildren<Text>();
        m_Country = GetComponentInChildren<Image>();
        m_RectTrans = GetComponent<RectTransform>();
        m_StartBinding = false;

        m_Name.gameObject.SetActive(false);
        m_Country.gameObject.SetActive(false);
    }

    private const float Interval = 20f;
    public void Init(string name, DancerInfo.Country country, GameObject bindingObj)
    {
        m_Name.text = name;
        m_BindingGameObject = bindingObj;

        float nameWidth = m_Name.preferredWidth;
        float width = nameWidth + m_Country.rectTransform.sizeDelta.x + Interval;
        float deltaWidth = (width - nameWidth) / 2f; 
        m_Name.rectTransform.localPosition = new Vector3(deltaWidth, 0, 0);
        m_Country.rectTransform.localPosition = new Vector3( -(nameWidth - width / 2f + Interval + m_Country.rectTransform.sizeDelta.x / 2f), 0 ,0);

        switch(country)
        {
            case DancerInfo.Country.America:
                m_Country.sprite = Resources.Load<Sprite>("CityFlag/en_US");
                break;
            case DancerInfo.Country.China:
                m_Country.sprite = Resources.Load<Sprite>("CityFlag/zh_CN");
                break;
        }

        m_StartBinding = true;


        StartCoroutine(DelayShow());
    }

    IEnumerator DelayShow()
    {
        yield return new WaitForSeconds(1f);
        m_Name.gameObject.SetActive(true);
        m_Country.gameObject.SetActive(true);
    }


    private void Update()
    {
        if(m_StartBinding)
        {
            m_RectTrans.position = Camera.main.WorldToScreenPoint(m_BindingGameObject.transform.position + new Vector3(0, 2f, 0)) ;
        }
    }
}
