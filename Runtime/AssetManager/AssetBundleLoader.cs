using System.Collections.Generic;
using UnityEngine;

namespace GameFramework
{
    public class AssetBundleLoader
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

        public void UnloadBundle(string bundleName, bool unloadAssets)
        {
            if (assets.TryGetValue(bundleName, out BundleAsset asset))
            {
                asset.UnloadBundle(unloadAssets);
            }
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

        public void UnloadAsset(string path)
        {
            string bundleKey = path.RemoveLastOf("/");
            string assetName = path.GetLastOf("/");
            if (assets.TryGetValue(bundleKey, out BundleAsset asset))
            {
                asset.UnloadAsset(assetName);
            }
        }

        public string[] GetAllDependencies(string bundleName)
        {
            if (!manifest)
            {
                string path = AssetSetting.Instance.BundleLoadPath;
                path = PathUtils.Combine(path, path.GetLastOf("/"));
                AssetBundle bundle = AssetBundle.LoadFromFile(path);
                manifest = bundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
            }

            return manifest.GetAllDependencies(bundleName);
        }
    }
}