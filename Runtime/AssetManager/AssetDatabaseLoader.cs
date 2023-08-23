using System.Collections.Generic;
using UnityEngine;

namespace GameFramework
{
    internal class AssetDatabaseLoader
    {
        private Dictionary<string, LocalAsset> assets = new Dictionary<string, LocalAsset>();

        public Object LoadAsset(string path)
        {
            if (!assets.TryGetValue(path, out LocalAsset asset))
            {
                asset = new LocalAsset(path);
                assets.Add(path, asset);
            }

            return asset.Load();
        }

        public AssetAsyncOperation LoadAssetAsync(string path)
        {
            if (!assets.TryGetValue(path, out LocalAsset asset))
            {
                asset = new LocalAsset(path);
                assets.Add(path, asset);
            }

            return asset.LoadAsync();
        }

        public void UnloadAsset(string path)
        {
            if (assets.TryGetValue(path, out LocalAsset asset))
            {
                asset.Unload();
            }
        }

        public void UnloadAllAssets()
        {
            foreach (LocalAsset asset in assets.Values)
            {
                asset.Unload();
            }

            assets.Clear();
            Resources.UnloadUnusedAssets();
        }

        public override string ToString()
        {
            return StringUtils.Join("\n", assets.Values);
        }
    }
}