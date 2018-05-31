using System;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class ClickResult
{
	public enum ClickType { ClickHitTime, ClickHitPosition, ClickNothing }

	private ClickType m_ClickStatus;
	public ClickType ClickStatus
	{
		get { return m_ClickStatus; }
		set { m_ClickStatus = value; }
	}

	private int m_ClickNo;
	public int ClickNo
	{
		get { return m_ClickNo; }
		set { m_ClickNo = value; }
	}

	private float m_ClickDis;
	public float ClickDis
	{
		get { return m_ClickDis; }
		set { m_ClickDis = value; }
	}

}

public class EiderSong : MonoBehaviour
{
	public enum EiderStatus { Nomal, Boss, Select, Batch, BatchCopy }
	public EiderStatus m_EiderStatus;
	public Transform m_SelectResult;
	public static EiderSong Instance;
	private bool AudioStatus;
	private string m_AudioFileName;
	public Transform m_Cibiao;
    public float m_CibiaoY;
    public float m_CibiaoSizeX;

	private float m_AudioTimeLast;

	private bool m_ChangeHitNode;
	public EditBossNode m_EditerBoss;
	public Toggle m_Toggle;
	public Toggle m_ToggleWord;

    private int m_HitTimeStart;
    private int m_HitTimeEnd;
	private bool m_SelectStatus;
    private Dictionary<int, HitNode> m_HitNodeNow = new Dictionary<int, HitNode>();

	public Transform m_StartToEnd;
	public bool m_ChangeSentence;
    private HitContentEditNode m_EditWordContent;

	public bool m_Editing;
	private void Awake()
	{
		Instance = this;
	}

	public string AudioFileName
	{
		set { m_AudioFileName = value; }
	}

	private void Start()
	{
		m_Editing = false;
		m_ChangeHitNode = false;
		m_SelectStatus = false;
		m_EiderStatus = EiderStatus.Nomal;
		ClearHitNodeNow();
		m_AudioTimeLast = 0f;
		m_StartToEnd = transform.Find("StartToEnd");
		
		m_Cibiao = transform.Find("Cibiao").Find("Cibiao1").Find("Cibiao1");
        m_CibiaoY = m_Cibiao.GetComponent<RectTransform>().localPosition.y;
        m_CibiaoSizeX = m_Cibiao.GetComponent<RectTransform>().sizeDelta.x;

		m_EditerBoss = transform.Find("EditerBoss").GetComponent<EditBossNode>();
		m_EditerBoss.Init();
		m_EditerBoss.Close();

		AudioStatus = false;
		m_ChangeSentence = false;
		m_StartToEnd.gameObject.SetActive(false);
		transform.Find("Select").GetComponent<Button>().onClick.AddListener(Select);
		transform.Find("Circle").GetComponent<Button>().onClick.AddListener(Circle);
		transform.Find("Enter").GetComponent<Button>().onClick.AddListener(Enter);
        transform.Find("Back").GetComponent<Button>().onClick.AddListener(Back);
		transform.Find("KeyList").GetComponent<Button>().onClick.AddListener(KeyList);
		transform.Find("Sentence").GetComponent<Button>().onClick.AddListener(Sentence);
		transform.Find("PlayAudio").Find("Play").GetComponent<Button>().onClick.AddListener(Play);
		transform.Find("PlayAudio").Find("Pause").GetComponent<Button>().onClick.AddListener(Pause);
		transform.Find("PlayAudio").Find("Stop").GetComponent<Button>().onClick.AddListener(Stop);
		transform.Find("Boss").GetComponent<Button>().onClick.AddListener(Boss);

		m_EditWordContent = transform.Find("HitContentEditNode").GetComponent<HitContentEditNode>();
		m_EditWordContent.Init();
		m_EditWordContent.Close();
	}

	private void Boss()
	{
		AudioStatus = false;
		CorePlayMusicPlayer.Instance.PauseMusic();
		m_EiderStatus = EiderStatus.Boss;
		m_EditerBoss.Open();
	}

	private void Play()
	{
		AudioStatus = true;
		CorePlayMusicPlayer.Instance.PlaySong(m_AudioFileName);
	}

