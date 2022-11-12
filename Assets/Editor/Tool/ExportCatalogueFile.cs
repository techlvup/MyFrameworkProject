using UnityEditor;
using System.Security.Cryptography;
using System.Text;
using System.IO;

public static class ExportCatalogueFile
{
    private static string m_luaPath = "Assets/Lua";

    private static string m_catalogueFilePath_Windows = "HotUpdateFiles/Windows/CatalogueFile.txt";
    private static string m_catalogueFilePath_MacOS = "HotUpdateFiles/MacOS/CatalogueFile.txt";
    private static string m_catalogueFilePath_Android = "HotUpdateFiles/Android/CatalogueFile.txt";
    private static string m_catalogueFilePath_IOS = "HotUpdateFiles/IOS/CatalogueFile.txt";

    private static StringBuilder m_filesContent = null;

    [MenuItem("ExportTool/BuildCatalogueFile/BuildCatalogueFile_Windows")]
    public static void BuildCatalogueFile_Windows()
    {
        CreeateFiles(m_catalogueFilePath_Windows);
    }

    [MenuItem("ExportTool/BuildCatalogueFile/BuildCatalogueFile_MacOS")]
    public static void BuildCatalogueFile_MacOS()
    {
        CreeateFiles(m_catalogueFilePath_MacOS);
    }

    [MenuItem("ExportTool/BuildCatalogueFile/BuildCatalogueFile_Android")]
    public static void BuildCatalogueFile_Android()
    {
        CreeateFiles(m_catalogueFilePath_Android);
    }

    [MenuItem("ExportTool/BuildCatalogueFile/BuildCatalogueFile_IOS")]
    public static void BuildCatalogueFile_IOS()
    {
        CreeateFiles(m_catalogueFilePath_IOS);
    }

    private static void CreeateFiles(string assetBundlesPath)
    {
        using (FileStream fs = new FileStream(assetBundlesPath, FileMode.Create))
        {
            using (StreamWriter sw = new StreamWriter(fs))
            {
                SetMd5Files(m_luaPath);
                SetMd5Files(assetBundlesPath.Substring(0, assetBundlesPath.Length - 17) + "AssetBundles");

                if (m_filesContent != null)
                {
                    sw.Write(m_filesContent.ToString());
                }

                m_filesContent = null;
            }
        }
    }

    private static void SetMd5Files(string directoryPath)
    {
        DirectoryInfo folder = new DirectoryInfo(directoryPath);

        //遍历文件
        foreach (FileInfo nextFile in folder.GetFiles())
        {
            if (nextFile.Name.Length >= 6 && nextFile.Name.Substring(nextFile.Name.Length - 5, 5) == ".meta")
            {
                goto A;
            }

            if (m_filesContent == null)
            {
                m_filesContent = new StringBuilder();
                m_filesContent.Append(directoryPath + "/" + nextFile.Name + "|" + Get32MD5(nextFile.OpenText().ReadToEnd()));
            }
            else
            {
                m_filesContent.Append("\n" + directoryPath + "/" + nextFile.Name + "|" + Get32MD5(nextFile.OpenText().ReadToEnd()));
            }

        A:;
        }

        //遍历文件夹
        foreach (DirectoryInfo nextFolder in folder.GetDirectories())
        {
            if (nextFolder.Name == ".idea")
            {
                goto A;
            }

            SetMd5Files(directoryPath + "/" + nextFolder.Name);

        A:;
        }
    }

    private static string Get32MD5(string content)
    {
        MD5 md5 = MD5.Create();

        StringBuilder stringBuilder = new StringBuilder();

        byte[] bytes = md5.ComputeHash(Encoding.UTF8.GetBytes(content)); //该方法的参数也可以传入Stream

        for (int i = 0; i < bytes.Length; i++)
        {
            stringBuilder.Append(bytes[i].ToString("X2"));
        }

        string md5Str = stringBuilder.ToString();

        return md5Str;
    }
}