using System.Collections;
using System.Collections.Generic;
using Snaplingo.UI;
using UnityEngine;
using UnityEngine.Video;

public class LoginScene : MonoBehaviour
{
	[SerializeField] private Transform[] m_models;
	public static bool isFirstGoingGame; 
	private void Awake()
	{
		isFirstGoingGame = string.IsNullOrEmpty(PlayerPrefs.GetString("isFirst")) ? true : false;
	}
	private void Start()
	{
		PageManager.Instance.OpenPage<MainPage>(false, AddNode);
	}
	void AddNode()
    {
        PageManager.Instance.CurrentPage.AddNode<StartSceneNode>(true);

        if (CorePlaySettings.Instance.m_UseMemoryPool)
        {
            VideoClip vc = ResourceLoadUtils.Load<VideoClip>("animation");
            StaticMemoryPool.AddIntoPool("startAnimation", vc);
        }
		PageManager.Instance.gameObject.GetComponent<Canvas>().worldCamera = Camera.main;
		for (int i = 0; i < m_models.Length; ++i)
		{
			m_models[i].gameObject.SetActive(true);
		}

    }


    
    
}
