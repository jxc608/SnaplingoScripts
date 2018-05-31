using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;

public class RoleModelClick : MonoBehaviour
{
	public SkeletonAnimation skeletonAnimation;
	private void Update()
	{
		if (Input.GetMouseButtonDown(0))
		{
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
			if (Physics.Raycast(ray, out hit))
			{
				if (hit.collider.gameObject==gameObject)
				{
					print(gameObject.name);
					StartCoroutine(StartPlayAnimation());
				}
			}
		}
	}
	private IEnumerator StartPlayAnimation()
	{
		skeletonAnimation.state.SetAnimation(0, "jump_lv1_01", false);
		print(skeletonAnimation.skeleton.data.Animations.Items[7].Duration);
		yield return new WaitForSeconds(skeletonAnimation.skeleton.data.Animations.Items[7].Duration);
		skeletonAnimation.state.SetAnimation(0, "pose_1", true);
	}
}
