local oldPos

Citizen.CreateThread(function()
	while true do
		Citizen.Wait(250)
		local pos = GetEntityCoords(GetPlayerPed(-1))

		if(oldPos ~= pos)then
			TriggerServerEvent('live_map:updatePositions', pos.x, pos.y, pos.z)
			oldPos = pos
		end

        sendVehicleData()

	end
end)

-- Example of extra data being attached to the player data
local lastVehicle = "normal"

function sendVehicleData()
    local currentVehicle = "normal"
    local vehicle = GetVehiclePedIsIn(GetPlayerPed (-1), false)
    --Citizen.Trace("Veh: " .. vehicle)
    if vehicle and vehicle ~= 0 then
        local vehicleClass = GetVehicleClass(vehicle)
        --Citizen.Trace("Vehicle class: " .. vehicleClass)

        if vehicleClass == 0 then -- Don't know if this would ever happen..
            currentVehicle = "normal"
        elseif vehicleClass == 9 then
            currentVehicle = "personalBike"
        elseif vehicleClass == 15 then
            currentVehicle = "boat"
        elseif vehicleClass == 16 then
            currentVehicle = "helicopter"
        elseif vehicleClass == 17 then
            currentVehicle = "plane"
        else
            -- Default to car
            currentVehicle = "personalCar"
        end
    else
        currentVehicle = "normal"
    end

    if lastVehicle ~= currentVehicle then
        -- Vehicle changed... Update it
        Citizen.Trace("Changing vehicle to " .. currentVehicle)
        if currentVehicle ~= "normal" then
            local vehicleName = GetDisplayNameFromVehicleModel(GetEntityModel(vehicle))
            TriggerServerEvent("live_map:clientAddString", "vehicle_name", vehicleName)
        else
            TriggerServerEvent("live_map:clientAddString", "vehicle_name", "")
        end

        TriggerServerEvent("live_map:clientAddString", "vehicle", currentVehicle)
        lastVehicle = currentVehicle
    end

end
