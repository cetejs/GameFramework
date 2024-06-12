namespace GameFramework
{
    public class DataTableSetting : ScriptableObjectSingleton<DataTableSetting>
    {
        [ReadOnly]
        public string RootPath;
        public string ExcelRootPath = "RawTables";
        public string TableBuildPath = "Configs/Tables";
        public string ScriptBuildPath = "Scripts/Tables";
        public string ScriptNamespace = "GameFramework";
        public string LocalizationName = "Localization";
        public CryptoType CryptoType;
        [EnumCondition("CryptoType", (int) CryptoType.AES)]
        public string Password = "password";

        private void OnEnable()
        {
            RootPath = PathUtils.ProjectPath;
        }

        public string OutputTablePath
        {
            get { return PathUtils.Combine(PathUtils.AssetPath, TableBuildPath); }
        }

        public string OutputScriptPath
        {
            get { return PathUtils.Combine(PathUtils.AssetPath, ScriptBuildPath); }
        }

        public string LoadTablePath
        {
            get { return TableBuildPath; }
        }

        public string LoadLocalizationPath
        {
            get { return PathUtils.Combine(TableBuildPath, LocalizationName); }
        }
    }
}