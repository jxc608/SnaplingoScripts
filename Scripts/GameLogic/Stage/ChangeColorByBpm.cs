using UnityEngine;
using System.Collections;

[RequireComponent (typeof (SpriteRenderer))]
public class ChangeColorByBpm : MonoBehaviour
{
	public enum ChangeType
	{
		Random,
		Switch,
		RandomSwitch,
	}

	#region [ --- ] Property
	public ChangeType changeType;
	public Color switchColor;
	Color initColor;
	bool isSwitch;
	public byte alpha = 255;
	#endregion

	#region [ --- ] Component
	//[Header (" [ --- ] Component")]
	SpriteRenderer render;
	#endregion




	#region [ Mono ]
	void Awake ()
	{
		render = GetComponent<SpriteRenderer> ();
		initColor = render.color;
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

		switch (changeType)
		{
			case ChangeType.Random:
				render.color = new Color32 ((byte)Random.Range (0, 255),
											(byte)Random.Range (0, 255),
											(byte)Random.Range (0, 255),
											alpha);
				break;
			case ChangeType.Switch:
				if (isSwitch)
					render.color = initColor;
				else
					render.color = switchColor;
				isSwitch = !isSwitch;
				break;
			case ChangeType.RandomSwitch:
				if (isSwitch)
					render.color = switchColor;
				else
				{
					render.color = new Color32 ((byte)Random.Range (0, 255),
												(byte)Random.Range (0, 255),
												(byte)Random.Range (0, 255),
												alpha);
				}
				isSwitch = !isSwitch;
				break;
		}


	}




}
//ChangeColorByBpm














