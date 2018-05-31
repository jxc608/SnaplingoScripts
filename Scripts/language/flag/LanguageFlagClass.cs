using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LanguageFlagClass : MonoBehaviour 
{
    public SelfType selfType;

    private void Start()
    {
		Refresh();
    }

	public void SetType(SelfType type)
    {
		selfType = type;
		Refresh();
    }

	private void Refresh()
    {
		if(selfType == SelfType.Default)
		{
			return;
		}

        string value = LanguageManager.Instance.GetValueByKey(selfType.ToString(), LanguageManager.languageType);
        if (null != gameObject.GetComponent<Image>())
        {
            Sprite sprite = Resources.Load<Sprite>("language/" + value);
            if (null != sprite)
            {
                gameObject.GetComponent<Image>().sprite = sprite;
                gameObject.GetComponent<Image>().SetNativeSize();
            }

        }
        else
        {
            if (null != gameObject.GetComponent<SpriteRenderer>())
            {
                Sprite sprite = Resources.Load<Sprite>("language/" + value);
                if (null != sprite)
                {
                    gameObject.GetComponent<SpriteRenderer>().sprite = sprite;
                }

            }
            else
            {
                gameObject.GetComponent<Text>().text = value;
            }

        }
    }
}
