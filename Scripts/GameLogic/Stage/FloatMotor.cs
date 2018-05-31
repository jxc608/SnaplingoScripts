using UnityEngine;
using System.Collections;

public class FloatMotor : MonoBehaviour
{

	#region [ Property --- ]
	public float range = 5f;
	public float speed = .2f;
	public float vibratoTime = 2f;
	Vector3 startLocalPos;
	Vector3 target;
	float timer;
	#endregion


	#region [ Object References --- ]
	//[Header(" --- Object References ---")]
	#endregion



	#region [ Mono --- ]
	void Start()
	{
		startLocalPos = transform.localPosition;
		target = (Vector3)Random.insideUnitCircle * range + startLocalPos;
	}
	void Update()
	{
		if (vibratoTime <= 0) return;

		timer += Time.deltaTime;
		Vector3 distance = transform.localPosition - target;
		if (distance.sqrMagnitude > 0.01f && timer < vibratoTime)
		{
			Vector3 pos = Vector3.MoveTowards(transform.localPosition, target, speed * Time.deltaTime);
			transform.localPosition = pos;
		}
		else
		{
			timer = 0;
            target = (Vector3)Random.insideUnitCircle * range + startLocalPos;
		}
	}
	#endregion


	#region [ Public --- ]
	#endregion


	#region [ Private --- ]
	#endregion

}
//FloatMotor














