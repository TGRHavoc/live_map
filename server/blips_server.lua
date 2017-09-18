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
    Blips!!!
    This file should expose the "blips.json" file to that sexy new http handler
]]

RegisterServerEvent("livemap:blipsGenerated")

local cache = {}

-- Set this to the player who did the "blips generate" command. Should stop
-- randomers triggering the event :)
local playerWhoGeneratedBlips = nil

function cacheIsEmpty()
    for _, _ in pairs(cache) do
        return false
    end
    return true
end

function sendBlips(res)
    -- Restrict the origin if set, otherwise allow everyone
    res.writeHead(200, { ["Access-Control-Allow-Origin"] = GetConvar("livemap_access_control", "*")} )
    
    if not cacheIsEmpty() then
        res.send(json.encode(cache))
        return
    else
        -- Send cached blips
        local blipFile = GetConvar("blip_file", "server/blips.json")
        local blipData = LoadResourceFile(GetCurrentResourceName(), blipFile)

        if not blipData then
            print("LIVE MAP ERROR: No blip file... Have you generated them yet?")
            res.send(json.encode({ error = "no blips found" }) )
            return
        end

        cache = json.decode(blipData)
        res.send(blipData)
    end
end

function saveBlips(blipTable)
    local blipFile = GetConvar("blip_file", "server/blips.json")
    local saved = SaveResourceFile(GetCurrentResourceName(), blipFile, json.encode(blipTable, {indent = true}), -1)

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

    saveBlips(blipTable)
    cache = blipTable
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
