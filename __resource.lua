resource_manifest_version "44febabe-d386-4d18-afbe-5e627f4af937"

dependency "yarn"
dependency "webpack"
webpack_config "webpack.config.js"

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
    "dist/livemap.js",
    "server/update_check.lua"
}
