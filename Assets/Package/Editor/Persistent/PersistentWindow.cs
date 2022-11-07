using System.Collections.Generic;
using System.IO;
using System.Text;
using GameFramework.Generic;
using GameFramework.Persistent;
using UnityEditor;
using UnityEngine;

namespace GameFramework
{
    internal class PersistentWindow : EditorWindow
    {
        private PersistentManager manager;
        private PersistentData newData = new PersistentData();
        private bool isShowNewDataBox;

        private readonly List<string> allKeys = new List<string>();
        private readonly List<PersistentData> dataList = new List<PersistentData>();

        private PersistentManager Manager
        {
            get
            {
                if (Application.isPlaying)
                {
                    return Global.GetService<PersistentManager>();
                }

                if (!manager)
                {
                    manager = new GameObject().AddComponent<PersistentManager>();
                    manager.gameObject.hideFlags = HideFlags.HideAndDontSave;
                }

                return manager;
            }
        }

        [MenuItem("GameFramework/PersistentWindow")]
        private static void CreatWindow()
        {
            PersistentWindow window = GetWindow<PersistentWindow>();
            window.titleContent.text = "PersistentWindow";
        }

        private void OnEnable()
        {
            RefreshData();
        }

        private void OnGUI()
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
            {
                Rect optionsRect = GUILayoutUtility.GetRect(0, 20, GUILayout.ExpandWidth(false));
                if (GUILayout.Button("Options", EditorStyles.toolbarDropDown, GUILayout.ExpandWidth(false)))
                {
                    GenericMenu options = new GenericMenu();
                    options.AddItem(new GUIContent("New Data..."), false, NewData);
                    options.AddSeparator("");
                    options.AddItem(new GUIContent("Import Data..."), false, ImportData);
                    options.AddItem(new GUIContent("Export Data..."), false, ExportData);
                    options.AddSeparator("");
                    options.AddItem(new GUIContent("Delete All"), false, DeleteAll);
                    options.DropDown(optionsRect);
                }
            }
            EditorGUILayout.EndHorizontal();
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

            if (GUILayout.Button("Save"))
            {
                Manager.Save();
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
                    key = key,
                    value = Manager.GetString(key)
                };
                dataList.Add(localData);
            }
        }

        private void DrawData(PersistentData data)
        {
            EditorGUILayout.BeginHorizontal();
            data.value = EditorGUILayout.TextField(data.key, data.value);
            if (GUILayout.Button("Set"))
            {
                Manager.SetData(data.key, data.value);
            }

            if (GUILayout.Button("Delete"))
            {
                Manager.DeleteKey(data.key);
                dataList.Remove(data);
            }

            EditorGUILayout.EndHorizontal();
        }

        private void NewData()
        {
            newData.key = default;
            newData.value = default;
            isShowNewDataBox = true;
        }

        private void DrawNewData()
        {
            EditorGUILayout.BeginHorizontal();
            newData.key = EditorGUILayout.TextField(newData.key);
            newData.value = EditorGUILayout.TextField(newData.value);
            if (GUILayout.Button("Add"))
            {
                bool isError = false;
                if (string.IsNullOrEmpty(newData.key))
                {
                    Debug.LogError("Key is invalid");
                    isError = true;
                }

                if (!isError && string.IsNullOrEmpty(newData.value))
                {
                    Debug.LogError("Value is invalid");
                    isError = true;
                }

                if (!isError && Manager.HasKey(newData.key))
                {
                    Debug.LogError($"Key {newData.key} is already exist");
                    isError = true;
                }

                if (!isError)
                {
                    Manager.SetData(newData.key, newData.value);
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
            public string key;
            public string value;
        }
    }
}