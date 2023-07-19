using System;
using UnityEngine;
using UnityEngine.UI;
using LuaInterface;

public static partial class LuaCallCS
{
    public static void AddClickListener(UnityEngine.Object obj, LuaFunction luaFunc)
    {
        Transform trans = GetTransform(obj);

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

            Sprite sprite = null;

            string[] spriteInfo = spritePath.Split('/');

            if (spriteInfo[0] == spriteInfo[1])
            {
                sprite = Resources.Load<Sprite>("Png/" + spriteInfo[1]);
                image.material = null;
            }
            else if (GetTextureRectByAtlasName(spriteInfo[0], spriteInfo[1], out float[] rect))
            {
                Texture2D atlas = Resources.Load<Texture2D>("Atlas/" + spriteInfo[0] + "/" + spriteInfo[0]);

                float x = rect[0] * atlas.width;
                float y = rect[1] * atlas.height;
                float width = rect[2] * atlas.width;
                float height = rect[3] * atlas.height;

                sprite = Sprite.Create(atlas, new Rect(x, y, width, height), new Vector2(0.5f, 0.5f));

                Material material = Resources.Load<Material>("Atlas/" + spriteInfo[0] + "/" + spriteInfo[0] + "Material");

                image.material = material;
            }

            if (sprite != null)
            {
                image.sprite = sprite;
            }

            if (isSetNativeSize)
            {
                SetSpriteImageNativeSize(image);
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

            Sprite sprite = null;

            string[] spriteInfo = texturePath.Split('/');

            if (spriteInfo[0] == spriteInfo[1])
            {
                sprite = Resources.Load<Sprite>("Png/" + spriteInfo[1]);
                rawImage.material = null;
            }
            else if (GetTextureRectByAtlasName(spriteInfo[0], spriteInfo[1], out float[] rect))
            {
                Texture2D atlas = Resources.Load<Texture2D>("Atlas/" + spriteInfo[0] + "/" + spriteInfo[0]);

                float x = rect[0] * atlas.width;
                float y = rect[1] * atlas.height;
                float width = rect[2] * atlas.width;
                float height = rect[3] * atlas.height;

                sprite = Sprite.Create(atlas, new Rect(x, y, width, height), new Vector2(0.5f, 0.5f));

                Material material = Resources.Load<Material>("Atlas/" + spriteInfo[0] + "/" + spriteInfo[0] + "Material");

                rawImage.material = material;
            }

            if (sprite != null)
            {
                rawImage.texture = sprite.texture;
            }

            if (isSetNativeSize)
            {
                SetTextureRawImageNativeSize(rawImage);
            }
        }
    }

    public static void SetTextureRawImageNativeSize(RawImage rawImage)
    {
        rawImage.SetNativeSize();
    }
}