	private void Pause()
	{
		AudioStatus = false;
		CorePlayMusicPlayer.Instance.PauseMusic();
	}

	private void Stop()
	{
		AudioStatus = false;
		CorePlayMusicPlayer.Instance.StopMusic();
       
	}

	private void Sentence()
	{
		if (AudioStatus)
		{
			AudioStatus = false;
			CorePlayMusicPlayer.Instance.PauseMusic();
		}
		if (m_ChangeSentence)
		{
			m_StartToEnd.gameObject.SetActive(false);
			m_ChangeSentence = false;
		}
		else
		{
			m_StartToEnd.gameObject.SetActive(true);
			m_ChangeSentence = true;
			ResetSentence();
		}

	}

    private void ClearHitNodeNow()
    {
        if (m_HitNodeNow == null)
            return;
        List<int> tempHitTime = new List<int>(m_HitNodeNow.Keys);
        for (int i = 0; i < tempHitTime.Count; i++)
        {
            Destroy(m_HitNodeNow[tempHitTime[i]].gameObject);
            m_HitNodeNow.Remove(tempHitTime[i]);
        }
        m_HitNodeNow.Clear();
    }

	private void Select()
	{
		if (AudioStatus)
		{
			AudioStatus = false;
			CorePlayMusicPlayer.Instance.PauseMusic();
		}
		m_EiderStatus = EiderStatus.Select;
	}

	private void Circle()
	{
		int hitTime = (int)(CorePlayMusicPlayer.Instance.GetBgmTime() * 1000);
		float timeDelta = 60000f / EiderToolPage.Instance.SongObject.SongInfos.BPM / 16;
		hitTime = (int)((int)(hitTime / timeDelta) * timeDelta);
		HitInfo ho = new HitInfo();
		ho.HitTime = hitTime;
		ho.SoundText = "SE";
		ho.ClickText = "Click";
		ho.Position = new Vector2(0, 0);
		EiderToolPage.Instance.SongObject.HitInfos = InsertHitNode(ho, EiderToolPage.Instance.SongObject.HitInfos);
	}

	private void Enter()
	{
		WriteSongEider.Instance.WriteSongtoTxt(EiderToolPage.Instance.SongObject);
		WriteSongEider.Instance.WriteSongKeyTime(EiderToolPage.Instance.SongObject);
		ClearHitNodeNow();
        EiderToolPage.Instance.BackToBegin();
	}

	private void Back()
	{
		CorePlayMusicPlayer.Instance.StopMusic();
		CorePlayMusicPlayer.Instance.ResetSongTime(0);
		ClearHitNodeNow();
		AudioStatus = false;
        EiderToolPage.Instance.BackToBegin();
	}

	private void KeyList()
	{
		WriteSongEider.Instance.WriteSongKeyTime(EiderToolPage.Instance.SongObject);
	}

