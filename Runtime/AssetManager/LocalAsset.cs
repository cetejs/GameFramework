using UnityEngine;

namespace GameFramework
{
    internal class LocalAsset
    {
        private string path;
        private Object asset;
        private AssetAsyncOperation operation;

        public LocalAsset(string path)
        {
            this.path = path;
        }

        public Object Load()
        {
            if (asset)
            {
                return asset;
            }

            path = PathUtils.Combine("Assets", AssetSetting.Instance.BundleAssetName, path);
            path = StringUtils.Concat(path, FileUtils.GetExtension(path));
#if UNITY_EDITOR
            asset = UnityEditor.AssetDatabase.LoadMainAssetAtPath(path);
#endif
            return asset;
        }

        public AssetAsyncOperation LoadAsync()
        {
            if (operation != null)
            {
                return operation;
            }

            operation = new AssetAsyncOperation();
            operation.Completed(Load());
            return operation;
        }

        public void Unload()
        {
            if (asset && asset is not GameObject)
            {
                Resources.UnloadAsset(asset);
            }

            asset = null;
            operation = null;
        }

        public override string ToString()
        {
            return $"{path}:{asset}";
        }
    }
}