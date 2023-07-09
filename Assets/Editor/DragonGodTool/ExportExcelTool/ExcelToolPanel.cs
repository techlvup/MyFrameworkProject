using Excel;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.IO;
using System.IO.Compression;
using Zstandard.Net;

public class ExcelToolPanel
{
    [MenuItem("DragonGodTool/导出Excel表的配置数据")]
    public static void ExportExcelDataToLuaTableString()
    {
        string configDataPath = Application.dataPath.Substring(0, Application.dataPath.LastIndexOf("/") + 1) + "ConfigData";

        DirectoryInfo directoryInfo = new DirectoryInfo(configDataPath);

        FileInfo[] fileInfos = directoryInfo.GetFiles();

        foreach (var item in fileInfos)
        {
            string excelPath = item.FullName.Replace("\\", "/");

            using (FileStream fileStream = new FileStream(excelPath, FileMode.Open))
            {
                IExcelDataReader excelReader = ExcelReaderFactory.CreateOpenXmlReader(fileStream);

                //判断Excel文件中是否存在至少一张数据表
                if (excelReader.ResultsCount > 0)
                {
                    ExcelConvert(excelReader);
                }
            }
        }

        EditorUtility.ClearProgressBar();

        AssetDatabase.Refresh();
    }

    private static void ExcelConvert(IExcelDataReader excelReader)
    {
        List<string> luaTableKeys = new List<string>();

        do
        {
            luaTableKeys.Clear();

            int rowCount = 0;

            using (DataTable dataTable = new DataTable())
            {
                dataTable.Load(excelReader);
                rowCount = dataTable.Rows.Count;
            }

            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("local ");
            stringBuilder.Append(excelReader.Name);
            stringBuilder.Append(" = {");

            while (excelReader.Read()/*下一行*/)
            {
                //遍历每一列
                for (int i = 0; i < excelReader.FieldCount; i++)
                {
                    string value = excelReader.IsDBNull(i) ? "" : excelReader.GetString(i);

                    if (excelReader.Depth == 1)
                    {
                        luaTableKeys.Add(value);
                    }
                    else
                    {
                        if (excelReader.Depth > 2 && i == 0)
                        {
                            stringBuilder.Append("\n");
                        }

                        if (i == 0)
                        {
                            stringBuilder.Append("\r\n\t[\"");
                            stringBuilder.Append(value);
                            stringBuilder.Append("\"]");
                            stringBuilder.Append(" = {");
                        }
                        else
                        {
                            stringBuilder.Append("\r\n\t\t[\"");
                            stringBuilder.Append(luaTableKeys[i]);
                            stringBuilder.Append("\"] = \"");
                            stringBuilder.Append(value);
                            stringBuilder.Append("\",");
                        }

                        if (i == excelReader.FieldCount - 1)
                        {
                            stringBuilder.Append("\r\n\t},");
                        }
                    }
                }

                EditorUtility.DisplayProgressBar("配置表" + excelReader.Name + "正在导出数据中", "导出进度", excelReader.Depth * 1.0f / rowCount);
            }

            stringBuilder.Append("\n}\n\nreturn ");
            stringBuilder.Append(excelReader.Name);

            using (FileStream fileStream = new FileStream(Application.streamingAssetsPath + "/ConfigData/" + excelReader.Name + "Data.bin", FileMode.Create))
            {
                using (ZstandardStream zstdStream = new ZstandardStream(fileStream, CompressionMode.Compress))
                {
                    byte[] byteData = Encoding.UTF8.GetBytes(stringBuilder.ToString());
                    zstdStream.Write(byteData, 0, byteData.Length);
                }
            }
        }
        while (excelReader.NextResult()/*下一张表*/);
    }
}