using UnityEditor;
using UnityEngine;
using System.IO;
using System.Collections.Generic;



public class AtlasBuilder
{
    private static string m_texturesRootPath = Application.dataPath + "/GameAssets/Textures";//图片文件夹的根路径
    private static string m_atlasRootPath = Application.dataPath + "/Resources/Atlas";//图集存储路径



    [MenuItem("GodDragonTool/打包所有图集")]
    public static void PackSpriteAtlas()
    {
        Dictionary<string, List<Texture2D>> atlasTextures = new Dictionary<string, List<Texture2D>>();
        Dictionary<string, Dictionary<string, float[]>> atlasRects = new Dictionary<string, Dictionary<string, float[]>>();

        string rootFolderPath = m_texturesRootPath.Replace(Application.dataPath, "Assets");

        string[] assetGUIDs = AssetDatabase.FindAssets("t:Sprite", new string[] { rootFolderPath });//会包括子文件夹内符合要求的文件

        for (int i = 0; i < assetGUIDs.Length; i++)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(assetGUIDs[i]);
            string name = Path.GetFileName(assetPath);
            string atlasName = "";

            if (assetPath != rootFolderPath + "/" + name)
            {
                atlasName = assetPath.Replace("Assets/GameAssets/Textures/", "");
                atlasName = atlasName.Replace("/" + name, "");
            }

            if (!string.IsNullOrEmpty(atlasName))
            {
                Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(assetPath);

                if (sprite != null)
                {
                    if (!atlasTextures.ContainsKey(atlasName))
                    {
                        atlasTextures[atlasName] = new List<Texture2D>();
                    }

                    atlasTextures[atlasName].Add(sprite.texture);
                }
            }
        }

        if (atlasTextures.Count > 0)
        {
            CreateAtlasAndMaterial(atlasTextures, ref atlasRects);
        }

        if (atlasRects.Count > 0)
        {
            SaveAtlasTexturesRect(atlasRects);
        }

        EditorUtility.ClearProgressBar();

        atlasTextures.Clear();

        atlasRects.Clear();

