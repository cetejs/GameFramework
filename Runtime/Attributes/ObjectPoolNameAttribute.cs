using System;

namespace GameFramework
{
    [AttributeUsage(AttributeTargets.Field)]
    public class ObjectPoolNameAttribute : BundleAssetNameAttribute
    {
        public ObjectPoolNameAttribute() : base(GameSettings.Instance.PoolAssetName, "prefab")
        {
        }
    }
}