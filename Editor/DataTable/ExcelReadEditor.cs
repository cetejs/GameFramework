﻿using System.IO;
using System.Data;
using Excel;

namespace GameFramework
{
    internal class ExcelReadEditor
    {
        public static readonly ExcelReadEditor Default = new ExcelReadEditor();

        public DataTableCollection ReadExcel(string path)
        {
            using FileStream stream = File.OpenRead(path);
            IExcelDataReader reader = ExcelReaderFactory.CreateOpenXmlReader(stream);
            DataSet dataSet = reader.AsDataSet();
            return dataSet.Tables;
        }
    }
}