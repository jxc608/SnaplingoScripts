using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Snaplingo.UI;


public class ExcepitonNode : Node {

	public Text m_exceptionText;
	public void SetException(string e)
	{
		if(null != m_exceptionText)
		{
			m_exceptionText.text = e;
		}
	}

	public void OnClick()
	{
		this.Close(true);
	}
}
