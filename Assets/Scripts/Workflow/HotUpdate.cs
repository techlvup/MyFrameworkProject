using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public class HotUpdate : MonoBehaviour
{
    private Launcher m_launcher;
    private int m_nowDownloadNum;
    private int m_needDownloadNum;
    private string m_catalogueFileWebURL;
    private string m_catalogueFileLocalURL;
    private GameLoadingPanel m_GameLoadingPanel;
    private float m_time;




    private void Awake()
    {
        m_launcher = gameObject.GetComponent<Launcher>();

        m_nowDownloadNum = 0;
        m_needDownloadNum = -1;

        m_time = 0;

#if UNITY_EDITOR
        m_needDownloadNum = 3;

#elif UNITY_STANDALONE_WIN
        m_catalogueFileWebURL = "http://MyFrameworkProject/HotUpdateAssetBundles/Windows/CatalogueFile.txt";
        m_catalogueFileLocalURL = Application.persistentDataPath + "/MyFrameworkProject/HotUpdateAssetBundles/Windows/CatalogueFile.txt";

#elif UNITY_STANDALONE_OSX
        m_catalogueFileWebURL = "http://MyFrameworkProject/HotUpdateAssetBundles/MacOS/CatalogueFile.txt";
        m_catalogueFileLocalURL = Application.persistentDataPath + "/MyFrameworkProject/HotUpdateAssetBundles/MacOS/CatalogueFile.txt";

#elif UNITY_ANDROID
        m_catalogueFileWebURL = "http://MyFrameworkProject/HotUpdateAssetBundles/Android/CatalogueFile.txt";
        m_catalogueFileLocalURL = Application.persistentDataPath + "/MyFrameworkProject/HotUpdateAssetBundles/Android/CatalogueFile.txt";

#elif UNITY_IOS
        m_catalogueFileWebURL = "http://MyFrameworkProject/HotUpdateAssetBundles/IOS/CatalogueFile.txt";
        m_catalogueFileLocalURL = Application.persistentDataPath + "/MyFrameworkProject/HotUpdateAssetBundles/IOS/CatalogueFile.txt";
#endif
    }

    private void Update()
    {
        if(m_GameLoadingPanel != null)
        {
            bool isEnd = false;

#if UNITY_EDITOR
            m_time = m_time + Time.deltaTime;

            m_GameLoadingPanel.SetProgressSlider(m_time / m_needDownloadNum);

            isEnd = m_time >= m_needDownloadNum;

#else
            m_GameLoadingPanel.SetProgressSlider(m_nowDownloadNum * 1.0f / m_needDownloadNum);

            isEnd = m_nowDownloadNum >= m_needDownloadNum;
#endif

            if (isEnd)
            {
                m_time = 0;

                m_nowDownloadNum = 0;

                m_needDownloadNum = -1;

                m_launcher.PlayGame();

                Destroy(m_GameLoadingPanel.gameObject);
            }
        }
    }



    public void StartHotUpdate()
    {
        GameObject GameLoadingPanel =  LuaCallCS.CreateGameObject("UI/GameLoadingPanel", "GameLoadingPanel");

        if (GameLoadingPanel != null)
        {
            m_GameLoadingPanel = GameLoadingPanel.GetComponent<GameLoadingPanel>();

#if !UNITY_EDITOR
            StartCoroutine(DownloadCatalogueFile());
#endif
        }
    }

    private IEnumerator DownloadCatalogueFile()
    {
        UnityWebRequest requestHandler = UnityWebRequest.Get(m_catalogueFileWebURL);//下载路径需要加上文件的后缀，没有后缀则不加

        yield return requestHandler.SendWebRequest();

        if (requestHandler.isHttpError || requestHandler.isNetworkError)
        {
            Debug.Log(requestHandler.error);
        }
        else
        {
            string downloadCatalogueText = requestHandler.downloadHandler.text;

            using (FileStream fs = new FileStream(m_catalogueFileLocalURL, FileMode.OpenOrCreate))
            {
                using (StreamReader sr = new StreamReader(fs))
                {
                    m_nowDownloadNum = 0;

                    string localCatalogueText = sr.ReadToEnd();

                    if (string.IsNullOrEmpty(localCatalogueText))
                    {
                        using (StreamWriter sw = new StreamWriter(fs))
                        {
                            sw.Write(downloadCatalogueText);

                            string[] filePath = downloadCatalogueText.Split('|', '\n');

                            if (filePath.Length > 0)
                            {
                                m_needDownloadNum = filePath.Length / 2;

                                for (int i = 0; i < filePath.Length; i++)
                                {
                                    if (i % 2 == 0)
                                    {
                                        if (filePath[i].Contains("/Lua/"))
                                        {
                                            int startIndex = filePath[i].IndexOf("/Lua/") + 4;
                                            StartCoroutine(DownloadWebFile("Lua", filePath[i].Substring(startIndex, filePath[i].Length - startIndex)));
                                        }
                                        else if (filePath[i].Contains("/AssetBundles/"))
                                        {
                                            int startIndex = filePath[i].IndexOf("/AssetBundles/") + 13;
                                            StartCoroutine(DownloadWebFile("AssetBundles", filePath[i].Substring(startIndex, filePath[i].Length - startIndex)));
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        using (StreamWriter sw = new StreamWriter(fs))
                        {
                            JudgeNeedDownloadFileNum(downloadCatalogueText, localCatalogueText);

                            CompareLocalAndWebFile(downloadCatalogueText, localCatalogueText);

                            sw.Write(downloadCatalogueText);
                        }
                    }
                }
            }
        }
    }

    private void JudgeNeedDownloadFileNum(string downloadCatalogueText, string localCatalogueText)
    {
        string[] webFilePath1 = downloadCatalogueText.Split('|', '\n');
        string[] localFilePath1 = localCatalogueText.Split('|', '\n');

        Dictionary<string, string> webFilePath2 = new Dictionary<string, string>();
        Dictionary<string, string> localFilePath2 = new Dictionary<string, string>();

        for (int i = 0; i < webFilePath1.Length; i++)
        {
            if (i % 2 == 0)
            {
                webFilePath2[webFilePath1[i]] = webFilePath1[i + 1];
            }
        }

        for (int i = 0; i < localFilePath1.Length; i++)
        {
            if (i % 2 == 0)
            {
                localFilePath2[localFilePath1[i]] = localFilePath1[i + 1];
            }
        }

        int needNum = 0;

        foreach (var key in webFilePath2.Keys)
        {
            if (!localFilePath2.ContainsKey(key) || (localFilePath2.ContainsKey(key) && localFilePath2[key] != webFilePath2[key]))
            {
                needNum++;
            }
        }

        m_needDownloadNum = needNum;
    }

    private void CompareLocalAndWebFile(string downloadCatalogueText, string localCatalogueText)
    {
        string[] webFilePath1 = downloadCatalogueText.Split('|', '\n');
        string[] localFilePath1 = localCatalogueText.Split('|', '\n');

        Dictionary<string, string> webFilePath2 = new Dictionary<string, string>();
        Dictionary<string, string> localFilePath2 = new Dictionary<string, string>();

        for (int i = 0; i < webFilePath1.Length; i++)
        {
            if (i % 2 == 0)
            {
                webFilePath2[webFilePath1[i]] = webFilePath1[i + 1];
            }
        }

        for (int i = 0; i < localFilePath1.Length; i++)
        {
            if (i % 2 == 0)
            {
                localFilePath2[localFilePath1[i]] = localFilePath1[i + 1];
            }
        }

        foreach (var key in webFilePath2.Keys)
        {
            if (!localFilePath2.ContainsKey(key) || (localFilePath2.ContainsKey(key) && localFilePath2[key] != webFilePath2[key]))
            {
                if (key.Contains("/Lua/"))
                {
                    int startIndex = key.IndexOf("/Lua/") + 4;
                    StartCoroutine(DownloadWebFile("Lua", key.Substring(startIndex, key.Length - startIndex)));
                }
                else if (key.Contains("/AssetBundles/"))
                {
                    int startIndex = key.IndexOf("/AssetBundles/") + 13;
                    StartCoroutine(DownloadWebFile("AssetBundles", key.Substring(startIndex, key.Length - startIndex)));
                }
            }
        }
    }

    private IEnumerator DownloadWebFile(string type, string path)
    {
        UnityWebRequest requestHandler = UnityWebRequest.Get(m_catalogueFileWebURL.Substring(0, m_catalogueFileWebURL.Length - 17) + type + path);//下载路径需要加上文件的后缀，没有后缀则不加

        yield return requestHandler.SendWebRequest();

        if (requestHandler.isHttpError || requestHandler.isNetworkError)
        {
            Debug.Log(requestHandler.error);
        }
        else
        {
            using (FileStream fs = new FileStream(m_catalogueFileLocalURL.Substring(0, m_catalogueFileLocalURL.Length - 17) + type + path, FileMode.Create))
            {
                fs.Write(requestHandler.downloadHandler.data, 0, requestHandler.downloadHandler.data.Length);
            }

            m_nowDownloadNum++;
        }
    }
}