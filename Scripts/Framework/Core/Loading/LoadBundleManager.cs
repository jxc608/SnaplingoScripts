using UnityEngine;
using Loxodon.Framework.Contexts;
using Loxodon.Framework.Bundles;
using Loxodon.Framework.Examples.Bundle;
using System;
using System.Collections;
using System.IO;
using Loxodon.Framework.Asynchronous;
using System.Collections.Generic;

public delegate void ProgressDelegate(float percent);

public class LoadBundleManager : MonoBehaviour
{
	public event ProgressDelegate downloadProgressEvent;


	#region [ Property --- ]
	ApplicationContext context;
	IDownloader downloader;
	bool downloading = false;
	#endregion



	#region [ Mono --- ]
	public static LoadBundleManager instance;
	void Awake()
	{
		instance = this;
		DontDestroyOnLoad(this.gameObject);
		context = Context.GetApplicationContext();
	}
	#endregion


	#region [ Public --- ]
	public void Init(Action callback)
	{
		// 初始化 Bundle Uri 下载地址
#if UNITY_EDITOR
		DirectoryInfo dir = new DirectoryInfo(ConfigurationController.Instance.BundleUri);
		if (!dir.Exists)
		{
			LogManager.Log("Directory '{0}' does not exist.", dir.FullName);
			return;
		}
		else
		{
			LogManager.Log("初始化 Bundle Uri 下载地址 " , dir.FullName);
		}
		Uri baseUri = new Uri(dir.FullName);
		this.downloader = new WWWDownloader(baseUri, false);
#elif UNITY_IOS
        // 正式资源 Uri 从服务器获取
		// ...
#elif UNITY_ANDROID
        // 正式资源 Uri 从服务器获取
        // ...
#endif
		if (File.Exists(BundleUtil.GetStorableDirectory() + BundleSetting.ManifestFilename))
		{
			IResources _resources = CreateResources();
			context.GetContainer().Register<IResources>(_resources);
			if (callback != null) callback.Invoke();
		}
		else
		{
			StartCoroutine(_CopyManifestFromStreamingAssets(callback));
		}
	}
	public void Download()
	{
		if (downloading == true) return;
		StartCoroutine(_CorDownload());
	}
	#endregion


	IEnumerator _CopyManifestFromStreamingAssets(Action callback)
	{
		DirectoryInfo dir = new DirectoryInfo(BundleUtil.GetReadOnlyDirectory() + "/");
		Uri baseUri = new Uri(dir.FullName);
		IDownloader downloader = new WWWDownloader(baseUri, false);

		// 下载 Manifest
		IProgressResult<Progress, BundleManifest> manifestResult = downloader.DownloadManifest(BundleSetting.ManifestFilename);
		yield return manifestResult.WaitForDone();
		if (manifestResult.Exception != null)
		{
		LogManager.Log("Downloads BundleManifest failure.Error:{0}", manifestResult.Exception);
			yield break;
		}

		IResources _resources = CreateResources();
		context.GetContainer().Register<IResources>(_resources);
		if (callback != null) callback.Invoke();
	}



	IResources CreateResources()
	{
		IResources resources = null;
#if UNITY_EDITOR
		if (SimulationSetting.IsSimulationMode)
		{
		    LogManager.Log("Use SimulationResources. Run In Editor");

			/* Create a PathInfoParser. */
			//IPathInfoParser pathInfoParser = new SimplePathInfoParser("@");
			IPathInfoParser pathInfoParser = new SimulationAutoMappingPathInfoParser();

			/* Create a BundleManager */
			IBundleManager manager = new SimulationBundleManager();

			/* Create a BundleResources */
			resources = new SimulationResources(pathInfoParser, manager);
		}
		else
#endif
		{
			/* Create a BundleManifestLoader. */
			IBundleManifestLoader manifestLoader = new BundleManifestLoader();


			/* Loads BundleManifest. */
			BundleManifest manifest;
			manifest = manifestLoader.Load(BundleUtil.GetStorableDirectory() + BundleSetting.ManifestFilename);

			//manifest.ActiveVariants = new string[] { "", "sd" };
			//manifest.ActiveVariants = new string[] { "", "hd" };

			/* Create a PathInfoParser. */
			//IPathInfoParser pathInfoParser = new SimplePathInfoParser("@");
			IPathInfoParser pathInfoParser = new AutoMappingPathInfoParser(manifest);

			/* Create a BundleLoaderBuilder */
			ILoaderBuilder builder;
			builder = new WWWComplexLoaderBuilder(new Uri(BundleUtil.GetStorableDirectory()), false);

			/* Create a BundleManager */
			IBundleManager manager = new BundleManager(manifest, builder);

			/* Create a BundleResources */
			resources = new BundleResources(pathInfoParser, manager);
		}
		return resources;
	}


	IEnumerator _CorDownload()
	{
		this.downloading = true;
		try
		{
			if (downloadProgressEvent != null) downloadProgressEvent(0);
			// 下载 Manifest
			IProgressResult<Progress, BundleManifest> manifestResult = this.downloader.DownloadManifest(BundleSetting.ManifestFilename);
			yield return manifestResult.WaitForDone();
			if (manifestResult.Exception != null)
			{
				LogManager.Log("Downloads BundleManifest failure.Error:{0}", manifestResult.Exception);
				yield break;
			}

			// 下载 BundleInfo
			BundleManifest manifest = manifestResult.Result;
			IProgressResult<float, List<BundleInfo>> bundlesResult = this.downloader.GetDownloadList(manifest);
			yield return bundlesResult.WaitForDone();
			List<BundleInfo> bundles = bundlesResult.Result;
			if (bundles == null || bundles.Count <= 0)
			{
				LogManager.Log("Please clear cache and remove StreamingAssets,try again.");
				yield break;
			}

			// 下载 Bundle
			IProgressResult<Progress, bool> downloadResult = this.downloader.DownloadBundles(bundles);
			downloadResult.Callbackable().OnProgressCallback(p =>
			{
				LogManager.Log("Downloading {0:F2}KB/{1:F2}KB {2:F3}KB/S", p.GetCompletedSize(UNIT.KB), p.GetTotalSize(UNIT.KB), p.GetSpeed(UNIT.KB));
				float percent = p.GetCompletedSize(UNIT.KB) / p.GetTotalSize(UNIT.KB);
				if (downloadProgressEvent != null) downloadProgressEvent(percent);
			});
			yield return downloadResult.WaitForDone();
			if (downloadResult.Exception != null)
			{
				LogManager.Log("Downloads AssetBundle failure.Error:{0}", downloadResult.Exception);
				yield break;
			}

			// 下载成功
			LogManager.Log(" 下载成功 ");
			IResources _resources = CreateResources();
			context.GetContainer().Unregister<IResources>();
			context.GetContainer().Register<IResources>(_resources);

#if UNITY_EDITOR
			UnityEditor.EditorUtility.OpenWithDefaultApp(BundleUtil.GetStorableDirectory());
#endif
		}
		finally
		{
			this.downloading = false;
		}
	}

}
//LoadBundleManager














