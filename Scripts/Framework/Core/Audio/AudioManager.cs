using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Snaplingo.SaveData;
using LitJson;
using System;
using System.Text;
using System.IO;

[RequireComponent(typeof(AudioSource))]
public class AudioManager : Manager, ISaveData
{
	// 注意，请不要在Awake中使用Instance，否则会出现死循环
	public static AudioManager Instance { get { return GetManager<AudioManager>(); } }

	private bool _mute;
	protected override void Init()
	{
		base.Init();

		var obj = new GameObject();
		obj.name = "BackgroundSound";
		obj.transform.SetParent(transform);
		GameObject lyricObj = new GameObject();
		lyricObj.name = "LyricSound";
		lyricObj.transform.SetParent(transform);

		_bgmSource = obj.AddComponent<AudioSource>();
		_bgmSource.playOnAwake = false;
		_sfxSource = GetComponent<AudioSource>();
		_sfxSource.playOnAwake = false;

		_lyricsSource = lyricObj.AddComponent<AudioSource>();
		_lyricsSource.playOnAwake = false;
		_mute = false;
		//InitBtnClick();

		SaveDataUtils.LoadTo(this);
	}

	string _folder = "Audio/";

	AudioSource _sfxSource;
	AudioSource _bgmSource;
	AudioSource _lyricsSource;

	public bool IsSfxPlaying
	{
		get
		{
			return _sfxSource.isPlaying;
		}
	}
	public bool IsBGMPlaying
	{
		get
		{
			return _bgmSource.isPlaying;
		}
	}

	public AudioSource BGMSource
	{
		get { return _bgmSource; }
	}

	public AudioSource LyricSource
	{
		get { return _lyricsSource; }
	}

	Queue<AudioSource> _coverSfxs = new Queue<AudioSource>();

	AudioSource GetIdleSfx()
	{
		var count = _coverSfxs.Count;
		while (count > 0)
		{
			count--;
			var sfx = _coverSfxs.Dequeue();
			_coverSfxs.Enqueue(sfx);
			if (!sfx.isPlaying)
				return sfx;
		}
		return CreateSfx();
	}

	AudioSource CreateSfx()
	{
		var audioSource = gameObject.AddComponent<AudioSource>();
		audioSource.playOnAwake = false;
		_coverSfxs.Enqueue(audioSource);
		audioSource.enabled = _sfxSource.enabled;
		audioSource.volume = _sfxSource.volume;
		return audioSource;
	}

	void BasePlaySfx(AudioSource audioSource, object type, bool isLoop = false)
	{
		if (type.GetType() == typeof(int))
			audioSource.clip = ResourceLoadUtils.Load<AudioClip>(_folder + AudioConfig.Instance.GetNameById((int)type), true);
		else if (type.GetType() == typeof(string))
			audioSource.clip = ResourceLoadUtils.Load<AudioClip>(_folder + AudioConfig.Instance.GetNameByKey((string)type), true);
		audioSource.loop = isLoop;
		audioSource.Play();
	}

	public void PlaySfx(int musicId, bool isLoop = false, bool cover = true)
	{
		if (_mute) return;

		if (!cover)
		{
			var sfx = GetIdleSfx();
			BasePlaySfx(sfx, musicId, isLoop);
		}
		else
			BasePlaySfx(_sfxSource, musicId, isLoop);

	}

	public void PlaySfx(string musicType, bool isLoop = false, bool cover = true)
	{
		if (_mute) return;

		if (!cover)
		{
			var sfx = GetIdleSfx();
			BasePlaySfx(sfx, musicType, isLoop);
		}
		else
			BasePlaySfx(_sfxSource, musicType, isLoop);
	}

	public void StopSfx()
	{
		_sfxSource.Stop();
		while (_coverSfxs.Count > 0)
		{
			var sfx = _coverSfxs.Dequeue();
			Destroy(sfx);
		}
	}

	public void PlayMusic(int musicId)
	{
		if (_mute) return;

		_bgmSource.clip = ResourceLoadUtils.Load<AudioClip>(_folder + AudioConfig.Instance.GetNameById(musicId), true);
		_bgmSource.loop = true;
		_bgmSource.Play();
	}

