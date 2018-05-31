using UnityEngine;
using UnityEngine.UI;
using Snaplingo.UI;
using LitJson;
using System;
using System.Collections.Generic;
using UnityEngine.Purchasing;
using LitJson;

class IAPIsBuy
{

	private GameObject IsBuyParent;

	private Text TitleTF;

	private Text InfoTF;

	private Button OKBtn;
	private Button NoBtn;

	private Action OkCallback;
	private Action NoCallback;

	public IAPIsBuy (GameObject parent)
	{
		IsBuyParent = parent;
	}

	public void StartIsBuy (string title, string info, Action okCallback, Action noCallback)
	{
		OkCallback = okCallback;
		NoCallback = noCallback;

		TitleTF = IsBuyParent.transform.Find ("Title").GetComponent<Text> ();
		InfoTF = IsBuyParent.transform.Find ("Info").GetComponent<Text> ();
		OKBtn = IsBuyParent.transform.Find ("OKBtn").GetComponent<Button> ();
		NoBtn = IsBuyParent.transform.Find ("NoBtn").GetComponent<Button> ();

		IsBuyParent.SetActive (false);

		if (OKBtn != null)
		{
			OKBtn.onClick.RemoveListener (ClickOkBtnHandle);
		}
		if (NoBtn != null)
		{
			NoBtn.onClick.RemoveListener (ClickOkBtnHandle);
		}
		OKBtn.onClick.AddListener (ClickOkBtnHandle);
		NoBtn.onClick.AddListener (ClickNoCallback);

		IsBuyParent.SetActive (true);
		TitleTF.text = title;
		InfoTF.text = info;

		AnalysisManager.Instance.OnEvent ("payConfirmNode", null);
	}

	private void ClickOkBtnHandle ()
	{
		IsBuyParent.SetActive (false);
		if (OkCallback != null)
		{
			OkCallback ();
		}
	}

	private void ClickNoCallback ()
	{
		IsBuyParent.SetActive (false);
		if (NoCallback != null)
		{
			NoCallback ();
		}
	}


}
