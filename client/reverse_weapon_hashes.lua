--[[
            LiveMap - A LiveMap for FiveM servers
              Copyright (C) 2017  Jordan Dalton

You should have received a copy of the GNU General Public License
along with this program in the file "LICENSE".  If not, see <http://www.gnu.org/licenses/>.
]]
local WEAPON_HASHES = {
    ["2578778090"] = "Knife",
    ["1737195953"] = "Nightstick",
    ["1317494643"] = "Hammer",
    ["2508868239"] = "Bat",
    ["1141786504"] = "Golf Club",
    ["2227010557"] = "Crowbar",
    ["4192643659"] = "Bottle",
    ["3756226112"] = "Switch Blade",
    ["453432689"] = "Pistol",
    ["1593441988"] = "Combat Pistol",
    ["584646201"] = "AP Pistol",
    ["2578377531"] = "Pistol 50",
    ["1198879012"] = "Flare Gun",
    ["3696079510"] = "Marksman Pistol",
    ["3249783761"] = "Revolver",
    ["324215364"] = "Micro SMG",
    ["736523883"] = "SMG",
    ["4024951519"] = "Assault SMG",
    ["171789620"] = "Combat PDW",
    ["3220176749"] = "Assault Rifle",
    ["2210333304"] = "Carbine Rifle",
    ["2937143193"] = "Advanced Rifle",
    ["1649403952"] = "Compact Rifle",
    ["2634544996"] = "MG",
    ["2144741730"] = "Combat MG",
    ["487013001"] = "Pump Shotgun",
    ["2017895192"] = "Sawn Off Shotgun",
    ["3800352039"] = "Assault Shotgun",
    ["2640438543"] = "Bullpup Shotgun",
    ["4019527611"] = "Double Barrel Shotgun",
    ["911657153"] = "Stun Gun",
    ["100416529"] = "Sniper Rifle",
    ["205991906"] = "Heavy Sniper",
    ["2726580491"] = "Grenade Launcher",
    ["1305664598"] = "Grenade Launcher Smoke",
    ["2982836145"] = "RPG",
    ["1119849093"] = "Minigun",
    ["2481070269"] = "Grenade",
    ["741814745"] = "Sticky Bomb",
    ["4256991824"] = "Smoke Grenade",
    ["2694266206"] = "BZ Gas",
    ["615608432"] = "Molotov",
    ["101631238"] = "Fire Extinguisher",
    ["883325847"] = "Petrol Can",
    ["3218215474"] = "SNS Pistol",
    ["3231910285"] = "Special Carbine",
    ["3523564046"] = "Heavy Pistol",
    ["2132975508"] = "Bullpup Rifle",
    ["1672152130"] = "Homing Launcher",
    ["2874559379"] = "Proximity Mine",
    ["126349499"] = "Snowball",
    ["137902532"] = "Vintage Pistol",
    ["2460120199"] = "Dagger",
    ["2138347493"] = "Firework",
    ["2828843422"] = "Musket",
    ["3342088282"] = "Marksman Rifle",
    ["984333226"] = "Heavy Shotgun",
    ["1627465347"] = "Gusenberg",
    ["4191993645"] = "Hatchet",
    ["1834241177"] = "Railgun",
    ["2725352035"] = "Unarmed",
    ["3638508604"] = "Knuckle Duster",
    ["3713923289"] = "Machete",
    ["3675956304"] = "Machine Pistol",
    ["2343591895"] = "Flashlight",
    ["600439132"] = "Ball",
    ["1233104067"] = "Flare",
    ["2803906140"] = "Night Vision",
    ["4222310262"] = "Parachute",
    ["317205821"] = "Sweeper Shotgun",
    ["3441901897"] = "Battle Axe",
    ["125959754"] = "Compact Grenade Launcher",
    ["3173288789"] = "Mini SMG",
    ["3125143736"] = "Pipe Bomb",
    ["2484171525"] = "Pool Cue",
    ["419712736"] = "Wrench"
}

function reverseWeaponHash(hash)
    if type(hash) ~= "string" then
        hash = tostring(hash)
    end
    
    local name = WEAPON_HASHES[hash]
    if name ~= nil then
        return name
    end

    Citizen.Trace("Error reversing weapon hash \"" .. hash .. "\". Maybe it's not been added yet?")
    return "Unarmed"
end
