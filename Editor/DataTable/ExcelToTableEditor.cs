using System.Data;
using System.IO;
using System.Text;

namespace GameFramework
{
    internal sealed class ExcelToTableEditor
    {
        public static readonly ExcelToTableEditor Default = new ExcelToTableEditor();

        public void ExcelToCsv(DataTable dataTable, string fileName)
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
            DataTableSetting setting = DataTableSetting.Instance;
            string fullPath = Path.Combine(setting.OutputTablePath, fileName);
            if (setting.EncryptionType == EncryptionType.AES)
            {
                FileUtils.WriteAllBytes(fullPath, EncryptionUtils.AES.EncryptToBytes(sb.ToString(), setting.Password));
            }
            else
            {
                FileUtils.WriteAllText(fullPath, sb.ToString());
            }
        }
    }
}