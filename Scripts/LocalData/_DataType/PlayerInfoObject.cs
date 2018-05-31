using UnityEngine;
using System.Collections;
using System;



[Serializable]
public class PlayerInfoObject
{
	public PlayerInfoObject(
		string nickName,
		int score = 0,
		float accuracy = 0,
		int rank = -1,
		int difficulty = 1,
		string avatar = "NoAvatar")
	{
		this.nickName = nickName;
		this.maxScore = score;
		this.accuracy = accuracy;
		this.rank = rank;
		this.difficulty = difficulty;
		this.avatar = avatar;
	}

	public string nickName;
	public string avatar;
	public int maxScore;
	public float accuracy;
	public int rank;
	public int difficulty;


	//亚男要的字段
	public int maxCombo;
	public int leftLife;
	public float readAccuracy;
	public float wordAccuracy;

}
//PlayerInfoObject