	private Vector3 m_selectStart;
	private Vector3 m_selectEnd;
	private Vector3 m_DragStart;
	private Vector3 m_DragEnd;
	private bool Down;
	private Dictionary<int, HitNode> m_SelectList = new Dictionary<int, HitNode>();
	private Dictionary<int, Vector3> m_SelectTimePosition = new Dictionary<int, Vector3>();
	private Vector3 m_SelectPosition = new Vector3();
	public List<int> m_CopyList = new List<int>();
	public float m_CopyTime;
	void Update()
	{
		CanvasScaler canvas = GameObject.Find("Canvas").GetComponent<CanvasScaler>();
		if (m_EiderStatus == EiderStatus.Nomal && m_Editing == false)
		{
			#region 普通点
			if (AudioStatus == false)
				CorePlayMusicPlayer.Instance.PauseMusic();

			if (Input.GetKeyDown(KeyCode.Space))
			{
				AudioStatus = !AudioStatus;
				if (AudioStatus)
				{
					CorePlayMusicPlayer.Instance.PlaySong(m_AudioFileName);
				}
				else
				{
					CorePlayMusicPlayer.Instance.PauseMusic();
				}
			}
			if (Input.GetKeyDown(KeyCode.Escape))
			{
				Back();
			}

			if (Input.GetKeyUp(KeyCode.V) && m_CopyList.Count > 0)
			{
				PasteList();
			}

			if (Input.GetKeyDown(KeyCode.LeftArrow))
			{
				if (AudioStatus)
				{
					AudioStatus = false;
					CorePlayMusicPlayer.Instance.PauseMusic();
				}
				CorePlayMusicPlayer.Instance.MoveSongTime(-60f / EiderToolPage.Instance.SongObject.SongInfos.BPM / 16);
			}
			if (Input.GetKeyDown(KeyCode.RightArrow))
			{
				if (AudioStatus)
				{
					AudioStatus = false;
					CorePlayMusicPlayer.Instance.PauseMusic();
				}
				CorePlayMusicPlayer.Instance.MoveSongTime(60f / EiderToolPage.Instance.SongObject.SongInfos.BPM / 16);
			}

			float a = Input.mouseScrollDelta.y;
			if (a != 0)
			{
				if (AudioStatus)
				{
					AudioStatus = false;
					CorePlayMusicPlayer.Instance.PauseMusic();
				}
				CorePlayMusicPlayer.Instance.MoveSongTime(a * 60f / EiderToolPage.Instance.SongObject.SongInfos.BPM / 16);
			}

			if (Input.GetMouseButtonDown(0))
			{
				m_SelectStatus = true;
				ClickResult click = GetClick();
				if (click.ClickStatus != ClickResult.ClickType.ClickNothing)
				{
					if (AudioStatus)
					{
						AudioStatus = false;
						CorePlayMusicPlayer.Instance.PauseMusic();
					}
				}
				else
				{
					m_SelectStatus = false;
				}

				if (click.ClickStatus == ClickResult.ClickType.ClickHitPosition)
				{
					m_HitNodeNow[click.ClickNo].ChangeStatus = HitNode.ChangeType.ChangePosition;
					m_HitNodeNow[click.ClickNo].transform.SetAsLastSibling();
				}
				else if (click.ClickStatus == ClickResult.ClickType.ClickHitTime)
				{
					m_HitNodeNow[click.ClickNo].ChangeStatus = HitNode.ChangeType.ChangeTime;
					m_HitNodeNow[click.ClickNo].transform.SetAsLastSibling();
				}
			}

			if (Input.GetMouseButtonDown(1))
			{
				m_SelectStatus = true;
				ClickResult click = GetClick();
				if (click.ClickStatus != ClickResult.ClickType.ClickNothing)
				{
					if (AudioStatus)
					{
						AudioStatus = false;
						CorePlayMusicPlayer.Instance.PauseMusic();
					}
					m_ChangeHitNode = true;
					EiderToolPage.Instance.SongObject.HitInfos = DeleteHitNode(click.ClickNo, EiderToolPage.Instance.SongObject.HitInfos);
					Destroy(m_HitNodeNow[click.ClickNo].gameObject);
					for (int i = click.ClickNo; i < EiderToolPage.Instance.SongObject.HitInfos.Count + 1; i++)
					{
						if (m_HitNodeNow.ContainsKey(i + 1))
						{
							m_HitNodeNow[i] = m_HitNodeNow[i + 1];
						}
						else
						{
							m_HitNodeNow.Remove(i);
							break;
						}
					}
					m_ChangeHitNode = false;
				}
				else
				{
					m_SelectStatus = false;
				}
			}

			if (Input.GetMouseButtonUp(0))
			{
				m_SelectStatus = false;
			}

			if (Input.GetMouseButtonUp(1))
			{
				m_SelectStatus = false;
			}

			float audioTimeNow = CorePlayMusicPlayer.Instance.GetBgmTime();
			int audioTime = (int)(audioTimeNow * 1000);
			int ms = audioTime % 1000;
			int s = (audioTime / 1000) % 60;
			int m = (audioTime / 1000) / 60;
			string timeNow = m + ":" + s + "." + ms;
			transform.Find("PlayAudio").Find("NowTime").Find("Text").GetComponent<Text>().text = timeNow;
			m_HitTimeStart = (int)(audioTimeNow * 1000 - 6000);
			m_HitTimeEnd = (int)(audioTimeNow * 1000 + 6000);
			MoveCibiao(audioTimeNow - m_AudioTimeLast);
			m_AudioTimeLast = audioTimeNow;
			if (m_ChangeHitNode == false)
				HitNodeStatus();
			if (AudioStatus && m_ChangeSentence)
			{
				ResetSentence();
			}
			#endregion
		}
		else if (m_EiderStatus == EiderStatus.Select && m_Editing == false )
		{
			if (Input.GetMouseButtonDown(0))
			{
				m_selectStart = Input.mousePosition;
				m_selectEnd = m_selectStart;
				Down = true;
			}
			else if (Input.GetMouseButton(0))
			{
				m_selectEnd = Input.mousePosition;
				m_SelectResult.position = (m_selectStart + m_selectEnd) / 2f;
				m_SelectResult.GetComponent<RectTransform>().sizeDelta = new Vector2(Mathf.Abs(m_selectStart.x - m_selectEnd.x) / Screen.width * canvas.referenceResolution.x, Mathf.Abs(m_selectStart.y - m_selectEnd.y) / Screen.height * canvas.referenceResolution.y);
			}
			else if (Input.GetMouseButtonUp(0) && Down )
			{
				m_selectEnd = Input.mousePosition;
				m_SelectResult.position = (m_selectStart + m_selectEnd) / 2f;
				m_SelectPosition = m_SelectResult.position;
				m_SelectResult.GetComponent<RectTransform>().sizeDelta = new Vector2(Mathf.Abs(m_selectStart.x - m_selectEnd.x) / Screen.width * canvas.referenceResolution.x, Mathf.Abs(m_selectStart.y - m_selectEnd.y) / Screen.height * canvas.referenceResolution.y);
				GetSelectList();
				if (m_SelectList.Count == 0)
					m_EiderStatus = EiderStatus.Nomal;
				else
					m_EiderStatus = EiderStatus.Batch;
				Down = false;
			}

		}
		else if (m_EiderStatus == EiderStatus.Batch && m_Editing == false)
		{
			if (Input.GetMouseButtonDown(1))
			{
				List<int> tempHitTime = new List<int>(m_SelectList.Keys);
				for (int i = 0; i < tempHitTime.Count; i++)
				{
					EiderToolPage.Instance.SongObject.HitInfos = DeleteHitNode(tempHitTime[0], EiderToolPage.Instance.SongObject.HitInfos);
					Destroy(m_HitNodeNow[tempHitTime[0]].gameObject);
					for (int j = tempHitTime[0]; j < EiderToolPage.Instance.SongObject.HitInfos.Count + 1; j++)
					{
						if (m_HitNodeNow.ContainsKey(j + 1))
						{
							m_HitNodeNow[j] = m_HitNodeNow[j + 1];
						}
						else
						{
							m_HitNodeNow.Remove(j);
							break;
						}
					}
				}
				m_SelectList.Clear();
				m_SelectTimePosition.Clear();
				m_SelectResult.localPosition = Vector3.zero;
				m_SelectResult.GetComponent<RectTransform>().sizeDelta = Vector2.zero;
				m_EiderStatus = EiderStatus.Nomal;
			}
			else if (Input.GetMouseButtonDown(0))
			{
				Down = true;
				m_DragStart = Input.mousePosition;
				m_DragEnd = m_DragStart;
			}
			else if (Input.GetMouseButton(0))
			{
				m_DragEnd = Input.mousePosition;
				BatchDrag();
			}
			else if (Input.GetMouseButtonUp(0))
			{
				m_DragEnd = Input.mousePosition;
				BatchDrag();
				Down = false;
				m_SelectResult.localPosition = Vector3.zero;
				m_SelectResult.GetComponent<RectTransform>().sizeDelta = Vector2.zero;
				m_SelectList.Clear();
				m_SelectTimePosition.Clear();
				m_EiderStatus = EiderStatus.Nomal;
			}
			else if (Input.GetKeyUp(KeyCode.C))
			{
				GetCopyList();
				m_SelectResult.localPosition = Vector3.zero;
				m_SelectResult.GetComponent<RectTransform>().sizeDelta = Vector2.zero;
				m_SelectList.Clear();
				m_SelectTimePosition.Clear();
				m_EiderStatus = EiderStatus.Nomal;
			}
		}
	}
	#region 普通点
	private float getDistance(Vector3 vet1, Vector3 vet2)
	{
		float deltaX = vet1.x - vet2.x;
		float deltaY = vet1.y - vet2.y;
		float dis = (float)Math.Sqrt(deltaX * deltaX + deltaY * deltaY);
		return dis;
	}

