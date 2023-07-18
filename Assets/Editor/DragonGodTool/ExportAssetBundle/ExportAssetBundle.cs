using System.IO;
using UnityEditor;



public class ExportAssetBundle
{
    [MenuItem("DragonGodTool/AssetBundles/打包AssetBundles/BuildAssetBundles_Windows")]
    public static void BuildAssetBundles_Windows()
    {
        string dir = "AssetBundles/Windows"; //输出路径的起始路径跟Assets文件夹在同一目录下

        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }

        BuildPipeline.BuildAssetBundles(dir, BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows64); //把项目所有资源打包成AssetBundle文件
    }

    [MenuItem("DragonGodTool/AssetBundles/打包AssetBundles/BuildAssetBundles_Android")]
    public static void BuildAssetBundles_Android()
    {
        string dir = "AssetBundles/Android"; //输出路径的起始路径跟Assets文件夹在同一目录下

        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }

        BuildPipeline.BuildAssetBundles(dir, BuildAssetBundleOptions.None, BuildTarget.Android); //把项目所有资源打包成AssetBundle文件
    }
}