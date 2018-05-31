using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Snaplingo.UI;
public enum RoleTitleResourcesEnum
{
    canweared,
    weared,
    unfinish,
}

public class RoleTitleItem : MonoBehaviour {
    public Image roleTitleIcon;
    public Text roleTitleText;
    public Slider roleTitleSlider;
    public Button roleTitleButton;
    public Text roleTitleSliderValue;
    public RoleTitleResourcesEnum roleTitleResourcesEnum; 
	private void Start()
	{
        roleTitleButton.onClick.AddListener(()=>
        {
            PageManager.Instance.CurrentPage.GetNode<RoleTitleNode>().UpdateRoleTitleStatus(this);
        });
	}
}
