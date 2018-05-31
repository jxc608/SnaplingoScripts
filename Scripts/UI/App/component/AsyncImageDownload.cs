using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.IO;

public class AsyncImageDownload : MonoBehaviour
{
	public Sprite placeholder;

	private static AsyncImageDownload _instance = null;
	public static AsyncImageDownload GetInstance() { return Instance; }
	public static AsyncImageDownload Instance
	{
		get
		{
			if (_instance == null)
			{
				GameObject obj = new GameObject("AsyncImageDownload");
				_instance = obj.AddComponent<AsyncImageDownload>();
				DontDestroyOnLoad(obj);
				_instance.Init();
			}
			return _instance;
		}
	}

	public bool Init()
	{
		if (!Directory.Exists(Application.persistentDataPath + "/ImageCache/"))
		{
			Directory.CreateDirectory(Application.persistentDataPath + "/ImageCache/");
		}
		if (placeholder == null)
		{
			placeholder = Resources.Load("placeholder") as Sprite;
		}
		return true;

	}

	public void SetAsyncImage(string url, Image image, Sprite _placeholder = null)
	{
		if (image == null)
		{
			return;
		}
		_placeholder = _placeholder == null ? placeholder : _placeholder;
		//开始下载图片前，将UITexture的主图片设置为占位图
		image.sprite = _placeholder;

		if (url == null || url == "")
		{
			return;
		}

		if (url == "NoAvatar")
		{
			image.sprite = Resources.Load<Sprite>("UI/_Avatars/NoAvatar");
			return;
		}

		//判断是否是第一次加载这张图片
		if (!File.Exists(path + url.GetHashCode()))
		{
			//如果之前不存在缓存文件
			StartCoroutine(DownloadImage(url, image));
		}
		else
		{
			StartCoroutine(LoadLocalImage(url, image));
		}
	}

	IEnumerator DownloadImage(string url, Image image)
	{
		LogManager.Log("downloading new image:" , path + url.GetHashCode());//url转换HD5作为名字
		WWW www = new WWW(url);
		yield return www;

		Texture2D tex2d = www.texture;
		//将图片保存至缓存路径
		byte[] pngData = tex2d.EncodeToPNG();
		File.WriteAllBytes(path + url.GetHashCode(), pngData);

		Sprite m_sprite = Sprite.Create(tex2d, new Rect(0, 0, tex2d.width, tex2d.height), new Vector2(0, 0));
		image.sprite = m_sprite;
	}

	IEnumerator LoadLocalImage(string url, Image image)
	{
		string filePath = "file:///" + path + url.GetHashCode();

		LogManager.Log("getting local image:" , filePath);
		WWW www = new WWW(filePath);
		yield return www;

		Texture2D texture = www.texture;
		Sprite m_sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0, 0));
		image.sprite = m_sprite;
	}

	public string path
	{
		get
		{
			//pc,ios //android :jar:file//
			return Application.persistentDataPath + "/ImageCache/";

		}
	}
}