using UnityEngine;
using UnityEditor;
using System.Text;
using System.IO;

public class ExcelToolPanel : EditorWindow
{
    private static ExcelToolPanel currWindow; //当前编辑器窗口实例

    private static string configDataPath; //项目根路径	

    private static readonly string[] formatOption = new string[] { "Lua" }; //输出格式选项

    private static int indexOfFormat = 0; //输出格式索引

    private static readonly string[] encodingOption = new string[] { "UTF-8", "GB2312" }; //编码格式选项

    private static int indexOfEncoding = 0; //编码索引



    [MenuItem("DragonGodTool/Excel表导出工具")]
    public static void ShowExcelConverTool()
    {
        configDataPath = Application.dataPath.Substring(0, Application.dataPath.LastIndexOf("/") + 1) + "ConfigData";

        currWindow = GetWindow<ExcelToolPanel>();

        currWindow.Show();
    }

    private void OnGUI()
    {
        GUILayout.BeginHorizontal();

        EditorGUILayout.LabelField("请选择格式类型:", GUILayout.Width(85));

        indexOfFormat = EditorGUILayout.Popup(indexOfFormat, formatOption, GUILayout.Width(125));//绘制下拉选项

        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();

        EditorGUILayout.LabelField("请选择编码类型:", GUILayout.Width(85));

        indexOfEncoding = EditorGUILayout.Popup(indexOfEncoding, encodingOption, GUILayout.Width(125));

        GUILayout.EndHorizontal();

        if (GUILayout.Button("转换"))
        {
            Convert();
        }
    }



    private static void Convert()
    {
        DirectoryInfo directoryInfo = new DirectoryInfo(configDataPath);

        FileInfo[] fileInfos = directoryInfo.GetFiles();

        foreach (var item in fileInfos)
        {
            string excelPath = item.FullName.Replace("\\", "/");

            //构造Excel工具类
            ExcelUtility excelUtility = new ExcelUtility(excelPath);

            //判断编码类型
            Encoding encoding = null;

            if (indexOfEncoding == 0 || indexOfEncoding == 3)
            {
                encoding = Encoding.GetEncoding("utf-8");
            }
            else if (indexOfEncoding == 1)
            {
                encoding = Encoding.GetEncoding("gb2312");
            }

            //根据选项转化类型
            if (indexOfFormat == 0)
            {
                excelUtility.ExcelConvert(excelPath, encoding, indexOfFormat);
            }
        }

        //刷新本地资源
        AssetDatabase.Refresh();

        //为了解决窗口再次点击时路径错误的Bug所以完成后关闭窗口
        currWindow.Close();
    }
}