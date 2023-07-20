using LuaInterface;



public static partial class LuaCallCS
{
    public static void SendMessage(string messageName, LuaTable msg)
    {
        MessageNetManager.Send(messageName + "|" + msg.ToString());
    }

    public static void BindReceiveMessage(string messageName, LuaFunction luaFunction)
    {
        MessageNetManager.BindReceiveMessage(messageName, luaFunction);
    }

    public static void UnbindReceiveMessage(string messageName, LuaFunction luaFunction)
    {
        MessageNetManager.UnbindReceiveMessage(messageName, luaFunction);
    }
}