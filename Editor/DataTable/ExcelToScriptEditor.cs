using System;
using System.Data;
using System.IO;
using System.Text;

namespace GameFramework
{
    internal sealed class ExcelToScriptEditor
    {
        public static readonly ExcelToScriptEditor Default = new ExcelToScriptEditor();

        public void ExcelToCs(DataTable dataTable, string fileName)
        {
            StringBuilder stringBuilder = new StringBuilder();
            StringBuilder propertyBuilder = new StringBuilder();
            StringBuilder propertySetBuilder = new StringBuilder();
            StringBuilder toStringBuilder = new StringBuilder();

            DataTableSetting setting = DataTableSetting.Instance;
            string tempTextPath = string.Concat(PathUtils.GetPackageFullPath(), "/Editor/DataTable/TemplateTable.txt");
            string tempTableText = File.ReadAllText(tempTextPath).ReplaceNewline();
            DataRow fieldRow = dataTable.Rows[0];
            // DataRow desRow = dataTable.Rows[1];
            DataRow typeRow = dataTable.Rows[2];
            // DataRow ruleRow = dataTable.Rows[3];
            int propertySetLastCount = 1;
            for (int i = 0; i < dataTable.Columns.Count; i++)
            {
                string field = fieldRow[i].ToString().InitialsToUpper();
                string type = typeRow[i].ToString().ToLower();
                propertyBuilder.AppendPadLeft($"public {type} {field}" + " { get; private set; }\n", 8);
                if (type.EndsWith("[]"))
                {
                    string fileLength = string.Concat(field.InitialsToLower(), "Length");
                    propertySetBuilder.AppendPadLeft($"int {fileLength} = reader.ReadInt32();\n", 12);
                    string argType = type.RemoveLastOf("[]");
                    propertySetBuilder.AppendPadLeft($"{field} = new {argType}[{fileLength}];\n", 12);
                    propertySetBuilder.AppendPadLeft($"for (int i = 0; i < {fileLength}; i++)\n", 12);
                    propertySetBuilder.AppendPadLeft("{\n", 12);
                    propertySetBuilder.AppendPadLeft($"{field}[i] = reader.Read{GetReadType(argType)}();\n", 16);
                    propertySetBuilder.AppendPadLeft("}\n\n", 12);
                    propertySetLastCount = 2;
                }
                else
                {
                    propertySetBuilder.AppendPadLeft($"{field} = reader.Read{GetReadType(type)}();\n", 12);
                    propertySetLastCount = 1;
                }

                if (type.Contains("[]"))
                {
                    toStringBuilder.Append(field + " = {string.Join(\", \", " + field + ")}; ");
                }
                else
                {
                    toStringBuilder.Append(field + " = {" + field + "}; ");
                }
            }

            propertyBuilder.RemoveLastCount();
            toStringBuilder.RemoveLastCount();
            propertySetBuilder.RemoveLastCount(propertySetLastCount);
            stringBuilder.Append(tempTableText);
            stringBuilder.Replace("#Namespace#", setting.ScriptNamespace);
            stringBuilder.Replace("#Name#", Path.GetFileNameWithoutExtension(fileName));
            stringBuilder.Replace("#Field#".PadLeft(15), propertyBuilder.ToString());
            stringBuilder.Replace("#FieldSet#".PadLeft(22), propertySetBuilder.ToString());
            stringBuilder.Replace("#ToString#", toStringBuilder.ToString());
            string fullPath = Path.Combine(setting.OutputScriptPath, fileName);
            FileUtils.WriteAllText(fullPath, stringBuilder.ToString());
        }

        private string GetReadType(string type)
        {
            switch (type)
            {
                case "bool":
                    return "Boolean";
                case "byte":
                    return "Byte";
                case "sbyte":
                    return "SByte";
                case "char":
                    return "Char";
                case "double":
                    return "Double";
                case "decimal":
                    return "Decimal";
                case "short":
                    return "Int16";
                case "ushort":
                    return "UInt16";
                case "int":
                    return "Int32";
                case "uint":
                    return "UInt32";
                case "long":
                    return "Int64";
                case "ulong":
                    return "UInt64";
                case "float":
                    return "Single";
                case "string":
                    return "String";
                default:
                    throw new NotSupportedException($"{type} is not support");
            }
        }
    }
}