        AssetDatabase.Refresh();
    }



    private static void CreateAtlasAndMaterial(Dictionary<string, List<Texture2D>> atlasTexture, ref Dictionary<string, Dictionary<string, float[]>> atlasRect)
    {
        int progressIndex = 0;

        foreach (var item in atlasTexture)
        {
            //创建图集
            string atlasName = item.Key;
            Texture2D[] textures = item.Value.ToArray();

            Texture2D atlas = new Texture2D(2048, 2048);

            Rect[] rects = atlas.PackTextures(textures, 2, 2048, false);

            Color[] atlasPixels = atlas.GetPixels();

            Dictionary<string, float[]> textureRect = new Dictionary<string, float[]>();

            for (int i = 0; i < textures.Length; i++)
            {
                Texture2D texture = textures[i];
                Rect rect = rects[i];
                Color[] texturePixels = texture.GetPixels();

                int x = (int)(rect.x * atlas.width);
                int y = (int)(rect.y * atlas.height);

                for (int h = 0; h < texture.height; h++)
                {
                    for (int w = 0; w < texture.width; w++)
                    {
                        int atlasX = x + w;
                        int atlasY = y + h;

                        int index = atlasX + atlasY * atlas.width;

                        atlasPixels[index] = texturePixels[w + h * texture.width];
                    }
                }

                textureRect.Add(texture.name, new float[4] { rect.x, rect.y, rect.width, rect.height });

                EditorUtility.DisplayProgressBar("设置" + atlasName + "图集的像素数据中......", "进度：" + i + "/" + textures.Length, i * 1.0f / textures.Length);
            }

            atlasRect.Add(atlasName, textureRect);

            atlas.SetPixels(atlasPixels);

            atlas.Apply();

            if (!Directory.Exists(m_atlasRootPath + "/" + atlasName))
            {
                Directory.CreateDirectory(m_atlasRootPath + "/" + atlasName);
            }

            byte[] bytes = atlas.EncodeToPNG();

            using (FileStream fileStream = new FileStream(m_atlasRootPath + "/" + atlasName + "/" + atlasName + ".png", FileMode.Create))
            {
                using (BinaryWriter binaryWriter = new BinaryWriter(fileStream))
                {
                    binaryWriter.Write(bytes);
                }
            }

            AssetDatabase.Refresh();

            string assetsAtlasPath = m_atlasRootPath.Replace(Application.dataPath, "Assets") + "/" + atlasName + "/" + atlasName + ".png";
            string assetsMaterialPath = assetsAtlasPath.Replace(".png", "Material.mat");

            //设置图集的ImportSettings
            SetAtlasImportSettings(assetsAtlasPath, atlasName, atlas, textures, rects);

            //创建图集的材质
            CreateAtlasMaterial(assetsMaterialPath, assetsAtlasPath);

            //设置图集材质的ImportSettings
            SetMaterialImportSettings(assetsMaterialPath, atlasName);

            progressIndex++;

            EditorUtility.DisplayProgressBar("生成" + atlasName + "图集及其材质" + "中......", "进度：" + progressIndex + "/" + atlasTexture.Count, progressIndex * 1.0f / atlasTexture.Count);
        }
    }

    private static void SetAtlasImportSettings(string assetsAtlasPath, string atlasName, Texture2D atlas, Texture2D[] textures, Rect[] rects)
    {
        TextureImporter atlasImporter = AssetImporter.GetAtPath(assetsAtlasPath) as TextureImporter;

        atlasImporter.textureType = TextureImporterType.Sprite;
        atlasImporter.spriteImportMode = SpriteImportMode.Multiple;

        List<SpriteMetaData> spriteMetaDatas = new List<SpriteMetaData>();

        for (int i = 0; i < textures.Length; i++)
        {
            Rect rect = rects[i];

            SpriteMetaData spriteMetaData = new SpriteMetaData();

            spriteMetaData.alignment = (int)SpriteAlignment.Center;
            spriteMetaData.name = textures[i].name;
            spriteMetaData.rect = new Rect(rect.x * atlas.width, rect.y * atlas.height, rect.width * atlas.width, rect.height * atlas.height);

            spriteMetaDatas.Add(spriteMetaData);

            EditorUtility.DisplayProgressBar("设置" + atlasName + "图集ImporterSetting中......", "进度：" + i + "/" + textures.Length, i * 1.0f / textures.Length);
        }

        atlasImporter.spritesheet = spriteMetaDatas.ToArray();

        atlasImporter.assetBundleName = "atlas/" + atlasName;
        atlasImporter.assetBundleVariant = "atlas";

        EditorUtility.SetDirty(atlasImporter);

        atlasImporter.SaveAndReimport();
    }

    private static void CreateAtlasMaterial(string assetsMaterialPath, string assetsAtlasPath)
    {
        Material material = new Material(Shader.Find("UI/Unlit/Transparent"));
        AssetDatabase.CreateAsset(material, assetsMaterialPath);//创建材质资源

        material.enableInstancing = true;//打开GPU实例化，提高性能

        Texture2D mainTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(assetsAtlasPath);
        material.mainTexture = mainTexture;//把图集纹理设置为材质的主纹理

        AssetDatabase.Refresh();
    }

    private static void SetMaterialImportSettings(string assetsMaterialPath, string atlasName)
    {
        AssetImporter materialImporter = AssetImporter.GetAtPath(assetsMaterialPath);

        materialImporter.assetBundleName = "atlas/" + atlasName + "Material";
        materialImporter.assetBundleVariant = "mat";

        EditorUtility.SetDirty(materialImporter);

        materialImporter.SaveAndReimport();
    }

    private static void SaveAtlasTexturesRect(Dictionary<string, Dictionary<string, float[]>> atlasRects)
    {
        foreach (var item in atlasRects)
        {
            LuaCallCS.SaveConfigDecryptData(item.Value, item.Key + ".bin");
        }
    }
}