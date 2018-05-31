using System.Collections.Generic;
using UnityEngine.UI;
using Snaplingo.UI;
using UnityEngine;
using UnityEngine.EventSystems;

public class HitNode : Node, IDragHandler, IPointerEnterHandler, IPointerExitHandler
{
	public enum ChangeType { DontChange, ChangeTime, ChangePosition }
	private ChangeType m_ChangeStatus;
	private bool m_ChangeStart;
	private bool m_PointerIn;
	private float m_HitTime;
	private Vector3 m_HitPosition;
	private Transform m_GetTime;
	private Transform m_GetPosition;
	private Transform m_Outlight;
	private Transform m_Out;
	private Transform m_Pure;
	private Transform m_TimeText;
	private Transform m_PositionText;
	private Color m_PureColor;
	private bool m_PlayAudio;
	private float m_StartHitTime;
	private float m_EndHitTime;
	private int m_HitNum;

	private string m_SoundText;
	private string m_ClickText;

	public string SoundText
	{
		get { return m_SoundText; }
		set { m_SoundText = value; }
	}
	public string ClickText
	{
		get { return m_ClickText; }
		set { m_ClickText = value; }
	}

	public float HitTime
	{
		get { return m_HitTime; }
		set { m_HitTime = value; }
	}

	public float StartHitTime
	{
		get { return m_StartHitTime; }
		set { m_StartHitTime = value; }
	}
	public float EndHitTime
	{
		get { return m_EndHitTime; }
		set { m_EndHitTime = value; }
	}

	public Vector3 HitPosition
	{
		get { return m_HitPosition; }
		set { m_HitPosition = value; }
	}

	public Vector3 TimePosition()
	{
		return m_GetTime.GetComponent<RectTransform>().position;
	}

	public Vector3 HitPosi()
	{
		return m_GetPosition.GetComponent<RectTransform>().position;
	}

	public ChangeType ChangeStatus
	{
		set { m_ChangeStatus = value; }
	}

	public override void Init(params object[] args)
	{
		base.Init();
		m_ChangeStart = false;
		m_ChangeStatus = ChangeType.DontChange;
		transform.GetComponent<RectTransform>().localPosition = Vector3.zero;
		transform.GetComponent<RectTransform>().localScale = Vector3.one;

		m_GetTime = transform.Find("GetTime");
		m_GetPosition = transform.Find("GetPosition");
		m_Out = m_GetPosition.Find("Out");
		m_Outlight = m_GetPosition.Find("Outlight");
		m_Pure = m_GetPosition.Find("Pure");
		m_TimeText = m_GetTime.Find("Text");
		m_PositionText = m_GetPosition.Find("Text");
		m_PlayAudio = false;

		m_PureColor = m_GetTime.Find("Pure").GetComponent<Image>().color;
	}

	public void TextNum(int k)
	{
		m_HitNum = k;
		m_PositionText.GetComponent<Text>().text = k.ToString() + "\n" + ClickText;
		m_TimeText.GetComponent<Text>().text = k.ToString() + "\n" + SoundText;
	}

	public int HitNum
	{
		get { return m_HitNum; }
	}

	private void Update()
	{
		float audioTime = CorePlayMusicPlayer.Instance.GetBgmTime();
		if (Input.GetMouseButtonUp(0))
		{
			m_ChangeStatus = ChangeType.DontChange;
		}

		if (Input.GetKeyUp(KeyCode.E) && m_PointerIn)
		{
			EditContent();
		}

		if (Input.GetKeyUp(KeyCode.C) && m_PointerIn)
		{
			/*
			HitInfo ho = new HitInfo();
			HitInfo temp = EiderToolPage.Instance.SongObject.HitInfos[m_HitNum - 1];
			ho.HitTime = temp.HitTime;
			ho.SoundText = temp.SoundText;
			ho.ClickText = temp.ClickText;
			ho.Position = temp.Position;
			EiderToolPage.Instance.SongObject.HitInfos = EiderSong.Instance.InsertHitNode(ho, EiderToolPage.Instance.SongObject.HitInfos);
			*/
			EiderSong.Instance.m_CopyList.Clear();
			EiderSong.Instance.m_CopyList.Add(HitNum - 1);
		}

		GetTimeStatus(audioTime);
		GetPositionStatus(audioTime);

	}

