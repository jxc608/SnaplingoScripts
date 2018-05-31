using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class TeachToolCustomMenu : MonoBehaviour
{
	private static string[] m_Paths = new string[] { "/Resources/", "/Prefabs/", "/Materials/", "/Artworks/", "/Fonts/", "/AssetBundle/" };
	//[MenuItem("自定义/工具/清理所有bundleName")]
	//static void ClearAllBundleNames()
	//{
	//    foreach(string path in m_Paths)
	//    {
	//        EditorAssetBundle.ClearDirectoryAssetName(Application.dataPath + path);
	//    }
	//    Debug.Log("Done");
	//}

	[MenuItem("自定义/工具/教学工具音乐bundle_Windows")]
	static void BuildMusicBundleWindows()
	{
		string path = Application.dataPath + "/TeachAudioBundles/Windows";
		CheckDir(path);
		SetEditFileList();
		BuildPipeline.BuildAssetBundles(path, BuildAssetBundleOptions.UncompressedAssetBundle, BuildTarget.StandaloneWindows64);
		Debug.Log("Done");
	}

	[MenuItem("自定义/工具/教学工具音乐bundle_MacOS")]
	static void BuildMusicBundleMacOS()
	{
		string path = Application.dataPath + "/TeachAudioBundles/MacOS";
		CheckDir(path);
		SetEditFileList();
		BuildPipeline.BuildAssetBundles(path, BuildAssetBundleOptions.UncompressedAssetBundle, BuildTarget.StandaloneOSXUniversal);
		Debug.Log("Done");
	}

	static void CheckDir(string path)
	{
		if (!Directory.Exists(path))
			Directory.CreateDirectory(path);
	}

	static void SetEditFileList()
	{
		AssetImporter ai = AssetImporter.GetAtPath("Assets/Resources/TeachTool/EditFileList.txt");
		ai.assetBundleName = "EditFileList";
	}
}
