using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class DotweenAnimationTest : MonoBehaviour {

    public Transform m_Cube;
    public Ease m_Ease;
	// Use this for initialization
	void Start () {
      
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnGUI()
    {
        if(GUI.Button(new Rect(50,50,50,50), "GO"))
        {
            //m_Cube.position = Vector3.zero;
            //Tweener tweener = m_Cube.DOMove(new Vector3(5,5,0), 1.5f);
            //tweener.SetEase(m_Ease);
            m_Cube.FromA2B(new Vector3(0,0,0), new Vector3(5,5,5), 0.5f);
        }    
    }
}
