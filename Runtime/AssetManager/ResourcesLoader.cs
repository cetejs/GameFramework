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
    }
}