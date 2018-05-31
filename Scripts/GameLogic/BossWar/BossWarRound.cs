using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using DG.Tweening;

public class BossWarRound : MonoBehaviour 
{
    List<Image> m_ImageList = new List<Image>();
    Image m_Image;
    Transform m_NumberParent;
    RectTransform m_Rect;
    public void Init()
    {
        ObjectPool.RegisterPool("ArtNumber", 2, "CorePlay/Number");
        m_Image = GetComponent<Image>();
        m_NumberParent = transform.Find("NumberParent");
        SetImageToAlpha(m_Image, 0);
        m_Rect = transform.GetComponent<RectTransform>();
    }

    private const float Duration = 2f;
    private const float Width = 80f;
    public void SetRound(int round, Action callback = null) 
    {
        m_ImageList.Clear();
        SetImageToAlpha(m_Image, 1);
        string text = round.ToString();
        for (int i = 0; i < text.Length; i++)
        {
            GameObject number = ObjectPool.GetOne("ArtNumber");
            ArtNumber an = number.GetComponent<ArtNumber>();
            Image image = number.GetComponent<Image>();
            SetImageToAlpha(image, 1);
            an.SetString(int.Parse(text[i].ToString()));
            m_ImageList.Add(image);
            number.transform.SetParent(m_NumberParent);
            number.transform.localScale = Vector3.one;
            image.rectTransform.localPosition = new Vector3(i * Width, 0, 0);
        }

        if(round > 9)
        {
            m_Rect.localPosition = new Vector3(-300,0,0);
        }
        else
        {
            m_Rect.localPosition = new Vector3(-191, 0, 0);
        }

        StartCoroutine(DelayFade(callback));
    }

    IEnumerator DelayFade(Action callback)
    {
        yield return new WaitForSeconds(Duration);

        Tweener tweener = m_Image.DOFade(0, 1);
        tweener.OnComplete<Tweener>(delegate () {
            if (callback != null)
                callback.Invoke();
            Delete();
        });

        foreach (Image image in m_ImageList)
        {
            SetImageToAlpha(image, 1);
            image.DOFade(0, 1);
        }
    }

    void SetImageToAlpha(Image image, float alpha)
    {
        image.color = new Color(image.color.r, image.color.g, image.color.b, alpha);
    }

    private void OnDestroy()
    {
        ObjectPool.ClearPool("ArtNumber");
    }

    void Delete()
    {
        for (int i = m_NumberParent.childCount - 1; i >= 0; i--)
        {
            ObjectPool.DeleteOne("ArtNumber",m_NumberParent.GetChild(i).gameObject);
        }
    }

    public void Restart()
    {
        Delete();
    }
}
