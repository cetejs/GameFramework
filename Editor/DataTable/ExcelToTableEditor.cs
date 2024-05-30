using System;
using System.Data;
using System.IO;
using System.Text;

namespace GameFramework
{
    internal sealed class ExcelToTableEditor
    {
        public static readonly ExcelToTableEditor Default = new ExcelToTableEditor();

        public void ExcelToBinary(DataTable dataTable, string fileName)
        {
            if (dataTable.Rows.Count < 4)
            {
                return;
            }

            DataTableSetting setting = DataTableSetting.Instance;
            string fullPath = Path.Combine(setting.OutputTablePath, fileName);
            DataRow typeRow = dataTable.Rows[2];
            using (MemoryStream stream = new MemoryStream())
            {
                using (BinaryWriter writer = new BinaryWriter(stream, Encoding.UTF8))
                {
                    int count = dataTable.Rows.Count - 4;
                    writer.Write(count);
                    byte[] offsets = new byte[count * 8];
                    writer.Seek(offsets.Length, SeekOrigin.Current);
                    for (int i = 4; i < dataTable.Rows.Count; i++)
                    {
                        byte[] offset = BitConverter.GetBytes(stream.Position);
                        Array.Copy(offset, 0, offsets, (i - 4) * 8, offset.Length);
                        for (int j = 0; j < dataTable.Columns.Count; j++)
                        {
                            string data = dataTable.Rows[i][j].ToString();
                            string type = typeRow[j].ToString();
                            Write(writer, type, data);
                        }
                    }

                    writer.Seek(4, SeekOrigin.Begin);
                    writer.Write(offsets);
                }

                if (setting.CryptoType == CryptoType.AES)
                {
                    FileUtils.WriteAllBytes(fullPath, CryptoUtils.Aes.EncryptBytesToBytes(stream.GetBuffer(), setting.Password));
                }
                else
                {
                    FileUtils.WriteAllBytes(fullPath, stream.GetBuffer());
                }
            }
        }

        private void Write(BinaryWriter writer, string type, string value)
        {
            if (type.EndsWith("[]"))
            {
                string argType = type.Replace("[]", "");
                string[] contents = value.Split('|', StringSplitOptions.RemoveEmptyEntries);
                writer.Write(contents.Length);
                foreach (string content in contents)
                {
                    Write(writer, argType, content);
                }
            }
            else
            {
                switch (type.ToLower())
                {
                    case "bool":
                        writer.Write(byte.Parse(value) > 0);
                        break;
                    case "byte":
                        writer.Write(byte.Parse(value));
                        break;
                    case "sbyte":
                        writer.Write(sbyte.Parse(value));
                        break;
                    case "char":
                        writer.Write(char.Parse(value));
                        break;
                    case "double":
                        writer.Write(double.Parse(value));
                        break;
                    case "decimal":
                        writer.Write(decimal.Parse(value));
                        break;
                    case "short":
                        writer.Write(short.Parse(value));
                        break;
                    case "ushort":
                        writer.Write(ushort.Parse(value));
                        break;
                    case "int":
                        writer.Write(int.Parse(value));
                        break;
                    case "uint":
                        writer.Write(uint.Parse(value));
                        break;
                    case "long":
                        writer.Write(long.Parse(value));
                        break;
                    case "ulong":
                        writer.Write(ulong.Parse(value));
                        break;
                    case "float":
                        writer.Write(float.Parse(value));
                        break;
                    case "string":
                        writer.Write(value);
                        break;
                    default:
                        throw new NotSupportedException($"{type} is not support");
                }
            }
        }
    }
}