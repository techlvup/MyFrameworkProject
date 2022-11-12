using System.IO;
using UnityEditor;

public class ExportAssetBundle
{
    [MenuItem("ExportTool/BuildAssetBundles/BuildAssetBundles_Windows")]
    public static void BuildAssetBundles_Windows()
    {
        string dir = "HotUpdateFiles/Windows/AssetBundles"; //输出路径的起始路径跟Assets文件夹在同一目录下

        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }

        BuildPipeline.BuildAssetBundles(dir, BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows64); //把项目所有资源打包成AssetBundle文件
    }

    [MenuItem("ExportTool/BuildAssetBundles/BuildAssetBundles_MacOS")]
    public static void BuildAssetBundles_MacOS()
    {
        string dir = "HotUpdateFiles/MacOS/AssetBundles"; //输出路径的起始路径跟Assets文件夹在同一目录下

        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }

        BuildPipeline.BuildAssetBundles(dir, BuildAssetBundleOptions.None, BuildTarget.StandaloneOSX); //把项目所有资源打包成AssetBundle文件
    }

    [MenuItem("ExportTool/BuildAssetBundles/BuildAssetBundles_Android")]
    public static void BuildAssetBundles_Android()
    {
        string dir = "HotUpdateFiles/Android/AssetBundles"; //输出路径的起始路径跟Assets文件夹在同一目录下

        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }

        BuildPipeline.BuildAssetBundles(dir, BuildAssetBundleOptions.None, BuildTarget.Android); //把项目所有资源打包成AssetBundle文件
    }

    [MenuItem("ExportTool/BuildAssetBundles/BuildAssetBundles_IOS")]
    public static void BuildAssetBundles_IOS()
    {
        string dir = "HotUpdateFiles/IOS/AssetBundles"; //输出路径的起始路径跟Assets文件夹在同一目录下

        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }

        BuildPipeline.BuildAssetBundles(dir, BuildAssetBundleOptions.None, BuildTarget.iOS); //把项目所有资源打包成AssetBundle文件
    }
}