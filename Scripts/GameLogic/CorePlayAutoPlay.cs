using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CorePlayAutoPlay : MonoBehaviour
{
	public void Show()
	{
		gameObject.SetActive(true);
	}

	public void Close()
	{
		gameObject.SetActive(false);
	}

	private float Height = -1f;
	private const float OSU_WIDTH = 512f;
	private const float OSU_HEIGHT = 384f;
	private const float ScreenWidth = 20.54f;
	private const float ScreenHeight = 15.3f;
	protected const float HalfScreenWidth = 10.27f;
	protected const float HalfScreenHeight = 7.65f;
	private float StartScale = 2;
	private float MaxScale = 3;
	public void Simulate(float duration, Vector2 target)
	{
		Vector2 pos = new Vector2((float)target.x / OSU_WIDTH * ScreenWidth - HalfScreenWidth, HalfScreenHeight - (float)target.y / OSU_HEIGHT * ScreenHeight);
		//LogManager.Log("balabala");
		transform.DOScale(MaxScale, duration * 0.5f).OnComplete(() =>
		{
			transform.DOScale(StartScale, duration * 0.5f).SetEase(Ease.InBack);
		});
		transform.DOJump(new Vector3(pos.x, pos.y + Height, 0), 2, 1, duration);
	}
}
