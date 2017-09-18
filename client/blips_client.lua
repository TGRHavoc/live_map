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
    --TODO: Get blips

    Citizen.Trace("Generating blip table.. This may take a while")
    local blipTable = {}

    -- Loop through ALL the fucking blips
    for _,spriteId in pairs(blip_types) do
        local blip = GetFirstBlipInfoId(spriteId)

        if DoesBlipExist(blip) then -- If the first blip exists, we're going to need an entry 
            blipTable[spriteId] = {}
        end
        while (DoesBlipExist(blip)) do

            local x,y,z = table.unpack(GetBlipCoords(blip))
            Citizen.Trace("Blip1: " .. x .. " " .. y .. " " .. z .. "")

            table.insert(blipTable[spriteId], {
                x = x, y = y, z = z
            })

            blip = GetNextBlipInfoId(spriteId)
        end
    end

    Citizen.Trace("Generated the blips!")
    TriggerServerEvent("livemap:blipsGenerated", blipTable)
end)
