using UnityEngine;
using System;
using System.Collections.Generic;

public class EffectSettings : ScriptableObject
{
	[Serializable]
	public class ParticleData
	{
		public string name;
		public int poolCount = 10;
		public ParticleSystem prefab;
		public short minBursts = 5;
		public short maxBursts = 30;
	}


	#region [ Property --- ]
	static string path = "Settings/EffectSettings";
	// Tap Key
	[Header("特效递增幅度")]
	public AnimationCurve emissonCurve;
	public int maxCombo;

	[Header("Combo 间隔")]
	public int comboInterval;

	[Header("震屏 概率")]
	public int shackRate;
	[Header("Combo B 特效 概率")]
	public int comboBRate;

	[Header(" --- Datas --- ")]
	public ParticleData[] datas;
	#endregion


	static EffectSettings _instance;
	public static EffectSettings Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = Resources.Load<EffectSettings>(path);
			}
			return _instance;
		}
	}


	#region [ Public --- ]
	#endregion



}
//EffectSettings














