using UnityEngine;

public class Launcher : MonoBehaviour
{
    private HotUpdate m_hotUpdate = null;//热更新脚本



    private void Awake()
    {
        LuaManager.Start();

        m_hotUpdate = gameObject.GetComponent<HotUpdate>();
        m_hotUpdate.StartHotUpdate();
    }

    private void OnDestroy()
    {
        LuaManager.Destroy();
    }



    public void PlayGame()
    {
        if (m_hotUpdate != null)
        {
            Destroy(m_hotUpdate);
            m_hotUpdate = null;
        }

        if (LuaManager.m_playGameFunc != null)
        {
            LuaManager.m_playGameFunc.Call();
        }
    }
}