	public void HitNodeStatus()
    {
        List<HitInfo> temp = EiderToolPage.Instance.SongObject.HitInfos;
        for (int i = 0; i < temp.Count; i++)
        {
            HitInfo ho = temp[i];
            if(ho.EndHitTime < m_HitTimeStart || ho.StartHitTime > m_HitTimeEnd)
            {
                if(m_HitNodeNow.ContainsKey(i))
                {
                    Destroy(m_HitNodeNow[i].gameObject);
                    m_HitNodeNow.Remove(i);
                }
            }
            else
            {
				if (m_HitNodeNow.ContainsKey(i) == false && m_SelectStatus == false)
				{
					HitNode tempNode = Instantiate(ResourceLoadUtils.Load<HitNode>("UI/Nodes/HitNode"));
					tempNode.transform.SetParent(transform.Find("ClickNode").transform);
					tempNode.Init();
					UpdateHitNode(tempNode, ho, i);
					m_HitNodeNow.Add(i, tempNode);
				}

				if (m_HitNodeNow.ContainsKey(i))
				{
					if( m_SelectStatus == false )
						UpdateHitNode(m_HitNodeNow[i], ho, i);
					else
						m_HitNodeNow[i].TextNum(i + 1);
				}
			}
        }
		transform.Find("Cibiao").SetAsLastSibling();
	}

