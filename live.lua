-- The port the websocket will listen on (change if needed)
local liveMapPort = 30121

-- Load our library
clr.System.Reflection.Assembly.LoadFrom("resources/live_map/libs/Live Map.dll")

print("creating websocket")
local liveMap = clr.Havoc.Live_Map.LiveMap(liveMapPort) -- Start the websocket

print("nil :" .. (liveMap == nil))

-- When the resource is stoped, close the websockett
AddEventHandler('onResourceStop', function(resourceName)
    if resourceName == GetInvokingResource() then
        liveMap.stop()
    end
end)
-- Keep the locations in the DLL up-to-date with the player's actual location
RegisterServerEvent("live_map:updatePositions")
AddEventHandler("live_map:updatePositions", function(newX, newY, newZ)
    liveMap.addPlayer(GetPlayerName(source), newX, newY, newZ) -- Adds or updates the player's location
end)
-- When a player disconnects, make sure the websocket knows..
AddEventHandler("playerDropped", function()
    liveMap.removePlayer(GetPlayerName(source))
end)

-- Send the blips from blips.lua to the DLL.
-- Allows our map to show the blips when it's loaded.
local enc = json.encode(BLIPS)
liveMap.addBlips( enc )

liveMap.start()
