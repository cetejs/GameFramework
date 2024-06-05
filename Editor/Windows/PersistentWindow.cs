using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

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
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
            OnPlayModeStateChanged(EditorApplication.isPlaying ? PlayModeStateChange.EnteredPlayMode : PlayModeStateChange.EnteredEditMode);
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
                manager.Unload(storageName);
                storage = manager.GetStorage(storageName);
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
            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
            Object.DestroyImmediate(settingEditor);
            if (manager != null)
            {
                Object.DestroyImmediate(manager);
            }
        }

        private void OnPlayModeStateChanged(PlayModeStateChange stateChange)
        {
            switch (stateChange)
            {
                case PlayModeStateChange.EnteredPlayMode:
                    if (manager != null)
                    {
                        Object.DestroyImmediate(manager.gameObject);
                    }

                    manager = PersistentManager.Instance;
                    storage = manager.GetStorage(storageName);
                    RefreshData();
                    break;
                case PlayModeStateChange.EnteredEditMode:
                    manager = new GameObject().AddComponent<PersistentManager>();
                    manager.gameObject.hideFlags = HideFlags.HideAndDontSave;
                    storage = manager.GetStorage(storageName);
                    RefreshData();
                    break;
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
                    Key = key
                };

                try
                {
                    localData.Value = storage.GetData<string>(key, default);
                }
                catch (Exception ex)
                {
                    localData.Value = null;
                }

                dataList.Add(localData);
            }
        }

        private void DrawData(PersistentData data)
        {
            EditorGUILayout.BeginHorizontal();
            if (data.Value == null)
            {
                GUI.enabled = false;
                EditorGUILayout.TextField(data.Key, "This type of display is not supported");
            }
            else
            {
                data.Value = EditorGUILayout.TextField(data.Key, data.Value);
            }

            if (GUILayout.Button("Set", GUILayout.Width(100)))
            {
                storage.SetData(data.Key, data.Value);
            }

            GUI.enabled = true;
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