using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using GameFramework.DataTableService;
using UnityEngine;
using UnityEditor;

namespace GameFramework
{
    internal sealed class TableBuildWindow : EditorWindow
    {
        private bool isSelectAll;
        private readonly List<ExcelData> excelDataList = new List<ExcelData>();
        private readonly List<ExcelData> lastDataList = new List<ExcelData>();

        [MenuItem("GameFramework/TableWindow")]
        private static void CreatWindow()
        {
            TableBuildWindow window = GetWindow<TableBuildWindow>();
            window.titleContent.text = "TableBuildWindow";
        }

        private void OnEnable()
        {
            titleContent.text = "TableBuildWindow";
            CollectExcel();
            SelectAllExcel(true);
        }

        private void OnGUI()
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
            {
                Rect optionsRect = GUILayoutUtility.GetRect(0, 20, GUILayout.ExpandWidth(false));
                if (GUILayout.Button("Options", EditorStyles.toolbarDropDown, GUILayout.ExpandWidth(false)))
                {
                    GenericMenu options = new GenericMenu();
                    options.AddItem(new GUIContent("Show in Explorer"), false, () =>
                    {
                        EditorUtility.RevealInFinder(Path.GetFullPath(DataTableConfig.Get().excelRootPath));
                    });
                    options.AddItem(new GUIContent("Select All"), false, () =>
                    {
                        SelectAllExcel(!isSelectAll);
                    });
                    options.AddSeparator("");
                    options.AddItem(new GUIContent("Excel To Table"), false, () =>
                    {
                        ExcelToTable(true);
                    });
                    options.AddItem(new GUIContent("Excel To Script"), false, ExcelToScript);
                    options.DropDown(optionsRect);
                }
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginVertical();
            {
                for (int i = 0; i < excelDataList.Count; i++)
                {
                    ExcelData excelData = excelDataList[i];
                    excelData.IsSelect = EditorGUILayout.Toggle(excelData.Name, excelData.IsSelect);
                    excelDataList[i] = excelData;
                }
            }
            EditorGUILayout.EndVertical();
            if (GUILayout.Button("Refresh"))
            {
                CollectExcel();
                SelectAllExcel(true);
            }

            if (GUILayout.Button("Build"))
            {
                ExcelToTable(true);
                ExcelToScript();
            }
        }

        private void CollectExcel()
        {
            DataTableConfig config = DataTableConfig.Get();
            lastDataList.Clear();
            lastDataList.AddRange(excelDataList);
            excelDataList.Clear();
            string fullPath = Path.GetFullPath(config.excelRootPath);
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
                    IsSelect = true
                };

                if (!lastDataList.Contains(excelData))
                {
                    excelData.IsSelect = false;
                }

                excelDataList.Add(excelData);
            }
        }

        private void SelectAllExcel(bool isSelect)
        {
            isSelectAll = isSelect;
            for (int i = 0; i < excelDataList.Count; i++)
            {
                ExcelData excelData = excelDataList[i];
                excelData.IsSelect = isSelect;
                excelDataList[i] = excelData;
            }
        }

        private void ExcelToTable(bool isCheck)
        {
            try
            {
                for (int i = 0; i < excelDataList.Count; i++)
                {
                    ExcelData excelData = excelDataList[i];
                    if (!excelData.IsSelect)
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
                        Debug.LogError($"Table {fileInfo.FullName} count is zero");
                        continue;
                    }

                    if (dataTables.Count == 1)
                    {
                        if(isCheck && !ExcelCheckEditor.Default.CheckExcel(dataTables[0], fileInfo.Name))
                        {
                            return;
                        }

                        ExcelToTableEditor.Default.ExcelToCsv(dataTables[0], fileInfo.Name.Replace(fileInfo.Extension, ".txt"));
                    }
                    else if (dataTables.Count > 1)
                    {
                        for (int j = 0; j < dataTables.Count; ++j)
                        {
                            if(isCheck && !ExcelCheckEditor.Default.CheckExcel(dataTables[j], fileInfo.Name))
                            {
                                return;
                            }

                            ExcelToTableEditor.Default.ExcelToCsv(dataTables[j], string.Concat(dataTables[j].TableName, ".txt"));
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
                    if (!excelData.IsSelect)
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
                        Debug.LogError($"Table {fileInfo.FullName} count is zero");
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

        private struct ExcelData
        {
            public string Name { get; set; }

            public string FullName { get; set; }

            public bool IsSelect { get; set; }
        }
    }
}