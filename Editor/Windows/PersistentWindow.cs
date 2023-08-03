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
        private bool isShowNewDataBox;
        private StorageType storageType;
        private int storageId;

        private List<string> allKeys = new List<string>();
        private List<PersistentData> dataList = new List<PersistentData>();

        private PersistentManager Manager
        {
            get
            {
                if (Application.isPlaying)
                {
                    return PersistentManager.Instance;
                }

                if (!manager)
                {
                    manager = new GameObject().AddComponent<PersistentManager>();
                    manager.gameObject.hideFlags = HideFlags.HideAndDontSave;
                    manager.ChangeStorage(StorageType.Default);
                }

                return manager;
            }
        }

        public override void Init(string name, GameWindow parent)
        {
            base.Init("PersistentData", parent);
            settingEditor = Editor.CreateEditor(PersistentSetting.Instance);
            RefreshData();
        }

        public override void OnGUI()
        {
            if (settingEditor.target != null)
            {
                settingEditor.OnInspectorGUI();
            }

            storageType = (StorageType) EditorGUILayout.EnumPopup("Storage Type", Manager.StorageType);
            storageId = EditorGUILayout.IntField("Storage Id", Manager.StorageId);
            if (storageType != Manager.StorageType || storageId != Manager.StorageId)
            {
                Manager.ChangeStorage(storageType, storageId);
                RefreshData();
            }

            for (int i = 0; i < dataList.Count; i++)
            {
                DrawData(dataList[i]);
            }

            if (isShowNewDataBox)
            {
                DrawNewData();
            }

            if (GUILayout.Button("Refresh"))
            {
                RefreshData();
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
                Manager.Save();
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
            Manager.GetAllKeys(allKeys);
            foreach (string key in allKeys)
            {
                PersistentData localData = new PersistentData
                {
                    Key = key,
                    Value = Manager.GetString(key)
                };
                dataList.Add(localData);
            }
        }

        private void DrawData(PersistentData data)
        {
            EditorGUILayout.BeginHorizontal();
            data.Value = EditorGUILayout.TextField(data.Key, data.Value);
            if (GUILayout.Button("Set"))
            {
                Manager.SetData(data.Key, data.Value);
            }

            if (GUILayout.Button("Delete"))
            {
                Manager.DeleteKey(data.Key);
                dataList.Remove(data);
            }

            EditorGUILayout.EndHorizontal();
        }

        private void NewData()
        {
            newData.Key = default;
            newData.Value = default;
            isShowNewDataBox = true;
        }

        private void DrawNewData()
        {
            EditorGUILayout.BeginHorizontal();
            newData.Key = EditorGUILayout.TextField(newData.Key);
            newData.Value = EditorGUILayout.TextField(newData.Value);
            if (GUILayout.Button("Add"))
            {
                bool isError = false;
                if (string.IsNullOrEmpty(newData.Key))
                {
                    Debug.LogError("Key is invalid");
                    isError = true;
                }

                if (!isError && string.IsNullOrEmpty(newData.Value))
                {
                    Debug.LogError("Value is invalid");
                    isError = true;
                }

                if (!isError && Manager.HasKey(newData.Key))
                {
                    Debug.LogError($"Key {newData.Key} is already exist");
                    isError = true;
                }

                if (!isError)
                {
                    Manager.SetData(newData.Key, newData.Value);
                    dataList.Add(newData);
                    newData = new PersistentData();
                    isShowNewDataBox = false;
                }
            }

            if (GUILayout.Button("Delete"))
            {
                isShowNewDataBox = false;
            }

            EditorGUILayout.EndHorizontal();
        }

        private void ImportData()
        {
            string importPath = EditorUtility.OpenFilePanel("Import persistent data", Path.GetFullPath("Assets/.."), "pd");
            if (string.IsNullOrEmpty(importPath))
            {
                return;
            }

            Manager.ImportData(File.ReadAllText(importPath, Encoding.UTF8));
            RefreshData();
        }

        private void ExportData()
        {
            string exportPath = EditorUtility.SaveFilePanel("Export persistent data", Path.GetFullPath("Assets/.."), PlayerSettings.productName, "pd");
            if (string.IsNullOrEmpty(exportPath))
            {
                return;
            }

            string json = Manager.ExportAllData();
            File.WriteAllText(exportPath, json, Encoding.UTF8);
        }

        private void DeleteAll()
        {
            Manager.DeleteAll();
            dataList.Clear();
            isShowNewDataBox = false;
        }

        private class PersistentData
        {
            public string Key;
            public string Value;
        }
    }
}