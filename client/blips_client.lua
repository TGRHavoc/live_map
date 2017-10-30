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
    A utility event that is triggered from the server to generate the blips.
    This will get all the blips that are shown on the map.
]]
RegisterNetEvent("livemap:getBlipsFromClient")

local blip_types = {
    1,16,36,38,40,43,47,50,51,52,
    56,60,61,64,65,66,68,71,72,73,75,76,77,78,79,84,85,86,88,89,90,93,94,
    100,102,103,106,108,109,110,112,119,120,121,122,126,128,134,135,136,140,137,
    147,149,150,151,152,153,154,155,156,157,158,159,160,162,163,164,173,174,175,176,184,188,197,198,
    205,206,225,226,237,251,266,267,269,273,279,280,
    304,305,306,307,308,309,310,311,313,314,315,316,318,350,351,352,354,355,356,
    357,358,359,360,361,362,364,365,366,367,368,369,370,371,372,374,375,376,377,378,379,380,381,382,383,384,385,386,387,388,389,
    400,401,402,403,404,405,408,409,410,411,415,419,420,421,426,427,428,430,431,432,434
}

AddEventHandler("livemap:getBlipsFromClient", function()
    Citizen.Trace("Generating blip table.. This may take a while for a large amount of blips")
    local blipTable = {}

    -- Loop through ALL the fucking blips
    for _,spriteId in pairs(blip_types) do
        local blip = GetFirstBlipInfoId(spriteId)

        if DoesBlipExist(blip) then -- If the first blip exists, we're going to need an entry
            blipTable[spriteId] = {}
        end
        while (DoesBlipExist(blip)) do

            local x,y,z = table.unpack(GetBlipCoords(blip))

            -- Damn! There's no way to get the fucking display text for the blip :(
            table.insert(blipTable[spriteId], {
                x = tonumber(string.format("%.2f", x)), -- Round them to 2dp
                y = tonumber(string.format("%.2f", y)),
                z = tonumber(string.format("%.2f", z))
            })

            blip = GetNextBlipInfoId(spriteId)
        end
    end

    Citizen.Trace("Generated the blips!")
    TriggerServerEvent("livemap:blipsGenerated", blipTable)
end)

RegisterCommand("blip", function(source, args, raw)

    for i=1, #args do
        print(i .. " = " .. args[i] .. " " .. type(args[i]))
    end

    if(args[1] == "add") then
        local pos = GetEntityCoords( PlayerPedId(), not IsPlayerDead(PlayerId()) )
        local spriteId = tonumber(args[2])

        if spriteId == nil then
            print("Sorry, spriteId must be a number")
            return
        end
        local name, description = "", ""
        if args[3] then
            name = args[3]
        end
        if args[4] then
            description = args[4]
        end

        print(spriteId .. " " .. name .. " " .. description .. " " .. pos.x .. " " .. pos.y .. " " .. pos.z)

        local blip = {
            sprite = spriteId,
            pos = {
                x = pos.x,
                y = pos.y,
                z = pos.z
            },
            name = name,
            description = description
        }

        TriggerServerEvent("livemap:AddBlip", blip)

    elseif(args[1] == "remove") then
        local pos = GetEntityCoords(PlayerPedId(), not IsPlayerDead(PlayerId()))
        
        TriggerServerEvent("livemap:RemoveClosestBlip", {x = pos.x, y = pos.y, z = pos.z})

    elseif(args[1] == "update") then

    else
        print("Sorry, the available blip arguments are as follows:")
        print("add <spriteId> [name] [descriptionn] -- Adds a blip at your current position")
        print("remove [radius] -- Removed the closes blip within the radius")
        print("update <spriteId> <name> <description> -- Updates the closes blip with this data")
    end
end, true)
