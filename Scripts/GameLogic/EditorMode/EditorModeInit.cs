using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Snaplingo.UI;

public class EditorModeInit : MonoBehaviour
{
	EditorModeNode m_EditorModeNode;

	void Awake()
	{
		ObjectPool.RegisterPool("EditorModeTimeLineNode", RuntimeConst.InitTimeLineNodeNumber, "CorePlay/EditorMode/TimeLineCircleNode");

		PageManager.Instance.OpenPage("CorePlayPage", false, () =>
		{
			m_EditorModeNode = PageManager.Instance.CurrentPage.AddNode<EditorModeNode>(true) as EditorModeNode;

			InitComponents();
		});
	}


	private const int EditPage = 3;
	void InitComponents()
	{
		m_EditorModeNode.transform.GetChild(EditPage).GetComponent<EditorModeManager>().Init();
	}
}
