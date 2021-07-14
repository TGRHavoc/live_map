--[[
    If the player has a weapon, set what it is and send it to the interface
]]
local temp = {}

Citizen.CreateThread(function()
    while true do
        Wait(10)

        if NetworkIsPlayerActive(PlayerId()) then

            -- Update weapons
            local found,weapon = GetCurrentPedWeapon(PlayerPedId(), true)
            if found and temp["weapon"] ~= weapon then
                local weaponName = exports[GetCurrentResourceName()]:reverseWeaponHash(weapon)
                TriggerServerEvent("livemap:UpdatePlayerData", "Weapon", weaponName)

                -- To make sure we don't call this more than we need to
                temp["weapon"] = weapon
            end

        end

    end
end)
