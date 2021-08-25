fx_version "bodacious"
game "gta5"

author "TGR_Havoc"
description ""
version "3.1.4"

--[[
    Hey!
    If you want to use the example client then, copy the "example_client" folder and rename it "client".
    Then you can uncomment the stuff below.

    This will allow you to update the resource without loosing your changes to the client code (if you've done any).
    Sorry if it's inconvenient for you. But, it's a massive ball ache if you loose your changes when updating so... I feel the benifits outweigh the ball ache.
]]

client_script "client/*.lua"
exports {
    "reverseWeaponHash", "reverseVehicleHash", "reverseStreetHash",
    "reverseZoneHash", "reverseAreaHash"
}

-- Don't remove. Blips_client is needed for the `blips generate` command to work.
client_script "example_client/blips_client.lua"

server_scripts {
    "server/update_check.lua",
    "dist/livemap.js"
}
