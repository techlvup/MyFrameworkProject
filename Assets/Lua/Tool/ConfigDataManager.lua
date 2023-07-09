---@class ConfigDataManager
ConfigDataManager = {}

function ConfigDataManager.Get(configName,fieldName)
    local config = LuaCallCS.LoadConfigData(configName)
    return config[fieldName]
end