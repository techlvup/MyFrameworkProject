using System.Collections.Generic;
using Excel;
using System.IO;
using System.Text;
using UnityEngine;

public class ExcelUtility
{
    private IExcelDataReader excelReader;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="excelFile">Excel file.</param>
    public ExcelUtility(string excelFile)
    {
        FileStream stream = File.Open(excelFile, FileMode.Open, FileAccess.Read);
        excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);
    }

    public void ExcelConvert(string excelPath, Encoding encoding, int indexOfFormat)
    {
        //判断Excel文件中是否存在至少一张数据表
        if (excelReader.ResultsCount < 1)
            return;

        List<string> savePaths = new List<string>();

        List<StringBuilder> stringBuilders = new List<StringBuilder>();

        if (indexOfFormat == 0)
        {
            ConvertToLua(ref stringBuilders, ref savePaths, excelPath);
        }

        //写入文件
        for (int i = 0; i < excelReader.ResultsCount; i++)
        {
            using (FileStream fileStream = new FileStream(savePaths[i], FileMode.Create, FileAccess.Write))
            {
                using (TextWriter textWriter = new StreamWriter(fileStream, encoding))
                {
                    textWriter.Write(stringBuilders[i].ToString());
                }
            }
        }
    }

    private void ConvertToLua(ref List<StringBuilder> stringBuilders, ref List<string> savePaths, string excelPath)
    {
        int h = 1;

        List<string> names = new List<string>();

        do
        {
            h = 1;

            names.Clear();

            string path = Application.dataPath + "/Lua/ConfigData/" + excelReader.Name + ".lua";
            savePaths.Add(path);

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

                    if (h == 1)
                    {
                        names.Add(value);
                    }
                    else
                    {
                        if (h > 2 && i == 0)
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
                            stringBuilder.Append(names[i]);
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

                h++;
            }

            stringBuilder.Append("\n}\n\nreturn ");
            stringBuilder.Append(excelReader.Name);

            stringBuilders.Add(stringBuilder);

        } while (excelReader.NextResult()/*下一张表*/);
    }
}