	private void GetTimeStatus(float audioTime)
	{
		Vector3 position = new Vector3(0, 0, 0);
		position.y = m_GetTime.GetComponent<RectTransform>().localPosition.y;
		position.x = (m_HitTime - audioTime) * (EiderSong.Instance.m_CibiaoSizeX / (600f / EiderToolPage.Instance.SongObject.SongInfos.BPM));
		m_GetTime.GetComponent<RectTransform>().localPosition = position;
	}

	private void GetPositionStatus(float audioTime)
	{
		m_GetPosition.GetComponent<RectTransform>().localPosition = m_HitPosition;

		if (audioTime < m_StartHitTime)
		{
			m_PlayAudio = false;
			if ((m_StartHitTime - audioTime) <= 0.1f)
			{
				float alph = 1 - (m_StartHitTime - audioTime) / 0.1f;
				m_PureColor.a = alph;
				m_PositionText.GetComponent<Text>().color = new Color(1, 1, 1, alph);
				m_Out.GetComponent<Image>().color = new Color(1, 1, 1, alph);
				m_Pure.GetComponent<Image>().color = m_PureColor;
			}
			else
			{
				float alph = 0f;
				m_PureColor.a = alph;
				m_PositionText.GetComponent<Text>().color = new Color(1, 1, 1, alph);
				m_Outlight.GetComponent<Image>().color = m_PureColor;
				m_Out.GetComponent<Image>().color = new Color(1, 1, 1, alph);
				m_Pure.GetComponent<Image>().color = m_PureColor;
			}
		}
		else if (audioTime <= m_EndHitTime)
		{
			float alph = 1f;
			m_PureColor.a = alph;
			m_PositionText.GetComponent<Text>().color = new Color(1, 1, 1, alph);
			m_Outlight.GetComponent<Image>().color = m_PureColor;
			m_Out.GetComponent<Image>().color = new Color(1, 1, 1, alph);
			m_Pure.GetComponent<Image>().color = m_PureColor;
		}
		else
		{
			if ((audioTime - m_EndHitTime) <= 0.1f)
			{
				float alph = 1 - (audioTime - m_EndHitTime) / 0.1f;
				m_PureColor.a = alph;
				m_PositionText.GetComponent<Text>().color = new Color(1, 1, 1, alph);
				m_Out.GetComponent<Image>().color = new Color(1, 1, 1, alph);
				m_Pure.GetComponent<Image>().color = m_PureColor;
			}
			else
			{
				float alph = 0f;
				m_PureColor.a = alph;
				m_PositionText.GetComponent<Text>().color = new Color(1, 1, 1, alph);
				m_Outlight.GetComponent<Image>().color = m_PureColor;
				m_Out.GetComponent<Image>().color = new Color(1, 1, 1, alph);
				m_Pure.GetComponent<Image>().color = m_PureColor;
				m_Outlight.localScale = Vector3.one;
			}
		}

		if (m_HitTime > audioTime)
		{
			m_PlayAudio = false;
		}
		else if (m_HitTime <= audioTime)
		{
			if (m_PlayAudio == false && EiderToolPage.Instance.SongObject.HitInfos[m_HitNum - 1].Type == SentenceInfo.SentenceType.ClickNode)
			{
				m_PlayAudio = true;
				if (EiderSong.Instance.m_ToggleWord.isOn)
				{
					string[] words = m_SoundText.Split(' ');
					List<string> wordList = new List<string>();
					for (int i = 0; i < words.Length; i++)
					{
						if (string.IsNullOrEmpty(words[i]) == false)
						{
							wordList.Add(words[i]);
						}
					}
					// 原来这是编辑器用的
					//LogManager.Log("!! AudioManager.Instance.PlayWordList: {0}", wordList);
					AudioManager.Instance.PlayWordList(wordList);
				}
				else if (EiderSong.Instance.m_Toggle.isOn)
				{
					AudioController.Play("Tap");
				}
			}
		}
		OutLight(audioTime);
	}

