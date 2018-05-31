using UnityEngine.UI;
using Snaplingo.UI;
using DG.Tweening;
using UnityEngine;
using System.Collections;

public class CorePlayPage : Page
{
	public GameObject but_Pause;
    public void ShowPauseButton()
	{
		but_Pause.SetActive(true);
	}
    public void HidePauseButton()
	{
		but_Pause.SetActive(false);
	}
	public override void Open()
	{
		base.Open();
	}
	public static void BackToMainProcess()
	{
		Time.timeScale = 1f;
		AudioController.StopMusic();
		CorePlayManager.Instance.CleanAll();
		StaticMonoBehaviour.Instance.StopAllCoroutines();
		ObjectPool.ClearPool("RhythmController");
		LoadSceneManager.Instance.LoadNormalScene(LoadSceneManager.BookSceneName);
	}
}
