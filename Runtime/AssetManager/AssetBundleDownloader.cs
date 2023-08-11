using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

namespace GameFramework
{
    public class AssetBundleDownloader
    {
        private CatalogsAsyncOperation operation;

        private const string CatalogsKey = nameof(AssetBundleDownloader);

        public CatalogsAsyncOperation UpdateCatalogs()
        {
            if (operation != null && !operation.IsDone)
            {
                return operation;
            }

            operation = new CatalogsAsyncOperation();
            AssetManager.Instance.StartCoroutine(InternalUpdateCatalogs());
            return operation;
        }

        private IEnumerator InternalUpdateCatalogs()
        {
            yield return CollectCatalogs();
            if (operation.Status == UpdateCatalogsStatus.NetworkError)
            {
                yield break;
            }

            yield return DownloadBundles();
            if (operation.Status == UpdateCatalogsStatus.NetworkError)
            {
                yield break;
            }

            yield return DeleteRedundancy();
            operation.Complete();
        }

        private IEnumerator CollectCatalogs()
        {
            operation.Status = UpdateCatalogsStatus.CollectCatalogs;
            string json = PlayerPrefs.GetString(CatalogsKey, null);
            if (!string.IsNullOrEmpty(json))
            {
                operation.Catalogs = JsonUtility.FromJson<List<BundleCatalog>>(json);
                yield break;
            }

            if (string.IsNullOrEmpty(AssetSetting.Instance.DownloadUri))
            {
                operation.Status = UpdateCatalogsStatus.NetworkError;
                GameLogger.LogError("DownloadUri is invalid");
                yield break;
            }

            string manifestUri = PathUtils.Combine(AssetSetting.Instance.RemoteBundleUri, AssetSetting.Instance.ManifestBundleName);
            UnityWebRequest remoteManifestRequest = UnityWebRequestAssetBundle.GetAssetBundle(manifestUri);
            yield return remoteManifestRequest.SendWebRequest();
            if (remoteManifestRequest.result != UnityWebRequest.Result.Success)
            {
                operation.Status = UpdateCatalogsStatus.NetworkError;
                GameLogger.LogError(remoteManifestRequest.error);
                yield break;
            }

            operation.Catalogs = new List<BundleCatalog>();
            AssetBundle remoteManifestBundle = DownloadHandlerAssetBundle.GetContent(remoteManifestRequest);
            Dictionary<string, Hash128> remoteBundleHash = CollectBundleHash(remoteManifestBundle);
            yield return null;

            string localManifestPath = AssetSetting.Instance.GetBundlePath(AssetSetting.Instance.ManifestBundleName);
            AssetBundle localManifestBundle = AssetBundle.LoadFromFile(localManifestPath);
            Dictionary<string, Hash128> localBundleHash = CollectBundleHash(localManifestBundle);
            yield return null;

            foreach (string remoteBundle in remoteBundleHash.Keys)
            {
                bool updateBundle = false;
                if (localBundleHash.TryGetValue(remoteBundle, out Hash128 localHash))
                {
                    if (remoteBundleHash[remoteBundle] != localHash)
                    {
                        updateBundle = true;
                    }
                }
                else
                {
                    updateBundle = true;
                }

                if (updateBundle)
                {
                    operation.Catalogs.Add(new BundleCatalog()
                    {
                        Bundle = remoteBundle,
                        Updated = true,
                        Competed = false
                    });
                }
            }

            foreach (string localBundle in localBundleHash.Keys)
            {
                if (!remoteBundleHash.ContainsKey(localBundle))
                {
                    operation.Catalogs.Add(new BundleCatalog()
                    {
                        Bundle = localBundle,
                        Updated = false,
                        Competed = false
                    });
                }
            }

            PlayerPrefs.SetString(CatalogsKey, JsonUtility.ToJson(operation.Catalogs));
        }

