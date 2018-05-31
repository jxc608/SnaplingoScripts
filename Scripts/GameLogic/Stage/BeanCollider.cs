using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeanCollider : MonoBehaviour
{
	public GameObject m_Particle;

	ParticleSystem stunned;
	ParticleSystem bounce;

	// Use this for initialization
	void Start ()
	{

	}

	void OnDestroy ()
	{
		if (EffectManager.instance == null) return;

		if (stunned != null)
			stunned.transform.SetParent (EffectManager.instance.transform);
		if (bounce != null)
			bounce.transform.SetParent (EffectManager.instance.transform);

	}

	public void PlayBounceEffect ()
	{
		bounce = EffectManager.Play ("MagicCharge", Vector3.zero);
		if (bounce != null)
		{
			bounce.transform.SetParent (transform, false);
			bounce.transform.localPosition = Vector3.zero;
		}
	}
}
