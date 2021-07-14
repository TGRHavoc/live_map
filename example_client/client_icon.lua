--[[
    This file is respoinsible for updating the player's icon.
    If they're dead, show the "dead" icon.
    If they're in a vehicle, show the vehicle icon etc, etc, etc.
]]
local currentIcon = 6
local temp = {}

function doIconUpdate()
    local ped = PlayerPedId()
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
            elseif (vehicleModel == h("brickade") or vehicleModel == h("stockade") or vehicleModel == h("stockade2")) then
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

    if currentIcon ~= newSprite then
        currentIcon = newSprite
        TriggerServerEvent("livemap:UpdatePlayerData", "icon", newSprite)
    end
end


Citizen.CreateThread(function()
    TriggerServerEvent("livemap:UpdatePlayerData", "icon", 6)
    while true do
        Wait(10)

        if NetworkIsPlayerActive(PlayerId()) then
            doIconUpdate()
        end

    end
end)
