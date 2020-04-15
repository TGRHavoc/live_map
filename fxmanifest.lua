fx_version "bodacious"
game "gta5"

clr_disable_task_scheduler "yes"

author "TGR_Havoc"
description ""
version "2.3.2"

--[[ If you want to use the example client, delete this line
client_script "example_client/*.lua"
exports {
    "reverseWeaponHash",
    "reverseVehicleHash",
    "reverseStreetHash",
    "reverseZoneHash",
    "reverseAreaHash"
}
]] -- Remove this line as well

server_scripts{
    "server/update_check.lua",
    "src/live_map/**/publish/*.net.dll"
}