	public void ChangeHitTime(int k, int l)
	{
		m_ChangeHitNode = true;

		if (m_HitNodeNow.ContainsKey(k) && m_HitNodeNow.ContainsKey(l))
		{
			HitNode temp = m_HitNodeNow[k];
			m_HitNodeNow[k] = m_HitNodeNow[l];
			m_HitNodeNow[l] = temp;
			m_HitNodeNow[k].TextNum(k + 1);
			m_HitNodeNow[l].TextNum(l + 1);
		}
		else if (m_HitNodeNow.ContainsKey(k))
		{
			m_HitNodeNow[l] = m_HitNodeNow[k];
			m_HitNodeNow[l].TextNum(l + 1);
			m_HitNodeNow.Remove(k);
		}
		else if (m_HitNodeNow.ContainsKey(l))
		{
			m_HitNodeNow[k] = m_HitNodeNow[l];
			m_HitNodeNow[k].TextNum(k + 1);
			m_HitNodeNow.Remove(l);
		}

		m_ChangeHitNode = false;
	}

	private void MoveCibiao(float deltaTime)
	{
		RectTransform rectTransfrom = transform.Find("Cibiao").Find("Cibiao1").Find("Cibiao1").GetComponent<RectTransform>();
		float width = rectTransfrom.sizeDelta.x;
		RectTransform rectTransform1 = transform.Find("Cibiao").Find("Cibiao1").GetComponent<RectTransform>();
		RectTransform rectTransform2 = transform.Find("Cibiao").Find("Cibiao2").GetComponent<RectTransform>();
		Vector3 moveDelta = new Vector3( width/(600f/EiderToolPage.Instance.SongObject.SongInfos.BPM) *deltaTime, 0, 0);
		rectTransform1.localPosition = rectTransform1.localPosition - moveDelta;
		rectTransform2.localPosition = rectTransform2.localPosition - moveDelta;
		if (rectTransform1.localPosition.x < -width)
		{
			rectTransform1.localPosition = rectTransform2.localPosition + new Vector3(width, 0, 0);
			rectTransform1.gameObject.name = "Cibiao2";
			rectTransform2.gameObject.name = "Cibiao1";
		}
		else if (rectTransform1.localPosition.x > 0)
		{
			rectTransform2.localPosition = rectTransform1.localPosition - new Vector3(width, 0, 0);
			rectTransform1.gameObject.name = "Cibiao2";
			rectTransform2.gameObject.name = "Cibiao1";
		}
	}
	
