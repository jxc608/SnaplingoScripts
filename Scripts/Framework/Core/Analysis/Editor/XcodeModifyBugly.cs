using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using System.IO;
using System.Collections.Generic;

#if UNITY_IPHONE
using UnityEditor.iOS.Xcode;
#endif

public class XcodeModifyBugly
{
	[PostProcessBuild(7)]
	public static void OnPostprocessBuild(BuildTarget buildTarget, string path)
	{
		if (buildTarget == BuildTarget.iOS)
		{
#if UNITY_IPHONE

			// 修改xcode工程
			string projPath = PBXProject.GetPBXProjectPath(path);
			PBXProject proj = new PBXProject();
			proj.ReadFromString(File.ReadAllText(projPath));
			string target = proj.TargetGuidByName("Unity-iPhone");

			proj.AddFrameworkToProject(target, "libc++.tbd", false);

			string fileName = Application.dataPath.Replace("Assets", "iOS/Bugly");
			XcodeModifyGeneral.CopyAndReplaceDirectory(fileName, Path.Combine(path, "Bugly"));
			List<string> filePaths = new List<string>();
			XcodeModifyGeneral.AddFilesToBuild(ref filePaths, path, "Bugly");
			foreach (var filepath in filePaths)
				proj.AddFileToBuild(target, proj.AddFile(filepath, filepath));

			proj.AddBuildProperty(target, "FRAMEWORK_SEARCH_PATHS", "$(PROJECT_DIR)/Bugly");

			proj.WriteToFile(projPath);

			//获取info.plist
			string plistPath = path + "/Info.plist";
			PlistDocument plist = new PlistDocument();
			plist.ReadFromString(File.ReadAllText(plistPath));
			PlistElementDict rootDict = plist.root;

			rootDict.SetString("NSLocationWhenInUseUsageDescription", Application.productName + "需要访问您的位置信息");

			plist.WriteToFile(plistPath);
#endif
		}
	}
}