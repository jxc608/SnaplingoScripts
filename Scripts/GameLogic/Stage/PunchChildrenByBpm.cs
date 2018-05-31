using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class PunchChildrenByBpm : MonoBehaviour
{

	#region [ --- ] Property
	public int count = 5;
	public float power = .25f;

	#endregion

	#region [ --- ] Component
	//[Header (" [ --- ] Component")]
	List<Transform> children = new List<Transform> ();
	#endregion




	#region [ Mono ]
	void Awake ()
	{
		for (int i = 0; i < transform.childCount; i++)
		{
			children.Add (transform.GetChild (i));
		}
		count = Mathf.Clamp (count, 0, children.Count);
	}
	void Start ()
	{
		StageManager.DanceBeat.AddListener (OnBeat);
	}
	void OnDestroy ()
	{
		StageManager.DanceBeat.RemoveListener (OnBeat);
	}
	#endregion





	#region [ - ]   ""
	void OnBeat (int beatNum)
	{
		var punchList = children.GetRandomList (count);
		Vector3 value = Random.insideUnitCircle * power;
		foreach (var item in punchList)
		{
			item.DOPunchPosition (value, .4f);
		}
	}
	#endregion



}
//PunchChildrenByBpm














