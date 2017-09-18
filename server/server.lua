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
    Blips!!!
    This file should expose the "blips.json" file to that sexy new http handler
]]

local cache = {}

function cacheIsEmpty()
    for _, _ in pairs(cache) do
        return true
    end
    return false
end

local sendBlips(res)

end

SetHttpHandler(function(req, res)
	local path = req.path

end)
