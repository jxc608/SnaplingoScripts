using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Chinese2PinYin;

public enum PinYinFrameStatus
{
	Show,
    Hide,
}
public class ChangePinYinControl : MonoBehaviour {
	/// <summary>
    /// 按钮等待时间
    /// </summary>
	private float time;
    /// <summary>
    /// 切换拼音按钮
    /// </summary>
	public Button pinYinButton;
    /// <summary>
    /// 渐入渐出按钮
    /// </summary>
	public Button moveButton;
    /// <summary>
    /// 切换拼音按钮的状态
    /// </summary>
	private PinYinFrameStatus status;
	private DOTweenAnimation dOTweenAnimation;
	private void Start()
	{
		time = 0f;
		SetButtonImage();
		status = PinYinFrameStatus.Show;
		gameObject.SetActive(LanguageManager.languageType == LanguageType.Chinese ? false : true);
		dOTweenAnimation = GetComponent<DOTweenAnimation>();
        //测试
		//LanguageManager.languageType = LanguageType.English;
		pinYinButton.GetComponent<DOTweenAnimation>().onComplete.AddListener(() =>
        {
            pinYinButton.GetComponent<DOTweenAnimation>().DOPlayBackwards();
        });
        pinYinButton.onClick.AddListener(() =>
        {
            time = 0f;
            pinYinButton.GetComponent<DOTweenAnimation>().DOPlayForward();
            PinYin.isShowPinYin = !PinYin.isShowPinYin;
            SetButtonImage();
        });
        moveButton.onClick.AddListener(() => {
            if (status == PinYinFrameStatus.Hide)
            {
                dOTweenAnimation.DOPlayBackwards();
                status = PinYinFrameStatus.Show;
            }
            else
            {
                dOTweenAnimation.DOPlayForward();
                status = PinYinFrameStatus.Hide;
            }
        });
	}
	//private void Update()
	//{
		//if(gameObject.activeInHierarchy&&LanguageManager.languageType == LanguageType.English&&status == PinYinFrameStatus.Show)
		//{
		//	//计时开始到一定时间之后按钮渐出
		//	time += Time.deltaTime;
		//	if(time>=3f)
		//	{
	//		//LogManager.Log(time);            
		//		dOTweenAnimation.DOPlayForward();
		//		status = PinYinFrameStatus.Hide; 
		//		time = 0f;
		//	}
		//}
	//}
    /// <summary>
    /// 设置切换拼音按钮的ICON
    /// </summary>
    private void SetButtonImage()
	{
		pinYinButton.GetComponent<Image>().sprite = PinYin.isShowPinYin != true ?  Resources.Load<Sprite>("language/pinyin_notselect") : Resources.Load<Sprite>("language/pinyin_select");
	}
}