	public void OutLight(float audioTime)
	{
		if (audioTime < m_HitTime)
		{
			m_PlayAudio = false;

			if ((m_HitTime - audioTime) < 0.1f)
			{
				m_PureColor.a = 1;
				m_Outlight.GetComponent<Image>().color = m_PureColor;
				m_Outlight.localScale = ((m_HitTime - audioTime) / 0.1f + 1) * Vector3.one;
			}
			else
			{
				m_PureColor.a = 0;
				m_Outlight.GetComponent<Image>().color = m_PureColor;
				m_Outlight.localScale = Vector3.one;
			}
		}
		else
		{
			m_PureColor.a = 0;
			m_Outlight.GetComponent<Image>().color = m_PureColor;
			m_Outlight.localScale = Vector3.one;
		}
	}

	public void OnDrag(PointerEventData data)
	{
		float audioTime = CorePlayMusicPlayer.Instance.GetBgmTime();
		if (Input.GetMouseButton(0) && m_ChangeStatus != ChangeType.DontChange && m_ChangeStart == false)
		{
			m_ChangeStart = true;
			int hitnum = m_HitNum - 1;
			List<HitInfo> temp = new List<HitInfo>();
			temp = EiderToolPage.Instance.SongObject.HitInfos;
			if (m_ChangeStatus == ChangeType.ChangePosition)
			{
				m_HitPosition = temp[hitnum].Position;
				m_GetPosition.GetComponent<RectTransform>().position = Input.mousePosition;
				temp[hitnum].Position = m_GetPosition.GetComponent<RectTransform>().localPosition;
				float x = temp[hitnum].Position.x;
				float y = temp[hitnum].Position.y;
				int x_num = (int)(x / 87.5f);
				int y_num = (int)(y / 87.5f);
				temp[hitnum].Position = new Vector2(x_num * 87.5f, y_num * 87.5f);
				EiderToolPage.Instance.SongObject.HitInfos = temp;
			}
			else if (m_ChangeStatus == ChangeType.ChangeTime)
			{
				m_GetTime.GetComponent<RectTransform>().position = new Vector3(Input.mousePosition.x, TimePosition().y, 0);
				ChangeTime(m_GetTime.GetComponent<RectTransform>().localPosition.x / EiderSong.Instance.m_CibiaoSizeX * (600f / EiderToolPage.Instance.SongObject.SongInfos.BPM) + audioTime);
			}
			m_ChangeStart = false;
		}
	}

	public void OnPointerEnter(PointerEventData data)
	{
		//LogManager.Log("pointer in");
		m_PointerIn = true;
	}

	public void OnPointerExit(PointerEventData data)
	{
		//LogManager.Log("pointer out");
		m_PointerIn = false;
	}

	void EditContent()
	{
		EiderSong.Instance.SetWordContent(m_SoundText, m_ClickText, SoundTextValueChangeCallback, ClickTextValueChangeCallback);
	}

	void SoundTextValueChangeCallback(string content)
	{
		EiderToolPage.Instance.SongObject.HitInfos[m_HitNum - 1].SoundText = content;
		//m_SoundText = content;
	}

	void ClickTextValueChangeCallback(string content)
	{
		EiderToolPage.Instance.SongObject.HitInfos[m_HitNum - 1].ClickText = content;
		//m_ClickText = content;
	}

	public void DragBatch(Vector3 position)
	{
		float audioTime = CorePlayMusicPlayer.Instance.GetBgmTime();
		m_GetTime.position = position;
		ChangeTime(m_GetTime.GetComponent<RectTransform>().localPosition.x / EiderSong.Instance.m_CibiaoSizeX * (600f / EiderToolPage.Instance.SongObject.SongInfos.BPM) + audioTime);
	}

