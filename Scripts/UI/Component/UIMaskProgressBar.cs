using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UIMaskProgressBar : MonoBehaviour 
{
    public float m_Width;

    private Image m_Progress;
    //private Image m_Pointer;
    public void Init()
    {
        m_Progress = transform.Find("Mask").GetComponent<Image>();
        //m_Pointer = transform.Find("Point").GetComponent<Image>();
        m_Width = m_Progress.rectTransform.sizeDelta.x;
        SetProgress(1);
    }

    private const float Duration = 0.5f;
    public void SetProgress(float progress)
    {
        float result = Mathf.Clamp01(progress);
        m_Progress.rectTransform.DOKill();
        //m_Pointer.rectTransform.DOKill();
        Tweener barTweener = m_Progress.rectTransform.DOSizeDelta(new Vector2(result * m_Width, m_Progress.rectTransform.sizeDelta.y), Duration);
        barTweener.SetEase( Ease.OutBounce);
        //Tweener pointerTweener = m_Pointer.rectTransform.DOLocalMoveX((result - 0.5f) * m_Width, Duration);
        //pointerTweener.SetEase(Ease.OutBounce);
    }

    //public void SetPointerSprite(Sprite spr)
    //{
    //    m_Pointer.sprite = spr;
    //}

    public void Restart()
    {
        SetProgress(1);
    }
}