        private Dictionary<string, Hash128> CollectBundleHash(AssetBundle manifestBundle)
        {
            Dictionary<string, Hash128> result = new Dictionary<string, Hash128>();
            if (manifestBundle != null)
            {
                AssetBundleManifest localManifest = manifestBundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
                string[] localBundles = localManifest.GetAllAssetBundles();
                foreach (string localBundle in localBundles)
                {
                    result.Add(localBundle, localManifest.GetAssetBundleHash(localBundle));
                }

                manifestBundle.Unload(true);
            }

            return result;
        }

        private IEnumerator DownloadBundles()
        {
            operation.BundleLengths = new List<long>(operation.Catalogs.Count);
            foreach (BundleCatalog catalog in operation.Catalogs)
            {
                if (!catalog.Updated)
                {
                    continue;
                }

                string bundleUri = PathUtils.Combine(AssetSetting.Instance.RemoteBundleUri, catalog.Bundle);
                UnityWebRequest request = UnityWebRequest.Head(bundleUri);
                if (request.uri.IsFile)
                {
                    FileInfo fileInfo = new FileInfo(bundleUri);
                    if (!fileInfo.Exists)
                    {
                        operation.Status = UpdateCatalogsStatus.NetworkError;
                        GameLogger.LogError($"{bundleUri} is not exist");
                        yield break;
                    }

                    operation.BundleLengths.Add(fileInfo.Length);
                }
                else
                {
                    yield return request.SendWebRequest();
                    if (request.result != UnityWebRequest.Result.Success)
                    {
                        operation.Status = UpdateCatalogsStatus.NetworkError;
                        GameLogger.LogError(request.error);
                        yield break;
                    }

                    long length = long.Parse(request.GetResponseHeader("Content-Length"));
                    operation.BundleLengths.Add(length);
                }
            }

            operation.Status = UpdateCatalogsStatus.DownloadBundles;
            foreach (BundleCatalog catalog in operation.Catalogs)
            {
                if (!catalog.Updated)
                {
                    continue;
                }

                if (catalog.Competed)
                {
                    operation.DownloadCount++;
                    continue;
                }

                string bundleUri = PathUtils.Combine(AssetSetting.Instance.RemoteBundleUri, catalog.Bundle);
                string bundlePath = PathUtils.Combine(AssetSetting.Instance.LocalBundlePath, catalog.Bundle);
                yield return DownloadBundle(bundleUri, bundlePath);
                if (operation.Status == UpdateCatalogsStatus.NetworkError)
                {
                    yield break;
                }

                operation.DownloadCount++;
                catalog.Competed = true;
                PlayerPrefs.SetString(CatalogsKey, JsonUtility.ToJson(operation.Catalogs));
            }

            string manifestUri = PathUtils.Combine(AssetSetting.Instance.RemoteBundleUri, AssetSetting.Instance.ManifestBundleName);
            string manifestPath = PathUtils.Combine(AssetSetting.Instance.LocalBundlePath, AssetSetting.Instance.ManifestBundleName);
            yield return DownloadBundle(manifestUri, manifestPath);
            if (operation.Status == UpdateCatalogsStatus.NetworkError)
            {
                yield break;
            }

            PlayerPrefs.DeleteKey(CatalogsKey);
        }

        private IEnumerator DownloadBundle(string uri, string path)
        {
            UnityWebRequest request = UnityWebRequest.Get(uri);
            yield return request.SendWebRequest();
            if (request.result != UnityWebRequest.Result.Success)
            {
                operation.Status = UpdateCatalogsStatus.NetworkError;
                GameLogger.LogError(request.error);
                yield break;
            }

            bool completed = false;
            FileUtils.WriteAllBytesAsync(path, request.downloadHandler.data, () =>
            {
                completed = true;
            });

            while (!completed)
            {
                yield return null;
            }
        }

        private IEnumerator DeleteRedundancy()
        {
            operation.Status = UpdateCatalogsStatus.DeleteRedundancy;
            foreach (BundleCatalog catalog in operation.Catalogs)
            {
                if (catalog.Updated)
                {
                    continue;
                }

                string bundlePath = PathUtils.Combine(AssetSetting.Instance.LocalBundlePath, catalog.Bundle);
                FileUtils.DeleteFile(bundlePath);
                yield return null;
            }
        }
    }
}