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
    Adds some data to a player. That will get sent to the UI via websockets

    @param k The name of the data to add to the player (e.g. "Name")
    @param v The value to set the data to (e.g. "Havoc")

    Note: This MUST be triggered from a client
]]
RegisterServerEvent("livemap:AddPlayerData")

--[[
    Updated some data asssociated with the player

    @param k The name of the data to add to the player (e.g. "Name")
    @param v The new value to set the data to (e.g. "Some Name")

    Note: This MUST be triggered from a client
]]
RegisterServerEvent("livemap:UpdatePlayerData")

--[[
    Removed some data associated with a player.

    @param k The name of the data to remove from the player (e.g. "Name")

    Note: This MUST be triggered from a client
]]
RegisterServerEvent("livemap:RemovePlayerData")

--[[
    Removes a player from the internal list and notifies any socket clients
        that said player has left.

    Note: This MUST be triggered from a client
]]
RegisterServerEvent("livemap:RemovePlayer")

--[[
    Associated some static data (data that won't change) with the newly spawned
        player. This is data that can only be found on the server.

    This adds the player's identifer and their username

    Note: This MUST be triggered from a client
]]
RegisterServerEvent("livemap:playerSpawned")

--[[
    ===================================
    =           Functions             =
    ===================================
]]

--[[
    Calls internal events (handlers in the DLL) to set some static data

    @param index The server ID of the player (generally source)
    @param id The first identifer of the player (e.g. "steam:HEX")
]]
function setStaticDataFor(index, id)
    TriggerEvent("livemap:internal_AddPlayerData", id, "identifer", id)
    TriggerEvent("livemap:internal_AddPlayerData", id, "name", GetPlayerName(index))
end

--[[
    ===================================
    =             Events              =
    ===================================
]]

AddEventHandler("livemap:AddPlayerData", function(k, d)
    local id = GetPlayerIdentifier(source, 0) -- Get the first id, it'll do
    --print("Adding data for " .. id)

    TriggerEvent("livemap:internal_AddPlayerData", id, k, d)
end)

AddEventHandler("livemap:UpdatePlayerData", function(k, d)
    local id = GetPlayerIdentifier(source, 0) -- Get the first id, it'll do
    --print("Updating data for " .. id)

    TriggerEvent("livemap:internal_UpdatePlayerData", id, k, d)
end)

AddEventHandler("livemap:RemovePlayerData", function(k)
    local id = GetPlayerIdentifier(source, 0)

    --print("Removing " .. k .. " from " .. id)
    TriggerEvent("livemap:internal_RemovePlayerData", id, k)
end)

AddEventHandler("livemap:RemovePlayer", function()
    local id = GetPlayerIdentifier(source, 0)
    --print("Removing player " ..  id)

    TriggerEvent("livemap:internal_RemovePlayer", id)
end)

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
    --print("NUMBER OF PLAYERS: " .. GetNumPlayerIndices())
    if name == GetCurrentResourceName() then
        if GetNumPlayerIndices() ~= 0 then -- There's players on the server :)
            for i=0, GetNumPlayerIndices()-1 do
                local id = GetPlayerIdentifier(GetPlayerFromIndex(i), 0)
                setStaticDataFor(GetPlayerFromIndex(i), id)
            end
        end
    end
end)