	public float GetBgmTime()
	{
		return _bgmSource.time;
	}

	float time;
	public void StartAudioFadeOut()
	{
		time = CorePlaySettings.Instance.m_AudioTimeLength;
		float deltaData = (AudioListener.volume - CorePlaySettings.Instance.m_AudioVolume) / 20;
		float deltaTime = time / 20;
		StartCoroutine(StartAudioFadeOut(deltaData, deltaTime));
	}

	IEnumerator StartAudioFadeOut(float data, float delta)
	{
		AudioListener.volume -= data;
		yield return new WaitForSeconds(delta);
		time -= delta;
		if (time > 0)
		{
			StartCoroutine(StartAudioFadeOut(data, delta));
		}
	}

	public void StartAudioFadeIn()
	{
		time = CorePlaySettings.Instance.m_AudioTimeLength;
		float deltaData = (1 - AudioListener.volume) / 20;
		float deltaTime = time / 20;
		StartCoroutine(StartAudioFadeIn(deltaData, deltaTime));
	}

	IEnumerator StartAudioFadeIn(float data, float delta)
	{
		AudioListener.volume += data;
		yield return new WaitForSeconds(delta);
		time -= delta;
		if (time > 0)
		{
			StartCoroutine(StartAudioFadeIn(data, delta));
		}
	}

	public void SetAudioPitch(float pitchValue)
	{
		_bgmSource.pitch = pitchValue;
	}

	public void SetAudioClipSpeed(float speed)
	{
		//_bgmSource.clip.
	}

	public void PlayMusic(string musicType)
	{//播放系统背景音乐
		if (_mute) return;

		_bgmSource.clip = ResourceLoadUtils.Load<AudioClip>(_folder + AudioConfig.Instance.GetNameByKey(musicType), true);
		_bgmSource.loop = true;
		_bgmSource.Play();
	}

	private Dictionary<string, AudioClip> _wordCache = new Dictionary<string, AudioClip>();
	public void ClearCache()
	{
		_wordCache.Clear();
		_songCache.Clear();
		_bgmSource.clip = null;
		_lyricsSource.clip = null;
	}

	private Dictionary<string, AudioClip> _songCache = new Dictionary<string, AudioClip>();
	private string _songFolder = "Audio/Songs/";
	private string _lyricFolder = "Audio/Lyrics/";
	private string _wordFolder = "Audio/Words/";
	private string _audioWordsFolder = "Audio/AudioWords/";
	public void LoadSong(string songName)
	{
		if (!_songCache.ContainsKey(songName))
		{
			AudioClip clip = ResourceLoadUtils.Load<AudioClip>(_songFolder + songName.ToLower(), true);
			_songCache.Add(songName, clip);
		}
	}

	public void PlaySong(string songName, bool loop = false)
	{//播放游戏歌曲
		if (_mute) return;

		LoadSong(songName);

		_bgmSource.clip = _songCache[songName];
		_bgmSource.loop = loop;
		_bgmSource.time = 0;
		_bgmSource.Play();
	}

	public void LoadLyric(string fileName)
	{
		if (!_songCache.ContainsKey(fileName))
		{
			AudioClip clip = ResourceLoadUtils.Load<AudioClip>(_lyricFolder + fileName.ToLower(), true);
			_songCache.Add(fileName, clip);
		}
	}

	public void PlayLyric(string fileName)
	{
		if (_mute) return;

		LoadLyric(fileName);

		_lyricsSource.clip = _songCache[fileName];
		_lyricsSource.loop = false;
		_lyricsSource.time = 0;
		_lyricsSource.Play();
	}


