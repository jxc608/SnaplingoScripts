using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Snaplingo.UI;
using UnityEngine.UI;

public class ShareNode : Node {
	public override void Open()
	{
        base.Open();
	}
	public override void Init(params object[] args)
	{
        base.Init(args);
        gameObject.SetActive(false);
	}
    public void DoTweenComplete()
    {
        print("OK");
        gameObject.SetActive(false);

    }
}
