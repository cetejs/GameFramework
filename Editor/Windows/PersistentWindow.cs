using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace GameFramework
{
    internal class PersistentWindow : SubWindow
    {
        private Editor settingEditor;
        private PersistentManager manager;
        private PersistentData newData = new PersistentData();
        private bool showNewDataBox;
        private string storageName;
        private IPersistentStorage storage;

        private List<string> allKeys = new List<string>();
        private List<PersistentData> dataList = new List<PersistentData>();

        public override void Init(string name, GameWindow parent)
        {
            base.Init("PersistentData", parent);
            settingEditor = Editor.CreateEditor(PersistentSetting.Instance);
            storageName = PersistentSetting.Instance.DefaultStorageName;
            if (Application.isPlaying)
            {
                manager = PersistentManager.Instance;
            }

            if (!manager)
            {
                manager = new GameObject().AddComponent<PersistentManager>();
                manager.gameObject.hideFlags = HideFlags.HideAndDontSave;
                storage = manager.GetStorage(storageName);
            }

            RefreshData();
        }

        public override void OnGUI()
        {
            if (settingEditor.target != null)
            {
                settingEditor.OnInspectorGUI();
            }

            EditorGUILayout.BeginHorizontal();
            storageName = EditorGUILayout.TextField("Storage Name", storageName);
            if (GUILayout.Button("Load", GUILayout.Width(100)))
            {
                storage.Load(storageName);
                RefreshData();
            }

            EditorGUILayout.EndHorizontal();

            for (int i = 0; i < dataList.Count; i++)
            {
                DrawData(dataList[i]);
            }

            if (showNewDataBox)
            {
                DrawNewData();
            }

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Import Data"))
            {
                ImportData();
            }

            if (GUILayout.Button("Export Data"))
            {
                ExportData();
            }

            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("New Data"))
            {
                NewData();
            }

            if (GUILayout.Button("Save"))
            {
                storage.Save();
                AssetDatabase.Refresh();
            }

            if (GUILayout.Button("Delete All"))
            {
                DeleteAll();
            }

            GUILayout.EndHorizontal();
        }

        public override void OnDestroy()
        {
            Object.DestroyImmediate(settingEditor);
            if (manager != null)
            {
                Object.DestroyImmediate(manager);
            }
        }

        private void RefreshData()
        {
            dataList.Clear();
            storage.GetAllKeys(allKeys);
            foreach (string key in allKeys)
            {
                PersistentData localData = new PersistentData
                {
                    Key = key,
                    Value = storage.GetData<string>(key, default)
                };
                dataList.Add(localData);
            }
        }

        private void DrawData(PersistentData data)
        {
            EditorGUILayout.BeginHorizontal();
            data.Value = EditorGUILayout.TextField(data.Key, data.Value);
            if (GUILayout.Button("Set", GUILayout.Width(100)))
            {
                storage.SetData(data.Key, data.Value);
            }

            if (GUILayout.Button("Delete", GUILayout.Width(100)))
            {
                storage.DeleteKey(data.Key);
                dataList.Remove(data);
            }

            EditorGUILayout.EndHorizontal();
        }

        private void NewData()
        {
            newData.Key = default;
            newData.Value = default;
            showNewDataBox = true;
        }

        private void DrawNewData()
        {
            EditorGUILayout.BeginHorizontal();
            newData.Key = EditorGUILayout.TextField(newData.Key);
            newData.Value = EditorGUILayout.TextField(newData.Value);
            if (GUILayout.Button("Add"))
            {
                bool error = false;
                if (string.IsNullOrEmpty(newData.Key))
                {
                    GameLogger.LogError("Add new data is fail, because key is invalid");
                    error = true;
                }

                if (!error && string.IsNullOrEmpty(newData.Value))
                {
                    GameLogger.LogError("Add new data is fail, because value is invalid");
                    error = true;
                }

                if (!error && storage.HasKey(newData.Key))
                {
                    GameLogger.LogError($"Add new data is fail, because key {newData.Key} is already exist");
                    error = true;
                }

                if (!error)
                {
                    storage.SetData(newData.Key, newData.Value);
                    dataList.Add(newData);
                    newData = new PersistentData();
                    showNewDataBox = false;
                }
            }

            if (GUILayout.Button("Delete"))
            {
                showNewDataBox = false;
            }

            EditorGUILayout.EndHorizontal();
        }

        private void ImportData()
        {
            string importPath = EditorUtility.OpenFilePanel("Import persistent data", Path.GetFullPath("Assets/.."), PersistentSetting.Instance.SaveDataExtension);
            if (string.IsNullOrEmpty(importPath))
            {
                return;
            }

            storage.ImportData(File.ReadAllText(importPath, Encoding.UTF8));
            RefreshData();
        }

        private void ExportData()
        {
            string exportPath = EditorUtility.SaveFilePanel("Export persistent data", Path.GetFullPath("Assets/.."), PlayerSettings.productName, PersistentSetting.Instance.SaveDataExtension);
            if (string.IsNullOrEmpty(exportPath))
            {
                return;
            }

            string json = storage.ExportAllData();
            File.WriteAllText(exportPath, json, Encoding.UTF8);
        }

        private void DeleteAll()
        {
            storage.DeleteAll();
            dataList.Clear();
            showNewDataBox = false;
        }

        private class PersistentData
        {
            public string Key;
            public string Value;
        }
    }
}