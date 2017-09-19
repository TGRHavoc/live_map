--[[
            LiveMap - A LiveMap for FiveM servers
              Copyright (C) 2017  Jordan Dalton

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program in the file "LICENSE".  If not, see <http://www.gnu.org/licenses/>.
]]


--[[
    This file is an example. You can create your own resource that does something
        similar but, updates diifferent values (if you want).
    This is literally to show you how to use the thing
]]

-- This is the data that is gooing to be sent over websockets
local defaultDataSet = {
    ["pos"] = { x=0, y=0, z=0 }, -- Position of the player (vector(x, y, z))
    ["Vehicle"] = "none", -- Vehicle player is in (if any)
    ["Weapon"] = "Unarmed", -- Weapon player has equiped (if any)
    ["icon"] = 6, -- Player blip id (will change with vehicles)
    ["Licence Plate"] = nil -- To showcase the removal off data :D
}

local temp = {}

-- Table to keep track of the updated data
local beenUpdated =  {}

function updateData(name, value)
    table.insert(beenUpdated, name)
    defaultDataSet[name] = value
end

local firstSpawn = true

RegisterNetEvent("chatMessage")
AddEventHandler('chatMessage', function(author, color, text)
    if text == "kill" then
        Citizen.Trace("Killing player")
        SetEntityHealth(GetPlayerPed(-1), 0)
        CancelEvent()
    end
end)

--[[
    When the player spawns, make sure we set their ID in the data that is going
        to be sent via sockets.
]]

AddEventHandler("playerSpawned", function(spawn)
    if firstSpawn then
        TriggerServerEvent("livemap:playerSpawned") -- Set's the ID in "playerData" so it will get send va sockets

        -- Now send the default data set
        for key,val in pairs(defaultDataSet) do
            TriggerServerEvent("livemap:AddPlayerData", key, val)
        end

        firstSpawn = false
    end

    -- TEMP
    local weapons = { "WEAPON_PISTOL50", "WEAPON_STUNGUN", "WEAPON_NIGHTSTICK", "WEAPON_PUMPSHOTGUN", "WEAPON_FLAREGUN", "WEAPON_ASSAULTSMG" }

    Citizen.CreateThread(function()
        for _,w in pairs(weapons) do
            GiveWeaponToPed(GetPlayerPed(-1), GetHashKey(w), 1000, 0, false)
        end
    end)
end)

function doVehicleUpdate()
    local vehicle = GetVehiclePedIsIn(GetPlayerPed(-1), 0)

    if temp["vehicle"] ~= vehicle and vehicle ~= 0 then
        -- Update it
        local vehicleClass = GetVehicleClass(vehicle)
        updateData("Vehicle", GetDisplayNameFromVehicleModel(GetEntityModel(vehicle)))
        temp["vehicle"] = vehicle
    end

    local plate = GetVehicleNumberPlateText(vehicle)

    if defaultDataSet["Licence Plate"] ~= plate then
        Citizen.Trace("Updating plate: " .. plate)
        updateData("Licence Plate", plate)
    end
end

