using System;
using UnityEngine;
using UnityEngine.UI;

public class ArtAlphabet
{
	public static Transform GetArtString(string text, bool spriteRenderer)
	{
		Transform tran = null;
		if (spriteRenderer)
		{
			tran = SpriteRendererArtString(text);
		}
		else
		{
			tran = ImageArtString(text);
		}
		return tran;
	}

	public static Transform ImageArtString(string text)
	{
		GameObject root = new GameObject();
		root.AddComponent<RectTransform>();
		float width = 0;
		float height = 0;
		float stringLen = text.Length;
		float nextLen = 0;
		float beforeLen = 0;
		float thisBeforeLen = 0;
		for (int i = 0; i < stringLen; i++)
		{
			float y = 0;
			string wordPath = "ArtAlphabet/";
			if (text[i] >= 'a' && text[i] <= 'z')
			{
				wordPath += "Small/" + text[i];
				beforeLen = nextLen;
				nextLen = FontConfig.Instance.GetNextStation(text[i].ToString());
				thisBeforeLen = FontConfig.Instance.GetBeforeStation(text[i].ToString());
			}
			else if (text[i] == ' ')
			{
				wordPath += "Emty";
				beforeLen = nextLen;
				nextLen = 0;
				thisBeforeLen = 0;
			}
			else if (text[i] == '-')
			{
				wordPath += "heng";
				y = 6;
				beforeLen = nextLen;
				nextLen = 0;
				thisBeforeLen = 0;
			}
			else if (text[i] == '\'')
			{
				wordPath += "pie";
				beforeLen = nextLen;
				nextLen = 0;
				thisBeforeLen = 0;
			}
			else
			{
				wordPath += "Big/" + text[i];
				beforeLen = nextLen;
				nextLen = FontConfig.Instance.GetNextStation(text[i].ToString());
				thisBeforeLen = FontConfig.Instance.GetBeforeStation(text[i].ToString());
			}
			if (i == 0)
				thisBeforeLen = 0;
			GameObject word = new GameObject();
			word.name = text[i].ToString();
			Sprite spr = GameObject.Instantiate(ResourceLoadUtils.Load<Sprite>(wordPath));
			Image image = word.AddComponent<Image>();
			image.sprite = spr;
			image.SetNativeSize();
			word.transform.localScale = Vector3.one;
			word.transform.SetParent(root.transform);
			RectTransform wordTransform = word.GetComponent<RectTransform>();
			wordTransform.localScale = Vector3.one;
			float wordWidth = wordTransform.sizeDelta.x;
			float wordHeight = wordTransform.sizeDelta.y;
			if (height < wordHeight)
			{
				height = wordHeight;
			}
			float wordX = width + 0.5f * wordWidth - beforeLen - thisBeforeLen;
			width = width + wordWidth - beforeLen - thisBeforeLen;
			wordTransform.localPosition = new Vector3(wordX, y, 0);
		}
		float removex = width / 2;
		for (int i = 0; i < stringLen; i++)
		{
			RectTransform wordTransform = root.transform.GetChild(i).GetComponent<RectTransform>();
			float wordx = wordTransform.localPosition.x - removex;
			float wordy = wordTransform.localPosition.y;
			wordTransform.localPosition = new Vector3(wordx, wordy, 0);
		}
		RectTransform thisRectTransform = root.transform.GetComponent<RectTransform>();
		thisRectTransform.sizeDelta = new Vector2(width, height);
		thisRectTransform.localPosition = new Vector3(0, 0, 0);

		return root.transform;
	}

