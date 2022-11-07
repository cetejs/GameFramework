using System.IO;
using GameFramework;
using GameFramework.DataTableService;
using GameFramework.Generic;
using GameFramework.Utils;
using UnityEngine;

public class DataTableTest : MonoBehaviour
{
    private DataTableManager manager;

    private void AdjustConfig()
    {
#if UNITY_EDITOR
        string fullPath = Path.Combine(Application.dataPath, "GameFramework/Resources/DataTableConfig.asset");
        if (!File.Exists(fullPath))
        {
            EditorFileUtils.CopyAsset("DataTableConfig.asset", fullPath);
        }

        DataTableConfig config = DataTableConfig.Get();
        string samplesPath = EditorFileUtils.GetSamplesPath().Replace(config.rootPath, "").RemoveFirstCount();
        config.excelRootPath = Path.Combine(samplesPath, "DataTable/RawTables");
        config.tableBuildPath = Path.Combine(samplesPath, "DataTable/Tables");
        config.scriptBuildPath = Path.Combine(samplesPath, "DataTable/Scripts/Tables");
        config.tableBundlePath = "Tables";
        UnityEditor.AssetDatabase.SaveAssetIfDirty(config);
        AddressableUtils.CreateOrMoveEntry(config.tableBuildPath, config.tableBundlePath);
#endif
    }

    private void Awake()
    {
        AdjustConfig();
    }

    private void Start()
    {
        manager = Global.GetService<DataTableManager>();
    }

    private void OnGUI()
    {
        GUIStyle style = new GUIStyle(GUI.skin.button);
        style.fontSize = Screen.height / 50;
        if (GUILayout.Button("Get[Test1(1)]", style))
        {
            Test1 t1 = manager.GetTable<Test1>("1");
            Debug.Log(t1);
        }

        if (GUILayout.Button("GetAsync[Test1(1)]", style))
        {
            GetTableAsync();
        }

        if (GUILayout.Button("GetAll[Test1]", style))
        {
            string[] allKeys = manager.GetAllKeys<Test1>();
            foreach (string key in allKeys)
            {
                Test1 t1 = manager.GetTable<Test1>(key);
                Debug.Log(t1);
            }
        }

        if (GUILayout.Button("GetAllAsync[Test1]", style))
        {
            GetAllKeysAsync();
        }

        if (GUILayout.Button("Preload(Test1)", style))
        {
            manager.PreloadTable<Test1>();
        }

        if (GUILayout.Button("Reload(Test1)", style))
        {
            manager.ReloadTable<Test1>();
        }
    }

    private async void GetTableAsync()
    {
        Test1 t1 = await manager.GetTableAsync<Test1>("1");
        Debug.Log(t1);
    }

    private async void GetAllKeysAsync()
    {
        string[] allKeys = await manager.GetAllKeysAsync<Test1>();
        foreach (string key in allKeys)
        {
            Test1 t1 = await manager.GetTableAsync<Test1>(key);
            Debug.Log(t1);
        }
    }
}