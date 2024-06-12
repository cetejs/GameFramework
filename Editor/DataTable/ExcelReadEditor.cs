using System.IO;
using System.Data;
using Excel;

namespace GameFramework
{
    internal static  class ExcelReadEditor
    {
        public static DataTableCollection ReadExcel(string path)
        {
            using (FileStream stream = File.OpenRead(path))
            {
                using (IExcelDataReader reader = ExcelReaderFactory.CreateOpenXmlReader(stream))
                {
                    return reader.AsDataSet().Tables;
                }
            }
        }
    }
}