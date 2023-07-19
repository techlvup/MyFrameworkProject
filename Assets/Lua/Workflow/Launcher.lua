---@class Launcher
local Launcher = {}

function Launcher.PlayGame()
    local objLoginPanel = LuaCallCS.CreateGameObject("UI/LoginPanel", "objLoginPanel")

    LuaCallCS.SetSpriteImage(objLoginPanel, "Image", "Atlas01/01_btn_Cheng2", true)

    local posTable = {
        [1] = Vector3.New(0,0,0),
        [2] = Vector3.New(100,100,0),
        [3] = Vector3.New(-100,200,0),
        [4] = Vector3.New(-100,100,0),
        [5] = Vector3.New(300,-500,0),
    }

    local animationTable = {
        [1] = LuaCallCS.PlayCurveAnimation(objLoginPanel, "Image", posTable, 3),
        [2] = LuaCallCS.PlayAlphaAnimation(objLoginPanel, "Image", 1, 0, 1, nil, 0, 2),
        [3] = LuaCallCS.PlayPositionAnimation(objLoginPanel, "Image", posTable[5], posTable[1], 1),
        [4] = LuaCallCS.PlayScaleAnimation(objLoginPanel, "Image", Vector3.New(3,3,3), Vector3.New(8,8,8), 1, function ()
            LuaCallCS.PlayScaleAnimation(objLoginPanel, "Image", Vector3.New(8,8,8), Vector3.New(3,3,3), 1, nil, 0, -1)
        end),
    }

    LuaCallCS.PlaySequentialAnimation(animationTable)

    local v = 5
end

Launcher.PlayGame()