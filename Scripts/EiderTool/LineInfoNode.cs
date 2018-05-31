using Snaplingo.UI;
using UnityEngine.UI;

public class LineInfoNode : Node
{
	public ContentBossNode m_ContentBossNode;
	public EditBossNode m_BossNode;
	private Text m_LineNo;

	public void Start()
	{
		m_LineNo = transform.Find("Text").GetComponent<Text>();
		transform.Find("Insert").GetComponent<Button>().onClick.AddListener(Insert);
		transform.Find("Reset").GetComponent<Button>().onClick.AddListener(ResetLine);
		transform.Find("Delete").GetComponent<Button>().onClick.AddListener(Delete);
	}

	private void Insert()
	{
		int lineNo = int.Parse(m_LineNo.text) - 1;
		m_BossNode.transform.Find("BossNode").gameObject.SetActive(false);
		m_ContentBossNode.Open();
		m_ContentBossNode.Insert(lineNo);
	}

	private void ResetLine()
	{
		int lineNo = int.Parse(m_LineNo.text) - 1;
		m_BossNode.transform.Find("BossNode").gameObject.SetActive(false);
		m_ContentBossNode.Open();
		m_ContentBossNode.Eider(lineNo);
	}

	private void Delete()
	{
		int lineNo = int.Parse(m_LineNo.text) - 1;
		EiderToolPage.Instance.SongObject.BossInfos.BossLineObject.RemoveAt(lineNo);
		m_BossNode.DisPlay();
	}
}