function doIconUpdate()
    local ped = GetPlayerPed(-1)
    local newSprite = 6 -- Default to the player one

    if IsEntityDead(ped) then
        newSprite = 163 -- Using GtaOPassive since I don't have a "death" icon :(
    else
        if IsPedSittingInAnyVehicle(ped) then
            -- Change icon to vehicle
            -- our temp table should still have the latest vehicle
            local vehicle = temp["vehicle"]
            local vehicleModel = GetEntityModel(vehicle)
            local h = GetHashKey

            if vehicleModel == h("rhino") then
                newSprite = 421
            elseif (vehicleModel == h("lazer") or vehicleModel == h("besra") or vehicleModel == h("hydra")) then
                newSprite = 16 -- Jet
            elseif IsThisModelAPlane(vehicleModel) then
                newSprite = 90 -- Airport (plane icon)
            elseif IsThisModelAHeli(vehicleModel) then
                newSprite = 64 -- Helicopter
            elseif (vehicleModel == h("technical") or vehicleModel == h("insurgent") or vehicleModel == h("insurgent2") or vehicleModel == h("limo2")) then
                newSprite = 426 -- GunCar
            elseif (vehicleModel == h("dinghy") or vehicleModel == h("dinghy2") or vehicleModel == h("dinghy3")) then
                newSprite = 404 -- Dinghy
            elseif (vehicleModel == h("submersible") or vehicleModel == h("submersible2")) then
                newSprite = 308 -- Sub
            elseif IsThisModelABoat(vehicleModel) then
                newSprite = 410
            elseif (IsThisModelABike(vehicleModel) or IsThisModelABicycle(vehicleModel)) then
                newSprite = 226
            elseif (vehicleModel == h("policeold2") or vehicleModel == h("policeold1") or vehicleModel == h("policet") or vehicleModel == h("police") or vehicleModel == h("police2") or vehicleModel == h("police3") or vehicleModel == h("policeb") or vehicleModel == h("riot") or vehicleModel == h("sheriff") or vehicleModel == h("sheriff2") or vehicleModel == h("pranger")) then
                newSprite = 56 -- PoliceCar
            elseif vehicleModel == h("taxi") then
                newSprite = 198
            elseif (vehicleModel == h("brickade") or vehicleModel == h("stoackage") or vehicleModel == h("stoackage2")) then
                newSprite = 66 -- ArmoredTruck
            elseif (vehicleModel == h("towtruck") or vehicleModel == h("towtruck")) then
                newSprite = 68
            elseif (vehicleModel == h("trash") or vehicleModel == h("trash2")) then
                newSprite = 318
            else
                newSprite = 225 -- PersonalVehicleCar
            end
        end
    end

    if defaultDataSet["icon"] ~= newSprite then
        updateData("icon", newSprite)
    end

end

Citizen.CreateThread(function()
    while true do
        Wait(10)

        -- Update position, if it has changed
        local x,y,z = table.unpack(GetEntityCoords(GetPlayerPed(-1)))
        local x1,y1,z1 = defaultDataSet["pos"].x, defaultDataSet["pos"].y, defaultDataSet["pos"].z

        local dist = Vdist(x, y, z, x1, y1, z1)

        if (dist >= 5) then
            -- Update every 5 meters.. Let's reduce the amount of spam
            -- TODO: Maybe make this into a convar (e.g. accuracy_distance)
            updateData("pos", {x = x, y=y, z=z} )
        end

        -- Update weapons
        local found,weapon = GetCurrentPedWeapon(GetPlayerPed(-1), true)
        if found and temp["weapon"] ~= weapon then
            local weaponName = exports[GetCurrentResourceName()]:reverseWeaponHash(tostring(weapon))
            updateData("Weapon", weaponName)
            -- To make sure we don't call this more than we need to
            temp["weapon"] = weapon
        end

        -- Update Vehicle (and icon)
        if IsPedInAnyVehicle(GetPlayerPed(-1)) then
            doVehicleUpdate()

        elseif defaultDataSet["Licence Plate"] ~= nil or defaultDataSet["Vehicle"] ~= nil then
            -- No longer in a vehicle, remove "Licence Plate" if present
            defaultDataSet["Licence Plate"] = nil
            defaultDataSet["Vehicle"] = nil
            temp["vehicle"] = nil
            -- Remove it from socket communication
            TriggerServerEvent("livemap:RemovePlayerData", "Licence Plate")
            TriggerServerEvent("livemap:RemovePlayerData", "Vehicle")
        end

        doIconUpdate()

        -- Make sure the updated data is up-to-date on socket server as well
        for i,k in pairs(beenUpdated) do
            Citizen.Trace("Updating " .. k)
            TriggerServerEvent("livemap:UpdatePlayerData", k, defaultDataSet[k])
            table.remove(beenUpdated, i)
        end
    end
end)
