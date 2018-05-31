using UnityEngine;
using System.Collections;
using Spine.Unity;
using DG.Tweening;

public class Boss00Controller : SceneController
{
	#region [ --- Property --- ]
	[Header(" --- Setting ---")]
	[SpineAnimation]
	public string attack;
	[SpineAnimation]
	public string damage, die, enter, enterTalking, idle, idle1;
	Spine.AnimationState state_Boss;

	Vector3 bossStartPos;
	public bool gameEnd;
	#endregion


	#region [ --- Object References --- ]
	[Header(" --- Object References ---")]
	public SkeletonAnimation spine_Boss;
	public BossElectricity bossElectricity;
	public BossFireProjectile bossFireProjectile;
	public MissileLauncher missileLauncher;
	public GameObject obj_WarningUI;
	#endregion



	#region [ --- Mono --- ]
	public override void Awake()
	{
		base.Awake();
		obj_WarningUI.SetActive(false);
	}
	public override void OnDestroy()
	{
		base.OnDestroy();
		CorePlaySceneManager.bossEnterFinishEvent.RemoveListener(OnBossEnterFinish);
		CorePlaySceneManager.voiceRepeatEvent.RemoveListener(OnVoiceRepeat);
	}
	#endregion


	#region [ --- Public Override --- ]
	public override void Init()
	{
		base.Init();
		gameEnd = false;
		bossStartPos = spine_Boss.transform.position;
		state_Boss = spine_Boss.AnimationState;
		state_Boss.Complete += OnAnimComplete;
		CorePlaySceneManager.bossEnterFinishEvent.AddListener(OnBossEnterFinish);
		CorePlaySceneManager.voiceRepeatEvent.AddListener(OnVoiceRepeat);
		Reset();
	}
	public override void Reset()
	{
		base.Reset();
		if (state_Boss == null)
			return;
		gameEnd = false;

		StopAllCoroutines();
		state_Boss.SetEmptyAnimation(0, 0);
		var tarck = state_Boss.AddAnimation(0, enterTalking, false, 0);
		tarck.TimeScale = 0;

		bossElectricity.Power = 0;
		bossFireProjectile.Stop();
		missileLauncher.Reset();
		obj_WarningUI.SetActive(false);

		DOTween.Complete(spine_Boss.transform);
		spine_Boss.transform.position = bossStartPos;

		AudioController.StopCategory("Boss00");
		AudioController.SetCategoryVolume("Boss00", 1f);
		AudioController.SetCategoryVolume("Sfx", 1f);
		Time.timeScale = 1f;
	}
	public override void OnLoadMissile(Vector3 pos, bool jump = false)
	{
		missileLauncher.LoadMissile(pos, jump);
	}
	#endregion



	#region [ --- Event Call Back --- ]
	void OnVoiceRepeat(bool value)
	{
		if (value)
		{
			AudioController.SetCategoryVolume("Boss00", .3f);
			AudioController.SetCategoryVolume("Sfx", .3f);
		}
		else
		{
			AudioController.SetCategoryVolume("Boss00", 1f);
			AudioController.SetCategoryVolume("Sfx", 1f);
		}
	}
	public override void OnSkipEnter()
	{
		StopAllCoroutines();
		CorePlaySceneManager.bossEnterFinishEvent.Invoke();
		AudioController.StopCategory("Boss00");

	}
	public override void OnBossEnter()
	{
		if (gameEnd == true)
			return;
		StartCoroutine(CorEnter());
	}
	IEnumerator CorEnter()
	{
		// warning 时间
		obj_WarningUI.SetActive(true);
		var auio1 = AudioController.GetAudioItem("Warning");
		float warningDuration = auio1.subItems[0].Clip.length * 3;
		yield return new WaitForSeconds(warningDuration);
		obj_WarningUI.SetActive(false);

		var auio2 = AudioController.Play("EnterTalking");
		float enterTalkDuration = auio2.clipLength;
		state_Boss.SetAnimation(0, enterTalking, true);

		float cut = 1f;
		yield return new WaitForSeconds(enterTalkDuration - cut);
		AudioController.Play("Enter");
		yield return new WaitForSeconds(cut);
		var track = state_Boss.SetAnimation(0, enter, false);
		track.AnimationStart = cut;

		yield return new WaitForSeconds(track.AnimationEnd - cut);
		CorePlaySceneManager.bossEnterFinishEvent.Invoke();
	}

