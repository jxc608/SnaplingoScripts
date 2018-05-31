using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class ChangeFoword : MonoBehaviour
{
	private bool isRewind;
	public void ChangeTargetFoword()
	{
		isRewind = false;
		if (transform.localPosition.x <= -18.5f)
		{
			transform.localRotation = Quaternion.Euler(0f, 180f, 0f);
			GetComponent<DOTweenAnimation>().DOPlayBackwards();
		}
	}
	private void Update()
	{
		if (transform.localPosition.x >= 18.5f&&!isRewind)
	      {
    			isRewind = true;
    	        transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
    	        GetComponent<DOTweenAnimation>().DOPlayForward();
        }
	}

    private void OnEnable()
    {
        GetComponent<DOTweenAnimation>().DORestart();
    }
}
