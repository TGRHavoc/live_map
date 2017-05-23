-- The port the websocket will listen on (change if needed)
local liveMapPort = 30121
local useSsl = false

-- Load our library
clr.System.Reflection.Assembly.LoadFrom("resources/live_map/libs/Live Map.dll")

print("creating websocket")
local liveMap = clr.Havoc.Live_Map.LiveMap(liveMapPort, useSsl) -- Start the websocket

-- When the resource is stoped, close the websockett
AddEventHandler('onResourceStop', function(resourceName)
    if resourceName == GetInvokingResource() then
        liveMap.stop()
    end
end)
-- Keep the locations in the DLL up-to-date with the player's actual location
RegisterServerEvent("live_map:updatePositions")
AddEventHandler("live_map:updatePositions", function(newX, newY, newZ)
    local ids = GetPlayerIdentifiers(source)
    --[[
        below could be written as
        liveMap.addFloat(ids[1], "x", newX)
        liveMap.addFloat(ids[1], "y", newY)
        liveMap.addFloat(ids[1], "z", newZ)
        liveMap.addString(ids[1], "name", GetPlayerName(source))
    ]]
    liveMap.addPlayer(ids[1], GetPlayerName(source), newX, newY, newZ) -- Adds or updates the player's location
end)

RegisterServerEvent("live_map:addBlip")
AddEventHandler("live_map:addBlip", function(blipName, blipDesc, blipType, x, y, z)
    print("checking type: " .. type(blipType))

    if type(blipType) ~= "string" and type(blipType) == "number" then
        print("blipType " .. blipType .. " is a number.. Converting")
        blipType = BLIP_IDS[blipType]
    end
    -- blipType should now be a string we can use..
    print("live_map: Adding blip to map. Name=\"".. blipName .. "\", type=\"" .. blipType .. "\"")
    liveMap.addBlip(blipName, blipDesc, blipType, x, y, z)
end)

RegisterServerEvent("live_map:clientAddString")
AddEventHandler("live_map:clientAddString", function(key, s)
    TriggerEvent("live_map:addString", source, key, s)
end)
RegisterServerEvent("live_map:clientAddFloat")
AddEventHandler("live_map:clientAddFloat", function(key, f)
    TriggerEvent("live_map:addFloat", source, key, f)
end)

AddEventHandler("live_map:addString", function(plrId, key, s)
    local ids = GetPlayerIdentifiers(plrId)

    print("live_map: Adding string with key \"" .. key .. "\" and val of \"" .. s .. "\"")
    liveMap.addPlayerString(ids[1], key, s)
end)

AddEventHandler("live_map:addFloat", function(plrId, key, f)
    local ids = GetPlayerIdentifiers(plrId)
    print("id: " .. ids[1])
    print("live_map: Adding float with key \"" .. key .. "\" and val of \"" .. f .. "\"")
    liveMap.addPlayerFloat(ids[1], key, f)
end)

-- When a player disconnects, make sure the websocket knows..
AddEventHandler("playerDropped", function()
    local ids = GetPlayerIdentifiers(source)
    liveMap.removePlayer(ids[1])
end)

-- Send the blips from blips.lua to the DLL.
-- Allows our map to show the blips when it's loaded.
local enc = json.encode(BLIPS)
liveMap.addBlips( enc )

liveMap.start()
