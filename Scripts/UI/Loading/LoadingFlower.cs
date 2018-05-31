using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingFlower : MonoBehaviour {

	[SerializeField] private Image m_leaf;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void Init()
	{
		m_leaf.fillAmount = 0;
	}

	public void UpdateProgress(float value)
	{
		m_leaf.fillAmount = value;
		//LogManager.Log("UpdateProgress: " , value);
	}
}
