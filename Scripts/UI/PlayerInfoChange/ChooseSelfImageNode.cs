using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class ChooseSelfImageNode : MonoBehaviour
{
	public Button btn_yes;
	public Button btn_no;
	public Button btn_befor;
	public Button btn_after;
	public List<AvatarInfo> list_avatar = new List<AvatarInfo>();

	private ChangeSelfInfoNode node_changeSelfInfoNode;
	private Sprite image_choose;
	private Sprite[] list_sprite;
	private int page_no;
	private int page_max;

	private AvatarInfo avatar_choose = new AvatarInfo();

	public void Init(ChangeSelfInfoNode node)
	{
		transform.localPosition = Vector3.zero;
		transform.localScale = Vector3.one;

		node_changeSelfInfoNode = node;
		image_choose = null;
		avatar_choose = null;
		page_no = 0;
		GetAllAvatarSprite();
		btn_yes.onClick.AddListener(()=> {
			AnalysisManager.Instance.OnEvent("100005", null, "头像修改", "确定个人头像选择");
			SaveData();
		});
		btn_no.onClick.AddListener(()=> {
			AnalysisManager.Instance.OnEvent("100005", null, "头像修改", "取消修改头像");
			Close();
			});

		btn_befor.onClick.AddListener(() =>
		{
			AnalysisManager.Instance.OnEvent("100005", null, "头像修改", "获取上一页头像");
			page_no -= 1;
			DisplayInfo();
		});
		btn_after.onClick.AddListener(() =>
		{
			AnalysisManager.Instance.OnEvent("100005", null, "头像修改", "获取下一页头像");
			page_no += 1;
			DisplayInfo();
		});
	}

	void GetAllAvatarSprite()
	{
		list_sprite = Resources.LoadAll<Sprite>("UI/_Avatars/");

		page_max = list_sprite.Length / 10;
		if (list_sprite.Length % 10 > 0)
		{
			page_max = page_max + 1;
		}
		if (page_max == 0)
			page_max = 1;

		DisplayInfo();
	}

	private void DisplayInfo()
	{
		image_choose = null;
		avatar_choose = null;
		if (page_no <= 0)
			page_no = 0;

		if (page_no >= page_max)
			page_no = page_max - 1;
		DisplayAvatars();
	}

	private void DisplayAvatars()
	{
		for (int i = 0; i < 10; i++)
		{
			int j = page_no * 10 + i;
			if (j < list_sprite.Length)
			{
				list_avatar[i].gameObject.SetActive(true);
				list_avatar[i].Init(this);
				list_avatar[i].image_avatar.sprite = list_sprite[j];
			}
			else
			{
				list_avatar[i].gameObject.SetActive(false);
			}
		}
	}

	public void Choose(AvatarInfo choose)
	{
		if (avatar_choose != null)
			avatar_choose.NotChoose();

		avatar_choose = choose;
		image_choose = choose.image_avatar.sprite;
	}

	void Close()
	{
		node_changeSelfInfoNode.gameObject.SetActive(true);
		Destroy(gameObject);
	}

	void SaveData()
	{
		node_changeSelfInfoNode.gameObject.SetActive(true);
		if (image_choose != null)
			node_changeSelfInfoNode.image_self.sprite = image_choose;
		Destroy(gameObject);
	}
}
