using System;
using UnityEngine;

namespace GameFramework
{
    [AttributeUsage(AttributeTargets.Field)]
    public class BundleAssetNameAttribute : PropertyAttribute
    {
        public readonly string AssetName;
        public readonly string Extension;

        public BundleAssetNameAttribute(string assetName, string extension)
        {
            AssetName = assetName;
            Extension = extension;
        }
    }
}