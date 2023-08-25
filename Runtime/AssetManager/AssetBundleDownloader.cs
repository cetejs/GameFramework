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
            operation.Status = UpdateCatalogsStatus.Success;
            operation.StatusInfo = null;
            operation.Complete();
        }

        private IEnumerator CollectCatalogs()
        {
            operation.Status = UpdateCatalogsStatus.CollectCatalogs;
            string json = PlayerPrefs.GetString(CatalogsKey, null);
            if (!string.IsNullOrEmpty(json))
            {
                operation.Catalogs = JsonUtils.ToObject<List<BundleCatalog>>(json);
            }
            else
            {
                operation.Catalogs = new List<BundleCatalog>();
            }

            if (string.IsNullOrEmpty(AssetSetting.Instance.DownloadUri))
            {
                operation.Status = UpdateCatalogsStatus.NetworkError;
                GameLogger.LogError("DownloadUri is invalid");
                yield break;
            }

            string manifestUri = PathUtils.Combine(AssetSetting.Instance.RemoteBundleUri, AssetSetting.Instance.ManifestBundleName);
            using UnityWebRequest remoteManifestRequest = UnityWebRequestAssetBundle.GetAssetBundle(manifestUri);
            yield return remoteManifestRequest.SendWebRequest();
            if (remoteManifestRequest.result != UnityWebRequest.Result.Success)
            {
                operation.Status = UpdateCatalogsStatus.NetworkError;
                operation.StatusInfo = remoteManifestRequest.error;
                GameLogger.LogError(remoteManifestRequest.error);
                yield break;
            }

            Dictionary<string, Hash128> remoteBundleHash = CollectBundleHash(remoteManifestRequest);
            yield return null;

            string localManifestPath = AssetSetting.Instance.GetBundlePath(AssetSetting.Instance.ManifestBundleName);
            using UnityWebRequest localManifestRequest = UnityWebRequestAssetBundle.GetAssetBundle(localManifestPath);
            yield return localManifestRequest.SendWebRequest();
            Dictionary<string, Hash128> localBundleHash = CollectBundleHash(localManifestRequest);
            yield return null;

            Dictionary<string, BundleCatalog> updateCatalogs = new Dictionary<string, BundleCatalog>();
            foreach (BundleCatalog catalog in operation.Catalogs)
            {
                if (catalog.Bundle == AssetSetting.Instance.ManifestBundleName)
                {
                    continue;
                }

                updateCatalogs.Add(catalog.Bundle, catalog);
            }

            operation.Catalogs.Clear();
            foreach (string remoteBundle in remoteBundleHash.Keys)
            {
                string hash128 = remoteBundleHash[remoteBundle].ToString();
                if (updateCatalogs.TryGetValue(remoteBundle, out BundleCatalog catalog))
                {
                    catalog.Updated = true;
                    if (catalog.Hash128 != hash128)
                    {
                        catalog.Hash128 = hash128;
                        catalog.Competed = false;
                    }

                    continue;
                }

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
                        Hash128 = hash128,
                        Updated = true,
                        Competed = false
                    });
                }
            }

            foreach (string localBundle in localBundleHash.Keys)
            {
                if (updateCatalogs.ContainsKey(localBundle))
                {
                    continue;
                }

                if (!remoteBundleHash.ContainsKey(localBundle))
                {
                    operation.Catalogs.Add(new BundleCatalog()
                    {
                        Bundle = localBundle,
                        Hash128 = null,
                        Updated = false,
                        Competed = false
                    });
                }
            }

            foreach (string bundle in updateCatalogs.Keys)
            {
                BundleCatalog catalog = updateCatalogs[bundle];
                if (!remoteBundleHash.ContainsKey(bundle))
                {
                    catalog.Hash128 = null;
                    catalog.Updated = false;
                    catalog.Competed = false;
                }

                operation.Catalogs.Add(catalog);
            }

            if (operation.Catalogs.Count > 0)
            {
                operation.Catalogs.Add(new BundleCatalog()
                {
                    Bundle = AssetSetting.Instance.ManifestBundleName,
                    Updated = true,
                    Competed = false
                });
            }

            PlayerPrefs.SetString(CatalogsKey, JsonUtils.ToJson(operation.Catalogs));
        }

        private Dictionary<string, Hash128> CollectBundleHash(UnityWebRequest request)
        {
            Dictionary<string, Hash128> result = new Dictionary<string, Hash128>();
            if (request.result == UnityWebRequest.Result.Success)
            {
                AssetBundle manifestBundle = DownloadHandlerAssetBundle.GetContent(request);
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
            foreach (BundleCatalog catalog in operation.Catalogs)
            {
                if (!catalog.Updated)
                {
                    continue;
                }

                string bundleUri = PathUtils.Combine(AssetSetting.Instance.RemoteBundleUri, catalog.Bundle);
                using UnityWebRequest request = UnityWebRequest.Head(bundleUri);
                if (request.uri.IsFile)
                {
                    FileInfo fileInfo = new FileInfo(bundleUri);
                    if (!fileInfo.Exists)
                    {
                        operation.Status = UpdateCatalogsStatus.NetworkError;
                        GameLogger.LogError($"{bundleUri} is not exist");
                        yield break;
                    }

                    operation.AddLength(fileInfo.Length);
                }
                else
                {
                    yield return request.SendWebRequest();
                    if (request.result != UnityWebRequest.Result.Success)
                    {
                        operation.Status = UpdateCatalogsStatus.NetworkError;
                        operation.StatusInfo = request.error;
                        GameLogger.LogError(request.error);
                        yield break;
                    }

                    long length = long.Parse(request.GetResponseHeader("Content-Length"));
                    operation.AddLength(length);
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
                    operation.AddDownload();
                    continue;
                }

                string bundleUri = PathUtils.Combine(AssetSetting.Instance.RemoteBundleUri, catalog.Bundle);
                string bundlePath = PathUtils.Combine(AssetSetting.Instance.LocalBundlePath, catalog.Bundle);
                operation.StatusInfo = catalog.Bundle;
                yield return DownloadBundle(bundleUri, bundlePath);
                if (operation.Status == UpdateCatalogsStatus.NetworkError)
                {
                    yield break;
                }

                operation.AddDownload();
                catalog.Competed = true;
                PlayerPrefs.SetString(CatalogsKey, JsonUtils.ToJson(operation.Catalogs));
            }
        }

        private IEnumerator DownloadBundle(string uri, string path)
        {
            using UnityWebRequest request = UnityWebRequest.Get(uri);
            request.SendWebRequest();
            while (!request.isDone)
            {
                operation.CurrentDownloadLength = (long) request.downloadedBytes;
                yield return null;
            }

            if (request.result != UnityWebRequest.Result.Success)
            {
                operation.Status = UpdateCatalogsStatus.NetworkError;
                operation.StatusInfo = request.error;
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
                operation.StatusInfo = catalog.Bundle;
                FileUtils.DeleteFile(bundlePath);
                catalog.Competed = true;
                PlayerPrefs.SetString(CatalogsKey, JsonUtils.ToJson(operation.Catalogs));
                yield return null;
            }

            PlayerPrefs.DeleteKey(CatalogsKey);
        }
    }
}