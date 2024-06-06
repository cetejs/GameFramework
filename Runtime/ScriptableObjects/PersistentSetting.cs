using UnityEngine.Serialization;

namespace GameFramework
{
    public class PersistentSetting : ScriptableObjectSingleton<PersistentSetting>
    {
        public string SaveDirectory = "SaveData";
        public string DefaultStorageName = "DefaultStorage";
        public string SaveDataExtension = "dat";
        public StorageMode StorageMode;
        public CryptoType CryptoType;
        [EnumCondition("CryptoType", (int) CryptoType.AES)]
        public string Password = "password";

        public string SaveDataPath
        {
            get
            {
#if UNITY_EDITOR
                return PathUtils.Combine(PathUtils.StreamingAssetsPath, SaveDirectory);
#else
                return PathUtils.Combine(PathUtils.PersistentDataPath, SaveDirectory);
#endif
            }
        }

        public string GetSavePath(string storageName)
        {
            string path = PathUtils.Combine(SaveDataPath, storageName);
            if (string.IsNullOrEmpty(SaveDataExtension) || storageName.LastIndexOf(".") >= 0)
            {
                return path;
            }

            return StringUtils.Concat(path, ".", SaveDataExtension);
        }
    }

    public enum StorageMode
    {
        Json,
        Binary
    }
}