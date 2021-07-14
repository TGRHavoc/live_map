--[[
    This file is responsible for updating the player's position and location string
]]
-- Our player's actual location co-ordinates and location string
local lastKnownPosition = {
    ["pos"] = {
        x = 0, 
        y = 0, 
        z = 0
    },
    ["Location"] = "Unknown"
}

-- What data have we updated? Used to only send the updated values to the socket 
local updatedData = {}

-- Temporary data store for the location string. Since it's composed of three parts, we want to check to see if any of them have changed in order to update the string
local temp = {}

-- Pop what we updated into the array and update it's value.
function updateData(name, value)
    table.insert(updatedData, name)
    lastKnownPosition[name] = value
end


Citizen.CreateThread(function()
    while true do
        Wait(10)

        if NetworkIsPlayerActive(PlayerId()) then

            -- Update position, if it has changed
            local x,y,z = table.unpack(GetEntityCoords(PlayerPedId()))
            local x1,y1,z1 = lastKnownPosition["pos"].x, lastKnownPosition["pos"].y, lastKnownPosition["pos"].z

            local dist = Vdist(x, y, z, x1, y1, z1)

            if (dist >= 5) then
                -- Update every 5 meters.. Let's reduce the amount of spam
                -- TODO: Maybe make this into a convar (e.g. accuracy_distance)
                updateData("pos", {x = x, y=y, z=z})

                -- Reverse the street, area and zone of the current location
                local streetname = exports[GetCurrentResourceName()]:reverseStreetHash(GetStreetNameAtCoord(x,y,z))
                local zone = exports[GetCurrentResourceName()]:reverseZoneHash(GetHashKey(GetNameOfZone(x, y, z)))
                local area = exports[GetCurrentResourceName()]:reverseAreaHash(GetHashOfMapAreaAtCoords(x, y, z))

                if (temp["streetname"] ~= streetname) or (temp["zone"] ~= zone) or (temp["area"] ~= area) then
                    local locationString = string.format("%s, %s (%s)", streetname, zone, area)

                    updateData("Location", locationString)
                    temp["streetname"] = streetname
                    temp["zone"] = zone
                    temp["area"] = area
                end

            end

            -- Make sure the updated data is up-to-date on socket server as well
            for i,k in pairs(updatedData) do
                --Citizen.Trace("Updating " .. k)
                TriggerServerEvent("livemap:UpdatePlayerData", k, lastKnownPosition[k])
                table.remove(updatedData, i)
            end

        end
    end
end)
