using System;
using UnityEngine;
using UnityEngine.UI;
using LuaInterface;



public static partial class LuaCallCS
{
    public static void AddClickListener(UnityEngine.Object obj, string childPath, LuaFunction luaFunc)
    {
        Transform trans = GetTransform(obj, childPath);

        PerfectButton btn = trans.GetComponent<PerfectButton>();

        if (btn == null)
        {
            return;
        }

        btn.AddClickListener(luaFunc);
    }

    public static void ReleaseClickListener(UnityEngine.Object obj)
    {
        Transform trans = GetTransform(obj);

        PerfectButton btn = trans.GetComponent<PerfectButton>();

        if (btn == null)
        {
            return;
        }

        btn.ReleaseClickListener();
    }

    public static void AddDownListener(UnityEngine.Object obj, LuaFunction luaFunc)
    {
        Transform trans = GetTransform(obj);

        PerfectButton btn = trans.GetComponent<PerfectButton>();

        if (btn == null)
        {
            return;
        }

        btn.AddDownListener(luaFunc);
    }

    public static void ReleaseDownListener(UnityEngine.Object obj)
    {
        Transform trans = GetTransform(obj);

        PerfectButton btn = trans.GetComponent<PerfectButton>();

        if (btn == null)
        {
            return;
        }

        btn.ReleaseDownListener();
    }

    public static void AddUpListener(UnityEngine.Object obj, LuaFunction luaFunc)
    {
        Transform trans = GetTransform(obj);

        PerfectButton btn = trans.GetComponent<PerfectButton>();

        if (btn == null)
        {
            return;
        }

        btn.AddUpListener(luaFunc);
    }

    public static void ReleaseUpListener(UnityEngine.Object obj)
    {
        Transform trans = GetTransform(obj);

        PerfectButton btn = trans.GetComponent<PerfectButton>();

        if (btn == null)
        {
            return;
        }

        btn.ReleaseUpListener();
    }

    public static void AddDoubleClickListener(UnityEngine.Object obj, LuaFunction luaFunc)
    {
        Transform trans = GetTransform(obj);

        PerfectButton btn = trans.GetComponent<PerfectButton>();

        if (btn == null)
        {
            return;
        }

        btn.AddDoubleClickListener(luaFunc);
    }

    public static void ReleaseDoubleClickListener(UnityEngine.Object obj)
    {
        Transform trans = GetTransform(obj);

        PerfectButton btn = trans.GetComponent<PerfectButton>();

        if (btn == null)
        {
            return;
        }

        btn.ReleaseDoubleClickListener();
    }

    public static void AddLongPressListener(UnityEngine.Object obj, LuaFunction luaFunc)
    {
        Transform trans = GetTransform(obj);

        PerfectButton btn = trans.GetComponent<PerfectButton>();

        if (btn == null)
        {
            return;
        }

        btn.AddLongPressListener(luaFunc);
    }

    public static void ReleaseLongPressListener(UnityEngine.Object obj)
    {
        Transform trans = GetTransform(obj);

        PerfectButton btn = trans.GetComponent<PerfectButton>();

        if (btn == null)
        {
            return;
        }

        btn.ReleaseLongPressListener();
    }

    public static void TextureToCircle(UnityEngine.Object obj, bool isSetNativeSize = false)
    {
        GameObject gameObject = GetGameObject(obj);

        if (gameObject == null)
        {
            return;
        }

        Image image = gameObject.GetComponent<Image>();

        if (image == null)
        {
            RawImage rawImage = gameObject.GetComponent<RawImage>();

            if (rawImage != null)
            {
                Texture texture = rawImage.texture;

                GameObject.Destroy(rawImage);

                CircleRawImage circleRawImage = gameObject.AddComponent<CircleRawImage>();

                circleRawImage.texture = texture;

                if (isSetNativeSize)
                {
                    circleRawImage.SetNativeSize();
                }
            }
        }
        else
        {
            Sprite sprite = image.sprite;

            GameObject.Destroy(image);

            CircleImage circleImage = gameObject.AddComponent<CircleImage>();

            circleImage.sprite = sprite;

            if (isSetNativeSize)
            {
                circleImage.SetNativeSize();
            }
        }
    }

    public static void TextureToOriginal(UnityEngine.Object obj, bool isSetNativeSize = false)
    {
        GameObject gameObject = GetGameObject(obj);

        if (gameObject == null)
        {
            return;
        }

        CircleImage circleImage = gameObject.GetComponent<CircleImage>();

        if (circleImage == null)
        {
            CircleRawImage circleRawImage = gameObject.GetComponent<CircleRawImage>();

            if (circleRawImage != null)
            {
                Texture texture = circleRawImage.texture;

                GameObject.Destroy(circleRawImage);

                RawImage rawImage = gameObject.AddComponent<RawImage>();

                rawImage.texture = texture;

                if (isSetNativeSize)
                {
                    rawImage.SetNativeSize();
                }
            }
        }
        else
        {
            Sprite sprite = circleImage.sprite;

            GameObject.Destroy(circleImage);

            Image image = gameObject.AddComponent<Image>();

            image.sprite = sprite;

            if (isSetNativeSize)
            {
                image.SetNativeSize();
            }
        }
    }

