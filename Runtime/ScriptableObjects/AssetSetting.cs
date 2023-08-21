using System.IO;
using UnityEngine;
using UnityEngine.Serialization;

namespace GameFramework
{
    public class AssetSetting : ScriptableObjectSingleton<AssetSetting>
    {
        [InspectorGroup("BundleSetting", 1)]
        public string BundleAssetName = "ToBundle";
        public string BundleExtension = "ab";
        public string BundleBuildPath = "StreamingAssets";
        public string BundleHashName = "BundleHash";
        public string BuiltinResourcesBundleName = "builtin_extra";
        [FormerlySerializedAs("DownloadURL")] public string DownloadUri;
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

        public string LocalBundlePath
        {
            get
            {
#if UNITY_EDITOR
                return BundleSavePath;
#else
                return PathUtils.Combine(PathUtils.PersistentDataPath, BundleAssetName);
#endif
            }
        }

        public string RemoteBundleUri
        {
            get { return PathUtils.Combine(Instance.DownloadUri, Instance.Platform, Instance.BundleAssetName); }
        }

        public string ManifestBundleName
        {
            get
            {
                string bundlePath = BundleAssetName.GetLastOf("/");
                if (!string.IsNullOrEmpty(bundlePath))
                {
                    return bundlePath;
                }

                return BundleAssetName;
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

        public string Platform
        {
            get
            {
                switch (Application.platform)
                {
                    case RuntimePlatform.WindowsEditor:
                    case RuntimePlatform.WindowsPlayer:
                        return "Windows";
                    case RuntimePlatform.OSXEditor:
                    case RuntimePlatform.OSXPlayer:
                        return "OSX";
                    case RuntimePlatform.Android:
                        return "Android";
                    case RuntimePlatform.IPhonePlayer:
                        return "iOS";
                    default:
                        return Application.platform.ToString();
                }
            }
        }

        public string GetBundlePath(string bundleName)
        {
            string persistentBundlePath = PathUtils.Combine(PathUtils.PersistentDataPath, BundleAssetName, bundleName);
            if (File.Exists(persistentBundlePath))
            {
                return persistentBundlePath;
            }
            
            return PathUtils.Combine(PathUtils.StreamingAssetsPath, BundleAssetName, bundleName);
        }
    }
}