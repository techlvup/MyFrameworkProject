using UnityEngine;

public class Launcher : MonoBehaviour
{
    private HotUpdate m_hotUpdate = null;//负责热更新流程的脚本



    private void Awake()
    {
        m_hotUpdate = gameObject.GetComponent<HotUpdate>();
        m_hotUpdate.StartHotUpdate();
    }

    private void OnDestroy()
    {
        LuaManager.Stop();
    }



    public void PlayGame()
    {
        if (m_hotUpdate != null)
        {
            Destroy(m_hotUpdate);
            m_hotUpdate = null;
        }

        LuaManager.Play();
    }
}