	public static Transform SpriteRendererArtString(string text)
	{
		GameObject root = new GameObject();
		float width = 0;
		float height = 0;
		float stringLen = text.Length;
		float beforeLen = 0;
		float nextLen = 0;
		float thisBeforeLen = 0f;
		for (int i = 0; i < stringLen; i++)
		{
			float y = 0;
			string wordPath = "ArtAlphabet/";
			if (text[i] >= 'a' && text[i] <= 'z')
			{
				wordPath += "Small/" + text[i];
				beforeLen = nextLen;
				nextLen = FontConfig.Instance.GetNextStation(text[i].ToString()) / 75f;
				thisBeforeLen = FontConfig.Instance.GetBeforeStation(text[i].ToString()) / 75f;
			}
			else if (text[i] == ' ')
			{
				wordPath += "Emty";
				beforeLen = nextLen;
				nextLen = 0;
				thisBeforeLen = 0;
			}
			else if (text[i] == '-')
			{
				wordPath += "heng";
				y = 0.08f;
				beforeLen = nextLen;
				nextLen = 0;
				thisBeforeLen = 0;
			}
			else if (text[i] == '\'')
			{
				wordPath += "pie";
				beforeLen = nextLen;
				nextLen = 0;
				thisBeforeLen = 0f;
			}
			else
			{
				wordPath += "Big/" + text[i];
				beforeLen = nextLen;
				nextLen = FontConfig.Instance.GetNextStation(text[i].ToString()) / 75f;
				thisBeforeLen = FontConfig.Instance.GetBeforeStation(text[i].ToString()) / 75f;
			}
			if (i == 0)
				thisBeforeLen = 0;
			GameObject word = new GameObject();
			word.name = (text[i]).ToString();
			Sprite spr = GameObject.Instantiate(ResourceLoadUtils.Load<Sprite>(wordPath));
			SpriteRenderer sprRenderer = word.AddComponent<SpriteRenderer>();
			sprRenderer.sprite = spr;
			sprRenderer.sortingLayerName = "Font";
			//sprRenderer.color = new Color(1, 151 / 255f, 0);
			word.transform.SetParent(root.transform);
			word.transform.localScale = Vector3.one;
			float wordWidth = sprRenderer.bounds.extents.x * 2;
			float wordHeight = sprRenderer.bounds.extents.y * 2;
			if (height < wordHeight)
			{
				height = wordHeight;
			}
			float wordX = width + 0.5f * wordWidth - beforeLen - thisBeforeLen;
			width = width + wordWidth - beforeLen - thisBeforeLen;
			word.transform.localPosition = new Vector3(wordX, y, 0);
		}
		float removex = width / 2;
		for (int i = 0; i < stringLen; i++)
		{
			Transform wordTransform = root.transform.GetChild(i);
			float wordx = wordTransform.localPosition.x - removex;
			float wordy = wordTransform.localPosition.y;
			wordTransform.localPosition = new Vector3(wordx, wordy, 0);
		}

		return root.transform;
	}

	public static Transform GetArtStringChinese(string text, bool spriteRenderer)
	{
		if (spriteRenderer)
		{
			return SpriteRendererChinese(text);
		}
		else
		{
			return ImageChinese(text);
		}
	}

	public static Transform ImageChinese(string text)
	{
		GameObject root = new GameObject();
		root.AddComponent<RectTransform>();
		float width = 0;
		float height = 0;
		float stringLen = text.Length;
		for (int i = 0; i < stringLen; i++)
		{
			string wordPath = "ArtAlphabet/Chinese/" + System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(text[i].ToString()));
			GameObject word = new GameObject();
			if (ResourceLoadUtils.Load<Sprite>(wordPath) != null)
			{
				word.name = text[i].ToString();
				Sprite spr = GameObject.Instantiate(ResourceLoadUtils.Load<Sprite>(wordPath));
				Image image = word.AddComponent<Image>();
				image.sprite = spr;
				image.SetNativeSize();
			}
			else
			{
				LogManager.LogWarning("词库中不存在中文字：" , text[i]);
				word = GameObject.Instantiate(ResourceLoadUtils.Load<GameObject>("ArtAlphabet/Text"));
				word.GetComponent<Text>().text = text[i].ToString();
			}
			word.transform.localScale = Vector3.one;
			word.transform.SetParent(root.transform);
			RectTransform wordTransform = word.GetComponent<RectTransform>();
			wordTransform.localScale = Vector3.one;
			float wordWidth = wordTransform.sizeDelta.x;
			float wordHeight = wordTransform.sizeDelta.y;
			if (height < wordHeight)
			{
				height = wordHeight;
			}
			float wordX = width + 0.5f * wordWidth;
			width = width + wordWidth;
			wordTransform.localPosition = new Vector3(wordX, 0, 0);
		}
		float removex = width / 2;
		for (int i = 0; i < stringLen; i++)
		{
			RectTransform wordTransform = root.transform.GetChild(i).GetComponent<RectTransform>();
			float wordx = wordTransform.position.x - removex;
			wordTransform.position = new Vector3(wordx, 0, 0);
		}
		RectTransform thisRectTransform = root.transform.GetComponent<RectTransform>();
		thisRectTransform.sizeDelta = new Vector2(width, height);
		thisRectTransform.position = new Vector3(0, 0, 0);