	public ClickResult GetClick()
	{
		ClickResult click = new ClickResult();
		if (m_HitNodeNow == null)
			click.ClickStatus = ClickResult.ClickType.ClickNothing;
		else
		{
			List<int> tempHitTime = new List<int>(m_HitNodeNow.Keys);
			click.ClickDis = 2000000f;
			for (int i = 0; i < tempHitTime.Count; i++)
			{
				float s = getDistance(Input.mousePosition, m_HitNodeNow[tempHitTime[i]].HitPosi());
				float audiotime = CorePlayMusicPlayer.Instance.GetBgmTime();
				if (s < click.ClickDis &&(audiotime+0.1f>m_HitNodeNow[tempHitTime[i]].StartHitTime && audiotime - 0.1f < m_HitNodeNow[tempHitTime[i]].EndHitTime))
				{
					click.ClickStatus = ClickResult.ClickType.ClickHitPosition;
					click.ClickDis = s;
					click.ClickNo = tempHitTime[i];
				}
				float d = getDistance(Input.mousePosition, m_HitNodeNow[tempHitTime[i]].TimePosition());
				if (d < click.ClickDis)
				{
					click.ClickStatus = ClickResult.ClickType.ClickHitTime;
					click.ClickDis = d;
					click.ClickNo = tempHitTime[i];
				}
			}

			if (click.ClickDis > 50f)
			{
				click.ClickStatus = ClickResult.ClickType.ClickNothing;
			}
		}
		return click;
	}

	public void UpdateHitNode(HitNode tempNode, HitInfo ho, int i)
	{
		tempNode.StartHitTime = ho.StartHitTime / 1000f;
		tempNode.EndHitTime = ho.EndHitTime / 1000f;
		tempNode.HitTime = ho.HitTime / 1000f;
		tempNode.HitPosition = new Vector3(ho.Position.x, ho.Position.y, 0);
		tempNode.SoundText = ho.SoundText;
		tempNode.ClickText = ho.ClickText;
		tempNode.TextNum(i + 1);
	}

	public List<HitInfo> InsertHitNode(HitInfo ho, List<HitInfo> temp)
	{
		m_ChangeHitNode = true;
		HitNode tempNode = Instantiate(ResourceLoadUtils.Load<HitNode>("UI/Nodes/HitNode"));
		tempNode.transform.SetParent(transform.Find("ClickNode").transform);
		tempNode.Init();
        UpdateHitNode(tempNode, ho, 0);
		int k = 0;
		if (temp.Count == 0)
		{
			ho.StartHitTime = ho.HitTime;
			ho.EndHitTime = ho.HitTime + 2000;
			ho.Type = SentenceInfo.SentenceType.ClickNode;
			temp.Add(ho);
			k = 0;
		}
		else if (ho.HitTime < temp[0].StartHitTime)
		{
			ho.StartHitTime = ho.HitTime;
			if (temp[0].Type == SentenceInfo.SentenceType.SoundNode)
				ho.EndHitTime = temp[0].StartHitTime - 1000;
			else
				ho.EndHitTime = temp[0].StartHitTime - 100;

			ho.Type = SentenceInfo.SentenceType.ClickNode;

			temp.Insert(0, ho);

			k = 0;
		}
		else
		{
			int i = 0;
			for (i = 0; i < temp.Count; i++)
			{
				if (ho.HitTime >= temp[i].StartHitTime && ho.HitTime <= temp[i].EndHitTime)
				{
					ho.StartHitTime = temp[i].StartHitTime;
					ho.EndHitTime = temp[i].EndHitTime;
					ho.Type = temp[i].Type;
				}

				if (temp[i].HitTime < ho.HitTime)
					continue;
				else
				{
					temp.Insert(i, ho);
					k = i;
					break;
				}
			}
			if (i == temp.Count)
			{
				if (temp[i - 1].EndHitTime <= ho.HitTime)
				{
					float lastEndTime = temp[i - 1].EndHitTime;
					ho.StartHitTime = ho.HitTime;
					ho.Type = SentenceInfo.SentenceType.ClickNode;
					ho.EndHitTime = ho.HitTime + 2000;
					for (int j = temp.Count - 1; j >= 0; j--)
					{
						if (temp[j].EndHitTime == lastEndTime)
						{
							temp[j].EndHitTime = ho.HitTime - 100;
						}
						else
						{
							break;
						}
					}
				}
				else
				{
					ho.StartHitTime = temp[i-1].StartHitTime;
					ho.EndHitTime = temp[i-1].EndHitTime;
					ho.Type = temp[i-1].Type;
				}
				temp.Add(ho);
				k = temp.Count - 1;
			}
		}
		
		if (m_HitNodeNow.ContainsKey(k) == false)
		{
			m_HitNodeNow.Add(k, tempNode);
		}
		else
		{
			HitNode node = tempNode;
			for (int i = k; i < temp.Count; i++ )
			{
				HitNode ttt;
				if (m_HitNodeNow.ContainsKey(i))
				{
					ttt = m_HitNodeNow[i];
					m_HitNodeNow[i] = node;
					node = ttt;
					m_HitNodeNow[i].TextNum(i + 1);
				}
				else
				{
					m_HitNodeNow.Add(i, node);
					m_HitNodeNow[i].TextNum(i + 1);
					break;
				}
			}
		}
		m_ChangeHitNode = false;
		return temp;
	}

