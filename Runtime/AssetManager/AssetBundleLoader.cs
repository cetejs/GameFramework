using System.Collections.Generic;
using UnityEngine;

namespace GameFramework
{
    internal class AssetBundleLoader
    {
        private AssetBundleManifest manifest;
        private Dictionary<string, BundleAsset> assets = new Dictionary<string, BundleAsset>();

        public AssetBundle LoadBundle(string bundleName)
        {
            bundleName = bundleName.RemoveLastOf(".");
            if (!assets.TryGetValue(bundleName, out BundleAsset asset))
            {
                asset = new BundleAsset(bundleName, this);
                assets.Add(bundleName, asset);
            }

            return asset.LoadBundle();
        }

        public void UnloadBundle(string bundleName, bool unloadAssets)
        {
            if (assets.TryGetValue(bundleName, out BundleAsset asset))
            {
                asset.UnloadBundle(unloadAssets);
            }
        }

        public AssetAsyncOperation LoadBundleAsync(string bundleName)
        {
            bundleName = bundleName.RemoveLastOf(".");
            if (!assets.TryGetValue(bundleName, out BundleAsset asset))
            {
                asset = new BundleAsset(bundleName, this);
                assets.Add(bundleName, asset);
            }

            return asset.LoadBundleAsync();
        }

        public AssetAsyncOperation UnloadBundleAsync(string bundleName, bool unloadAssets)
        {
            if (!assets.TryGetValue(bundleName, out BundleAsset asset))
            {
                asset = new BundleAsset(bundleName, this);
                assets.Add(bundleName, asset);
            }

            return asset.UnloadBundleAsync(unloadAssets);
        }

        public void UnloadAllBundles(bool unloadAssets)
        {
            foreach (BundleAsset asset in assets.Values)
            {
                asset.UnloadBundle(unloadAssets);
            }
        }
        
        public AssetAsyncOperation UnloadAllBundlesAsync(bool unloadAssets)
        {
            AssetListAsyncOperation operation = new AssetListAsyncOperation();
            if (assets.Count == 0)
            {
                operation.Completed(null);
            }
            else
            {
                foreach (BundleAsset asset in assets.Values)
                {
                    operation.AddOperation(asset.UnloadBundleAsync(unloadAssets));
                }
            }

            return operation;
        }

        public Object LoadAsset(string path)
        {
            string bundleName = path.RemoveLastOf("/");
            string assetName = path.GetLastOf("/");
            if (!assets.TryGetValue(bundleName, out BundleAsset asset))
            {
                asset = new BundleAsset(bundleName, this);
                assets.Add(bundleName, asset);
            }

            return asset.LoadAsset(assetName);
        }

        public void UnloadAsset(string path)
        {
            string bundleKey = path.RemoveLastOf("/");
            string assetName = path.GetLastOf("/");
            if (assets.TryGetValue(bundleKey, out BundleAsset asset))
            {
                asset.UnloadAsset(assetName);
            }
        }

        public AssetAsyncOperation LoadAssetAsync(string path)
        {
            string bundleName = path.RemoveLastOf("/");
            string assetName = path.GetLastOf("/");
            if (!assets.TryGetValue(bundleName, out BundleAsset asset))
            {
                asset = new BundleAsset(bundleName, this);
                assets.Add(bundleName, asset);
            }

            return asset.LoadAssetAsync(assetName);
        }

        public void UnloadAllAssets()
        {
            foreach (BundleAsset asset in assets.Values)
            {
                asset.UnloadBundle(true);
            }

            assets.Clear();
            Resources.UnloadUnusedAssets();
        }

        public string[] GetAllDependencies(string bundleName)
        {
            if (!manifest)
            {
                string bundlePath = AssetSetting.Instance.GetBundlePath(AssetSetting.Instance.ManifestBundleName);
                AssetBundle bundle = AssetBundle.LoadFromFile(bundlePath);
                manifest = bundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
            }

            return manifest.GetAllDependencies(bundleName);
        }
    }
}