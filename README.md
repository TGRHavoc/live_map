# fivem-live_map

This is the "backend" code for the live_map addon for FiveM.

## How to install

Download the ZIP file. And extract the contents into `resources/live_map/`.

Add `live_map` to your `AutoStartResources` list in the "citmp-server.yml" file.


## Configuration

At the moment you don't need to configure much to get this addon working.

#### live.lua

Inside the `live.lua` file you can change the port the websocket server listen on and whether you want to use SSL (more on how to configure this later).

```lua
local liveMapPort = 30121 -- Change to the port you want the websocket to listen on (must not be in use)
local useSsl = false -- Set to true if you want to use WSS (secure websocket)
```

#### blips.lua

At the moment, this file contains all the blips you want to appear on the map when it's loaded in the browser.

All blips in this file __must__ follow the format of `{name="NAME", type="TYPE", x=-0, y=0.000, z=0}`.

```
name - The name of the blip (shown when clicked on the map)
description (optional) - Provide a description of what the blip represents
type - The type of the blip (determines the icon shown on the map)
x,y,z = The location of the blip in game
```

#### blip_helper.lua
This file is just used to translate the integer blip types to a string which can be used on the web interface. It's recommended that you don't touch this file.


#### Using SSL
You cannot use SSL straight a way, you need to recompile the source code of the library to use it. Below you can find a brief description of how to do this.

If you're not using Windows and you want to use SSL, please ask a developer to do this for you. They will know how to compile the source code on your system. __Do not ask me to do it for you__.

You're going to need a valid SSL certificate signed by a certificate authority. If you do not have one or, know how to get one. __DO NOT USE SSL__. I can't tell everyone how to do this. Just look it up on Gooogle or something.


Once you have your certificate file and private key file (preferably in PEM format), you will need to convert them into PFX (I know, but websockets will only accept PFX files). You can do this by using [this site](https://www.sslshopper.com/ssl-converter.html). Just select "PFX/PKCS#12" in the type to convert to section and give it the files needed.

Now that you have the PFX file, we need to convert it to Base64 (this is the last step, I promise). You can do this by going to [this site](https://www.base64encode.org/). It will download a TXT file which, will contain the Base64 encoded certificate.

Now, open the project (if using Windows, open the `.csproj` file in Visual Studio).

Paste the Base64 encoded certificate into the `cert` variable in the "LiveMap.cs" file.

Once you've done that, build the library (`Ctrl + Shift + B` in Visual Studio). Now, drop the DLL file from `bin/Debug/` into the `libs` folder.

Now you can use secure websockets :grin:

## Events

In an effort to make the addon useful to other developers, I've created a few events that can be used to make changes to the map.

Below you can find some info on the events.

```lua
-- Adds a blip to the map at the provided coords. (Can be called from client)
TriggerEvent("live_map:addBlip", blipName --[[ string ]], blipDesc --[[string]], blipType --[[string or int]], x --[[float]], y--[[float]], z--[[float]] )

--[[
Adds custom string data to player object (e.g. player name)
must be called from client.
key = the key of the data (e.g. "name")
str = the data to be stored (e.g. "Havoc")
]]
TriggerServerEvent("live_map:clientAddString", key --[[string]], str--[[string]])

--[[
Add custom number data to the player object (e.g. player coords)
must be called from client
key = the key of the data (e.g. "x")
flt = the data to be stored (e.g. 10 or 4.4848)
]]
TriggerServerEvent("live_map:clientAddFloat", key --[[string]], flt--[[float]])

-- The above events call the server events below where plrId is the server id of the player

TriggerEvent("live_map:addString", plrId --[[int]], key --[[string]], s --[[string]])
TriggerEvent("live_map:addFloat", plrId --[[int]], key --[[string]], f --[[float]])

```
