﻿//this source code was auto-generated by tolua#, do not modify it
using System;
using LuaInterface;

public class PrefabInstanceWrap
{
	public static void Register(LuaState L)
	{
		L.BeginClass(typeof(PrefabInstance), typeof(UnityEngine.MonoBehaviour));
		L.RegFunction("GetLuaTable", GetLuaTable);
		L.RegFunction("GetTime", GetTime);
		L.RegFunction("__eq", op_Equality);
		L.RegFunction("__tostring", ToLua.op_ToString);
		L.EndClass();
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetLuaTable(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 1);
			PrefabInstance obj = (PrefabInstance)ToLua.CheckObject<PrefabInstance>(L, 1);
			LuaInterface.LuaTable o = obj.GetLuaTable();
			ToLua.Push(L, o);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetTime(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 1);
			PrefabInstance obj = (PrefabInstance)ToLua.CheckObject<PrefabInstance>(L, 1);
			float o = obj.GetTime();
			LuaDLL.lua_pushnumber(L, o);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int op_Equality(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 2);
			UnityEngine.Object arg0 = (UnityEngine.Object)ToLua.ToObject(L, 1);
			UnityEngine.Object arg1 = (UnityEngine.Object)ToLua.ToObject(L, 2);
			bool o = arg0 == arg1;
			LuaDLL.lua_pushboolean(L, o);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}
}

