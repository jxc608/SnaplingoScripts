using System.Collections;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using Snaplingo.UI;
using System;

public class CorePlayIntrada
{
	private float m_HalfBPM;
	private Action m_FinishCallback;
	public CorePlayIntrada(float halfBMP, Action callback = null)
	{
		m_HalfBPM = halfBMP;
		m_FinishCallback = callback;
	}

	public void Show()
	{
		StaticMonoBehaviour.Instance.StartCoroutine(Show(m_HalfBPM));
	}

	IEnumerator Show(float bpmDelta)
	{
		StaticMonoBehaviour.Instance.StartCoroutine(DelayDelete(LoadReady("3"), Three, bpmDelta));
		yield return new WaitForSeconds(bpmDelta);
		StaticMonoBehaviour.Instance.StartCoroutine(DelayDelete(LoadReady("2"), Two, bpmDelta));
		yield return new WaitForSeconds(bpmDelta);
		StaticMonoBehaviour.Instance.StartCoroutine(DelayDelete(LoadReady("1"), One, bpmDelta));
		yield return new WaitForSeconds(bpmDelta);
		StaticMonoBehaviour.Instance.StartCoroutine(DelayDelete(LoadReady("go"), Go, bpmDelta));
		yield return new WaitForSeconds(bpmDelta);
		if (m_FinishCallback != null)
			m_FinishCallback.Invoke();
	}

	private GameObject LoadReady(string objName)
	{
		GameObject obj = new GameObject();
		obj.gameObject.name = objName;
		obj.tag = "intrada";
		Image image = obj.AddComponent<Image>();
		image.raycastTarget = false;
		image.sprite = GameObject.Instantiate(ResourceLoadUtils.Load<Sprite>("CorePlay/Pretip/" + objName));
		RectTransform rt = image.GetComponent<RectTransform>();
		rt.transform.SetParent(PageManager.Instance.CurrentPage.transform);
		rt.localScale = Vector3.one;
		image.SetNativeSize();
		rt.position = new Vector3(-rt.sizeDelta.x / 2f, -rt.sizeDelta.y / 2f, 0);
		return obj;
	}

	private const float m_DelayTimeLength = 1f;
	private const int Three = 3;
	private const int Two = 2;
	private const int One = 1;
	private const int Ready = 0;
	private const int Go = -1;
	IEnumerator DelayDelete(GameObject obj, int num, float bpmDelta)
	{
		RectTransform rt = obj.GetComponent<RectTransform>();
		rt.position = new Vector3(Screen.width / 2f, Screen.height / 2f, 0);
		switch (num)
		{
			case Three:
				AudioController.Play("count3s");
				rt.localScale = new Vector3(4, 4, 4);
				obj.transform.DOScale(new Vector3(1, 1, 1), bpmDelta);
				obj.transform.GetComponent<Image>().DOFade(0, bpmDelta);
				break;
			case Two:
				AudioController.Play("count2s");
				rt.localScale = new Vector3(4, 4, 4);
				obj.transform.DOScale(new Vector3(1, 1, 1), bpmDelta);
				obj.transform.GetComponent<Image>().DOFade(0, bpmDelta);
				break;
			case One:
				AudioController.Play("count1s");
				rt.localScale = new Vector3(4, 4, 4);
				obj.transform.DOScale(new Vector3(1, 1, 1), bpmDelta);
				obj.transform.GetComponent<Image>().DOFade(0, bpmDelta);
				break;
			case Go:
				AudioController.Play("gos");
				rt.localScale = Vector3.one;
				obj.transform.DOScale(new Vector3(4f, 4f, 4f), bpmDelta);
				obj.transform.GetComponent<Image>().DOFade(0, bpmDelta);
				break;

		}
		yield return new WaitForSeconds(bpmDelta);
		if (obj != null)
		{
			GameObject.Destroy(obj);
		}
	}
}
