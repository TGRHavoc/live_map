--[[
    This file is responsible for making sure that the client's identifiers exist on the websocket server.
    If this doesn't run then the interface will just be sent the player's position with no way to associate it with a player.
    Then, it won't show the player. And you'll complain.
    So... 
]]
local firstSpawn = true

--[[
    When the player spawns, make sure we set their ID in the data that is going
        to be sent via sockets.
]]
AddEventHandler("playerSpawned", function(spawn)
    if firstSpawn then
        TriggerServerEvent("livemap:playerSpawned") -- Set's the ID in "playerData" so it will get sent va sockets
        firstSpawn = false
    end
end)