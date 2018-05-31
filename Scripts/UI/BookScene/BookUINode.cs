using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Snaplingo.UI;

public class BookUINode : Node
{

	#region [ Property --- ]
	bool isPageUp;
	#endregion


	#region [ Object References --- ]
	//[Header(" --- Object References ---")]
	public Button btn_close, btn_lastPage, btn_nextPage;
	#endregion

	public override void Init (params object[] args)
	{
		btn_close.onClick.AddListener (TA.BookSceneManager.instance.CloseActorBook);
		btn_nextPage.onClick.AddListener (() => OnPageTurning (true));
		btn_lastPage.onClick.AddListener (() => OnPageTurning (false));
	}

	public override void Open ()
	{
		base.Open ();
		gameObject.SetActive (true);
		SetButtonsDisplay ();
	}

	public override void Close (bool destroy = false)
	{
		StopAllCoroutines ();
		HideAllButtons ();
		gameObject.SetActive (false);
	}


	//Vector2 clickPos;
	//bool touchMoveEnoughDistance;
	//private void Update()
	//{
	//	if (InputUtils.OnPressed())
	//	{
	//		clickPos = InputUtils.GetTouchPosition();
	//		touchMoveEnoughDistance = false;
	//	}
	//	if (InputUtils.OnHeld())
	//	{
	//		if (!touchMoveEnoughDistance)
	//		{
	//			float dis = (InputUtils.GetTouchPosition() - clickPos).magnitude;
	//			if (dis > Screen.width / 2)
	//			{
	//				touchMoveEnoughDistance = true;
	//				isPageUp = InputUtils.GetTouchPosition().x < clickPos.x;
	//			}
	//		}
	//	}
	//	if (InputUtils.OnReleased() && touchMoveEnoughDistance)
	//	{
	//		OnPageTurning(isPageUp);
	//	}
	//}



	#region [ Private --- ]
	void OnPageTurning (bool pageUp)
	{
		StopAllCoroutines ();
		StartCoroutine (_CorPageTurning (pageUp));
	}
	IEnumerator _CorPageTurning (bool pageUp)
	{
		HideAllButtons ();
		TA.BookSceneManager.instance.CloseStagePage ();
		yield return new WaitForSeconds (.5f);
		TA.BookSceneManager.instance.ActorBookPageTurning (pageUp);
		yield return new WaitForSeconds (1f);
		TA.BookSceneManager.instance.SetupNewPage (pageUp);
		TA.BookSceneManager.instance.OpenStagePage ();
		yield return new WaitForSeconds (1.5f);
		SetButtonsDisplay ();
	}

	void SetButtonsDisplay ()
	{
		btn_close.gameObject.SetActive (false);
		if (CorePlayData.pageID <= 0)
		{
			btn_lastPage.gameObject.SetActive (false);
			btn_nextPage.gameObject.SetActive (true);
		}
		else if (CorePlayData.pageID >= (TA.BookSceneManager.instance.prefab_BookPages.Length - 1))
		{
			btn_lastPage.gameObject.SetActive (true);
			btn_nextPage.gameObject.SetActive (false);
		}
		else
		{
			btn_lastPage.gameObject.SetActive (true);
			btn_nextPage.gameObject.SetActive (true);
		}
	}
	void HideAllButtons ()
	{
		btn_close.gameObject.SetActive (false);
		btn_lastPage.gameObject.SetActive (false);
		btn_nextPage.gameObject.SetActive (false);
	}

	#endregion





}
//BookUINode














