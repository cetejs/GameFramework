using System.IO;
using GameFramework.Generic;
using UnityEngine;

namespace GameFramework.DataTableService
{
    public class DataTableConfig : ScriptableObject
    {
        [ReadOnly]
        public string rootPath = Path.GetFullPath(".");
        public string excelRootPath = "RawTables";
        public string tableBuildPath = "Assets/Configs/Tables";
        public string scriptBuildPath = "Assets/Scripts/Tables";
        public string tableBundlePath = "Configs/Tables";
        public string scriptNamespace = "GameFramework";

        private static DataTableConfig instance;

        public static DataTableConfig Get()
        {
            if (instance)
            {
                return instance;
            }

            DataTableConfig config = Resources.Load<DataTableConfig>("DataTableConfig");
            if (!config)
            {
                Debug.LogError("Please press GameFramework/ImportConfig");
                return null;
            }

            instance = config;
            return instance;
        }
    }
}