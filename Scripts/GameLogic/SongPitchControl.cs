using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SongPitchControl : MonoBehaviour {

    public float m_Pitch = 1f;
    // Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnGUI()
    {
        if(GUI.Button(new Rect(50,50,50,50), "set"))
        {
            AudioManager.Instance.SetAudioPitch(m_Pitch);
        }
    }
}
