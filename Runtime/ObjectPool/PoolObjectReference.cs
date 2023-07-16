// using System;
// using UnityEngine;
// using UnityEngine.AddressableAssets;
// using Object = UnityEngine.Object;
//
// namespace GameFramework.ObjectPoolService
// {
//     [Serializable]
//     public class PoolObjectReference : AssetReference
//     {
//         [SerializeField]
//         private string localPath;
//
//         public string LocalPath
//         {
//             get { return localPath; }
//         }
//
//         public override bool RuntimeKeyIsValid()
//         {
//             return base.RuntimeKeyIsValid() && !string.IsNullOrEmpty(localPath);
//         }
//
// #if UNITY_EDITOR
//         public override Object editorAsset
//         {
//             get
//             {
//                 Object asset = base.editorAsset;
//                 if (asset)
//                 {
//                     string path = UnityEditor.AssetDatabase.GetAssetPath(asset);
//                     if (ValidateAsset(path))
//                     {
//                         ObjectPoolConfig config = ObjectPoolConfig.Get();
//                         if (!path.EndsWith(string.Concat(config.poolBundlePath, "/", localPath, ".prefab")))
//                         {
//                             Debug.LogError($"Pool Object local path {localPath} is invalid");
//                             SetEditorAsset(null);
//                             return null;
//                         }
//                     }
//                 }
//
//                 return asset;
//             }
//         }
//
//         public override bool SetEditorAsset(Object value)
//         {
//             if (base.SetEditorAsset(value) && value)
//             {
//                 ObjectPoolConfig config = ObjectPoolConfig.Get();
//                 string path = UnityEditor.AssetDatabase.GetAssetPath(value);
//                 int index = path.IndexOf(config.poolBundlePath, StringComparison.Ordinal);
//                 int startIndex = index + config.poolBundlePath.Length + 1;
//                 localPath = path.Substring(startIndex, path.Length - startIndex - 7);
//                 return true;
//             }
//
//             localPath = null;
//             return false;
//         }
//
//         public override bool ValidateAsset(string path)
//         {
//             ObjectPoolConfig config = ObjectPoolConfig.Get();
//             if (!config)
//             {
//                 return false;
//             }
//
//             return path.IndexOf(config.poolBundlePath, StringComparison.Ordinal) > -1;
//         }
// #endif
//     }
// }