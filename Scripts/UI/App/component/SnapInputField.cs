using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

class SnapInputField : InputField
{

	public delegate void AfterRegisterDelegate();
	public AfterRegisterDelegate onSelect;

	public override void OnSelect(BaseEventData eventData)
	{
		base.OnSelect(eventData);
		if (onSelect != null)
		{
			onSelect();
		}
	}

}
