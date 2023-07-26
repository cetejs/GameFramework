namespace GameFramework
{
    public class PersistentSetting : ScriptableObjectSingleton<PersistentSetting>
    {
        public string SaveDataName = "SaveData/SaveData";
        public string SaveDataExtension = "pd";

        public string SaveDataPath
        {
            get
            {
#if UNITY_EDITOR
                return PathUtils.Combine(PathUtils.StreamingAssetsPath, SaveDataName);
#else
                return PathUtils.Combine(PathUtils.PersistentDataPath, SaveDataName);
#endif
            }
        }

        public string GetSavePath(int storageId)
        {
            if (string.IsNullOrEmpty(SaveDataExtension))
            {
                return StringUtils.Concat(SaveDataPath, storageId);
            }

            return StringUtils.Concat(SaveDataPath, storageId, ".", SaveDataExtension);
        }
    }
}