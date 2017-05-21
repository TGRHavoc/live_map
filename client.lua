local oldPos

Citizen.CreateThread(function()
	while true do
		Citizen.Wait(250)
		local pos = GetEntityCoords(GetPlayerPed(-1))

		if(oldPos ~= pos)then
			TriggerServerEvent('live_map:updatePositions', pos.x, pos.y, pos.z)
			oldPos = pos
		end
	end
end)