using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SceneController : MonoBehaviour
{
	#region [ --- Property --- ]
	#endregion


	#region [ --- Object References --- ]
	//[Header(" --- Object References ---")]
	public GameObject midground;
	#endregion



	#region [ --- Mono --- ]
	public static SceneController instance;
	public virtual void Awake ()
	{
		instance = this;
	}
	void Start ()
	{
	}
	public virtual void OnDestroy ()
	{
		CorePlaySceneManager.bossEnterEvent.RemoveListener (OnBossEnter);
		CorePlaySceneManager.bossPrepareAttackEvent.RemoveListener (OnBossPrepareAttack);
		CorePlaySceneManager.bossAttackEvent.RemoveListener (OnBossAttack);
		CorePlaySceneManager.bossDamageEvent.RemoveListener (OnBossDamage);
		CorePlaySceneManager.bossDrawEvent.RemoveListener (OnBossDraw);
		CorePlaySceneManager.bossFinishEvent.RemoveListener (OnBossFinish);
	}
	#endregion


	#region [ --- Public --- ]
	public virtual void Init ()
	{
		CorePlaySceneManager.bossEnterEvent.AddListener (OnBossEnter);
		CorePlaySceneManager.bossPrepareAttackEvent.AddListener (OnBossPrepareAttack);
		CorePlaySceneManager.bossAttackEvent.AddListener (OnBossAttack);
		CorePlaySceneManager.bossDamageEvent.AddListener (OnBossDamage);
		CorePlaySceneManager.bossDrawEvent.AddListener (OnBossDraw);
		CorePlaySceneManager.bossFinishEvent.AddListener (OnBossFinish);
	}
	public virtual void Reset ()
	{
		if (midground != null)
		{
			midground.SetActive (true);
		}
	}
	public void HideMidground ()
	{
		if (midground != null)
		{
			midground.SetActive (false);
		}
	}
	#endregion

	#region [ --- Event Call Back --- ]
	public virtual void OnSkipEnter () { }
	public virtual void OnBossEnter () { }
	public virtual void OnBossPrepareAttack () { }
	public virtual void OnBossAttack () { }
	public virtual void OnBossDamage () { }
	public virtual void OnBossDraw () { }
	public virtual void OnBossFinish (bool isWin) { }
	public virtual void OnLoadMissile (Vector3 pos, bool jump = false) { }
	#endregion

	#region [ --- Private --- ]
	#endregion

}
//SceneController













