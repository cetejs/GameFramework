using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace GameFramework
{
    [InitializeOnLoad]
    internal static class AssetInspector
    {
        static AssetInspector()
        {
            Editor.finishedDefaultHeaderGUI += OnPostHeaderGUI;
        }

        private static void OnPostHeaderGUI(Editor editor)
        {
            if (editor.targets.Length == 0)
            {
                return;
            }

            foreach (Object target in editor.targets)
            {
                string path = AssetDatabase.GetAssetPath(target).ReplaceSeparator();
                if (!AssetDatabase.IsValidFolder(path) || IsAssets(path) || IsInResources(path) || IsInEditor(path) || IsInPackage(path))
                {
                    return;
                }

                string guid = AssetDatabase.AssetPathToGUID(path);
                List<string> bundleAssetGuids = AssetSetting.Instance.BundleAssetGuids;
                bool isBundleAsset = bundleAssetGuids.Contains(guid);
                bool isSubBundleAsset = false;
                if (!isBundleAsset)
                {
                    isSubBundleAsset = IsSubBundleAsset(path);
                }

                GUILayout.BeginHorizontal();
                bool prevEnabledState = GUI.enabled;
                if (isSubBundleAsset)
                {
                    GUI.enabled = false;
                    GUILayout.Toggle(true, "BundleAsset");
                }
                else
                {
                    if (isBundleAsset != GUILayout.Toggle(isBundleAsset, "BundleAsset"))
                    {
                        if (isBundleAsset)
                        {
                            bundleAssetGuids.Remove(guid);
                        }
                        else
                        {
                            bundleAssetGuids.RemoveAll(tempGuid =>
                            {
                                string tempPath = AssetDatabase.GUIDToAssetPath(tempGuid).ReplaceSeparator();
                                return tempPath.StartsWith(path);
                            });

                            bundleAssetGuids.Add(guid);
                        }
                    }
                }

                if (isBundleAsset || isSubBundleAsset)
                {
                    GUILayout.Label(path.Substring(7, path.Length - 7));
                }

                GUI.enabled = prevEnabledState;
                GUILayout.EndHorizontal();
            }
        }

        private static bool IsSubBundleAsset(string path)
        {
            string[] content = path.Split("/");
            path = "Assets";
            for (int i = 1; i < content.Length - 1; i++)
            {
                path = PathUtils.Combine(path, content[i]);
                string guid = AssetDatabase.AssetPathToGUID(path);
                if (AssetSetting.Instance.BundleAssetGuids.Contains(guid))
                {
                    return true;
                }
            }

            return false;
        }

        private static bool IsAssets(string path)
        {
            return path == "Assets";
        }

        private static bool IsInResources(string path)
        {
            return path.Contains("/Resources/", StringComparison.OrdinalIgnoreCase) ||
                   path.EndsWith("/Resources", StringComparison.OrdinalIgnoreCase);
        }

        private static bool IsInEditor(string path)
        {
            return path.Contains("/Editor/", StringComparison.OrdinalIgnoreCase) ||
                   path.EndsWith("/Editor", StringComparison.OrdinalIgnoreCase);
        }

        private static bool IsInPackage(string path)
        {
            return path.StartsWith("Packages");
        }
    }
}