	void OnBossEnterFinish()
	{
		obj_WarningUI.SetActive(false);
	}

	public override void OnBossPrepareAttack()
	{
		if (gameEnd == true)
			return;
		StartCoroutine(CorPrepareAttack());
	}
	IEnumerator CorPrepareAttack()
	{
		yield return null;
		//LogManager.Log("Go = OnBossPrepareAttack ");
		state_Boss.SetAnimation(0, idle, true);
	}

	public override void OnBossAttack()
	{
		if (gameEnd == true)
			return;
		StopAllCoroutines();
		StartCoroutine(CorAttack());
	}
	IEnumerator CorAttack()
	{
		AudioController.StopCategory("Boss00");
		yield return null;

		missileLauncher.Reset();
		bossFireProjectile.FireAll();
		var track = state_Boss.SetAnimation(0, attack, false);
		state_Boss.AddAnimation(0, idle, true, track.AnimationEnd);

		yield return new WaitForSeconds(track.AnimationEnd);
		CorePlayBossWar.BossAttackFinishEvent.Invoke();
	}

	public override void OnBossDamage()
	{
		if (gameEnd == true)
			return;
		StopAllCoroutines();
		StartCoroutine(CorDamage());
	}
	IEnumerator CorDamage()
	{
		AudioController.StopCategory("Boss00");
		yield return null;

		yield return new WaitForSeconds(0.5f);
		missileLauncher.LaunchAll();
		yield return new WaitForSeconds(missileLauncher.launchDuration + .2f);
		AudioController.Play("Hurt");
		bossElectricity.Power += 1;

		var track = state_Boss.SetAnimation(0, damage, false);
		spine_Boss.skeleton.FlipX = Random.value > .5f;

		yield return new WaitForSeconds(track.AnimationEnd);
		state_Boss.AddAnimation(0, idle, true, track.AnimationEnd);

		CorePlayBossWar.BossAttackFinishEvent.Invoke();
		missileLauncher.Reset();
	}

	public override void OnBossDraw()
	{
		if (gameEnd == true)
			return;
		StopAllCoroutines();
		StartCoroutine(_CorDraw());
	}
	IEnumerator _CorDraw()
	{
		yield return new WaitForSeconds(0.5f);
		missileLauncher.DropAll();
		yield return new WaitForSeconds(missileLauncher.launchDuration + 2f);
		CorePlayBossWar.BossAttackFinishEvent.Invoke();
		missileLauncher.Reset();
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
			LogManager.Log(" 小朋友 胜利!! ");
			StartCoroutine(CorDie());
		}
	}
	IEnumerator CorDie()
	{
		AudioController.StopCategory("Boss00");
		yield return null;
		var _audio = AudioController.Play("Die");
		bossElectricity.Power = 0;
		state_Boss.SetAnimation(0, die, false);

		yield return new WaitForSeconds(1f);
		_audio.pitch = 0.1f;
		Time.timeScale = 0.1f;
		yield return new WaitForSeconds(.3f);
		_audio.pitch = 1f;
		Time.timeScale = 1f;

		//yield return new WaitForSeconds(track.AnimationEnd - 1.3f);
		yield return new WaitForSeconds(12f);
		//LogManager.Log("track.AnimationEnd = " , track.AnimationEnd);
		CorePlayBossWar.BossFinishEvent.Invoke();

	}
	#endregion



	#region [ --- Private --- ]
	void OnAnimComplete(Spine.TrackEntry trackEntry)
	{
		string trackName = trackEntry.ToString();
		if (trackName == attack)
		{
		}
		else if (trackName == damage)
		{
		}
		else if (trackName == enter)
		{
		}
		else if (trackName == die)
		{
		}
	}
	#endregion

}
//Boss00Controller













