﻿using UnityEngine;
using System.Runtime.InteropServices;



public class SdkMsgManager : Singleton<SdkMsgManager>
{
    private GameObject m_sdkObject = null;
    private MessageReceiver m_messageReceiver = null;
    private AndroidJavaClass m_gameHelperJavaClass = null;



    public void Init()
    {
#if !UNITY_EDITOR
        if (m_sdkObject == null)
        {
            m_sdkObject = new GameObject("SdkObject");
            m_sdkObject.hideFlags = HideFlags.HideAndDontSave;
            m_messageReceiver = m_sdkObject.AddComponent<MessageReceiver>();
            GameObject.DontDestroyOnLoad(m_sdkObject);
        }

        if (m_gameHelperJavaClass == null)
        {
            m_gameHelperJavaClass = new AndroidJavaClass("com.GodDragon.MyFrameworkProject.GameHelper");
        }
#endif
    }

    /// <summary>
    /// QQ登录
    /// </summary>
    public void LoginQQ()
    {
        if (m_gameHelperJavaClass == null)
        {
            return;
        }

        m_gameHelperJavaClass.CallStatic("LoginQQ");
    }
}