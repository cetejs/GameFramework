using UnityEngine;

namespace GameFramework
{
    public static class ShaderUtils
    {
        public static void WarmUp()
        {
            if (AssetSetting.Instance.UseAssetBundle)
            {
                string shaderVariantsName = AssetSetting.Instance.ShaderVariantsAssetPath.GetLastOf("/");
                string shaderBundleName = AssetSetting.Instance.ShaderBundleName;
                string path = PathUtils.Combine(shaderBundleName, shaderVariantsName);
                AssetManager.Instance.UnloadBundle(shaderBundleName, true);
                ShaderVariantCollection shaderVariants = AssetManager.Instance.LoadAsset<ShaderVariantCollection>(path);
                if (shaderVariants != null)
                {
                    shaderVariants.WarmUp();
                }
            }
        }

        public static void WarmUpAsync()
        {
            if (AssetSetting.Instance.UseAssetBundle)
            {
                string shaderVariantsName = AssetSetting.Instance.ShaderVariantsAssetPath.GetLastOf("/");
                string shaderBundleName = AssetSetting.Instance.ShaderBundleName;
                string path = PathUtils.Combine(shaderBundleName, shaderVariantsName);
                AssetAsyncOperation unloadOperation = AssetManager.Instance.UnloadBundleAsync(shaderBundleName, true);
                unloadOperation.OnCompleted += _ =>
                {
                    AssetAsyncOperation loadOperation = AssetManager.Instance.LoadAssetAsync(path);
                    loadOperation.OnCompleted += _ =>
                    {
                        ShaderVariantCollection shaderVariants = loadOperation.GetResult<ShaderVariantCollection>();
                        if (shaderVariants != null)
                        {
                            shaderVariants.WarmUp();
                        }
                    };
                };
            }
        }
    }
}