using System;
using UnityEngine;
using System.Data;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using Object = UnityEngine.Object;

namespace GameFramework
{
    internal class DataTableWindow : SubWindow
    {
        private List<ExcelData> excelDataList = new List<ExcelData>();
        private List<ExcelData> lastDataList = new List<ExcelData>();
        private ExportOperation operation;
        private bool allSelected;
        private bool showSettings;
        private Editor settingEditor;

        private const string ShowSettingsKey = "DataTableWindow.ShowSettings";

        public override void Init(string name, GameWindow parent)
        {
            base.Init("DataTable", parent);
            settingEditor = Editor.CreateEditor(DataTableSetting.Instance);
            showSettings = EditorPrefs.GetBool(ShowSettingsKey, showSettings);
            CollectAllExcels();
            SelectAllExcels(true);
        }

        public override void OnGUI()
        {
            if (settingEditor.target != null)
            {
                settingEditor.OnInspectorGUI();
            }

            operation = (ExportOperation) EditorGUILayout.EnumPopup("Export Operation", operation);

            for (int i = 0; i < excelDataList.Count; i++)
            {
                ExcelData excelData = excelDataList[i];
                
                EditorGUILayout.BeginHorizontal();
                excelData.Selected = EditorGUILayout.Toggle(excelData.Name, excelData.Selected);
                EditorGUILayout.EndHorizontal();
                excelDataList[i] = excelData;
            }

            EditorGUILayout.BeginHorizontal();

            string selectButtonName = allSelected ? "Unselect All" : "Select All";
            if (GUILayout.Button(selectButtonName))
            {
                SelectAllExcels(!allSelected);
            }

            if (GUILayout.Button("Refresh"))
            {
                CollectAllExcels();
            }

            if (GUILayout.Button("Build"))
            {
                switch (operation)
                {
                    case ExportOperation.ExcelToTable:
                        ExcelToTable(true);
                        break;
                    case ExportOperation.ExcelToScript:
                        ExcelToScript();
                        break;
                    default:
                        ExcelToTable(true);
                        ExcelToScript();
                        break;
                }
            }

            EditorGUILayout.EndHorizontal();
        }

        public override void OnDestroy()
        {
            Object.DestroyImmediate(settingEditor);
        }

        private void CollectAllExcels()
        {
            DataTableSetting setting = DataTableSetting.Instance;
            lastDataList.Clear();
            lastDataList.AddRange(excelDataList);
            excelDataList.Clear();
            string fullPath = Path.GetFullPath(setting.ExcelRootPath);
            if (!Directory.Exists(fullPath))
            {
                Directory.CreateDirectory(fullPath);
            }

            DirectoryInfo directoryInfo = new DirectoryInfo(fullPath);
            FileInfo[] filesInfo = directoryInfo.GetFiles("*.xlsx", SearchOption.AllDirectories);
            foreach (FileInfo fileInfo in filesInfo)
            {
                if (fileInfo.Name.StartsWith("~"))
                {
                    continue;
                }

                ExcelData excelData = new ExcelData
                {
                    Name = fileInfo.Name,
                    FullName = fileInfo.FullName,
                    Selected = true
                };

                if (!lastDataList.Contains(excelData))
                {
                    excelData.Selected = false;
                }

                excelDataList.Add(excelData);
            }
        }

        private void SelectAllExcels(bool selected)
        {
            allSelected = selected;
            for (int i = 0; i < excelDataList.Count; i++)
            {
                ExcelData excelData = excelDataList[i];
                excelData.Selected = selected;
                excelDataList[i] = excelData;
            }
        }

        private void ExcelToTable(bool check)
        {
            try
            {
                for (int i = 0; i < excelDataList.Count; i++)
                {
                    ExcelData excelData = excelDataList[i];
                    if (!excelData.Selected)
                    {
                        continue;
                    }

                    FileInfo fileInfo = new FileInfo(excelData.FullName);
                    if (fileInfo.Name.StartsWith("~"))
                    {
                        continue;
                    }

                    DataTableCollection dataTables = ExcelReadEditor.Default.ReadExcel(fileInfo.FullName);
                    if (dataTables.Count <= 0)
                    {
                        Debug.LogError($"table {fileInfo.FullName} count is zero");
                        continue;
                    }

                    if (dataTables.Count == 1)
                    {
                        if (check && !ExcelCheckEditor.Default.CheckExcel(dataTables[0], fileInfo.Name))
                        {
                            return;
                        }

                        ExcelToTableEditor.Default.ExcelToBinary(dataTables[0], fileInfo.Name.Replace(fileInfo.Extension, ".txt"));
                    }
                    else if (dataTables.Count > 1)
                    {
                        for (int j = 0; j < dataTables.Count; ++j)
                        {
                            if (check && !ExcelCheckEditor.Default.CheckExcel(dataTables[j], fileInfo.Name))
                            {
                                return;
                            }

                            ExcelToTableEditor.Default.ExcelToBinary(dataTables[j], string.Concat(dataTables[j].TableName, ".txt"));
                        }
                    }

                    EditorUtility.DisplayProgressBar("ExcelToTable", fileInfo.Name, i / (float) fileInfo.Length);
                }
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }

            EditorUtility.ClearProgressBar();
            AssetDatabase.Refresh();
        }

        private void ExcelToScript()
        {
            try
            {
                for (int i = 0; i < excelDataList.Count; i++)
                {
                    ExcelData excelData = excelDataList[i];
                    if (!excelData.Selected)
                    {
                        continue;
                    }

                    FileInfo fileInfo = new FileInfo(excelData.FullName);
                    if (fileInfo.Name.StartsWith("~"))
                    {
                        continue;
                    }

                    DataTableCollection dataTables = ExcelReadEditor.Default.ReadExcel(fileInfo.FullName);
                    if (dataTables.Count <= 0)
                    {
                        Debug.LogError($"table {fileInfo.FullName} count is zero");
                        continue;
                    }

                    if (dataTables.Count == 1)
                    {
                        ExcelToScriptEditor.Default.ExcelToCs(dataTables[0], fileInfo.Name.Replace(fileInfo.Extension, ".cs"));
                    }
                    else if (dataTables.Count > 1)
                    {
                        for (int j = 0; j < dataTables.Count; ++j)
                        {
                            ExcelToScriptEditor.Default.ExcelToCs(dataTables[j], string.Concat(dataTables[j].TableName, ".cs"));
                        }
                    }

                    EditorUtility.DisplayProgressBar("ExcelToScript", fileInfo.Name, i / (float) fileInfo.Length);
                }
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }

            EditorUtility.ClearProgressBar();
            AssetDatabase.Refresh();
        }
        
        private enum ExportOperation
        {
            All,
            ExcelToTable,
            ExcelToScript
        }

        private struct ExcelData
        {
            public string Name;
            public string FullName;
            public bool Selected;
        }
    }
}