		return root.transform;
	}

	public static Transform SpriteRendererChinese(string text)
	{
		GameObject root = new GameObject();
		float width = 0;
		float height = 0;
		float stringLen = text.Length;
		for (int i = 0; i < stringLen; i++)
		{
			string wordPath = "ArtAlphabet/Chinese/" + System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(text[i].ToString()));
			GameObject word = new GameObject();
			float wordWidth;
			float wordHeight;
			if (ResourceLoadUtils.Load<Sprite>(wordPath) != null)
			{
				word.name = text[i].ToString();
				Sprite spr = GameObject.Instantiate(ResourceLoadUtils.Load<Sprite>(wordPath));
				SpriteRenderer sprRenderer = word.AddComponent<SpriteRenderer>();
				sprRenderer.sprite = spr;
				sprRenderer.sortingLayerName = "Font";
				wordWidth = sprRenderer.bounds.extents.x * 2;
				wordHeight = sprRenderer.bounds.extents.y * 2;
				sprRenderer.color = new Color(1, 151 / 255f, 0);
			}
			else
			{
				LogManager.LogWarning("词库中不存在中文字：" + text[i]);
				word = GameObject.Instantiate(ResourceLoadUtils.Load<GameObject>("ArtAlphabet/Text3D"));
				word.GetComponent<TextMesh>().text = text[i].ToString();
				wordWidth = word.GetComponent<MeshRenderer>().bounds.extents.x * 2;
				wordHeight = word.GetComponent<MeshRenderer>().bounds.extents.y * 2;

			}
			word.transform.SetParent(root.transform);
			word.transform.localScale = Vector3.one;
			if (height < wordHeight)
			{
				height = wordHeight;
			}
			float wordX = width + 0.5f * wordWidth;
			width = width + wordWidth;
			word.transform.localPosition = new Vector3(wordX, 0, 0);
		}
		float removex = width / 2;
		for (int i = 0; i < stringLen; i++)
		{
			Transform wordTransform = root.transform.GetChild(i);
			float wordx = wordTransform.position.x - removex;
			wordTransform.position = new Vector3(wordx, 0, 0);
		}

		return root.transform;
	}
}

public class SetLevel
{
	public static int setLevel(float rat)
	{
		LogManager.Log(rat);
		int result = (int)(Math.Round(rat * 10, 0));
		if (result >= CorePlaySettings.Instance.m_LevelSS)
			return 0;
		else if (result >= CorePlaySettings.Instance.m_LevelS)
			return 1;
		else if (result >= CorePlaySettings.Instance.m_LevelA)
			return 2;
		else if (result >= CorePlaySettings.Instance.m_LevelB)
			return 3;
		else if (result>=CorePlaySettings.Instance.m_LevelC)
			return 4;
		else
			return 5;
	}
	//public static double Round(double v,int x)
	//{
	//	bool isNegative = false;
 //       if(v<0)
	//	{
	//		isNegative = true;
	//		v = -v;
	//	}
	//	int Ivalue = 1;
	//	for (int i = 1; i <= x;i++)
	//	{
	//		Ivalue = Ivalue * 10;
	//	}
	//	double Int = Math.Round(v*Ivalue+0.5,0);
	//	v = Int / Ivalue;
	//	if(isNegative)
	//	{
	//		v = -v;
	//	}
	//	return v;
	//}
}
