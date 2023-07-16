namespace GameFramework
{
    public class AssetSetting : ScriptableObjectSingleton<AssetSetting>
    {
        public string BundleAssetName = "ToBundle";
        public string BundleExtension = "ab";
        public string BundleBuildPath = PathUtils.StreamingAssetsPath;
        public string BuiltinResourcesBundleName = "builtin_extra";
        public string ShaderBundleName = "shaders";
        public string ShaderVariantsAssetPath = "Assets/Shaders/ShaderVariants";
        public bool DeleteShaderVariantsWhenBuild = false;
        public AssetLoadOption AssetLoadOption;

        public string BundleAssetPath
        {
            get { return PathUtils.Combine(PathUtils.AssetPath, BundleAssetName); }
        }

        public string BundleSavePath
        {
            get { return PathUtils.Combine(BundleBuildPath, BundleAssetName); }
        }

        public string BundleLoadPath
        {
            get
            {
#if UNITY_EDITOR
                return BundleSavePath;
#else
                return PathUtils.Combine(PathUtils.PersistentDataPath, BundleAssetPath);
#endif
            }
        }

        public bool UseAssetBundle
        {
            get
            {
                if (AssetLoadOption == AssetLoadOption.AssetBundle)
                {
                    return true;
                }

                if (AssetLoadOption == AssetLoadOption.Simulate)
                {
#if UNITY_EDITOR
                    return false;
#else
                    return true;
#endif
                }

                return false;
            }
        }
    }
}