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
        private int selectIndex;
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

            DrawChangeLanguage();
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
                        ExcelToLocalization();
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

                    if (excelData.Name.RemoveLastOf(".") == DataTableSetting.Instance.LocalizationName)
                    {
                        continue;
                    }

                    FileInfo fileInfo = new FileInfo(excelData.FullName);
                    if (fileInfo.Name.StartsWith("~"))
                    {
                        continue;
                    }

                    DataTableCollection dataTables = ExcelReadEditor.ReadExcel(fileInfo.FullName);
                    if (dataTables.Count <= 0)
                    {
                        GameLogger.LogError($"Excel to table is fail, because table {fileInfo.FullName} count is zero");
                        continue;
                    }

                    if (dataTables.Count == 1)
                    {
                        if (check && !ExcelCheckEditor.Check(dataTables[0], fileInfo.Name))
                        {
                            return;
                        }

                        ExcelToTableEditor.Build(dataTables[0], fileInfo.Name.Replace(fileInfo.Extension, ".bytes"));
                    }
                    else if (dataTables.Count > 1)
                    {
                        for (int j = 0; j < dataTables.Count; ++j)
                        {
                            if (check && !ExcelCheckEditor.Check(dataTables[j], fileInfo.Name))
                            {
                                return;
                            }

                            ExcelToTableEditor.Build(dataTables[j], string.Concat(dataTables[j].TableName, ".bytes"));
                        }
                    }

                    EditorUtility.DisplayProgressBar("ExcelToTable", fileInfo.Name, i / (float) fileInfo.Length);
                }
            }
            catch (Exception ex)
            {
                GameLogger.LogException(ex);
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

                    if (excelData.Name.RemoveLastOf(".") == DataTableSetting.Instance.LocalizationName)
                    {
                        continue;
                    }

                    FileInfo fileInfo = new FileInfo(excelData.FullName);
                    if (fileInfo.Name.StartsWith("~"))
                    {
                        continue;
                    }

                    DataTableCollection dataTables = ExcelReadEditor.ReadExcel(fileInfo.FullName);
                    if (dataTables.Count <= 0)
                    {
                        GameLogger.LogError($"Excel to script is fail, because table {fileInfo.FullName} count is zero");
                        continue;
                    }

                    if (dataTables.Count == 1)
                    {
                        ExcelToScriptEditor.Build(dataTables[0], fileInfo.Name.Replace(fileInfo.Extension, ".cs"));
                    }
                    else if (dataTables.Count > 1)
                    {
                        for (int j = 0; j < dataTables.Count; ++j)
                        {
                            ExcelToScriptEditor.Build(dataTables[j], string.Concat(dataTables[j].TableName, ".cs"));
                        }
                    }

                    EditorUtility.DisplayProgressBar("ExcelToScript", fileInfo.Name, i / (float) fileInfo.Length);
                }
            }
            catch (Exception ex)
            {
                GameLogger.LogException(ex);
            }

            EditorUtility.ClearProgressBar();
            AssetDatabase.Refresh();
        }

        private void ExcelToLocalization()
        {
            for (int i = 0; i < excelDataList.Count; i++)
            {
                ExcelData excelData = excelDataList[i];
                if (!excelData.Selected)
                {
                    continue;
                }

                if (excelData.Name.RemoveLastOf(".") != DataTableSetting.Instance.LocalizationName)
                {
                    continue;
                }

                FileInfo fileInfo = new FileInfo(excelData.FullName);
                if (fileInfo.Name.StartsWith("~"))
                {
                    continue;
                }

                DataTableCollection dataTables = ExcelReadEditor.ReadExcel(fileInfo.FullName);
                if (dataTables.Count <= 0)
                {
                    GameLogger.LogError($"Excel to localization is fail, because table {fileInfo.FullName} count is zero");
                    continue;
                }

                LocalizationWriteEditor.Build(dataTables[0]);
            }
        }

        private void DrawChangeLanguage()
        {
            EditorGUILayout.BeginHorizontal();
            string[] languageTypes = AssetDatabase.GetSubFolders(StringUtils.Concat("Assets/", DataTableSetting.Instance.LoadLocalizationPath));
            bool hasLanguageType = languageTypes.Length > 0;
            if (!hasLanguageType)
            {
                languageTypes = new[]
                {
                    "None"
                };
            }

            selectIndex = Mathf.Clamp(selectIndex, 0, languageTypes.Length - 1);
            for (int i = 0; i < languageTypes.Length; i++)
            {
                languageTypes[i] = languageTypes[i].GetLastOf("/");
            }

            selectIndex = EditorGUILayout.Popup("Change Language", selectIndex, languageTypes);
            if (GUILayout.Button("Load", GUILayout.Width(50f)))
            {
                if (hasLanguageType)
                {
                    LocalizationReadEditor.ChangeLanguage(languageTypes[selectIndex]);
                    LocalizationText[] texts = Object.FindObjectsOfType<LocalizationText>();
                    foreach (LocalizationText text in texts)
                    {
                        string language = LocalizationReadEditor.GetLanguage(text.LanguageKey);
                        text.SetLanguage(language);
                        EditorUtility.SetDirty(text);
                    }
                }
                else
                {
                    GameLogger.LogError("Not selecting the correct language type");
                }
            }

            EditorGUILayout.EndHorizontal();
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