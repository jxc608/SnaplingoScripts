using UnityEngine;
using System.Collections;
using UnityEngine.Events;

public class BoolEvent : UnityEvent<bool> { }
public class StringEvent : UnityEvent<string> { }
public class VectorThreeEvent : UnityEvent<Vector3> { }


public class CorePlaySceneManager : MonoBehaviour
{
	#region [ --- Event --- ]
	// 正常
	public static UnityEvent wordEvent = new UnityEvent ();
	public static BoolEvent inputCheckEvent = new BoolEvent ();
	public static VectorThreeEvent tapRightEvent = new VectorThreeEvent ();
	public static UnityEvent checkSentenceEvent = new UnityEvent ();
	public static BoolEvent voiceRepeatEvent = new BoolEvent ();

	// Boss 战
	public static UnityEvent bossEnterEvent = new UnityEvent ();
	public static UnityEvent bossEnterFinishEvent = new UnityEvent ();
	public static UnityEvent bossPrepareAttackEvent = new UnityEvent ();
	public static UnityEvent bossAttackEvent = new UnityEvent ();
	public static UnityEvent bossDrawEvent = new UnityEvent ();
	public static UnityEvent bossDamageEvent = new UnityEvent ();
	public static BoolEvent bossFinishEvent = new BoolEvent ();
	public static UnityEvent bossWordClickWrongEvent = new UnityEvent ();
	public static UnityEvent bossWordRe_EnterEvent = new UnityEvent ();
	public static UnityEvent bossSentenceCompleteEvent = new UnityEvent ();
	#endregion

	#region [ --- Object References --- ]
	[Header (" --- Object References ---")]
	public Transform[] cannons;
	public Transform starTarget;
	public GameObject prefab_blowEffect;
	#endregion



	public static CorePlaySceneManager instance;
	#region [ --- Mono --- ]
	void Awake ()
	{
		instance = this;
	}
	void Start ()
	{
		SceneController.instance.Init ();
		EffectManager.instance.ResetCamera ();

		Resources.UnloadUnusedAssets ();
		System.GC.Collect ();
	}
	void OnDestroy ()
	{
		// 正常
		wordEvent.RemoveAllListeners ();
		inputCheckEvent.RemoveAllListeners ();
		checkSentenceEvent.RemoveAllListeners ();
		tapRightEvent.RemoveAllListeners ();
		voiceRepeatEvent.RemoveAllListeners ();


		// Boss 战
		bossEnterEvent.RemoveAllListeners ();
		bossEnterFinishEvent.RemoveAllListeners ();
		bossPrepareAttackEvent.RemoveAllListeners ();
		bossAttackEvent.RemoveAllListeners ();
		bossDamageEvent.RemoveAllListeners ();
		bossDrawEvent.RemoveAllListeners ();
		bossFinishEvent.RemoveAllListeners ();
		bossWordRe_EnterEvent.RemoveAllListeners ();
		bossWordClickWrongEvent.RemoveAllListeners ();
		bossSentenceCompleteEvent.RemoveAllListeners ();
	}
	#endregion


	#region [ --- Public --- ]
	public static Vector3 GetRandomCannonPos ()
	{
		int id = Random.Range (0, instance.cannons.Length);
		return instance.cannons[id].position;
	}
	#endregion


	#region [ --- Private --- ]
	#endregion

}
//CorePlaySceneManager













