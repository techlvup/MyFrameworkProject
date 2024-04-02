using UnityEngine;
using LuaInterface;
using System;
using System.Collections;

public delegate void LoadAssetBundleCallBack(object assetBundleObj);
public delegate Coroutine IEnumeratorStartCoroutine(IEnumerator routine);
public delegate Coroutine StringStartCoroutine(string methodName);

public class Launcher : MonoBehaviour
{
    public static Launcher Instance = null;
    public string m_fileRootPath = "";
    private GameLoadingPanel m_GameLoadingPanel = null;
    public bool isUpdate = false;//写上自己的服务器根目录后默认true即可



    private void Awake()
    {
        GameObject GameLoadingPanel = LuaCallCS.CreateGameObject("UI/GameLoadingPanel", "GameLoadingPanel");

        if (GameLoadingPanel != null)
        {
            m_GameLoadingPanel = GameLoadingPanel.GetComponent<GameLoadingPanel>();
        }
        else
        {
            return;
        }

        HotUpdateManager.Instance.Init(isUpdate);

        if (isUpdate)
        {
            m_fileRootPath = Application.persistentDataPath + "/";

            StartCoroutine(HotUpdateManager.Instance.DownloadCatalogueFile(StartCoroutine, StartCoroutine));
        }
        else
        {
            m_fileRootPath = Application.streamingAssetsPath.Substring(0, Application.streamingAssetsPath.LastIndexOf("/") + 16) + "/";
        }

        Instance = this;
    }

    private void Update()
    {
        CheckUpdate();
    }

    private void OnDestroy()
    {
        LuaManager.Instance.Stop();
        MessageNetManager.Instance.Stop();
    }



    public void CheckUpdate()
    {
        if (m_GameLoadingPanel != null)
        {
            if (!isUpdate)
            {
                HotUpdateManager.Instance.m_nowDownloadNum += Time.deltaTime;
            }

            m_GameLoadingPanel.SetProgressSlider(HotUpdateManager.Instance.m_nowDownloadNum / HotUpdateManager.Instance.m_needDownloadNum);

            if (HotUpdateManager.Instance.m_nowDownloadNum >= HotUpdateManager.Instance.m_needDownloadNum)
            {
                PlayGame();
            }
        }
    }

    public void PlayGame()
    {
        LuaManager.Instance.Play();
        SdkMsgManager.Instance.Init();
        MessageNetManager.Instance.Play();
        Application.logMessageReceived += DebugCSErrorLuaStackTrace;
        LuaCallCS.ClosePrefabPanel("GameLoadingPanel");
        m_GameLoadingPanel = null;
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

    private void DebugCSErrorLuaStackTrace(string logString, string stackTrace, LogType logType)
    {
        if(logType == LogType.Exception)
        {
            string luaLog = LuaManager.Instance.m_luaState.GetFunction("debug.traceback").Invoke<object, object, object, string>(null, null, null);

            if (!string.IsNullOrEmpty(luaLog))
            {
                Debug.LogError(luaLog);
            }
        }
    }
}