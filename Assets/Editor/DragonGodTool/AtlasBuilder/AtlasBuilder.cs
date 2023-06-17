using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.U2D;
using UnityEngine;
using UnityEngine.U2D;

public class AtlasBuilder
{
    //图片文件夹的根路径
    private static string texturesRootPath = Application.dataPath + "/GameAssets/Textures";

    //图集存储路径
    private static string atlasRootPath = Application.dataPath + "/Resources/Atlas";

    //每个需要打图集的文件夹名，即图集名
    private static string spritefilePathName;

    //每个图集的所有图片路径，用之前先清空
    private static List<string> texturesPathList = new List<string>();



    [MenuItem("DragonGodTool/UpdateAtlas/打包并更新图集")]
    public static void CreateAllSpriteAtlas()
    {
        DirectoryInfo info = new DirectoryInfo(texturesRootPath);

        int index = 0;

        // 遍历根目录
        foreach (DirectoryInfo item in info.GetDirectories())
        {
            spritefilePathName = item.Name;

            string path = atlasRootPath + "/" + spritefilePathName + ".spriteatlas";

            path = path.Replace(Application.dataPath, "Assets");

            SpriteAtlas spriteAtlas = AssetDatabase.LoadAssetAtPath(path, typeof(Object)) as SpriteAtlas;

            // 不存在则创建后更新图集
            if (spriteAtlas == null)
            {
                spriteAtlas = CreateSpriteAtlas(spritefilePathName);
            }

            string spriteFilePath = texturesRootPath + "/" + spritefilePathName;

            UpdateAtlas(spriteAtlas, spriteFilePath);

            // 打包进度
            EditorUtility.DisplayProgressBar("打包图集中...", "正在处理:" + item, index / info.GetDirectories().Length);

            index++;
        }

        EditorUtility.ClearProgressBar();

        AssetDatabase.Refresh();
    }



    /// <summary>
    /// 创建图集
    /// </summary>
    /// <param name="atlasName">图集名字</param>
    private static SpriteAtlas CreateSpriteAtlas(string atlasName)
    {
        SpriteAtlas atlas = new SpriteAtlas();

        #region 图集基础设置

        SpriteAtlasPackingSettings packSetting = new SpriteAtlasPackingSettings()
        {
            blockOffset = 1,
            enableRotation = false,
            enableTightPacking = false,
            padding = 8,
        };

        atlas.SetPackingSettings(packSetting);

        #endregion

        #region 图集纹理设置

        SpriteAtlasTextureSettings textureSettings = new SpriteAtlasTextureSettings()
        {
            readable = false,
            generateMipMaps = false,
            sRGB = true,
            filterMode = FilterMode.Bilinear,
        };

        atlas.SetTextureSettings(textureSettings);

        #endregion

        #region 分平台设置图集格式
        // 需要多端同步，就要再写一份（Android）
        TextureImporterPlatformSettings platformSetting = atlas.GetPlatformSettings("Android");

        platformSetting.overridden = true;
        platformSetting.maxTextureSize = 2048;
        platformSetting.textureCompression = TextureImporterCompression.Compressed;
        platformSetting.format = TextureImporterFormat.ASTC_RGB_6x6;

        atlas.SetPlatformSettings(platformSetting);

        // 需要多端同步，就要再写一份（StandaloneWindows64）
        platformSetting = atlas.GetPlatformSettings("Standalone");

        platformSetting.overridden = true;
        platformSetting.maxTextureSize = 2048;
        platformSetting.textureCompression = TextureImporterCompression.Compressed;
        platformSetting.format = TextureImporterFormat.DXT5Crunched;

        atlas.SetPlatformSettings(platformSetting);

        #endregion

        string atlasPath = atlasRootPath + "/" + atlasName + ".spriteatlas";

        atlasPath = atlasPath.Replace(Application.dataPath, "Assets");

        AssetDatabase.CreateAsset(atlas, atlasPath);

        AssetDatabase.SaveAssets();

        return atlas;
    }

    /// <summary>
    /// 更新图集内容
    /// </summary>
    /// <param name="atlas">图集</param>
    static void UpdateAtlas(SpriteAtlas atlas, string spriteFilePath)
    {
        texturesPathList.Clear();

        FillTexturePath(spriteFilePath);

        // 获取图集下图片
        List<Object> packables = new List<Object>(atlas.GetPackables());

        foreach (string path in texturesPathList)
        {
            // 加载指定目录
            Object spriteObj = AssetDatabase.LoadAssetAtPath(path, typeof(Object));

            if (!packables.Contains(spriteObj))
            {
                atlas.Add(new Object[] { spriteObj });
            }
        }
    }

    /// <summary>
    /// 递归文件夹下的图片
    /// </summary>
    /// <param name="folderPath"></param>
    static void FillTexturePath(string folderPath)
    {
        DirectoryInfo info = new DirectoryInfo(folderPath);

        foreach (DirectoryInfo item in info.GetDirectories())
        {
            FillTexturePath(item.FullName);
        }

        foreach (FileInfo item in info.GetFiles())
        {
            string path = item.FullName.Replace("\\", "/");

            string Extension = Path.GetExtension(path);

            if (Extension == ".png" || Extension == ".jpg")
            {
                texturesPathList.Add("Assets" + path.Replace(Application.dataPath, ""));
            }
        }
    }
}