	public List<HitInfo> DeleteHitNode(int hitNo, List<HitInfo> temp)
	{
		if (hitNo == 0)
		{
			if (temp.Count == 1)
				temp.RemoveAt(0);
			else
			{
				for (int i = 1; i < temp.Count; i++)
				{
					if (temp[i].StartHitTime == temp[0].StartHitTime)
					{
						temp[i].StartHitTime = temp[1].HitTime;
					}
					else
					{
						break;
					}
				}
				temp.RemoveAt(0);
			}

		}
		else if (hitNo == temp.Count - 1)
		{
			temp.RemoveAt(hitNo);
		}
		else
		{
			if (temp[hitNo - 1].StartHitTime == temp[hitNo].StartHitTime)
				temp.RemoveAt(hitNo);
			else
			{
				int sss = 100;
				if (temp[hitNo].Type == SentenceInfo.SentenceType.SoundNode)
					sss = 1000;
				for (int i = hitNo - 1; i >= 0; i--)
				{
					if (temp[i].StartHitTime == temp[hitNo - 1].StartHitTime)
						temp[i].EndHitTime = temp[hitNo + 1].HitTime - sss;
					else
						break;
				}
				for (int i = hitNo + 1; i < temp.Count; i++)
				{
					if (temp[i].EndHitTime == temp[hitNo].EndHitTime)
						temp[i].StartHitTime = temp[hitNo + 1].HitTime;
					else
						break;
				}
				temp.RemoveAt(hitNo);
			}
		}
		return temp;
	}

	public void ResetSentence()
	{
		int audioTime = (int)(CorePlayMusicPlayer.Instance.GetBgmTime()*1000);
		List<HitInfo> temp = EiderToolPage.Instance.SongObject.HitInfos;
		int start = temp.Count;
		int end = -1;
		SentenceInfo.SentenceType type = SentenceInfo.SentenceType.ClickNode;
		for (int i = 0; i < temp.Count; i++)
		{
			if (temp[i].StartHitTime <= audioTime && temp[i].EndHitTime >= audioTime)
			{
				if (i < start)
					start = i;
				if (i > end)
					end = i;
				type = temp[i].Type;
			}
			else if (temp[i].StartHitTime > audioTime)
				break;
		}
		if (start <= end)
		{
			m_StartToEnd.Find("Start").GetComponent<InputField>().text = (start+1).ToString();
			m_StartToEnd.Find("End").GetComponent<InputField>().text = (end+1).ToString();
			if (type == SentenceInfo.SentenceType.ClickNode)
				m_StartToEnd.Find("Type").Find("Label").GetComponent<Text>().text = "Click";
			else
				m_StartToEnd.Find("Type").Find("Label").GetComponent<Text>().text = "Sound";
		}
		else
		{
			m_StartToEnd.Find("Start").GetComponent<InputField>().text = "";
			m_StartToEnd.Find("End").GetComponent<InputField>().text = "";
			m_StartToEnd.Find("Type").Find("Label").GetComponent<Text>().text = "Click";
		}
	}

