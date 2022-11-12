local gameObject1 = LuaCallCS.CreateGameObject("Test/TestPrefab1", nil, nil)
local gameObject2 = LuaCallCS.CreateGameObject("Test/TestPrefab2", nil, gameObject1)
local gameObject3 = LuaCallCS.CreateGameObject("Test/TestPrefab2", nil, gameObject2)

local btn1 = LuaCallCS.AddComponent(gameObject2, nil, "PerfectButton")
local btn2 = LuaCallCS.AddComponent(gameObject3, nil, "PerfectButton")

LuaCallCS.AddClickListener(btn1, function ()
    print("===单击事件===")
end)

LuaCallCS.AddDoubleClickListener(btn1, function ()
    print("===双击事件===")
end)

LuaCallCS.AddLongPressListener(btn1, function ()
    print("===长按事件===")
end)

LuaCallCS.AddClickListener(btn2, function ()
    print("===单击事件===")
end)

print("结束")