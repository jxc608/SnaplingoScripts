using UnityEngine;
using UnityEngine.UI;

public class AvatarInfo : MonoBehaviour
{
	public Button btn_avatar;
	public Image image_avatar;
	public Image image_light;

	private ChooseSelfImageNode node_chooseSelfImage;

	public void Init( ChooseSelfImageNode node)
	{
		node_chooseSelfImage = node;

		transform.localScale = 1f * Vector3.one;
		image_light.gameObject.SetActive(false);

		btn_avatar.onClick.RemoveAllListeners();
		btn_avatar.onClick.AddListener(IsChoose);
	}

	private void IsChoose()
	{
		AnalysisManager.Instance.OnEvent("100005", null, "头像选择", "选择头像："+image_avatar.name);

		transform.localScale = 1.1f * Vector3.one;
		image_light.gameObject.SetActive(true);

		node_chooseSelfImage.Choose(this);
	}

	public void NotChoose()
	{
		transform.localScale = 1f * Vector3.one;
		image_light.gameObject.SetActive(false);
	}
}