    public void SetWordContent(string curSound, string curClick, Action<string> soundCallback, Action<string> clickCallback)
    {
        m_EditWordContent.Open();
        m_EditWordContent.transform.SetAsLastSibling();
        m_EditWordContent.SetCallback(curSound, curClick, soundCallback, clickCallback);
    }
	#endregion 普通点

	#region selectNode
	private void GetSelectList()
	{
		m_SelectList.Clear();
		m_SelectTimePosition.Clear();
		float start_Time = (m_SelectResult.localPosition.x - m_SelectResult.GetComponent<RectTransform>().sizeDelta.x/2f) / m_CibiaoSizeX * (600f / EiderToolPage.Instance.SongObject.SongInfos.BPM) + CorePlayMusicPlayer.Instance.GetBgmTime();
		float end_Time = (m_SelectResult.localPosition.x + m_SelectResult.GetComponent<RectTransform>().sizeDelta.x/2f) / m_CibiaoSizeX * (600f / EiderToolPage.Instance.SongObject.SongInfos.BPM) + CorePlayMusicPlayer.Instance.GetBgmTime();

		m_CopyTime = (end_Time + start_Time)/2f;

		List<int> tempHitTime = new List<int>(m_HitNodeNow.Keys);
		for (int i = 0; i < tempHitTime.Count; i++)
		{
			if (m_HitNodeNow[tempHitTime[i]].HitTime >= start_Time && m_HitNodeNow[tempHitTime[i]].HitTime <= end_Time)
			{
				m_SelectList.Add(tempHitTime[i], m_HitNodeNow[tempHitTime[i]]);
				m_SelectTimePosition.Add(tempHitTime[i], m_HitNodeNow[tempHitTime[i]].TimePosition());
			}
		}
	}

	private void BatchDrag()
	{
		float dis = m_DragEnd.x - m_DragStart.x;
		List<int> temp = new List<int>(m_SelectList.Keys);
		m_SelectResult.position = new Vector3(m_SelectPosition.x + dis, m_SelectPosition.y, m_SelectPosition.z);

		for (int i = 0; i < temp.Count; i++)
		{
			Vector3 posi = m_SelectTimePosition[temp[i]] + new Vector3(dis, 0, 0);
			m_SelectList[temp[i]].DragBatch(posi);
		}
	}

	private void GetCopyList()
	{
		m_CopyList.Clear();
		List<int> temp = new List<int>(m_SelectList.Keys);
		for (int i = 0; i < temp.Count; i++)
		{
			m_CopyList.Add(m_SelectList[temp[i]].HitNum - 1);
		}
	}

	private void PasteList()
	{
		float timeHit = CorePlayMusicPlayer.Instance.GetBgmTime();
		float timeDelta = 1000f / 16;
		List<HitInfo> temp = EiderToolPage.Instance.SongObject.HitInfos;
		if (m_CopyList.Count == 1)
		{
			int hitTime = (int)( timeHit* 1000);
			hitTime = (int)((int)(hitTime / timeDelta) * timeDelta);
			HitInfo ho = new HitInfo();
			ho.HitTime = hitTime;
			ho.SoundText = temp[m_CopyList[0]].SoundText;
			ho.ClickText = temp[m_CopyList[0]].ClickText;
			ho.Position = temp[m_CopyList[0]].Position;
			EiderToolPage.Instance.SongObject.HitInfos = InsertHitNode(ho, EiderToolPage.Instance.SongObject.HitInfos);
		}
		else
		{
			float deltaTime = timeHit - m_CopyTime;
			for (int i = 0; i < m_CopyList.Count; i++)
			{
				int hitTime = (int)((deltaTime + temp[m_CopyList[i]].HitTime/1000f) * 1000);
				hitTime = (int)((int)(hitTime / timeDelta) * timeDelta);
				HitInfo ho = new HitInfo();
				ho.HitTime = hitTime;
				ho.SoundText = temp[m_CopyList[i]].SoundText;
				ho.ClickText = temp[m_CopyList[i]].ClickText;
				ho.Position = temp[m_CopyList[i]].Position;
				EiderToolPage.Instance.SongObject.HitInfos = InsertHitNode(ho, EiderToolPage.Instance.SongObject.HitInfos);
			}
		}
	}
	#endregion
}
