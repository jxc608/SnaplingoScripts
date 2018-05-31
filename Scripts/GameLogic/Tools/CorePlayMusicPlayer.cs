using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CorePlayMusicPlayer : MonoBehaviour
{
	public static CorePlayMusicPlayer Instance;

	private AudioSource m_BGMSource;
	public float GetBgmTime()
	{
		return m_BGMSource.time;
	}
	private float m_Time;
	private void Awake()
	{
		Instance = this;
		m_BGMSource = gameObject.AddComponent<AudioSource>();
	}

	public void StartAudioFadeOut()
	{
		m_Time = CorePlaySettings.Instance.m_AudioTimeLength;
		float deltaData = (AudioListener.volume - CorePlaySettings.Instance.m_AudioVolume) / 20;
		float deltaTime = m_Time / 20;
		StartCoroutine(StartAudioFadeOut(deltaData, deltaTime));
	}

	IEnumerator StartAudioFadeOut(float data, float delta)
	{
		AudioListener.volume -= data;
		yield return new WaitForSeconds(delta);
		m_Time -= delta;
		if (m_Time > 0)
		{
			StartCoroutine(StartAudioFadeOut(data, delta));
		}
	}

	public void StartAudioFadeIn()
	{
		m_Time = CorePlaySettings.Instance.m_AudioTimeLength;
		float deltaData = (1 - AudioListener.volume) / 20;
		float deltaTime = m_Time / 20;
		StartCoroutine(StartAudioFadeIn(deltaData, deltaTime));
	}

	IEnumerator StartAudioFadeIn(float data, float delta)
	{
		AudioListener.volume += data;
		yield return new WaitForSeconds(delta);
		m_Time -= delta;
		if (m_Time > 0)
		{
			StartCoroutine(StartAudioFadeIn(data, delta));
		}
	}

	private Dictionary<string, AudioClip> _wordCache = new Dictionary<string, AudioClip>();
	public void ClearWordCache()
	{
		_wordCache.Clear();
	}

	private Dictionary<string, AudioClip> _songCache = new Dictionary<string, AudioClip>();
	public void LoadSong(string songName)
	{
		if (!_songCache.ContainsKey(songName))
		{
			AudioClip clip = ResourceLoadUtils.Load<AudioClip>("Audio/" + songName.ToLower(), true);
			if (clip == null)
			{
				string extraPath = Application.dataPath + "/TeachAudioBundles/" + songName;
				AssetBundle bundle = AssetBundle.LoadFromFile(extraPath);
				clip = bundle.LoadAsset<AudioClip>(songName);
			}

			if (clip != null)
			{
				_songCache.Add(songName, clip);
			}
			else
			{
				PromptManager.Instance.MessageBox(PromptManager.Type.FloatingTip, "找不到音乐 " + songName);
			}
		}
	}

	public void PlaySong(string songName)
	{//播放游戏歌曲
		LoadSong(songName);

		m_BGMSource.clip = _songCache[songName];
		m_BGMSource.loop = false;
		m_BGMSource.Play();
	}

	public void StopMusic()
	{
		m_BGMSource.Stop();

		if (AudioController.Instance != null)
			AudioController.StopAll();
	}

	public void PauseMusic()
	{
		m_BGMSource.Pause();

		if (AudioController.Instance != null)
			AudioController.PauseMusic();
	}

	public void ResetSongTime(float audioTime)
	{
		//AudioManager.Instance.ClearWordQueue();
		if( audioTime > m_BGMSource.clip.length)
			m_BGMSource.time = m_BGMSource.clip.length;
		else
			m_BGMSource.time = audioTime;
		//AudioController.UnpauseMusic();
	}

	public void LoadSongTest(string songName)
	{
		if (!_songCache.ContainsKey(songName))
		{
			AudioClip clip = ResourceLoadUtils.Load<AudioClip>("Audio/Songs/" + songName.ToLower(), true);
			if (clip == null)
			{
				string extraPath = Application.dataPath + "/TeachAudioBundles/" + songName;
				AssetBundle bundle = AssetBundle.LoadFromFile(extraPath);
				clip = bundle.LoadAsset<AudioClip>(songName);
			}

			if (clip != null)
			{
				_songCache.Add(songName, clip);
			}
			else
			{
				PromptManager.Instance.MessageBox(PromptManager.Type.FloatingTip, "找不到音乐 " + songName);
			}
		}
	}

	public float GetAudioTimeLengh(string songName)
	{
		return _songCache[songName].length;
	}

	public void MoveSongTime(float changeDelta)
	{
		float tempTime = m_BGMSource.time + changeDelta;
		if (tempTime < 0)
			m_BGMSource.time = 0;
		else if (tempTime > m_BGMSource.clip.length)
			m_BGMSource.time = m_BGMSource.clip.length;
		else
			m_BGMSource.time = tempTime;
	}

}
