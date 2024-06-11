using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace GameFramework
{
    [CustomPropertyDrawer(typeof(BundleAssetNameAttribute))]
    internal class BundleAssetNameAttributeDrawer : SupportReadOnlyDrawer
    {
        public override void OnPropertyGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            BundleAssetNameAttribute bundleAssetName = (BundleAssetNameAttribute) attribute;
            string assetPath = PathUtils.Combine(PathUtils.AssetPath, bundleAssetName.AssetName);
            List<string> assets = new List<string>();
            if (Directory.Exists(assetPath))
            {
                string prefix = StringUtils.Concat(bundleAssetName.AssetName, "/");
                string suffix = StringUtils.Concat(".", bundleAssetName.Extension);
                assets.AddRange(Directory.GetFiles(assetPath, StringUtils.Concat("*", suffix), SearchOption.AllDirectories));
                for (int i = 0; i < assets.Count; i++)
                {
                    assets[i] = assets[i].ReplaceSeparator().RemoveFirstOf(prefix).RemoveLastOf(suffix);
                }
            }

            if (assets.Count == 0)
            {
                GUIStyle style = new GUIStyle(GUI.skin.label);
                style.normal.textColor = Color.red;
                string assetName = PathUtils.Combine("Assets", bundleAssetName.AssetName);
                EditorGUI.LabelField(position, label.text, $"There are no {bundleAssetName.Extension} in {assetName}", style);
                return;
            }

            int index = 0;
            for (int i = 0; i < assets.Count; i++)
            {
                if (assets[i] == property.stringValue)
                {
                    index = i;
                    break;
                }
            }

            index = EditorGUI.Popup(position, label.text, index, assets.ToArray());
            property.stringValue = assets[index];
        }
    }
}