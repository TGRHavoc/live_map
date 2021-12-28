fx_version "bodacious"
game "gta5"

author "TGR_Havoc"
description ""
version "3.2.1"

client_script "example_client/*.lua"
exports {
    "reverseWeaponHash", "reverseVehicleHash", "reverseStreetHash",
    "reverseZoneHash", "reverseAreaHash"
}

-- Don't remove. Blips_client is needed for the `blips generate` command to work.
client_script "example_client/blips_client.lua"

server_scripts {
    "server/update_check.lua",
    "server/setup_nucleus.lua",
    "dist/livemap.js"
}
