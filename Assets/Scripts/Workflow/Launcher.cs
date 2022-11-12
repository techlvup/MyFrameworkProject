using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LuaInterface;

public class Launcher : MonoBehaviour
{
    private HotUpdate m_hotUpdate = null;//热更新脚本
    private LuaState m_luaState = null;//Lua的虚拟机

    private void Awake()
    {
#if !UNITY_EDITOR
        m_hotUpdate = gameObject.GetComponent<HotUpdate>();
        m_hotUpdate.StartHotUpdate();
#endif
    }

    private void OnDestroy()
    {
        if(m_luaState != null)
        {
            m_luaState.Dispose();
        }

        Debug.Log("销毁虚拟机");
    }

    public void StartWorkflow()
    {
        StartLuaWorkflow();

        Debug.Log("启动流程结束");
    }

    private void StartLuaWorkflow()
    {
        new LuaResLoader();//加载Lua文件

        m_luaState = new LuaState();//虚拟机初始化

        m_luaState.Start();//开始虚拟机

        LuaBinder.Bind(m_luaState);//绑定虚拟机

        m_luaState.DoFile("Workflow/Launcher.lua");//不写绝对路径则使用Lua文件夹为绝对路径
    }
}