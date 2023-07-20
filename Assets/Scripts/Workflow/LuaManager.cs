using LuaInterface;
using System.Collections.Generic;



public class LuaManager
{
    public static LuaState m_luaState = null;//Lua的虚拟机
    public static Dictionary<string, LuaTable> m_luaClassList;//所有预制体对应的lua脚本



    public static void Play()
    {
        if (m_luaState != null)
        {
            return;
        }

        m_luaClassList = new Dictionary<string, LuaTable>();

        new LuaResLoader();//加载Lua文件

        m_luaState = new LuaState();//虚拟机初始化

        m_luaState.Start();//开始虚拟机

        LuaBinder.Bind(m_luaState);//绑定虚拟机

        m_luaState.DoFile("Workflow/Main.lua");//写绝对路径或者是以Lua文件夹下的文件或文件夹开头的路径
    }

    public static void Stop()
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
}