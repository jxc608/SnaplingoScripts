using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine.UI;

public class StarPanel : MonoBehaviour
{
	#region [ --- Property --- ]
	List<RectTransform> poolStars = new List<RectTransform>();
	RectTransform canvasRect;
	#endregion


	#region [ --- Object References --- ]
	[Header(" --- Object References ---")]
	[SerializeField]
	GameObject prefab_star;
	Canvas canvas;
	#endregion



	#region [ --- Mono --- ]
	void Awake()
	{
		for (int i = 0; i < 10; i++)
		{
			var obj = Instantiate(prefab_star, transform.position, Quaternion.identity, transform);
			poolStars.Add(obj.GetComponent<RectTransform>());
			obj.SetActive(false);
		}
	}
	void Start()
	{
		canvas = GetComponentInParent<Canvas>();
		canvasRect = canvas.GetComponent<RectTransform>();
	}
	#endregion


	#region [ --- Public --- ]
	public void SetDefault()
	{
		foreach (var item in poolStars)
		{
			item.gameObject.SetActive(false);
		}
	}
	public void LoadStar(Vector3 worldPos, Vector3 targetPos,Image image)
	{

		foreach (var item in poolStars)
		{
			if (!item.gameObject.activeSelf)
			{
				//LogManager.Log("OnTapRight = ");
				var screenPos = Camera.main.WorldToScreenPoint(worldPos);
				var viewportPos = Camera.main.ScreenToViewportPoint(screenPos);
				Vector2 anchoredPosition = new Vector2(
					((viewportPos.x * canvasRect.rect.width) - (canvasRect.rect.width * 0.5f)),
					((viewportPos.y * canvasRect.rect.height) - (canvasRect.rect.height * 0.5f)));
				item.anchoredPosition = anchoredPosition;
				item.gameObject.SetActive(true);
				item.transform.DOMove(targetPos, 0.6f)
					.OnComplete(() => item.gameObject.SetActive(false));
				item.transform.DOScale(0.25f, 0.6f).OnComplete(()=>{
					item.transform.localScale = Vector3.one;
					if (image != null)
					{
						//image.transform.DOPunchScale(new Vector3(0.35f, 0.35f, 0.35f), 0.5f,1,0.1f);
						image.transform.DOScale(1.35f, 0.1f).OnComplete(()=>{
							image.transform.DOScale(1f, 0.1f);
						});
					}
				});
				break;
			}
		}
	}
	#endregion


	#region [ --- Private --- ]
	#endregion

}
//StarPanel













