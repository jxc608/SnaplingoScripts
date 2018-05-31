using UnityEngine;
using System.Collections;
using Spine.Unity;
using DG.Tweening;

public class BossTestController : SceneController
{
	#region [ --- Property --- ]
	[Header(" --- Setting ---")]
	[SpineAnimation]
	public string idle;
	[SpineAnimation]
	public string enterTalk, enter, prepareAttack, attack, damage, bossLose;

	Spine.AnimationState state_Boss;
	Spine.AnimationState state_BossDamage;
	Spine.AnimationState state_BossDie;
	Spine.AnimationState state_BossAttack;

	Vector3 bossStartPos;
	bool gameEnd;
	GameObject[] obj_missilePieces;
	#endregion


	#region [ --- Object References --- ]
	[Header(" --- Object References ---")]
	public SkeletonAnimation spine_Boss;
	public SkeletonAnimation spine_BossDamage;
	public SkeletonAnimation spine_BossDie;
	public SkeletonAnimation spine_BossAttack;
	public GameObject obj_electricity;
	//public FireProjectile fireProjectile;
	public BossFireProjectile bossFireProjectile;
	public MissileLauncher missileLauncher;
    AudioSource audio_Source;
	#endregion




	#region [ --- Mono --- ]
	public override void Awake()
	{
		base.Awake();
		obj_electricity.SetActive(false);
		audio_Source = GetComponent<AudioSource>();
	}
	void Start()
	{
		spine_Boss.gameObject.SetActive(true);
		spine_BossAttack.gameObject.SetActive(false);
		spine_BossDamage.gameObject.SetActive(false);
		spine_BossDie.gameObject.SetActive(false);
	}
	public override void OnDestroy()
	{
		base.OnDestroy();
	}
	#endregion




	#region [ --- Public --- ]
	public override void Init()
	{
		base.Init();
		gameEnd = false;
		bossStartPos = spine_Boss.transform.position;

		state_Boss = spine_Boss.AnimationState;
		state_BossAttack = spine_BossAttack.AnimationState;
		state_BossDamage = spine_BossDamage.AnimationState;
		state_BossDie = spine_BossDie.AnimationState;

		state_Boss.Complete += OnAnimComplete;
	}
	public override void Reset()
	{
		if (state_Boss == null)
			return;
		gameEnd = false;

		//LogManager.Log("Go = Restart");

		spine_Boss.gameObject.SetActive(true);
		spine_BossAttack.gameObject.SetActive(false);
		spine_BossDamage.gameObject.SetActive(false);
		spine_BossDie.gameObject.SetActive(false);

		state_Boss.SetEmptyAnimation(0, 0);
		var tarck = state_Boss.AddAnimation(0, enterTalk, false, 0);
		tarck.TimeScale = 0;

		obj_electricity.SetActive(false);
		bossFireProjectile.Stop();
		missileLauncher.Reset();

		DOTween.Complete(spine_Boss.transform);
		spine_Boss.transform.position = bossStartPos;
	}

	public override void OnLoadMissile(Vector3 pos, bool jump = false)
	{
		missileLauncher.LoadMissile(pos, jump);
	}
	#endregion



	#region [ --- Event Call Back --- ]
	public override void OnBossEnter()
	{
		if (gameEnd == true)
			return;

		//LogManager.Log("Go = OnBossEnter");

		audio_Source.Play();
		float enterTalkDuration = audio_Source.clip.length;
		state_Boss.SetAnimation(0, enterTalk, true);
		var track = state_Boss.AddAnimation(0, enter, false, enterTalkDuration);
		track.AnimationStart = 2f;
		//spine_Boss.transform.DOMoveY(-5f, 1.3f);
	}

	public override void OnBossPrepareAttack()
	{
		if (gameEnd == true)
			return;

		obj_electricity.SetActive(true);
		state_Boss.SetAnimation(0, prepareAttack, true);
	}
	public override void OnBossAttack()
	{
		if (gameEnd == true)
			return;

		obj_electricity.SetActive(false);
		StartCoroutine(CorAttack(3f));
	}
	public override void OnBossDamage()
	{
		if (gameEnd == true)
			return;

		obj_electricity.SetActive(false);
		StartCoroutine(CorDamage());
	}
	public override void OnBossFinish(bool isBossWin)
	{
		if (gameEnd == true)
			return;

		gameEnd = true;
		if (isBossWin)
		{
			LogManager.Log(" Boss 胜利!! ");
			CorePlayBossWar.BossFinishEvent.Invoke();
		}
		else
		{
			StartCoroutine(CorDie());
		}

	}
	void OnAnimComplete(Spine.TrackEntry trackEntry)
	{
		string trackName = trackEntry.ToString();
		if (trackName == enter)
		{
			//LogManager.Log("trackName == enter ");
			CorePlaySceneManager.bossEnterFinishEvent.Invoke();
		}
		else if (trackName == bossLose)
		{
			//LogManager.Log("trackName == bossLose ");
			//CorePlayBossWar.BossFinishEvent.Invoke();
		}
	}
	#endregion



	#region [ --- Private --- ]
	IEnumerator CorAttack(float duration)
	{
		missileLauncher.Reset();
		//fireProjectile.Fire();
		bossFireProjectile.FireAll();

		//隐藏/显示
		spine_Boss.gameObject.SetActive(false);
		spine_BossAttack.gameObject.SetActive(true);
		//
		var track = state_BossAttack.SetAnimation(0, attack, false);
		state_BossAttack.AddEmptyAnimation(0, 0, track.AnimationEnd);
		//state_BossAttack.SetAnimation(0, attack, false);

		yield return new WaitForSeconds(duration);
		//隐藏/显示
		spine_Boss.gameObject.SetActive(true);
		spine_BossAttack.gameObject.SetActive(false);

		CorePlayBossWar.BossAttackFinishEvent.Invoke();
	}
	IEnumerator CorDamage()
	{
		yield return new WaitForSeconds(0.5f);
		missileLauncher.LaunchAll();
		yield return new WaitForSeconds(1.2f);

		spine_Boss.gameObject.SetActive(false);
		spine_BossAttack.gameObject.SetActive(false);
		spine_BossDamage.gameObject.SetActive(true);
		spine_BossDie.gameObject.SetActive(false);

		var track = state_BossDamage.SetAnimation(0, damage, false);
		spine_BossDamage.skeleton.FlipX = Random.value > .5f;
		//var track = state_BossDamage.AddAnimation(0, idle, true, 0.5f * duration);
		//track.MixDuration = 0.5f * duration;
		//LogManager.Log("track.TrackTime = " , track.AnimationEnd);

		yield return new WaitForSeconds(track.AnimationEnd);
		spine_BossDamage.skeleton.FlipX = false;


		spine_Boss.gameObject.SetActive(true);
		spine_BossAttack.gameObject.SetActive(false);
		spine_BossDamage.gameObject.SetActive(false);
		spine_BossDie.gameObject.SetActive(false);

		CorePlayBossWar.BossAttackFinishEvent.Invoke();
		missileLauncher.Reset();
	}
	IEnumerator CorDie()
	{
		LogManager.Log(" 小朋友 胜利!! ");
		//隐藏/显示
		spine_Boss.gameObject.SetActive(false);
		spine_BossDie.gameObject.SetActive(true);
		var track = state_BossDie.SetAnimation(0, "animation", false);
		yield return new WaitForSeconds(track.animationEnd);
		CorePlayBossWar.BossFinishEvent.Invoke();

	}
	#endregion
}
//BossTestController













