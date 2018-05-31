using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using Snaplingo.UI;
using LitJson;
using System;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using LitJson;
using Snaplingo.SaveData;
using Snaplingo.UI;
using UnityEngine;
using UnityEngine.UI;

class BookSceneUINode : MonoBehaviour
{

	//public Image mHeadImg;
	//public Button mHeadImgBtn;

	//public Text mUserNameTF;

	//public Image mUserExpBar;
	//private float mUserExpProgressBarWidth;
	//public Text mUserExpTF;

	//public Button mEmailBtn;

	//public Button mSuccssBtn;

	//public Button mSettingBtn;

	//public Text mHPTF;

	//public Text mEnergyTF;

	public Button mGoTeachBtn;

	public Button mBuyBtn;

	public Text supportAllCount;
	public GameObject supportEffect;
    public Button UIBackGround;
	//public Button createRole;

	public void Start()
	{
		//mUserExpProgressBarWidth = mUserExpBar.rectTransform.sizeDelta.x;
		PlaySupportEffect();
		refresh();
	}
	private void PlaySupportEffect()
    {
		SupportManager.Instance.GetUserSupportCountFromUid(SelfPlayerData.Uuid, supportEffect, supportAllCount.gameObject);
		//StartCoroutine(SupportManager.Instance.StartPlaySupportEffect(supportEffect, supportEffect.GetChildren().Length));
    }

	private void refresh()
	{

		//AsyncImageDownload.GetInstance().SetAsyncImage(SelfPlayerData.AvatarUrl, mHeadImg);

		//mUserNameTF.text = SelfPlayerData.Nickname;

		//经验
		int expTemp = SelfPlayerData.Experience;
		ExpBasic expItem = ExpConfig.Instance.getExpInfo(expTemp);
		float value = (float)expTemp / (float)expItem.expEnd;
		//mUserExpBar.rectTransform.sizeDelta = new Vector2(value * mUserExpProgressBarWidth, mUserExpBar.rectTransform.sizeDelta.y);
		//mUserExpTF.text = "exp:" + expTemp + " / " + expItem.expEnd;

		//mHPTF.text = "" + SelfPlayerData.hp;
		//mEnergyTF.text = "" + SelfPlayerData.Energy;

		//mHeadImgBtn.onClick.AddListener(onClickUserHeadHandle);
		//mEmailBtn.onClick.AddListener(onClickEmailHandle);
		//mSuccssBtn.onClick.AddListener(onClickSuccessHandle);
		//mSettingBtn.onClick.AddListener(onClickSettingHandle);
		mGoTeachBtn.onClick.AddListener(onClickGoTeachHandle);
		mBuyBtn.onClick.AddListener(onClickBuyToIAP);
        supportEffect.transform.parent.GetComponent<Button>().onClick.AddListener(()=>
        {
            LogManager.Log("OK");
            UIBackGround.gameObject.SetActive(true);
        });
        UIBackGround.onClick.AddListener(()=>{
            LogManager.Log("点击其他地方！");
            UIBackGround.gameObject.SetActive(false);
        });
		//createRole.onClick.AddListener(()=>
		//{
		//	LoadSceneManager.Instance.LoadNormalScene("CreateUserRole");
		//});
	}

	/// <summary>
	/// 点击头像
	/// </summary>
	private void onClickUserHeadHandle()
	{
		LogManager.Log("点击头像");
		PageManager.Instance.CurrentPage.AddNode<UserInfoNode>(true, SelfPlayerData.Uuid);
	}

	/// <summary>
	/// 点击Email
	/// </summary>
	private void onClickEmailHandle()
	{
		LogManager.Log("点击Email");
	}

	/// <summary>
	/// 点击成就
	/// </summary>
	private void onClickSuccessHandle()
	{
		LogManager.Log("点击成就");
	}

	/// <summary>
	/// 点击设置
	/// </summary>
	private void onClickSettingHandle()
	{
		LogManager.Log("点击设置");
	}

	/// <summary>
	/// 点击去上课
	/// </summary>
	private void onClickGoTeachHandle()
	{
		LogManager.Log("点击去上课");
		//暂时把称号入口放在这里
		PageManager.Instance.CurrentPage.GetNode<RoleTitleNode>().Open();
		PageManager.Instance.CurrentPage.GetNode<RoleTitleNode>().GetComponent<DOTweenAnimation>().DOPlayForward();
	}

	/// <summary>
	/// 点击购买
	/// </summary>
	private void onClickBuyToIAP()
	{

		#if !UNITY_EDITOR
			if (!GlobalConst.LoginToApp)
			{
				PromptManager.Instance.MessageBox(PromptManager.Type.FloatingTip, "登陆后才能进入商店");
				return;
			}
		#endif
		LogManager.Log("点击商店");
		//SnapHttpConfig.LOGINED_APP_TOKEN = "hfdjhidfhis";
		PageManager.Instance.CurrentPage.AddNode<IAPNode>(true);
	}

}
