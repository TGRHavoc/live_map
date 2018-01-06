resource_manifest_version "44febabe-d386-4d18-afbe-5e627f4af937"

client_scripts{
    "client/client.lua",
    "client/reverse_weapon_hashes.lua",
    "client/reverse_car_hashes.lua",
    "client/blips_client.lua"
}

exports{
    "reverseWeaponHash",
    "reverseVehicleHash"
}

server_scripts{
    "server/update_check.lua",
    "server/live_map.net.dll",
    "server/wrapper.lua",
    "server/blips_server.lua"
}
