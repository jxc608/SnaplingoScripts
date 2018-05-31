using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartSceneRhythmScaler : MonoBehaviour
{

	#region [ --- Property --- ]
	public float scaleSize = 0.1f;
	Vector3 startScale;
	#endregion

	void Start()
	{
		startScale = transform.localScale;


	}

	private void Update()
	{
		var music = AudioController.GetCurrentMusic();
		if (music == null)
			return;

		float time = AudioController.GetCurrentMusic().audioTime;
		float beat = time % 0.5f;
		transform.localScale = beat * scaleSize * Vector3.one + startScale;
	}

}
