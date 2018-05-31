using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum ItemType
{
	Body,
    Face,
}
public class RoleItemInput : MonoBehaviour {
	public ItemType itemType;
	public RoleManager roleManager;
	private void Start()
	{
		gameObject.GetComponent<Button>().onClick.AddListener(OnClickItem);
	}
    public void OnClickItem()
	{
		if(itemType== ItemType.Face)
		{
			//更换角色皮肤
			roleManager.ChangeUserRoleEmotion(name);         
		}
		else
		{
			//更换角色
			roleManager.ChangeUserRoleModel(name);
		}
	}
}
