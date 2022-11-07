using System.Data;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using GameFramework.DataTableService;
using GameFramework.Utils;

namespace GameFramework
{
    internal sealed class ExcelToTableEditor
    {
        public static readonly ExcelToTableEditor Default = new ExcelToTableEditor();

        public async void ExcelToCsv(DataTable dataTable, string fileName)
        {
            await Task.Run(() =>
            {
                StringBuilder sb = new StringBuilder();
                for (int i = 4; i < dataTable.Rows.Count; i++)
                {
                    for (int j = 0; j < dataTable.Columns.Count; ++j)
                    {
                        string data = dataTable.Rows[i][j].ToString();
                        if (!string.IsNullOrEmpty(data))
                        {
                            sb.Append(data);
                            sb.Append(",");
                        }
                    }

                    sb.RemoveLastCount();
                    sb.Append("\n");
                }

                sb.RemoveLastCount();
                DataTableConfig config = DataTableConfig.Get();
                string fullPath = Path.Combine(Path.GetFullPath(config.tableBuildPath), fileName);
                EditorFileUtils.CheckDirectory(fullPath);
                File.WriteAllText(fullPath, sb.ToString());
            });
        }
    }
}