	private AudioSource _playWordSource;
	private Action _playWordCompleteCallback;
	private string _songID;
	public void PlayWord(string word, Action callback = null)
	{
		if (_mute) return;

		try
		{
			IsPause = false;
			_playWordCompleteCallback = callback;
			_playWordSource = GetIdleSfx();
			if (_wordCache.ContainsKey(word))
			{
				_playWordSource.clip = _wordCache[word];
			}
			else
			{
				_playWordSource.clip = ResourceLoadUtils.Load<AudioClip>(_wordFolder + word.Replace(' ', '_').ToLower(), true);
				_wordCache.Add(word, _playWordSource.clip);
			}
			_playWordSource.loop = false;
			_playWordSource.Play();
		}
		catch (Exception e)
		{
			LogManager.Log(e.Message);
		}

	}

	public void PlayAudioWord(string word, string songID, Action callback = null)
	{
		return;
		//try
		//{
		//	IsPause = false;
		//	_playWordCompleteCallback = callback;
		//	_playWordSource = GetIdleSfx();
		//	if (_wordCache.ContainsKey(word))
		//	{
		//		_playWordSource.clip = _wordCache[word];
		//	}
		//	else
		//	{
		//		_playWordSource.clip = ResourceLoadUtils.Load<AudioClip>(_audioWordsFolder + songID + "/" + word.Replace(' ', '_').ToLower(), true);
		//		_wordCache.Add(word, _playWordSource.clip);
		//	}
		//	_playWordSource.loop = false;
		//	_playWordSource.Play();
		//}
		//catch (Exception e)
		//{
		//	LogManager.Log(e.Message);
		//}

	}

	public void StopMusic()
	{
		_bgmSource.Stop();
		_lyricsSource.Stop();
	}

	private bool IsPause;
	public void PauseSound()
	{
		_bgmSource.Pause();
		_sfxSource.Pause();
		_lyricsSource.Pause();
		if (_playWordSource != null)
			_playWordSource.Pause();
		IsPause = true;
	}

	public void ResumeSound()
	{
		_bgmSource.UnPause();
		_sfxSource.UnPause();
		_lyricsSource.UnPause();
		if (_playWordSource != null)
			_playWordSource.UnPause();
		IsPause = false;
	}

	public void StopSound()
	{
		_bgmSource.Stop();
		_sfxSource.Stop();
		_lyricsSource.Stop();
	}

	public void StopWordSource()
	{
		if (_playWordSource != null)
		{
			_playWordSource.Stop();
			_playWordSource = null;
		}
		IsPause = false;
	}

	private float _bgmClipPauseTime;
	public void Mute()
	{
		_mute = true;
		if (_bgmSource.isPlaying)
		{
			_bgmClipPauseTime = _bgmSource.time;
		}
		else
		{
			_bgmClipPauseTime = -1;
		}

		foreach (AudioSource audioSource in _coverSfxs)
		{
			audioSource.Stop();
		}

		_muteTimeLength = 0;
		_bgmSource.Pause();
		_lyricsSource.Pause();
		_sfxSource.Stop();
	}

	public void ResumeFromMute()
	{
		_mute = false;
		if (_bgmClipPauseTime > 0)
		{
			_bgmSource.time = _bgmClipPauseTime + _muteTimeLength;
		}

		_bgmSource.UnPause();
	}

	public void PlayWordList(List<string> wordList, string songID = null, Action callback = null)
	{
		if (wordList.Count == 0)
		{
			if (callback != null)
				callback.Invoke();
			return;
		}

		_waitQueue.Clear();
		_waitQueue.AddRange(wordList);
		IsPause = false;
		if (_mute) return;

		try
		{
			_playWordCompleteCallback = callback;
			_songID = songID;
			if (_playWordSource == null)
			{
				_playWordSource = GetIdleSfx();
			}
			if (_wordCache.ContainsKey(_waitQueue[0]))
			{
				_playWordSource.clip = _wordCache[_waitQueue[0]];
			}
			else
			{
				if (songID == null)
				{
					_playWordSource.clip = ResourceLoadUtils.Load<AudioClip>(_wordFolder + _waitQueue[0].ToLower(), true);
				}
				else
				{
					_playWordSource.clip = ResourceLoadUtils.Load<AudioClip>(_audioWordsFolder + songID + "/" + _waitQueue[0].ToLower(), true);
				}
				_wordCache.Add(_waitQueue[0], _playWordSource.clip);
			}
			_playWordSource.loop = false;
			_playWordSource.Play();
			_waitQueue.RemoveAt(0);
		}
		catch (Exception e)
		{
			LogManager.Log(e.Message);
		}
	}

