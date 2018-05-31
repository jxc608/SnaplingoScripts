using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;
using Snaplingo.UI;
using System;

public class VideoPlay : Manager
{
	public static VideoPlay Instance { get { return GetManager<VideoPlay>(); } }

	bool m_StartFade;
	VideoPlayer m_Player;
	private float m_Timer;
	private GameObject m_Camera;
	protected override void Init()
	{
		m_StartFade = false;
		CreateObj();
		InitVideoPlay();
	}

	private const int VideoPlayCameraDepth = 2;
	private void CreateObj()
	{
		m_Camera = new GameObject();
		m_Camera.name = "VideoPlay";

		Camera _camera = m_Camera.AddComponent<Camera>();

		_camera.clearFlags = CameraClearFlags.SolidColor;
		_camera.backgroundColor = Color.black;
		_camera.depth = VideoPlayCameraDepth;
		_camera.cullingMask = 0;
		// VideoPlayer automatically targets the camera backplane when it is added
		// to a camera object, no need to change videoPlayer.targetCamera.
		m_Player = m_Camera.AddComponent<UnityEngine.Video.VideoPlayer>();
	}

	private void InitVideoPlay()
	{
		// Play on awake defaults to true. Set it to false to avoid the url set
		// below to auto-start playback since we're in Start().
		m_Player.playOnAwake = false;
		m_Player.aspectRatio = VideoAspectRatio.FitVertically;
		// By default, VideoPlayers added to a camera will use the far plane.
		// Let's target the near plane instead.
		m_Player.renderMode = UnityEngine.Video.VideoRenderMode.CameraNearPlane;

		// This will cause our scene to be visible through the video being played.
		m_Player.targetCameraAlpha = 1F;

		// Set the video to play. URL supports local absolute or relative paths.
		// Here, using absolute.
		//videoPlayer.url = Application.dataPath + "/";
		m_Player.source = VideoSource.VideoClip;

		m_Player.loopPointReached += PlayEnd;

		// Skip the first 100 frames.
		m_Player.frame = 100;

		// Restart from beginning when done.
		m_Player.isLooping = false;

		// Each time we reach the end, we slow down the playback by a factor of 10.

	}

	VideoPlayer.EventHandler m_Callback;
	public void PlayVideo(VideoClip clip, VideoPlayer.EventHandler callback)
	{
		m_Callback = callback;
		m_Camera.SetActive(true);
		m_Player.clip = clip;
		if (callback != null)
			m_Player.loopPointReached += callback;

		// Start playback. This means the VideoPlayer may have to prepare (reserve
		// resources, pre-load a few frames, etc.). To better control the delays
		// associated with this preparation one can use videoPlayer.Prepare() along with
		// its prepareCompleted event.
		m_Player.Play();
		//LogManager.Log(" Play >> " , Time.realtimeSinceStartup);
	}

	private void PlayEnd(VideoPlayer source)
	{
		m_Camera.SetActive(false);
		ClearCallback();
	}

	private void ClearCallback()
	{
		if (m_Callback != null)
			m_Player.loopPointReached -= m_Callback;
		m_Callback = null;
	}

	private Action m_FadeCallback;
	public void FadeOut(Action callback)
	{
		ClearCallback();
		m_FadeCallback = callback;
		m_StartFade = true;
		m_Timer = 0;
		m_Camera.GetComponent<Camera>().clearFlags = CameraClearFlags.Depth;
	}

	public void SetAudioMute()
	{
		m_Player.SetDirectAudioMute(0, true);
	}

	private void Update()
	{
		if (m_StartFade)
		{
			m_Timer += Time.deltaTime;
			if (m_Timer >= 1)
			{
				m_Player.Stop();
				m_Camera.SetActive(false);
				m_StartFade = false;
				m_Timer = 1;
				if (m_FadeCallback != null)
				{
					m_FadeCallback.Invoke();
					m_FadeCallback = null;
				}
			}
			m_Player.targetCameraAlpha = 1 - m_Timer;
		}
	}
}
