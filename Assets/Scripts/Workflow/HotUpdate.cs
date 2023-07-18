using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public class HotUpdate : MonoBehaviour
{
    private Launcher m_launcher;
    private float m_nowDownloadNum;
    private float m_needDownloadNum;
    private string m_webRootPath;
    private string m_localRootPath;
    private string m_catalogueFileWebURL;
    private string m_catalogueFileLocalPath;
    private GameLoadingPanel m_GameLoadingPanel;



    private void Awake()
    {
        m_launcher = gameObject.GetComponent<Launcher>();

        m_nowDownloadNum = 0;
        m_needDownloadNum = -1;

        m_webRootPath = "https://github.com/techlvup/MyFrameworkProject/blob/main/";
        m_localRootPath = Application.persistentDataPath + "/";

#if UNITY_EDITOR
        m_needDownloadNum = 3;

#elif UNITY_ANDROID
        m_catalogueFileWebURL = m_webRootPath + "CatalogueFiles/Android/CatalogueFile.txt";
        m_catalogueFileLocalPath = m_localRootPath + "CatalogueFiles/Android/CatalogueFile.txt";
#endif
    }

    private void Update()
    {
        if(m_GameLoadingPanel != null)
        {
#if UNITY_EDITOR
            m_nowDownloadNum += Time.deltaTime;
#endif

            m_GameLoadingPanel.SetProgressSlider(m_nowDownloadNum / m_needDownloadNum);

            if (m_nowDownloadNum >= m_needDownloadNum)
            {
                m_launcher.PlayGame();
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

            using (FileStream fileStream = new FileStream(m_catalogueFileLocalPath, FileMode.OpenOrCreate))
            {
                using (StreamReader streamReader = new StreamReader(fileStream))
                {
                    m_nowDownloadNum = 0;

                    string localCatalogueText = streamReader.ReadToEnd();

                    if (string.IsNullOrEmpty(localCatalogueText))
                    {
                        using (StreamWriter streamWriter = new StreamWriter(fileStream))
                        {
                            streamWriter.Write(downloadCatalogueText);

                            string[] filePath = downloadCatalogueText.Split('|', '\n');

                            if (filePath.Length > 0)
                            {
                                m_needDownloadNum = filePath.Length / 2;

                                for (int i = 0; i < filePath.Length; i++)
                                {
                                    if (i % 2 == 0)
                                    {
                                        StartCoroutine(DownloadWebFile(filePath[i]));
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        using (StreamWriter streamWriter = new StreamWriter(fileStream))
                        {
                            JudgeNeedDownloadFileNum(downloadCatalogueText, localCatalogueText);

                            CompareLocalAndWebFile(downloadCatalogueText, localCatalogueText);

                            streamWriter.Write(downloadCatalogueText);
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
                StartCoroutine(key);
            }
        }
    }

    private IEnumerator DownloadWebFile(string path)
    {
        UnityWebRequest requestHandler = UnityWebRequest.Get(m_webRootPath + path);//下载路径需要加上文件的后缀，没有后缀则不加

        yield return requestHandler.SendWebRequest();

        if (requestHandler.isHttpError || requestHandler.isNetworkError)
        {
            Debug.Log(requestHandler.error);
        }
        else
        {
            using (FileStream fileStream = new FileStream(m_localRootPath + path, FileMode.Create))
            {
                using (BinaryWriter binaryWriter = new BinaryWriter(fileStream))
                {
                    binaryWriter.Write(requestHandler.downloadHandler.data);
                }
            }

            m_nowDownloadNum++;
        }
    }
}