using UnityEngine;
using System.Collections;

[RequireComponent (typeof (ParticleSystem))]
public class PlayParticleByBpm : MonoBehaviour
{

	#region [ --- ] Property
	#endregion

	#region [ --- ] Component
	//[Header (" [ --- ] Component")]
	ParticleSystem ps;
	#endregion




	#region [ Mono ]
	void Awake ()
	{
		ps = GetComponent<ParticleSystem> ();
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



	void OnBeat (int beatNum)
	{
		ps.Play ();
	}



}
//PlayParticleByBpm














