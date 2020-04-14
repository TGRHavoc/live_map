fx_version "bodacious"
game "gta5"

author "TGR_Havoc"
description ""
version "1.0.0"

client_scripts{
    "client/client.lua",
    "client/reverse_weapon_hashes.lua",
    "client/reverse_car_hashes.lua",
    "client/reverse_location_hashes.lua",
    "client/blips_client.lua"
}

exports {
    "reverseWeaponHash",
    "reverseVehicleHash",
    "reverseStreetHash",
    "reverseZoneHash",
    "reverseAreaHash"
}

server_scripts{
    --"src/server.js",
    "server/sse.lua",
    "server/update_check.lua"
}
