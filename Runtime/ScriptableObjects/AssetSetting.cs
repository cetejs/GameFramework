namespace GameFramework
{
    public class AssetSetting : ScriptableObjectSingleton<AssetSetting>
    {
        [InspectorGroup("BundleSetting", 1)]
        public string BundleAssetName = "ToBundle";
        public string BundleExtension = "ab";
        public string BundleBuildPath = "StreamingAssets";
        public string BuiltinResourcesBundleName = "builtin_extra";
        public AssetLoadOption AssetLoadOption;
        [InspectorGroup("ShaderSetting", 3)]
        public string ShaderBundleName = "shaders";
        public string ShaderVariantsAssetPath = "Shaders/ShaderVariants";
        public bool DeleteShaderVariantsWhenBuild = false;
        [InspectorGroup("SpriteAtlasSetting", 6)]
        [EnumPopUp("2", "4", "8")]
        public int SpriteAtlasPackingPadding = 0;
        public string SpriteAtlasAssetPath = "SpriteAtlas";
        public bool DeleteSpriteAtlasWhenBuild = false;

        public string BundleAssetPath
        {
            get { return PathUtils.Combine(PathUtils.AssetPath, BundleAssetName); }
        }

        public string BundleSavePath
        {
            get { return PathUtils.Combine(PathUtils.AssetPath, BundleBuildPath, BundleAssetName); }
        }

        public string BundleLoadPath
        {
            get
            {
#if UNITY_EDITOR
                return PathUtils.Combine(PathUtils.AssetPath, BundleBuildPath);
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