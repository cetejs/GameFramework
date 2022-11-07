using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;

namespace GameFramework
{
    public static class AddressableUtils
    {
        public static void CreateOrMoveEntry(string assetPath, string address)
        {
            string assetGuid = AssetDatabase.AssetPathToGUID(assetPath);
            AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.GetSettings(true);
            AddressableAssetEntry entry = settings.CreateOrMoveEntry(assetGuid, settings.DefaultGroup);
            entry.address = address;
        }
    }
}