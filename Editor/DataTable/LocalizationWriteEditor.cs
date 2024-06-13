using System.Data;
using System.IO;
using System.Text;
using UnityEditor;

namespace GameFramework
{
    internal static class LocalizationWriteEditor
    {
        public static void Build(DataTable dataTable)
        {
            if (dataTable.Rows.Count < 3 || dataTable.Columns.Count < 2)
            {
                return;
            }

            DataTableSetting setting = DataTableSetting.Instance;

            using (MemoryStream stream = new MemoryStream())
            {
                using (BinaryWriter writer = new BinaryWriter(stream, Encoding.UTF8))
                {
                    writer.Write(dataTable.Rows.Count - 2);
                    for (int i = 2; i < dataTable.Rows.Count; i++)
                    {
                        writer.Write(dataTable.Rows[i][0].ToString());
                    }
                }

                string fullPath = Path.Combine(setting.OutputTablePath, setting.LocalizationName, "LanguageKey.bytes");
                if (setting.CryptoType == CryptoType.AES)
                {
                    FileUtils.WriteAllBytes(fullPath, CryptoUtils.Aes.EncryptBytesToBytes(stream.ToArray(), setting.Password));
                }
                else
                {
                    FileUtils.WriteAllBytes(fullPath, stream.ToArray());
                }
            }

            for (int j = 1; j < dataTable.Columns.Count; j++)
            {
                using (MemoryStream stream = new MemoryStream())
                {
                    using (BinaryWriter writer = new BinaryWriter(stream, Encoding.UTF8))
                    {
                        writer.Write(dataTable.Rows.Count - 2);
                        for (int i = 2; i < dataTable.Rows.Count; i++)
                        {
                            writer.Write(dataTable.Rows[i][j].ToString());
                        }
                    }

                    string fullPath = Path.Combine(setting.OutputTablePath, setting.LocalizationName, dataTable.Rows[0][j].ToString(), "Language.bytes");
                    if (setting.CryptoType == CryptoType.AES)
                    {
                        FileUtils.WriteAllBytes(fullPath, CryptoUtils.Aes.EncryptBytesToBytes(stream.ToArray(), setting.Password));
                    }
                    else
                    {
                        FileUtils.WriteAllBytes(fullPath, stream.ToArray());
                    }
                }
            }

            LocalizationReadEditor.Reset();
            AssetDatabase.Refresh();
        }
    }
}