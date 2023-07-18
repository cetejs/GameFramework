using System.Collections.Generic;
using UnityEngine;

namespace GameFramework
{
    internal class BundleAsset
    {
        private string bundleName;
        private AssetBundleLoader loader;
        private AssetBundle bundle;
        private BundleAsyncOperation loadOperation;
        private AssetAsyncOperation unloadOperation;
        private Dictionary<string, BundleObject> assets = new Dictionary<string, BundleObject>();

        public BundleAsset(string bundleName, AssetBundleLoader loader)
        {
            string extension = AssetSetting.Instance.BundleExtension;
            if (!bundleName.EndsWith(extension))
            {
                bundleName = StringUtils.Concat(bundleName, ".", extension);
            }

            this.bundleName = bundleName;
            this.loader = loader;
        }

        public AssetBundle LoadBundle()
        {
            if (bundle != null)
            {
                return bundle;
            }

            string[] dependencies = loader.GetAllDependencies(bundleName);
            foreach (string dependency in dependencies)
            {
                loader.LoadBundle(dependency);
            }

            string fullPath = PathUtils.Combine(AssetSetting.Instance.BundleLoadPath, bundleName);
            bundle = AssetBundle.LoadFromFile(fullPath);
            unloadOperation = null;

            if (loadOperation != null)
            {
                loadOperation?.Completed(bundle);
            }

            return bundle;
        }

        public AssetAsyncOperation LoadBundleAsync()
        {
            if (loadOperation != null)
            {
                return loadOperation;
            }

            if (bundle != null)
            {
                loadOperation = new BundleAsyncOperation();
                loadOperation.Completed(bundle);
                return loadOperation;
            }

            string[] dependencies = loader.GetAllDependencies(bundleName);
            List<AssetAsyncOperation> dps = new List<AssetAsyncOperation>();
            foreach (string dependency in dependencies)
            {
                dps.Add(loader.LoadBundleAsync(dependency));
            }

            string fullPath = PathUtils.Combine(AssetSetting.Instance.BundleLoadPath, bundleName);
            AssetBundleCreateRequest request = AssetBundle.LoadFromFileAsync(fullPath);
            loadOperation = new BundleAsyncOperation();
            loadOperation.SetOperation(request);
            loadOperation.SetDependencies(dps);
            request.completed += _ =>
            {
                bundle = request.assetBundle;
                unloadOperation = null;
                loadOperation.Completed(bundle);
            };

            return loadOperation;
        }

        public Object LoadAsset(string assetName)
        {
            if (!assets.TryGetValue(assetName, out BundleObject asset))
            {
                asset = new BundleObject(assetName, this);
                assets.Add(assetName, asset);
            }

            return asset.Load();
        }

        public AssetAsyncOperation LoadAssetAsync(string assetName)
        {
            if (!assets.TryGetValue(assetName, out BundleObject asset))
            {
               
                asset = new BundleObject(assetName, this);
                assets.Add(assetName, asset);
            }

            return asset.LoadAsync();
        }

        public void UnloadBundle(bool unloadAssets)
        {
            if (bundle == null)
            {
                if (unloadAssets)
                {
                    UnloadAssets();
                }

                return;
            }

            bundle.Unload(unloadAssets);
            bundle = null;
            loadOperation = null;
            if (unloadAssets)
            {
                assets.Clear();
            }
        }

        public AssetAsyncOperation UnloadBundleAsync(bool unloadAssets)
        {
            if (unloadOperation != null)
            {
                if (unloadAssets)
                {
                    UnloadAssets();
                }

                return unloadOperation;
            }

            unloadOperation = new AssetAsyncOperation();
            if (bundle == null)
            {
                if (unloadAssets)
                {
                    UnloadAssets();
                }

                unloadOperation.Completed(null);
                return unloadOperation;
            }

            AsyncOperation operation = bundle.UnloadAsync(unloadAssets);
            unloadOperation.SetOperation(operation);
            operation.completed += _ =>
            {
                bundle = null;
                loadOperation = null;
                if (unloadAssets)
                {
                    assets.Clear();
                }

                unloadOperation.Completed(null);
            };

            return unloadOperation;
        }

        public void UnloadAsset(string assetName)
        {
            if (assets.TryGetValue(assetName, out BundleObject asset))
            {
                asset.Unload();
                assets.Remove(assetName);
            }
        }

        public void UnloadAssets()
        {
            foreach (BundleObject asset in assets.Values)
            {
                asset.Unload();
            }

            assets.Clear();
        }
    }
}