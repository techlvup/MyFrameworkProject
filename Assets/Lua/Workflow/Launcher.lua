---@class Launcher
local Launcher = {}

function Launcher.PlayGame()
    local objLoginPanel = LuaCallCS.CreateGameObject("UI/LoginPanel", "objLoginPanel")

    LuaCallCS.SetSpriteImage(objLoginPanel, "Btn_Login", "Atlas01/01_btn_Cheng2", true)

    local posTable = {
        [1] = Vector3.New(0,0,0),
        [2] = Vector3.New(100,100,0),
        [3] = Vector3.New(-100,200,0),
        [4] = Vector3.New(-100,100,0),
        [5] = Vector3.New(300,-500,0),
    }

    local animationTable = {
        [1] = LuaCallCS.PlayCurveAnimation(objLoginPanel, "Btn_Login", posTable, 3),
        [2] = LuaCallCS.PlayAlphaAnimation(objLoginPanel, "Btn_Login", 1, 0, 1, nil, 0, 2),
        [3] = LuaCallCS.PlayPositionAnimation(objLoginPanel, "Btn_Login", posTable[5], posTable[1], 1),
        [4] = LuaCallCS.PlayScaleAnimation(objLoginPanel, "Btn_Login", Vector3.New(3,3,3), Vector3.New(8,8,8), 1, function ()
            LuaCallCS.PlayScaleAnimation(objLoginPanel, "Btn_Login", Vector3.New(8,8,8), Vector3.New(3,3,3), 1, nil, 0, -1)
            LuaCallCS.SetText(objLoginPanel, "Btn_Login/Text", "登录")
            LuaCallCS.AddClickListener(objLoginPanel, "Btn_Login", function()
                SdkMsgManager.Instance:LoginQQ()
                print("登录QQ")
            end)
        end),
    }

    LuaCallCS.PlaySequentialAnimation(animationTable)

    print("登录动画播放结束！")
end

Launcher.PlayGame()