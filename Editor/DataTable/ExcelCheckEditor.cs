using System.Data;
using System.Collections.Generic;
using System.IO;

namespace GameFramework
{
    internal sealed class ExcelCheckEditor
    {
        public static readonly ExcelCheckEditor Default = new ExcelCheckEditor();

        public bool CheckExcel(DataTable dataTable, string fileName)
        {
            List<int> uniqueList = new List<int>();
            List<int> linkList = new List<int>();
            for (int i = 0; i < dataTable.Columns.Count; i++)
            {
                string role = dataTable.Rows[3][i].ToString();
                if (role == "unique")
                {
                    uniqueList.Add(i);
                }
                else if (role.StartsWith("&"))
                {
                    linkList.Add(i);
                }
            }

            return CheckUnique(dataTable, uniqueList, fileName) && CheckLink(dataTable, linkList, fileName);
        }

        private bool CheckUnique(DataTable dataTable, List<int> columns, string fileName)
        {
            HashSet<string> uniqueDataSet = new HashSet<string>();
            for (int i = 0; i < columns.Count; i++)
            {
                uniqueDataSet.Clear();
                for (int j = 4; j < dataTable.Rows.Count; ++j)
                {
                    string data = dataTable.Rows[j][columns[i]].ToString();
                    if (uniqueDataSet.Contains(data))
                    {
                        GameLogger.LogError($"[unique] Excel {fileName} has data {data} in row {j + 1} column {columns[i] + 1}");
                        return false;
                    }

                    uniqueDataSet.Add(data);
                }
            }

            return true;
        }

        private bool CheckLink(DataTable dataTable, List<int> columns, string fileName)
        {
            HashSet<string> lineTableColumns = new HashSet<string>();
            for (int i = 0; i < columns.Count; i++)
            {
                string linkName = dataTable.Rows[3][columns[i]].ToString();
                string[] linkNameChip = linkName.Split('&', '.');
                DataTableCollection dataTables = ExcelReadEditor.Default.ReadExcel($"{Path.GetFullPath(DataTableSetting.Instance.ExcelRootPath)}/{linkNameChip[1]}.xlsx");
                DataTable linkTable = null;
                string field;
                if (linkNameChip.Length < 4)
                {
                    linkTable = dataTables[0];
                    field = linkNameChip[2];
                }
                else
                {
                    field = linkNameChip[3];
                    for (int j = 0; j < dataTables.Count; ++j)
                    {
                        if (dataTables[j].TableName == linkNameChip[2])
                        {
                            linkTable = dataTables[j];
                        }
                    }
                }

                if (linkTable == null)
                {
                    GameLogger.LogError($"[link] Excel {fileName} has link name {linkName} is invalid");
                    return false;
                }

                GetDataColumns(linkTable, field, lineTableColumns);
                if (lineTableColumns.Count <= 0)
                {
                    GameLogger.LogError($"[link] Excel {fileName} link excel {linkName.Split('.')[0]} not found field {field}");
                }

                for (int j = 4; j < dataTable.Rows.Count; ++j)
                {
                    string data = dataTable.Rows[j][columns[i]].ToString();
                    if (!lineTableColumns.Contains(data))
                    {
                        GameLogger.LogError($"[link] Excel {fileName} link excel {linkName.Split('.')[0]} has field {field} not exist data {data}");
                        return false;
                    }
                }
            }

            return true;
        }

        private void GetDataColumns(DataTable dataTable, string field, HashSet<string> dataColumns)
        {
            dataColumns.Clear();
            for (int i = 0; i < dataTable.Columns.Count; i++)
            {
                if (dataTable.Rows[0][i].ToString() == field)
                {
                    for (int j = 4; j < dataTable.Rows.Count; ++j)
                    {
                        dataColumns.Add(dataTable.Rows[j][i].ToString());
                    }

                    break;
                }
            }
        }
    }
}