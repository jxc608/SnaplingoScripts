using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ArtNumber : MonoBehaviour 
{
    public Sprite[] m_Images;
    private Image m_Image;

    public void SetString(int i)
    {
        if (i < 0 || i > 9)
            return;
        if(m_Image == null)
            m_Image = GetComponent<Image>();
        m_Image.sprite = m_Images[i];
        m_Image.SetNativeSize();
    }
}
