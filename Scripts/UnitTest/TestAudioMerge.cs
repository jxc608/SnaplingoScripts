using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NAudio.Lame;
using NAudio.Wave.WZT;

public class TestAudioMerge : MonoBehaviour {

    public AudioClip m_Clip1;
    public AudioClip m_Clip2;

    AudioSource m_AudioSource;

	void Start () {
        m_AudioSource = gameObject.AddComponent<AudioSource>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnGUI()
    {
        if(GUI.Button(new Rect(50,50,200, 50), "Splice"))
        {
            List<AudioClip> list = new List<AudioClip>();
            list.Add(m_Clip1);
            list.Add(m_Clip2);
            m_AudioSource.clip = AudioMerge.SpliceAudioClips(list);
            m_AudioSource.Play();
        }

        if(GUI.Button(new Rect(50,100,200,50), "Merge"))
        {
            m_AudioSource.clip = AudioMerge.MergeTwoClip(m_Clip1, m_Clip2);
            m_AudioSource.Play();
            string wavPath = Application.dataPath + "/temp.wav";
            string mp3Path = Application.dataPath + "/temp.mp3";
            SavWav.Save(wavPath, m_AudioSource.clip);
            using (var reader = new WaveFileReader(wavPath))
            using (var writer = new LameMP3FileWriter(mp3Path, reader.WaveFormat, 144000))
                reader.CopyTo(writer);
        }
    }
}
