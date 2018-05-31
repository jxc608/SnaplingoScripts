using System;
using UnityEngine;
using System.IO;
using System.Collections;
using System.Collections.Generic;

using Loxodon.Log;
using Loxodon.Framework.Asynchronous;
using Loxodon.Framework.Bundles;

namespace Loxodon.Framework.Examples.Bundle
{
    public class WWWDownloader : AbstractDownloader
    {
        private static readonly ILog log = LogManagerLoxodon.GetLogger(typeof(WWWDownloader));

        protected bool useCache = false;
        public WWWDownloader(Uri baseUri, bool useCache) : this(baseUri, SystemInfo.processorCount * 2, useCache)
        {
        }

        public WWWDownloader(Uri baseUri, int maxTaskCount, bool useCache) : base(baseUri, maxTaskCount)
        {
            this.useCache = useCache;
        }

        protected override IEnumerator DoDownloadBundles(IProgressPromise<Progress, bool> promise, List<BundleInfo> bundles)
        {
            long totalSize = 0;
            long downloadedSize = 0;
            Progress progress = new Progress();
            List<BundleInfo> list = new List<BundleInfo>();
            for (int i = 0; i < bundles.Count; i++)
            {
                var info = bundles[i];
                totalSize += info.FileSize;
                if (BundleUtil.Exists(info))
                {
                    downloadedSize += info.FileSize;
                    continue;
                }
                list.Add(info);
            }

            progress.TotalSize = totalSize;
            progress.CompletedSize = downloadedSize;
            yield return null;

            List<KeyValuePair<BundleInfo, WWW>> tasks = new List<KeyValuePair<BundleInfo, WWW>>();
            for (int i = 0; i < list.Count; i++)
            {
                BundleInfo bundleInfo = list[i];
                WWW www = (useCache && !bundleInfo.IsEncrypted) ? WWW.LoadFromCacheOrDownload(GetAbsoluteUri(bundleInfo.Filename), bundleInfo.Hash) : new WWW(GetAbsoluteUri(bundleInfo.Filename));
                tasks.Add(new KeyValuePair<BundleInfo, WWW>(bundleInfo, www));

                while (tasks.Count >= this.MaxTaskCount || (i == list.Count - 1 && tasks.Count > 0))
                {
                    long tmpSize = 0;
                    for (int j = tasks.Count - 1; j >= 0; j--)
                    {
                        var task = tasks[j];
                        BundleInfo _bundleInfo = task.Key;
                        WWW _www = task.Value;

                        if (!_www.isDone)
                        {
                            tmpSize += (long)(_www.progress * _bundleInfo.FileSize);
                            continue;
                        }

                        tasks.RemoveAt(j);
                        downloadedSize += _bundleInfo.FileSize;
                        if (!string.IsNullOrEmpty(_www.error))
                        {
                            promise.SetException(new Exception(_www.error));
                            if (log.IsErrorEnabled)
                                log.ErrorFormat("Downloads AssetBundle '{0}' failure from the address '{1}'.Reason:{2}", _bundleInfo.FullName, GetAbsoluteUri(_bundleInfo.Filename), _www.error);
                            yield break;
                        }

                        try
                        {
                            if (useCache && !_bundleInfo.IsEncrypted)
                            {
                                AssetBundle bundle = _www.assetBundle;
                                if (bundle != null)
                                    bundle.Unload(true);
                            }
                            else
                            {
                                string fullname = BundleUtil.GetStorableDirectory() + _bundleInfo.Filename;
                                FileInfo info = new FileInfo(fullname);
                                if (info.Exists)
                                    info.Delete();

                                if (!info.Directory.Exists)
                                    info.Directory.Create();

                                File.WriteAllBytes(info.FullName, _www.bytes);
                            }
                        }
                        catch (Exception e)
                        {
                            promise.SetException(e);
                            if (log.IsErrorEnabled)
                                log.ErrorFormat("Downloads AssetBundle '{0}' failure from the address '{1}'.Reason:{2}", _bundleInfo.FullName, GetAbsoluteUri(_bundleInfo.Filename), e);
                            yield break;
                        }
                    }

                    progress.CompletedSize = downloadedSize + tmpSize;
                    promise.UpdateProgress(progress);

                    yield return null;
                }
            }
            promise.SetResult(true);
        }
    }
}
