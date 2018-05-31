using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossTap : RhythmObject
{
	public void Init(string showWord, string matchWord, Vector3 pos, int sentenceIndex)
	{
		m_HitWord = matchWord;
		m_SentenceIndex = sentenceIndex;

		var tran = ObjectPool.GetOne("RhythmController").transform;

		// Set RhythmController
		m_RhythmController = tran.GetComponent<RhythmController>();
		m_RhythmController.Init(this,
								matchWord,
								showWord, pos, EffectInOutTiming.Default);
		m_ClickAble = false;
	}

	public void SetClickable()
	{
		m_ClickAble = true;
	}

	public void ShowWord()
	{
		if (m_RhythmController != null)
		{
			//m_RhythmController.gameObject.SetActive(true);
			m_RhythmController.On_Show();
		}
	}

	public void SetPosition(Vector3 pos)
	{
		m_RhythmController.SetPosition(pos);
	}

	public Vector2 GetScaler()
	{
		return m_RhythmController.GetScaler();
	}

	public Vector3 GetPosition()
	{
		return m_RhythmController.GetPosition();
	}
}
