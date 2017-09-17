--[[
    Simple file that wraps all the internal events so that they can just be called
    by the client without any extra steps needed by the end-developer :)
]]

RegisterServerEvent("livemap:AddPlayerData")
RegisterServerEvent("livemap:UpdatePlayerData")

RegisterServerEvent("livemap:playerSpawned")

AddEventHandler("livemap:AddPlayerData", function(k, d)
    local id = GetPlayerIdentifier(source, 0) -- Get the first id, it'll do
    print("Adding data for " .. id)

    TriggerEvent("livemap:internal_AddPlayerData", id, k, d)
end)

AddEventHandler("livemap:UpdatePlayerData", function(k, d)
    local id = GetPlayerIdentifier(source, 0) -- Get the first id, it'll do
    print("Updating data for " .. id)

    TriggerEvent("livemap:internal_UpdatePlayerData", id, k, d)
end)


function setStaticDataFor(index, id)
    TriggerEvent("livemap:internal_AddPlayerData", id, "identifer", id)
    TriggerEvent("livemap:internal_AddPlayerData", id, "name", GetPlayerName(index))
end

--[[
    Set some static data that isn't going to change
    e.g. Player's Identifer
]]
AddEventHandler("livemap:playerSpawned", function()
    local id = GetPlayerIdentifier(source, 0) -- Get the first id, it'll do
    setStaticDataFor(source, id)
end)

-- If any players are already on the server
AddEventHandler("onResourceStart", function(name)
    if name == GetCurrentResourceName() then
        if GetNumPlayerIndices() ~= -1 then
            for i=0, GetNumPlayerIndices()-1 do
                local id = GetPlayerIdentifier(GetPlayerFromIndex(i), 0)
                setStaticDataFor(GetPlayerFromIndex(i), id)
            end
        end
    end
end)
