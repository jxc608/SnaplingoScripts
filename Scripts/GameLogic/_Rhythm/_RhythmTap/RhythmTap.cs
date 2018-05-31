using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

public class RhythmTap : RhythmObject
{

	#region [ --- Object References --- ]
	public RhythmController contrller;
	#endregion

	public int Tutorial_num;
	public bool IsTutorial;

	bool m_Clickable;
	private bool m_TicWord;
	public bool TicWord
	{
		get { return m_TicWord; }
	}
	public void Start(CorePlayManager manager,
					  ClickObj clickWord,
					  EffectInOutTiming inOut,
					  int sentenceIndex)
	{
		IsTutorial = false;
		m_Manager = manager;
		m_Clickable = true;
		m_HitWord = clickWord.m_Word;
		m_SentenceIndex = sentenceIndex;
		// 创建
		m_ObjectTrans = ObjectPool.GetOne("RhythmController").transform;

		// Set Transform
		Vector3 pos = clickWord.m_Position;
		pos = new Vector3(pos.x / OSU_WIDTH * ScreenWidth - HalfScreenWidth,
										   HalfScreenHeight - pos.y / OSU_HEIGHT * ScreenHeight,
										 0);

		// Set RhythmController
		contrller = m_ObjectTrans.GetComponent<RhythmController>();
		contrller.Init(this,
					   clickWord.m_Word,
					   clickWord.m_ShowWord, pos, inOut);


	}

	public void TutNum(int k)
	{
		Tutorial_num = k;
	}

	public void SetTicWord()
	{
		m_TicWord = true;
	}

	public override void Delete()
	{
		if (contrller != null)
		{
			contrller.Destroy();
			contrller = null;
		}
	}

	private int m_Result;
	public override void OnPointerDown()
	{
		if (!m_Clickable)
			return;

		if (IsTutorial == false)
		{
			m_Result = m_Manager.GotHit(m_HitWord, m_SentenceIndex);
		}
		else
		{
			m_Result = TutorialScene.Instance.GetResult(Tutorial_num);
		}

		//LogManager.Log(m_Result);
		switch (m_Result)
		{
			case RuntimeConst.HitRight:
				contrller.On_Right();
				break;
			case RuntimeConst.HitPerfect:
				contrller.On_Perfect();
				break;
			case RuntimeConst.HitWrong:
				contrller.On_Wrong();
				break;
			default:
				break;
		}
	}

	#region [ --- RhythmTapCreator Call Back --- ]
	public void OnSentenceEnd(bool allCorrect)
	{
		if (allCorrect)
			contrller.OnSentenceCorrect();
		else
			contrller.OnSentenceWrong();
	}
	#endregion

}
