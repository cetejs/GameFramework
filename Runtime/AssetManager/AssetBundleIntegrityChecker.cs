using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace GameFramework
{
    internal class AssetBundleIntegrityChecker
    {
        private IntegrityAsyncOperation operation;
        private List<string> downloadBundles = new List<string>();

        public IntegrityAsyncOperation CheckIntegrity()
        {
            if (operation != null)
            {
                return operation;
            }

            operation = new IntegrityAsyncOperation();
            AssetManager.Instance.StartCoroutine(InternalCheckIntegrity());
            return operation;
        }

        private IEnumerator InternalCheckIntegrity()
        {
            yield return CompareHash();
            if (operation.Status == IntegrityStatus.NetworkError)
            {
                yield break;
            }

            yield return DownloadBundles();
            if (operation.Status == IntegrityStatus.NetworkError)
            {
                yield break;
            }

            operation.Status = IntegrityStatus.Success;
            operation.Complete();
        }
        
        private IEnumerator CompareHash()
        {
            operation.Status = IntegrityStatus.CompareHash;
            if (string.IsNullOrEmpty(AssetSetting.Instance.DownloadUri))
            {
                operation.Status = IntegrityStatus.NetworkError;
                operation.StatusInfo = "DownloadUri is invalid";
                GameLogger.LogError("DownloadUri is invalid");
                yield break;
            }

            string hashUri = StringUtils.Concat(AssetSetting.Instance.RemoteBundleUri, "/", AssetSetting.Instance.BundleHashName, ".txt");
            UnityWebRequest hashRequest = UnityWebRequest.Get(hashUri);
            yield return hashRequest.SendWebRequest();
            if (hashRequest.result != UnityWebRequest.Result.Success)
            {
                operation.Status = IntegrityStatus.NetworkError;
                operation.StatusInfo = hashRequest.error;
                GameLogger.LogError(hashRequest.error);
                yield break;
            }

            string hashText = hashRequest.downloadHandler.text;
            string[] hashLines = hashText.Split("\n");
            operation.TotalLength = hashLines.Length;
            foreach (string hashLine in hashLines)
            {
                string[] hashRow = hashLine.Split(",");
                string bundleName = hashRow[0];
                string hash = hashRow[1];
                string bundlePath = AssetSetting.Instance.GetBundlePath(bundleName);
                UnityWebRequest bundleRequest = UnityWebRequest.Get(bundlePath);
                operation.StatusInfo = bundleName;
                yield return bundleRequest.SendWebRequest();
                if (bundleRequest.result != UnityWebRequest.Result.Success)
                {
                    downloadBundles.Add(bundleName);
                }
                else
                {
                    Hash128 hash128 = Hash128.Compute(bundleRequest.downloadHandler.data);
                    if (hash != hash128.ToString())
                    {
                        downloadBundles.Add(bundleName);
                    }
                }

                operation.Length++;
                yield return null;
            }
        }

        private IEnumerator DownloadBundles()
        {
            operation.Status = IntegrityStatus.DownloadBundles;
            foreach (string bundle in downloadBundles)
            {
                string bundleUri = PathUtils.Combine(AssetSetting.Instance.RemoteBundleUri, bundle);
                string bundlePath = PathUtils.Combine(AssetSetting.Instance.LocalBundlePath, bundle);
                UnityWebRequest request = UnityWebRequest.Get(bundleUri);
                operation.StatusInfo = bundle;
                yield return request.SendWebRequest();
                if (request.result != UnityWebRequest.Result.Success)
                {
                    operation.Status = IntegrityStatus.NetworkError;
                    GameLogger.LogError(request.error);
                    yield break;
                }

                bool completed = false;
                FileUtils.WriteAllBytesAsync(bundlePath, request.downloadHandler.data, () =>
                {
                    completed = true;
                });

                while (!completed)
                {
                    yield return null;
                }
            }
        }
    }
}