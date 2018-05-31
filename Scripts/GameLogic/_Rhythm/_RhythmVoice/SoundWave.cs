using UnityEngine;
using System.Collections;

public class SoundWave : MonoBehaviour
{
	#region [ --- Property --- ]
	public float Volume
	{
		get { return _volume; }
		set { _volume = Mathf.Clamp01(value); }
	}

	[Range(0, 1f)]
	[SerializeField]
	float _volume;
	[SerializeField]
	float _amp = 1;
	[SerializeField]
	float _walkAuto;
	#endregion

	#region [ --- Object References --- ]
	//[Header(" --- Object References ---")]
	LineWave[] waves;
	#endregion



	#region [ --- Mono --- ]
	void Awake()
	{
		waves = GetComponentsInChildren<LineWave>();
	}
	void Update()
	{
		foreach (var wave in waves)
		{
			wave.amp = _amp * _volume;
			wave.walkAuto = _walkAuto * (_volume + 1);
		}
	}
	#endregion


}
//SoundWave













