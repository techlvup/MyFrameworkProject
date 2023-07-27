using UnityEngine;
using LuaInterface;
using System;
using System.Collections;

public delegate void LoadAssetBundleCallBack(object assetBundleObj);

public class Launcher : MonoBehaviour
{
    private HotUpdate m_hotUpdate = null;//负责热更新流程的脚本
    public static Launcher Instance = null;
    public string m_fileRootPath = "";



    private void Awake()
    {
        m_hotUpdate = gameObject.GetComponent<HotUpdate>();
        m_hotUpdate.StartHotUpdate();

#if UNITY_EDITOR
        m_fileRootPath = Application.dataPath.Replace("Assets", "");
#else
        m_fileRootPath = Application.persistentDataPath + "/";
#endif

        Instance = this;
    }

    private void OnDestroy()
    {
        LuaManager.Stop();

        MessageNetManager.Stop();
    }



    public void PlayGame()
    {
        if (m_hotUpdate != null)
        {
            Destroy(m_hotUpdate);
            m_hotUpdate = null;
        }

        LuaManager.Play();

        MessageNetManager.Play();
    }

    public void StartLoadAssetBundle(string assetBundlePath, string assetName, LoadAssetBundleCallBack callBack)
    {
        StartCoroutine(LoadAssetBundle(assetBundlePath, assetName, callBack));
    }

    private IEnumerator LoadAssetBundle(string assetBundlePath, string assetName, LoadAssetBundleCallBack callBack)
    {
        AssetBundleCreateRequest assetBundleRequest = AssetBundle.LoadFromFileAsync(assetBundlePath);

        yield return assetBundleRequest;

        AssetBundle assetBundle = assetBundleRequest.assetBundle;
        AssetBundleRequest assetRequest = assetBundle.LoadAssetAsync(assetName);

        yield return assetRequest;

        callBack(assetRequest.asset);
    }

    public IEnumerator PlayAnimationSequence(UnityEngine.Object obj, string childPath = "", string animName = "", LuaFunction callBack = null)
    {
        if (!string.IsNullOrEmpty(animName))
        {
            Transform trans = LuaCallCS.GetTransform(obj);

            if (trans != null)
            {
                if (!string.IsNullOrEmpty(childPath))
                {
                    trans = trans.Find(childPath);
                }

                if (trans != null)
                {
                    Animation animation = trans.GetComponent<Animation>();

                    if (animation != null)
                    {
                        animation.Play(animName);

                        yield return new WaitWhile(() => animation.isPlaying);

                        if (callBack != null)
                        {
                            callBack.Call();
                        }
                    }
                }
            }
        }
    }

    public IEnumerator PlayAnimationSequence(UnityEngine.Object obj, string childPath = "", string animName = "", Action callBack = null)
    {
        if (!string.IsNullOrEmpty(animName))
        {
            Transform trans = LuaCallCS.GetTransform(obj);

            if (trans != null)
            {
                if (!string.IsNullOrEmpty(childPath))
                {
                    trans = trans.Find(childPath);
                }

                if (trans != null)
                {
                    Animation animation = trans.GetComponent<Animation>();

                    if (animation != null)
                    {
                        animation.Play(animName);

                        yield return new WaitWhile(() => animation.isPlaying);

                        if (callBack != null)
                        {
                            callBack();
                        }
                    }
                }
            }
        }
    }
}