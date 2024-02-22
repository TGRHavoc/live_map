
--[[
    LiveMap configuration file.
]]

livemap_debug "trace" -- Log level for LiveMap. Possible values: trace/[all], debug, info, warn, error, critical, off/anything else
livemap_blipFile "blips.json" -- File to load blips from
livemap_accessControl "*" -- Access control for LiveMap.

fx_version "bodacious"
game "gta5"

author "TGR_Havoc"
description ""
version "4.0.0-1"
repository "https://github.com/TGRHavoc/live_map"

mono_rt2 'Prerelease expiring 2023-06-30. See https://aka.cfx.re/mono-rt2-preview for info.'

-- client_script "example_client/*.lua"
-- exports {
--     "reverseWeaponHash", "reverseVehicleHash", "reverseStreetHash",
--     "reverseZoneHash", "reverseAreaHash"
-- }

-- Don't remove. Blips_client is needed for the `blips generate` command to work.
--client_script "example_client/blips_client.lua"

-- server_scripts {
--     "server/update_check.lua",
--     "server/setup_nucleus.lua",
--     "dist/livemap.js"
-- }

server_scripts {
    "dist/CitizenFX.FiveM.Native.dll",
    "dist/*.net.dll"
}
