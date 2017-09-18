--[[
            LiveMap - A LiveMap for FiveM servers
              Copyright (C) 2017  Jordan Dalton

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program in the file "LICENSE".  If not, see <http://www.gnu.org/licenses/>.
]]

--[[
    Simple file that wraps all the internal events so that they can just be called
    by the client without any extra steps needed by the end-developer :)
]]

RegisterServerEvent("livemap:AddPlayerData")
RegisterServerEvent("livemap:UpdatePlayerData")

RegisterServerEvent("livemap:RemovePlayerData")
RegisterServerEvent("livemap:RemovePlayer")

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

AddEventHandler("livemap:RemovePlayerData", function(k)
    local id = GetPlayerIdentifier(source, 0)

    print("Removing " .. k .. " from " .. id)
    TriggerEvent("livemap:internal_RemovePlayerData", id, k)
end)

AddEventHandler("livemap:RemovePlayer", function()
    local id = GetPlayerIdentifier(source, 0)
    print("Removing player " ..  id)

    TriggerEvent("livemap:internal_RemovePlayer", id)
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
