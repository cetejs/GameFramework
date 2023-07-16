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
            DataRow primitiveRow = dataTable.Rows[2];
            // DataRow ruleRow = dataTable.Rows[3];
            string propertyTab = GetPreLineTab(tempTableText, "#Field#");
            string propertySetTab = GetPreLineTab(tempTableText, "#FieldSet#");
            int propertySetLastCount = 1;
            for (int i = 0; i < dataTable.Columns.Count; i++)
            {
                string field = fieldRow[i].ToString().InitialsToUpper();
                string primitive = primitiveRow[i].ToString().ToLower();
                propertyBuilder.Append($"{propertyTab}public {primitive} {field}" + " { get; private set; }\n");
                if (primitive.Contains("[]"))
                {
                    string fileContent = string.Concat(field.InitialsToLower(), "Array");
                    string primitiveContent = primitive.RemoveLastOf("[]");
                    propertySetBuilder.Append($"{propertySetTab}string[] {fileContent} = columns[{i}].Split('|');\n");
                    propertySetBuilder.Append($"{propertySetTab}{field} = new {primitiveContent}[{fileContent}.Length];\n");
                    propertySetBuilder.Append($"{propertySetTab}for (int j = 0; j < {fileContent}.Length; j++)\n");
                    propertySetBuilder.Append(propertySetTab + "{\n");
                    if (primitiveContent == "string")
                    {
                        propertySetBuilder.Append($"{propertySetTab}    {field}[j] = {fileContent}[j];\n");
                    }
                    else if (primitiveContent == "bool")
                    {
                        propertySetBuilder.Append($"{propertySetTab}    {field}[j] = byte.Parse({fileContent}[j]) > 0;\n");
                    }
                    else
                    {
                        propertySetBuilder.Append($"{propertySetTab}    {field}[j] = {primitiveContent}.Parse({fileContent}[j]);\n");
                    }

                    propertySetBuilder.Append(propertySetTab + "}\n\n");
                    propertySetLastCount = 2;
                }
                else
                {
                    if (primitive == "string")
                    {
                        propertySetBuilder.Append($"{propertySetTab}{field} = columns[{i}];\n");
                    }
                    else if (primitive == "bool")
                    {
                        propertySetBuilder.Append($"{propertySetTab}{field} = byte.Parse(columns[{i}]) > 0;\n");
                    }
                    else
                    {
                        propertySetBuilder.Append($"{propertySetTab}{field} = {primitive}.Parse(columns[{i}]);\n");
                    }

                    propertySetLastCount = 1;
                }

                if (primitive.Contains("[]"))
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
            stringBuilder.Replace(string.Concat(propertyTab, "#Field#"), propertyBuilder.ToString());
            stringBuilder.Replace(string.Concat(propertySetTab, "#FieldSet#"), propertySetBuilder.ToString());
            stringBuilder.Replace("#ToString#", toStringBuilder.ToString());
            string fullPath = Path.Combine(setting.OutputScriptPath, fileName);
            FileUtils.WriteAllText(fullPath, stringBuilder.ToString());
        }

        private string GetPreLineTab(string text, string content)
        {
            return text.GetFirstOf(content).GetLastOf("\n");
        }
    }
}