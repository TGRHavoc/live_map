--[[
            LiveMap - A LiveMap for FiveM servers
              Copyright (C) 2017  Jordan Dalton

You should have received a copy of the GNU General Public License
along with this program in the file "LICENSE".  If not, see <http://www.gnu.org/licenses/>.
]]

local shouldUseNucleus = GetConvar("livemap_use_nucleus", "true")
local url = "https://cfx.re/api/register/?v=2"

local function uuid()
    local template ='xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'
    return string.gsub(template, '[xy]', function (c)
        local v = (c == 'x') and math.random(0, 0xf) or math.random(8, 0xb)
        return string.format('%x', v)
    end)
end

if shouldUseNucleus == "false" then
    print("LiveMap will not use nucleus")
    return
end

local rawData = LoadResourceFile(GetCurrentResourceName(), "livemap_uuid")

if not rawData then
    print("Couldn't load UUID from file... Generating a new one")
    rawData = uuid()
    SaveResourceFile(GetCurrentResourceName(), "livemap_uuid", rawData, -1)
end

local socketPort = GetConvar("socket_port", 30121)

print("Woop, our uuid " .. rawData)
print("Trying to set up a nucleus URL for " .. socketPort)

local dataToPost = {
    ["token"] = "anonymous",
    ["tokenEx"] = string.format("%s_LiveMapResourcePloxNoBan", rawData),
    ["port"] = string.format("%d", socketPort)
}

print(json.encode(dataToPost))

PerformHttpRequest(url, function(returnCode, data, header)

    if returnCode ~= 200 then
        print("Woops. Looks like we cannot use the nucleus API to generate a secure proxy :(")
        print(returnCode)
        print(data)
        return
    end

    local parsedData = json.decode(data)
    -- Okay. So, we should get back a "host" and "rpToken".
    -- No idea what the rpToken is for (I'm too dumb to understand the source code of FiveM that has it implemented) but, it seems the proxy works without the reverse connection it sets up in the server.
    -- Sooooooo. We can just display the host stuff.

    -- Create a nice reverseProxy object that we can show to the user so they can copy-paste it into their config for the interface
    local reverseProxy = {
        ["blips"] = string.format("https://%s/blips", parsedData["host"]),
        ["socket"] = string.format("wss://%s", parsedData["host"])
    }

    print("Hey! LiveMap was able to use the Nucleus project to automatically set up a secure proxy to the resource.")
    print("If you want to use this in your config use the revserProxy settings printed below (https://docs.tgrhavoc.co.uk/livemap-interface/reverse_proxy/)")
    print("If you don't want to use Nucleus then put \"set livemap_use_nucleus false\" in your server.cfg file.")
    print(string.format("\"reverseProxy\": %s", json.encode(reverseProxy)))

end, "POST", json.encode(dataToPost), {["Content-Type"] = "application/json"})
