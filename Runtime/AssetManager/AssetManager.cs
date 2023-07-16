using UnityEngine;

namespace GameFramework
{
    public class AssetManager : PersistentSingleton<AssetManager>
    {
        private AssetBundleLoader bundleLoader = new AssetBundleLoader();
        private AssetDatabaseLoader databaseLoader = new AssetDatabaseLoader();
        private ResourcesLoader resourcesLoader = new ResourcesLoader();

        public T LoadAsset<T>(string path) where T : Object
        {
            return LoadAsset(path) as T;
        }

        public Object LoadAsset(string path)
        {
            switch (AssetSetting.Instance.AssetLoadOption)
            {
                case AssetLoadOption.Simulate:
#if UNITY_EDITOR
                    return databaseLoader.LoadAsset(path);
#else
                    return bundleLoader.LoadAsset(path);
#endif
                case AssetLoadOption.AssetBundle:
                    return bundleLoader.LoadAsset(path);
                default:
                    return resourcesLoader.LoadAsset(path);
            }
        }

        public AssetAsyncOperation LoadAssetAsync(string path)
        {
            switch (AssetSetting.Instance.AssetLoadOption)
            {
                case AssetLoadOption.Simulate:
#if UNITY_EDITOR
                    return databaseLoader.LoadAssetAsync(path);
#else
                    return bundleLoader.LoadAssetAsync(path);
#endif
                case AssetLoadOption.AssetBundle:
                    return bundleLoader.LoadAssetAsync(path);
                default:
                    return resourcesLoader.LoadAssetAsync(path);
            }
        }

        public void UnloadBundle(string bundleName, bool unloadAssets)
        {
            bundleLoader.UnloadBundle(bundleName, unloadAssets);
        }

        public AssetAsyncOperation UnloadBundleAsync(string bundleName, bool unloadAssets)
        {
            return bundleLoader.UnloadBundleAsync(bundleName, unloadAssets);
        }

        public void UnloadAsset(string path)
        {
            switch (AssetSetting.Instance.AssetLoadOption)
            {
                case AssetLoadOption.Simulate:
#if UNITY_EDITOR
                    databaseLoader.UnloadAsset(path);
#else
                    bundleLoader.UnloadAsset(path);
#endif
                    break;
                case AssetLoadOption.AssetBundle:
                    bundleLoader.UnloadAsset(path);
                    break;
                default:
                    resourcesLoader.UnloadAsset(path);
                    break;
            }
        }

        
    }
}