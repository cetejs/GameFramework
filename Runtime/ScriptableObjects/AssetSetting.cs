using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace GameFramework
{
    public class AssetSetting : ScriptableObjectSingleton<AssetSetting>
    {
        [InspectorGroup("BundleSetting", 1)]
        public string BundleExtension = "ab";
        public string BundleBuildPath = "StreamingAssets";
        public string BundleHashName = "BundleHash";
        public string BuiltinResourcesBundleName = "builtin_extra";
        public string DownloadUri = "http://127.0.0.1:3000/AssetBundle";
        public AssetLoadOption AssetLoadOption;
        [HideInInspector]
        public List<string> BundleAssetGuids = new List<string>();
        [InspectorGroup("ShaderSetting", 3)]
        public string ShaderBundleName = "shaders";
        public string ShaderVariantsAssetPath = "Shaders/ShaderVariants";
        public bool DeleteShaderVariantsWhenBuild = false;
        [InspectorGroup("SpriteAtlasSetting", 6)]
        [EnumPopUp("2", "4", "8")]
        public int SpriteAtlasPackingPadding = 0;
        public string SpriteAtlasAssetPath = "SpriteAtlas";
        public bool DeleteSpriteAtlasWhenBuild = false;

        private string BundleAssetName = "AssetBundles";

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
            get { return PathUtils.Combine(DownloadUri, Platform, BundleAssetName); }
        }

        public string ManifestBundleName
        {
            get
            {
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