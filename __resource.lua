resource_manifest_version "44febabe-d386-4d18-afbe-5e627f4af937"

client_scripts{
    "client/client.lua",
    "client/reverse_weapon_hashes.lua"
}

exports{
    "reverseWeaponHash"
}

server_scripts{
    "server/live_map.net.dll",
    "server/wrapper.lua"
}
