using UnityEngine;
using UnityEngine.UI;
using Snaplingo.SaveData;

public class ChangeSelfInfoNode : MonoBehaviour
{
	public ChooseSelfImageNode node_chooseSelfImage;
	public Image image_self;
	public InputField text_name;
	public Button btn_close;
	public Button btn_avatar;
	public Button btn_no;
	public Button btn_yes;

	void Awake()
	{
		SelfPlayerData.AvatarUrl = SelfPlayerData.AvatarUrl.Replace("(Clone)", "");
		text_name.text = SelfPlayerData.Nickname;
		var sprite = Resources.Load<Sprite>("UI/_Avatars/" + SelfPlayerData.AvatarUrl);
		if (sprite == null)
		{
			SelfPlayerData.AvatarUrl = "NoAvatar";
			sprite = Resources.Load<Sprite>("UI/_Avatars/" + SelfPlayerData.AvatarUrl);
		}
		image_self.sprite = sprite;

		transform.localPosition = Vector3.zero;
		transform.localScale = Vector3.one;

		btn_close.onClick.AddListener(() =>
		{
			AnalysisManager.Instance.OnEvent("100005", null, "个人信息修改", "关闭修改个人信息界面");
			Close();
		});
		btn_no.onClick.AddListener(() =>
		{
			AnalysisManager.Instance.OnEvent("100005", null, "个人信息修改", "取消修改个人信息");
			Close();
		});
		btn_yes.onClick.AddListener(() =>
		{
			AnalysisManager.Instance.OnEvent("100005", null, "个人信息修改", "确定修改个人信息");
			OnClickYes();
		});
		btn_avatar.onClick.AddListener(ChangeAvatar);
	}

	void ChangeAvatar()
	{
		AnalysisManager.Instance.OnEvent("100005", null, "个人信息修改", "打开修改个人头像界面");

		ChooseSelfImageNode node = Instantiate(node_chooseSelfImage);
		node.transform.SetParent(transform.parent);
		node.Init(this);
		transform.gameObject.SetActive(false);
	}

	void Close()
	{
		Destroy(gameObject);
	}

	void OnClickYes()
	{
		SelfPlayerData.Nickname = text_name.text;
		SelfPlayerData.AvatarUrl = image_self.sprite.name.Replace("(Clone)", "");
		SelfPlayerLevelData.Instance.changeNameEvent.Invoke();

		SaveDataUtils.Save<SelfPlayerData>();
		if (MiscUtils.IsOnline() && !string.IsNullOrEmpty(SelfPlayerData.Uuid))
			HttpHandler.UploadUser();
		//node_scoreListNode.text_playerName.text = text_name.text;
		Close();
	}
}