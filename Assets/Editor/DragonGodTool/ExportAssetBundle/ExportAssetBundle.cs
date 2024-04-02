using System.IO;
using UnityEditor;
using UnityEngine;



public class ExportAssetBundle
{
    private static string m_rootPath = Application.streamingAssetsPath.Substring(0, Application.streamingAssetsPath.LastIndexOf("/") + 16);
    private static string m_luaPath = Application.dataPath.Substring(0, Application.streamingAssetsPath.LastIndexOf("/")) + "/Lua";



    [MenuItem("GodDragonTool/AssetBundles/打包AssetBundles/BuildAssetBundles_Windows")]
    public static void BuildAssetBundles_Windows()
    {
        string dir = m_rootPath + "/AssetBundles/Windows";

        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }

        SetAssetBundles1();

        BuildPipeline.BuildAssetBundles(dir, BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows64); //把项目所有资源打包成AssetBundle文件

        SetAssetBundles2();
    }

    [MenuItem("GodDragonTool/AssetBundles/打包AssetBundles/BuildAssetBundles_Android")]
    public static void BuildAssetBundles_Android()
    {
        string dir = m_rootPath + "/AssetBundles/Android";

        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }

        SetAssetBundles1();

        BuildPipeline.BuildAssetBundles(dir, BuildAssetBundleOptions.None, BuildTarget.Android); //把项目所有资源打包成AssetBundle文件

        SetAssetBundles2();
    }

    //设置Lua脚本的AssetBundles信息
    public static void SetAssetBundles1()
    {
        DirectoryInfo rootDirectory = new DirectoryInfo(m_luaPath);
        SetAssetBundles(rootDirectory, 1);
        AssetDatabase.Refresh();
    }

    //还原Lua脚本文件
    public static void SetAssetBundles2()
    {
        DirectoryInfo rootDirectory = new DirectoryInfo(m_luaPath);
        SetAssetBundles(rootDirectory, 2);
        AssetDatabase.Refresh();
    }

    public static void SetAssetBundles(DirectoryInfo directoryInfo, int type)
    {
        DirectoryInfo[] directoryInfos = directoryInfo.GetDirectories();
        FileInfo[] fileInfos = directoryInfo.GetFiles();

        if (directoryInfos.Length > 0)
        {
            for (int i = 0; i < directoryInfos.Length; i++)
            {
                DirectoryInfo directoryInfo2 = directoryInfos[i];
                SetAssetBundles(directoryInfo2, type);
            }
        }

        if (fileInfos.Length > 0)
        {
            SetAssetBundles(fileInfos, type);
        }
    }

    public static void SetAssetBundles(FileInfo[] fileInfos, int type)
    {
        for (int i = 0; i < fileInfos.Length; i++)
        {
            FileInfo fileInfos2 = fileInfos[i];

            string extension = Path.GetExtension(fileInfos2.Name);
            string path = fileInfos2.FullName.Replace("\\", "/");

            if (type == 1 && extension == ".lua")
            {
                File.Move(path, path + ".bytes");

                AssetDatabase.Refresh();

                path = path.Replace(Application.dataPath, "Assets");
                string name = Path.GetFileNameWithoutExtension(fileInfos2.Name);

                AssetImporter materialImporter = AssetImporter.GetAtPath(path + ".bytes");

                string assetBundleName = path.Replace("Assets/Lua/", "lua/");
                assetBundleName = assetBundleName.Replace(".lua", "");

                materialImporter.assetBundleName = assetBundleName;
                materialImporter.assetBundleVariant = "lua_ab";

                EditorUtility.SetDirty(materialImporter);

                materialImporter.SaveAndReimport();
            }
            else if (type == 2 && extension == ".bytes")
            {
                File.Move(path, path.Replace(".bytes", ""));
            }
        }
    }
}