    public static void SetSpriteImage(UnityEngine.Object obj, string childPath = "", string spritePath = "", bool isSetNativeSize = false)
    {
        if (string.IsNullOrEmpty(spritePath))
        {
            return;
        }

        Transform trans = GetTransform(obj);

        if (!string.IsNullOrEmpty(childPath))
        {
            trans = trans.Find(childPath);
        }

        if (trans != null)
        {
            Image image = trans.GetComponent<Image>();

            string[] spriteInfo = spritePath.Split('/');

            if (spriteInfo[0] == spriteInfo[1])
            {
                Launcher.Instance.StartLoadAssetBundle(Launcher.Instance.m_fileRootPath + "AssetBundles/Android/atlas/" + spriteInfo[0] + ".atlas", spriteInfo[0] + ".png", (asset) => {
                    Texture2D atlas = asset as Texture2D;

                    Sprite sprite = Sprite.Create(atlas, new Rect(0, 0, atlas.width, atlas.height), new Vector2(0.5f, 0.5f));

                    if (sprite != null)
                    {
                        image.sprite = sprite;
                    }

                    if (isSetNativeSize)
                    {
                        SetSpriteImageNativeSize(image);
                    }
                });

                image.material = null;
            }
            else if (GetTextureRectByAtlasName(spriteInfo[0], spriteInfo[1], out float[] rect))
            {
                Launcher.Instance.StartLoadAssetBundle(Launcher.Instance.m_fileRootPath + "AssetBundles/Android/atlas/" + spriteInfo[0] + ".atlas", spriteInfo[0] + ".png", (asset) => {
                    Texture2D atlas = asset as Texture2D;

                    float x = rect[0] * atlas.width;
                    float y = rect[1] * atlas.height;
                    float width = rect[2] * atlas.width;
                    float height = rect[3] * atlas.height;

                    Sprite sprite = Sprite.Create(atlas, new Rect(x, y, width, height), new Vector2(0.5f, 0.5f));

                    if (sprite != null)
                    {
                        image.sprite = sprite;
                    }

                    if (isSetNativeSize)
                    {
                        SetSpriteImageNativeSize(image);
                    }
                });

                Launcher.Instance.StartLoadAssetBundle(Launcher.Instance.m_fileRootPath + "AssetBundles/Android/atlas/" + spriteInfo[0] + "material.mat", spriteInfo[0] + "Material.mat", (asset) => {
                    Material material = asset as Material;
                    image.material = material;
                });
            }
        }
    }

    public static void SetSpriteImageNativeSize(Image image)
    {
        image.SetNativeSize();
    }

    public static void SetTextureRawImage(UnityEngine.Object obj, string childPath = "", string texturePath = "", bool isSetNativeSize = false)
    {
        if (string.IsNullOrEmpty(texturePath))
        {
            return;
        }

        Transform trans = GetTransform(obj);

        if (!string.IsNullOrEmpty(childPath))
        {
            trans = trans.Find(childPath);
        }

        if (trans != null)
        {
            RawImage rawImage = trans.GetComponent<RawImage>();

            string[] spriteInfo = texturePath.Split('/');

            if (spriteInfo[0] == spriteInfo[1])
            {
                Launcher.Instance.StartLoadAssetBundle(Launcher.Instance.m_fileRootPath + "AssetBundles/Android/atlas/" + spriteInfo[0] + ".atlas", spriteInfo[0] + ".png", (asset) => {
                    Texture2D texture = asset as Texture2D;

                    if (texture != null)
                    {
                        rawImage.texture = texture;
                    }

                    if (isSetNativeSize)
                    {
                        SetTextureRawImageNativeSize(rawImage);
                    }
                });

                rawImage.material = null;
            }
            else if (GetTextureRectByAtlasName(spriteInfo[0], spriteInfo[1], out float[] rect))
            {
                Launcher.Instance.StartLoadAssetBundle(Launcher.Instance.m_fileRootPath + "AssetBundles/Android/atlas/" + spriteInfo[0] + ".atlas", spriteInfo[0] + ".png", (asset) => {
                    Texture2D atlas = asset as Texture2D;

                    float x = rect[0] * atlas.width;
                    float y = rect[1] * atlas.height;
                    float width = rect[2] * atlas.width;
                    float height = rect[3] * atlas.height;

                    Sprite sprite = Sprite.Create(atlas, new Rect(x, y, width, height), new Vector2(0.5f, 0.5f));

                    if (sprite != null)
                    {
                        rawImage.texture = sprite.texture;
                    }

                    if (isSetNativeSize)
                    {
                        SetTextureRawImageNativeSize(rawImage);
                    }
                });

                Launcher.Instance.StartLoadAssetBundle(Launcher.Instance.m_fileRootPath + "AssetBundles/Android/atlas/" + spriteInfo[0] + "material.mat", spriteInfo[0] + "Material.mat", (asset) => {
                    Material material = asset as Material;
                    rawImage.material = material;
                });
            }
        }
    }

    public static void SetTextureRawImageNativeSize(RawImage rawImage)
    {
        rawImage.SetNativeSize();
    }

    public static void SetText(UnityEngine.Object obj, string childPath = "", string des = "")
    {
        Transform trans = GetTransform(obj, childPath);

        if(trans != null)
        {
            Text text = trans.GetComponent<Text>();

            if (text != null)
            {
                text.text = des;
            }
        }
    }
}