using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.UI;
using UnityEngine;

public class EiderToolPage : MonoBehaviour
{
	public static EiderToolPage Instance;
	private GameObject m_BeginPage;
	private GameObject m_SelectSongPage;
	private GameObject m_SelectAudioPage;
	private GameObject m_EiderPage;

	private SongTxtObject m_SongTxtObject = new SongTxtObject();

	public SongTxtObject SongObject
	{
		get { return m_SongTxtObject; }
		set { m_SongTxtObject = value; }
	}

	public GameObject SelectAudioPage
	{
		get { return m_SelectAudioPage; }
	}

	public GameObject EiderPage
	{
		get { return m_EiderPage; }
	}

	private void Awake()
	{
		Instance = this;
	}

	void Start()
	{
		m_BeginPage = transform.Find("BeginPage").gameObject;
		m_SelectSongPage = transform.Find("SelectSongPage").gameObject;
		m_SelectAudioPage = transform.Find("SelectAudioPage").gameObject;
		m_EiderPage = transform.Find("EiderPage").gameObject;

		m_BeginPage.SetActive(true);
		transform.Find("BeginPage/Add").GetComponent<Button>().onClick.AddListener(AddNewSong);
		transform.Find("BeginPage/Resave").GetComponent<Button>().onClick.AddListener(Resave);

		m_SelectSongPage.SetActive(false);
		m_SelectAudioPage.SetActive(false);
		m_EiderPage.SetActive(false);
	}

	void AddNewSong()
	{
		m_BeginPage.SetActive(false);
		m_SelectAudioPage.SetActive(true);
	}

	void Resave()
	{
		m_BeginPage.SetActive(false);
		m_SelectSongPage.SetActive(true);
	}

	public void BackToBegin()
	{
		m_BeginPage.SetActive(true);
		m_SelectSongPage.SetActive(false);
		m_SelectAudioPage.SetActive(false);
		m_EiderPage.SetActive(false);
	}

	public void OpenEiderPage()
	{
		m_SelectAudioPage.SetActive(false);
		m_SelectSongPage.SetActive(false);
		m_EiderPage.SetActive(true);
	}

}
