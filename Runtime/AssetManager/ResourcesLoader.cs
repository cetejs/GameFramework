using System.Collections.Generic;
using UnityEngine;

namespace GameFramework
{
    internal class ResourcesLoader
    {
        private Dictionary<string, ResourcesAsset> assets = new Dictionary<string, ResourcesAsset>();

        public Object LoadAsset(string path)
        {
            if (!assets.TryGetValue(path, out ResourcesAsset asset))
            {
                asset = new ResourcesAsset(path);
                assets.Add(path, asset);
            }

            return asset.Load();
        }

        public AssetAsyncOperation LoadAssetAsync(string path)
        {
            if (!assets.TryGetValue(path, out ResourcesAsset asset))
            {
                asset = new ResourcesAsset(path);
                assets.Add(path, asset);
            }

            return asset.LoadAsync();
        }

        public void UnloadAsset(string path)
        {
            if (assets.TryGetValue(path, out ResourcesAsset asset))
            {
                asset.Unload();
            }
        }

        public void UnloadAllAssets()
        {
            foreach (ResourcesAsset asset in assets.Values)
            {
                asset.Unload();
            }

            assets.Clear();
            Resources.UnloadUnusedAssets();
        }
        
        public AssetAsyncOperation UnloadAllAssetsAsync()
        {
            AssetListAsyncOperation operation = new AssetListAsyncOperation();
            foreach (ResourcesAsset asset in assets.Values)
            {
                asset.Unload();
            }

            assets.Clear();
            Resources.UnloadUnusedAssets();
            operation.Completed(null);
            return operation;
        }

        public override string ToString()
        {
            string result = "";
            bool first = true;
            foreach (string path in assets.Keys)
            {
                if (first)
                {
                    first = false;
                }
                else
                {
                    result += "\n";
                }

                result += path;
            }

            return result;
        }
    }
}