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

RegisterServerEvent("livemap:blipsGenerated")
RegisterServerEvent("livemap:AddBlip")
RegisterServerEvent("livemap:UpdateBlip")

local blips = {}

-- Set this to the player who did the "blips generate" command. Should stop
-- randomers triggering the event :)
local playerWhoGeneratedBlips = nil

function blipsIsEmpty()
    for _, _ in pairs(blips) do
        return false
    end
    return true
end

function blipsContainsBlip(id, x, y, z)
    if blips[id] == nil then
        return -1
    end

    for k,v in ipairs(blips[id]) do
        if v.x == x and v.y == y and v.z == z then
            return k
        end
    end

    return -1
end

function sendBlips(res)
    -- Restrict the origin if set, otherwise allow everyone
    res.writeHead(200, { ["Access-Control-Allow-Origin"] = GetConvar("livemap_access_control", "*")} )

    if not blipsIsEmpty() then
        res.send(json.encode(blips))
        return
    else
        res.send(json.encode({
            error = "blip cache is empty"
        }))
    end
end

AddEventHandler("livemap:AddBlip", function(blip)
    if not blip["sprite"] or blip["sprite"] == nil then
        -- We don't have a sprite id... we can't save it :(
        print("LiveMap Error: AddBlip cannot run because no sprite was supplied.")
        return
    end
    local id = tostring(blip["sprite"])

    blip["x"] = tonumber(string.format("%.2f", blip["x"]))
    blip["z"] = tonumber(string.format("%.2f", blip["z"]))
    blip["y"] = tonumber(string.format("%.2f", blip["y"]))

    if blipsContainsBlip(id, blip.x, blip.y, blip.z) == -1 then
        if blips[id] == nil then
            blips[id] = {}
        end

        table.insert(blips[id], blip)
    else
        print("Blip already exists at " .. blip.x .. "," .. blip.y .. "," .. blip.z)
    end
end)

AddEventHandler("livemap:UpdateBlip", function(blip)
    if not blip["sprite"] or blip["sprite"] == nil then
        -- We don't have a sprite id... we can't save it :(
        print("LiveMap Error: AddBlip cannot run because no sprite was supplied.")
        return
    end
    local id = tostring(blip["sprite"])

    blip["x"] = tonumber(string.format("%.2f", blip["x"]))
    blip["z"] = tonumber(string.format("%.2f", blip["z"]))
    blip["y"] = tonumber(string.format("%.2f", blip["y"]))

    local index = blipsContainsBlip(id, blip.x, blip.y, blip.z)

    if index ~= -1 then
        blips[id][index] = blip
    else
        -- Blip doesn't exist, add it?
        if blips[id] == nil then
            blips[id] = {}
        end

        table.insert(blips[id], blip)
    end
end)

function saveBlips()
    if blipsIsEmpty() then
        print("Blips are empty... No need to save")
        return
    end

    local blipFile = GetConvar("blip_file", "server/blips.json")
    local saved = SaveResourceFile(GetCurrentResourceName(), blipFile, json.encode(blips, {indent = true}), -1)

    if not saved then
        print("LiveMap couldn't save the blips to file.. Maybe the directory isn't writable?")
    end

end

AddEventHandler("livemap:blipsGenerated", function(blipTable)
    local id = GetPlayerIdentifier(source, 0)

    if playerWhoGeneratedBlips == nil or playerWhoGeneratedBlips ~= id then
        print("playerWhoGeneratedBlips was incorrect.. Maybe \"" .. id .. "\" triggered the event themselves?\nEither way, I'm not doing it..")
        return
    end

    blips = blipTable

    saveBlips()
end)


RegisterCommand("blips", function(source, args, rawCommand)
    local playerId = GetPlayerIdentifier(source, 0)
    if args[1] == "generate" then
        print("Generating blips using the in-game native: Player " .. playerId .. " is generating (I hope you know them)")
        playerWhoGeneratedBlips = playerId

        TriggerClientEvent("livemap:getBlipsFromClient", source)

        return
    end

end, true)

AddEventHandler("onResourceStop", function(name)
    if name == GetCurrentResourceName() then
        saveBlips() -- Make sure the blips file is saved
    end
end)

AddEventHandler("onResourceStart", function(name)
    if name == GetCurrentResourceName() then
        local blipFile = GetConvar("blip_file", "server/blips.json")
        local blipData = LoadResourceFile(GetCurrentResourceName(), blipFile)

        if blipData then
            blips = json.decode(blipData)
            print("loaded blip cache from file")
        end
    end
end)


--[[
    Handle HTTP requests. Mainly used for getting the blips via ajax from the UI
]]
SetHttpHandler(function(req, res)
	local path = req.path

    if path == "/blips" or path == "/blips.json" then
        return sendBlips(res)
    end

    return res.send(json.encode({
        error = "path \"" .. path .."\" not found"
    }))
end)
