---@class LuaCallCS
local Launcher = {}

function Launcher.PlayGame()
    print("开始游戏啦！！！")

    local a = ConfigDataManager.Get("Player","2_3","Stageicon")
    local b = LuaCallCS.GetConfigLuaTableKeysByName("Player")

    local v = 5
end

LuaCallCS.SetPlayGameFunc(Launcher.PlayGame)