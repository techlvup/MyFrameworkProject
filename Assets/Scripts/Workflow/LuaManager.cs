using LuaInterface;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;



public class LuaManager : Singleton<LuaManager>
{
    public LuaState m_luaState = null;//Lua的虚拟机
    public Dictionary<string, LuaTable> m_luaClassList;//所有预制体对应的lua脚本



    public void Play()
    {
        if (m_luaState != null)
        {
            return;
        }

        SetLuaSearchPaths();

        m_luaClassList = new Dictionary<string, LuaTable>();

        new LuaResLoader();//加载Lua文件

        m_luaState = new LuaState();//虚拟机初始化

        m_luaState.Start();//开始虚拟机

        LuaBinder.Bind(m_luaState);//绑定虚拟机

        m_luaState.DoFile("Workflow/Main.lua");//写绝对路径或者是以Lua文件夹下的文件或文件夹开头的路径
    }

    public void Stop()
    {
        if (m_luaState != null)
        {
            m_luaState.Dispose();
        }

        if(m_luaClassList != null)
        {
            m_luaClassList.Clear();
        }
    }

    private void SetLuaSearchPaths()
    {
        // 构建可能的搜索路径
        string assetsPath = Application.dataPath;
        string streamingAssetsPath = Application.streamingAssetsPath + "/Lua";
        string persistentDataPath = Application.persistentDataPath + "/Lua";

        // 设置搜索路径
        string[] searchPaths = {
            assetsPath,
            streamingAssetsPath,
            persistentDataPath,
        };

        // 设置Lua虚拟机的package.path
        string luaSearchPath = string.Join(";", searchPaths) + ";" + Environment.GetEnvironmentVariable("LUA_PATH");
        Environment.SetEnvironmentVariable("LUA_PATH", luaSearchPath);

        // 设置Lua虚拟机的package.cpath（如果需要的话）
        // string cLuaSearchPath = string.Join(";", searchPaths) + ";" + Environment.GetEnvironmentVariable("LUA_CPATH");
        // Environment.SetEnvironmentVariable("LUA_CPATH", cLuaSearchPath);

        // 调用Lua执行DoFile之类的操作
        // Lua.DoFile("example.lua");
    }
}