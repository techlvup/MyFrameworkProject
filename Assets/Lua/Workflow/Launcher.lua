---@class LuaCallCS
local Launcher = {}

function Launcher.PlayGame()
    print("开始游戏啦！！！")
end

LuaCallCS.SetPlayGameFunc(Launcher.PlayGame)