--[[
    This file is responsible for updating the player's vehicle information.
    What type and license plate
]]
local data = {
    ["Vehicle"] = "",
    ["License Plate"] = ""
}

local temp = {}


function doVehicleUpdate()
    local vehicle = GetVehiclePedIsIn(PlayerPedId(), 0)

    if temp["vehicle"] ~= vehicle and vehicle ~= 0 then
        -- Update it
        local vehicleClass = GetVehicleClass(vehicle)

        -- Added reverseWeaponHash to the vehicle display name to convert the vehicle name to a nicer version.
        -- To change, modify the "reverse_car_hashes.lua" file
        local vehNameHash = GetHashKey(GetDisplayNameFromVehicleModel(GetEntityModel(vehicle)))
        local reversedVehicleName = exports[GetCurrentResourceName()]:reverseVehicleHash(vehNameHash)

        data["Vehicle"] = reversedVehicleName
        TriggerServerEvent("livemap:UpdatePlayerData", "Vehicle", reversedVehicleName)

        temp["vehicle"] = vehicle
    end

    local plate = GetVehicleNumberPlateText(vehicle)
    plate = plate:gsub("^%s*(.-)%s*$", "%1") -- Remove whitespace at the start and end of the plate (for plate check)

    if plate == "FIVE M" then
        plate = plate .. " (Spawned In)"
    end

    if data["License Plate"] ~= plate then
        data["License Plate"] =  plate
        TriggerServerEvent("livemap:UpdatePlayerData", "License Plate", plate)
    end
end

Citizen.CreateThread(function()
    while true do
        Wait(10)

        if NetworkIsPlayerActive(PlayerId()) then

            -- Update Vehicle (and icon)
            if IsPedInAnyVehicle(PlayerPedId()) then
                doVehicleUpdate()

            elseif data["License Plate"] ~= nil or data["Vehicle"] ~= nil then
                -- No longer in a vehicle, remove "License Plate" if present
                data["License Plate"] = nil
                data["Vehicle"] = nil
                temp["vehicle"] = nil

                -- Remove it from socket communication
                TriggerServerEvent("livemap:RemovePlayerData", "License Plate")
                TriggerServerEvent("livemap:RemovePlayerData", "Vehicle")
            end
        end
    end
end)