	private void ChangeTime(float hitTime)
	{
		int hitnum = m_HitNum - 1;
		List<HitInfo> temp = new List<HitInfo>();
		temp = EiderToolPage.Instance.SongObject.HitInfos;
		int oldHitTime = (int)(HitTime * 1000);
		HitTime = hitTime;
		int tempTime = (int)(HitTime * 1000);
		float timeDelta = 60000f / EiderToolPage.Instance.SongObject.SongInfos.BPM / 16;
		HitTime = (int)(tempTime / timeDelta) * timeDelta / 1000f;
		temp[hitnum].HitTime = (int)(HitTime * 1000);
		if (temp[hitnum].HitTime > oldHitTime)
		{
			int i = 0;
			for (i = hitnum; i < temp.Count; i++)
			{
				if (i < temp.Count - 1 && temp[i].HitTime > temp[i + 1].HitTime)
				{
					if (temp[i].EndHitTime == temp[i + 1].EndHitTime)
					{
						if (i == 0)
						{
							for (int j = 0; j < temp.Count; j++)
							{
								if (temp[j].EndHitTime == temp[i].EndHitTime)
									temp[j].StartHitTime = temp[i + 1].HitTime;
								else
									break;
							}
						}
						else if (temp[i].EndHitTime != temp[i - 1].EndHitTime)
						{
							for (int j = i; j < temp.Count; j++)
							{
								if (temp[j].EndHitTime == temp[i].EndHitTime)
									temp[j].StartHitTime = temp[i + 1].HitTime;
								else
									break;
							}
							int dddd = 100;
							if (temp[i + 1].Type == SentenceInfo.SentenceType.SoundNode)
								dddd = 1000;
							for (int j = i - 1; j >= 0; j--)
							{
								if (temp[j].StartHitTime == temp[i - 1].StartHitTime)
									temp[j].EndHitTime = temp[i + 1].HitTime - dddd;
								else
									break;
							}
						}
					}
					else
					{
						if (i == 0)
						{
							temp[i].StartHitTime = temp[i + 1].StartHitTime;
							temp[i].EndHitTime = temp[i + 1].EndHitTime;
						}
						else if (temp[i - 1].EndHitTime != temp[i].EndHitTime)
						{
							temp[i].StartHitTime = temp[i + 1].StartHitTime;
							temp[i].EndHitTime = temp[i + 1].EndHitTime;
							int dddd = 100;
							if (temp[i + 1].Type == SentenceInfo.SentenceType.SoundNode)
								dddd = 1000;
							for (int j = i - 1; j >= 0; j--)
							{
								if (temp[j].StartHitTime == temp[i - 1].StartHitTime)
									temp[j].EndHitTime = temp[i + 1].HitTime - dddd;
								else
									break;
							}
						}
					}

					if (i == temp.Count - 2 && temp[i].HitTime > temp[i].EndHitTime)
					{
						for (int j = temp.Count - 1; j >= 0; j--)
						{
							if (temp[j].StartHitTime == temp[i].StartHitTime)
								temp[j].EndHitTime = temp[i].HitTime + 2000;
						}
					}
					EiderSong.Instance.ChangeHitTime(i, i + 1);
					HitInfo hh = temp[i];
					temp[i] = temp[i + 1];
					temp[i + 1] = hh;
					m_TimeText.GetComponent<Text>().text = (i + 2).ToString();
				}
				else
				{
					if (i == hitnum)
					{
						if (i == 0)
						{
							for (int j = 0; j < temp.Count; j++)
							{
								if (temp[j].EndHitTime == temp[i].EndHitTime)
									temp[j].StartHitTime = temp[i].HitTime;
								else
									break;
							}
						}
						else if (temp[i - 1].EndHitTime != temp[i].EndHitTime)
						{
							for (int j = i; j < temp.Count; j++)
							{
								if (temp[j].EndHitTime == temp[i].EndHitTime)
									temp[j].StartHitTime = temp[i].HitTime;
								else
									break;
							}
							int dddd = 100;
							if (temp[i].Type == SentenceInfo.SentenceType.SoundNode)
								dddd = 1000;
							for (int j = i - 1; j >= 0; j--)
							{
								if (temp[j].StartHitTime == temp[i - 1].StartHitTime)
									temp[j].EndHitTime = temp[i].HitTime - dddd;
								else
									break;
							}
						}

						if (temp[i].HitTime >= temp[i].EndHitTime)
						{
							for (int j = i; j >= 0; j--)
							{
								if (temp[j].StartHitTime == temp[i].StartHitTime)
									temp[j].EndHitTime = temp[i].HitTime + 2000;
								else
									break;
							}
						}
					}
					break;
				}
			}
		}
		else if (temp[hitnum].HitTime < oldHitTime)
		{
			int i = hitnum;
			for (i = hitnum; i >= 0; i--)
			{
				if (i > 0 && temp[i].HitTime < temp[i - 1].HitTime)
				{
					if (temp[i].EndHitTime == temp[i - 1].EndHitTime)
					{
						if (i == 1)
						{
							for (int j = 0; j < temp.Count; j++)
							{
								if (temp[j].EndHitTime == temp[i].EndHitTime)
									temp[j].StartHitTime = temp[1].HitTime;
								else
									break;
							}
						}
						else if (temp[i].StartHitTime != temp[i - 2].StartHitTime)
						{
							temp[i].StartHitTime = temp[i - 2].StartHitTime;
							temp[i].EndHitTime = temp[i - 2].EndHitTime;
						}
					}
					else
					{
						if (i == temp.Count - 1)
						{
							if (i == 1)
							{
								temp[0].StartHitTime = temp[1].HitTime;
								temp[1].StartHitTime = temp[1].HitTime;
								temp[1].EndHitTime = temp[0].EndHitTime;
							}
							else
							{
								temp[i].StartHitTime = temp[i - 1].StartHitTime;
								temp[i].EndHitTime = temp[i - 1].EndHitTime;
							}
						}
						else if (i == 1)
						{
							if (i == temp.Count - 1)
							{
								temp[0].StartHitTime = temp[1].HitTime;
								temp[1].StartHitTime = temp[1].HitTime;
								temp[1].EndHitTime = temp[0].EndHitTime;
							}
							else
							{
								for (int j = i + 1; j < temp.Count; j++)
								{
									if (temp[j].EndHitTime == temp[i].EndHitTime)
										temp[j].StartHitTime = temp[i + 1].HitTime;
									else
										break;
								}
								int dddd = 100;
								if (temp[i + 1].Type == SentenceInfo.SentenceType.SoundNode)
									dddd = 1000;
								temp[i - 1].EndHitTime = temp[i + 1].HitTime - dddd;
								temp[i - 1].StartHitTime = temp[i].HitTime;
								temp[i].StartHitTime = temp[i].HitTime;
								temp[i].EndHitTime = temp[i + 1].HitTime - dddd;
							}
						}
						else
						{
							for (int j = i + 1; j < temp.Count; j++)
							{
								if (temp[j].EndHitTime == temp[i].EndHitTime)
									temp[j].StartHitTime = temp[i + 1].HitTime;
								else
									break;
							}
							int dddd = 100;
							if (temp[i + 1].Type == SentenceInfo.SentenceType.SoundNode)
								dddd = 1000;
							for (int j = i - 1; j >= 0; j--)
							{
								if (temp[j].StartHitTime == temp[i - 1].StartHitTime)
									temp[j].EndHitTime = temp[i + 1].HitTime - dddd;
								else
									break;
							}
							if (temp[i].HitTime < temp[i - 1].StartHitTime || temp[i - 1].StartHitTime == temp[i - 2].StartHitTime)
							{
								temp[i].StartHitTime = temp[i - 1].StartHitTime;
								temp[i].EndHitTime = temp[i - 1].EndHitTime;
							}
							else
							{
								temp[i].StartHitTime = temp[i - 2].StartHitTime;
								temp[i].EndHitTime = temp[i - 2].EndHitTime;
							}
						}
					}
					EiderSong.Instance.ChangeHitTime(i, i - 1);
					HitInfo hh = temp[i];
					temp[i] = temp[i - 1];
					temp[i - 1] = hh;
					m_TimeText.GetComponent<Text>().text = i.ToString();
				}
				else
				{
					if (i == hitnum)
					{
						if (i == 0)
						{
							for (int j = 0; j < temp.Count; j++)
							{
								if (temp[j].EndHitTime == temp[i].EndHitTime)
									temp[j].StartHitTime = temp[i].HitTime;
								else
									break;
							}
						}
						else if (temp[i - 1].EndHitTime != temp[i].EndHitTime)
						{
							for (int j = i; j < temp.Count; j++)
							{
								if (temp[j].EndHitTime == temp[i].EndHitTime)
									temp[j].StartHitTime = temp[i].HitTime;
								else
									break;
							}
							int dddd = 100;
							if (temp[i].Type == SentenceInfo.SentenceType.SoundNode)
								dddd = 1000;
							for (int j = i - 1; j >= 0; j--)
							{
								if (temp[j].StartHitTime == temp[i - 1].StartHitTime)
									temp[j].EndHitTime = temp[i].HitTime - dddd;
								else
									break;
							}
						}
					}
					break;
				}
			}
		}
		EiderToolPage.Instance.SongObject.HitInfos = temp;
	}
}