	public void ClearWordQueue()
	{
		_waitQueue.Clear();
	}

	private float _muteTimeLength;
	List<string> _waitQueue = new List<string>();
	void Update()
	{
		if (_mute)
		{
			_muteTimeLength += Time.deltaTime;
		}

		if (_playWordCompleteCallback != null || _waitQueue.Count > 0)
		{
			if (_playWordSource != null && !_playWordSource.isPlaying && !IsPause)
			{
				if (_waitQueue.Count > 0)
				{
					if (_songID != null)
					{
						PlayAudioWord(_waitQueue[0], _songID, _playWordCompleteCallback);
					}
					else
					{
						PlayWord(_waitQueue[0], _playWordCompleteCallback);
					}

					_waitQueue.RemoveAt(0);
				}
				else
				{
					_playWordCompleteCallback.Invoke();
					_playWordCompleteCallback = null;
					_playWordSource = null;
					_songID = null;
				}
			}
		}
	}

	public void ClearPlayWordSources()
	{
		IsPause = false;
		_playWordCompleteCallback = null;
		_playWordSource = null;
	}

	public void InitBtnClick(GameObject pageObj = null)
	{
		Button[] btnAry;
		if (pageObj != null)
			btnAry = pageObj.GetComponentsInChildren<Button>(true);
		else
			btnAry = FindObjectsOfType<Button>();
		foreach (var btn in btnAry)
		{
			if (string.IsNullOrEmpty(CustomAudioBtn.Get(btn.gameObject).m_AudioType))
			{
				CustomAudioBtn.Get(btn.gameObject).m_AudioType = "BtnClick";
			}
		}
	}

	#region ISaveData
	System.Action _musicOnCallback = null;
	public void AddMusicOnCallback(System.Action action)
	{
		_musicOnCallback += action;
	}

	public void RemoveMusicOnCallback(System.Action action)
	{
		_musicOnCallback -= action;
	}

	bool _musicEnable = true;
	bool _sfxEnable = true;

	public bool Music
	{
		get
		{
			return _musicEnable;
		}
		set
		{
			var oldMusicEnable = _musicEnable;
			_musicEnable = value;
			_bgmSource.enabled = _musicEnable;
			_bgmSource.volume = _musicEnable ? 1 : 0;
			SaveDataUtils.Save(this);
			if (_musicEnable && !oldMusicEnable)
			{
				if (_musicOnCallback != null)
					_musicOnCallback();
			}
		}
	}

	public bool Sfx
	{
		get
		{
			return _sfxEnable;
		}
		set
		{
			_sfxEnable = value;
			_sfxSource.enabled = _sfxEnable;
			_sfxSource.volume = _sfxEnable ? 1 : 0;
			foreach (var source in _coverSfxs)
			{
				source.enabled = _sfxEnable;
				source.volume = _sfxEnable ? 1 : 0;
			}
			SaveDataUtils.Save(this);
		}
	}

	public void SetSound(bool enabled)
	{
		Music = enabled;
		Sfx = enabled;
		SaveDataUtils.Save(this);
	}

	public void SetMusicVolumn(float vol)
	{
		if (Music)
			_bgmSource.volume = vol;
	}

	public void SetSfxVolumn(float vol)
	{
		if (Sfx)
			_sfxSource.volume = vol;
	}

	public string SaveTag()
	{
		return "AudioInfo";
	}

	public string SaveAsJson()
	{
		JsonData data = new JsonData();
		data["music"] = Music;
		data["sfx"] = Sfx;
		return data.ToJson();
	}

	public void LoadFromJson(string json)
	{
		JsonData data = JsonMapper.ToObject(json);
		Music = bool.Parse(data.TryGetString("music"));
		Sfx = bool.Parse(data.TryGetString("sfx"));
	}

	#endregion

}
