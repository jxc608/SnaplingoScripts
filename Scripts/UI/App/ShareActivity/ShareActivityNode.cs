using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Snaplingo.UI;
using System.Text;
using System.Linq;
using System;
using UnityEngine.UI;
using LitJson;

public class ShareActivityNode : Node
{
	public Image ImgUnder;
	public Image ImgFloat;
	public Button PlayButton;
	public Button ShareBtn;
	public Button CloseBtn;

	public override void Init (params object[] args)
	{
		base.Init ();
		initEvents ();
		//gameObject.SetActive(false);
	}

	public override void Open ()
	{
		base.Open ();
		PlayButton.gameObject.SetActive (true);
		initImg ();
	}

	private void initImg ()
	{
		string url = "Activity/ShareActivity";
		string levelPath = StaticData.LevelID == 0 ? "1001" : StaticData.LevelID.ToString ();
		ImgUnder.sprite = Resources.Load<Sprite> (url + "/posterCard" + levelPath);
		ImgFloat.sprite = Resources.Load<Sprite> (url + "/card" + levelPath);
		ImgFloat.transform.gameObject.SetActive (false);
	}

	private void initEvents ()
	{

		LoginRpcProxy.getInstance ().shareDelegate += closeHandler;

		ShareBtn.onClick.AddListener (clickShareHandler);

		CloseBtn.onClick.AddListener (() => {
			Close (false);
			PageManager.Instance.CurrentPage.GetNode<WinNode> ().Open ();
		});

		PlayButton.onClick.AddListener (() => {
			MicManager.Instance.PlayAllVoice (SelfPlayerData.Uuid, StaticData.LevelID);
			PlayButton.gameObject.SetActive (false);
			ImgFloat.gameObject.gameObject.SetActive (true);
			ImgUnder.gameObject.gameObject.SetActive (true);
		});
	}

	void clickShareHandler ()
	{
		if (GlobalConst.LoginToApp == false)
		{
			PageManager.Instance.CurrentPage.AddNode<ShareInputNode> (true);
			return;
		}
		LoginRpcProxy.getInstance ().ShareWeChatGameByLevelID ();
		PageManager.Instance.CurrentPage.GetNode<WinNode> ().Open ();
	}

	private void closeHandler ()
	{
		LoginRpcProxy.getInstance ().shareDelegate -= closeHandler;
		Close (false);
	}

}
