using UnityEngine;
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

        m_gameHelperJavaClass = new AndroidJavaClass("com.GodDragon.MyFrameworkProject.GameHelper");
#endif
    }

    /// <summary>
    /// 发送Unity消息到Android
    /// </summary>
    public void SendUnityMessageToAndroid(int iMsgId, int iParam1 = 0, int iParam2 = 0, int iParam3 = 0, string strParam1 = "", string strParam2 = "", string strParam3 = "")
    {
        if (m_gameHelperJavaClass == null)
        {
            return;
        }

        m_gameHelperJavaClass.CallStatic("AndroidReceiveUnityMessage", iMsgId, iParam1, iParam2, iParam3, strParam1, strParam2, strParam3);
    }
}