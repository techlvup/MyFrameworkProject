using LuaInterface;

public class LuaManager
{
    public static LuaState m_luaState = null;//Lua的虚拟机



    public static void Play()
    {
        if (m_luaState != null)
        {
            return;
        }

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
    }
}