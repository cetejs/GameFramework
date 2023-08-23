using UnityEngine;

namespace GameFramework
{
    internal class BundleObject
    {
        private string name;
        private Object asset;
        private BundleAsset bundleAsset;
        private AssetAsyncOperation assetOperation;

        public BundleObject(string name, BundleAsset bundleAsset)
        {
            this.name = name;
            this.bundleAsset = bundleAsset;
        }

        public Object Load()
        {
            if (asset != null)
            {
                return asset;
            }

            AssetBundle bundle = bundleAsset.LoadBundle();
            if (bundle == null)
            {
                GameLogger.LogError($"Asset {name} is load fail, because asset bundle is null");
                return null;
            }

            asset = bundle.LoadAsset(name);
            return asset;
        }

        public AssetAsyncOperation LoadAsync()
        {
            if (assetOperation != null)
            {
                return assetOperation;
            }

            assetOperation = new AssetAsyncOperation();
            if (asset != null)
            {
                assetOperation.Completed(asset);
                return assetOperation;
            }

            AssetAsyncOperation bundleOperation = bundleAsset.LoadBundleAsync();
            assetOperation.SetDependency(bundleOperation);
            bundleOperation.OnCompleted += _ =>
            {
                if (assetOperation == null)
                {
                    GameLogger.LogError($"Asset {name} is load fail, because asset is unloaded before loading");
                    return;
                }

                AssetBundle bundle = (AssetBundle) bundleOperation.Result;
                if (bundle == null)
                {
                    GameLogger.LogError($"Asset {name} is load fail, because asset bundle is null");
                    assetOperation.Completed(null);
                    return;
                }

                AssetBundleRequest request = bundle.LoadAssetAsync(name);
                assetOperation.SetOperation(request);
                request.completed += _ =>
                {
                    asset = request.asset;
                    assetOperation.Completed(request.asset);
                };
            };

            return assetOperation;
        }

        public void Unload()
        {
            if (asset && asset is not GameObject)
            {
                Resources.UnloadAsset(asset);
            }

            asset = null;
            bundleAsset = null;
            assetOperation = null;
        }

        public override string ToString()
        {
            return name;
        }
    }
}