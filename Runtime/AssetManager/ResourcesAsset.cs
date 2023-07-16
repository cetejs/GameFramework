using UnityEngine;

namespace GameFramework
{
    internal class ResourcesAsset
    {
        private string path;
        private Object asset;
        private AssetAsyncOperation operation;

        public ResourcesAsset(string path)
        {
            this.path = path;
        }

        public Object Load()
        {
            if (asset)
            {
                return asset;
            }

            asset = Resources.Load(path);
            return asset;
        }

        public AssetAsyncOperation LoadAsync()
        {
            if (operation != null)
            {
                return operation;
            }

            if (asset != null)
            {
                operation = new AssetAsyncOperation();
                operation.Completed(asset);
                return operation;
            }

            ResourceRequest request = Resources.LoadAsync(path);
            operation = new AssetAsyncOperation();
            operation.SetOperation(request);
            request.completed += _ =>
            {
                asset = request.asset;
                operation.Completed(request.asset);
            };

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
    }
}