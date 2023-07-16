using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace GameFramework
{
    [CustomPropertyDrawer(typeof(ObjectPoolNameAttribute))]
    internal class ObjectPoolNameAttributeDrawer : SupportReadOnlyDrawer
    {
        private MethodInfo methodInfo;

        public override void OnPropertyGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            string poolAssetPath = PathUtils.Combine(AssetSetting.Instance.BundleAssetPath, ObjectPoolSetting.Instance.PoolAssetName);
            List<string> assets = new List<string>();
            if (Directory.Exists(poolAssetPath))
            {
                string prefix = StringUtils.Concat(ObjectPoolSetting.Instance.PoolAssetName, "/");
                string suffix = ".prefab";
                assets.AddRange(Directory.GetFiles(poolAssetPath, "*.prefab", SearchOption.AllDirectories));
                for (int i = 0; i < assets.Count; i++)
                {
                    assets[i] = assets[i].ReplaceSeparator().RemoveFirstOf(prefix).RemoveLastOf(suffix);
                }
            }

            if (assets.Count == 0)
            {
                GUIStyle style = new GUIStyle(GUI.skin.label);
                style.normal.textColor = Color.red;;
                string poolAssetName = PathUtils.Combine("Assets", AssetSetting.Instance.BundleAssetName, ObjectPoolSetting.Instance.PoolAssetName);
                EditorGUI.LabelField(position, label.text, $"There are no prefab in {poolAssetName}", style);
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