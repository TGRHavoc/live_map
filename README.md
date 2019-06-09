# fivem-live_map

[![Master Build](https://travis-ci.org/TGRHavoc/live_map.svg?branch=master)](https://travis-ci.org/TGRHavoc/live_map)

This is the "backend" code for the live_map addon for FiveM that is ran on the
game server.
It creates a websocket server so that it can communicate to the
web interface.

<!-- Testing webhook -->

## How to install

Download the ZIP file. And extract the contents into `resources/live_map/`.

Add the following to your server.cfg file.

```
set socket_port 30121
set livemap_debug "warn" # "[all]" 'trace', 'debug', 'info', 'warn', 'error', 'fatal', 'off'
set blip_file "server/blips.json"
set livemap_access_control "*"

start live_map
```

To get the in-game blips to show on the interface, you will need to generate a "blips" file.
This can be easily done with the in-game command `blips generate` (must be ran as admin).

## Configuration

### Convars
The following convars are available for you to change

| Name                    | Type           | Default Value       | Description |
| ----------------------- | -------------  | ------------------: | ----------- |
| socket_port             | int            | 30121               | Sets the port the socket server should listen on |
| livemap_debug           | int            | 0                   | Sets how much information gets printed to the console (0 = none, 1 = basic information, 2 = all) |
| blip_file               | string         | "server/blips.json" | Sets the file that will contain the generated blips that is exposed via HTTP |
| livemap_access_control  | string         | "*"                 | Sets the domain that is allowed to access the blips.json file (E.g. "https://example.com" will only allow the UI on http://example.com to get the blips), "*" will allow everyone |

## Events
In an effort to make the addon useful to other developers, I've created a few events that can be used to make changes to the data being sent to the UI.

#### Client to server

Below you can find some info on the server events that __must__ be triggered by the client.

Note, when using `livemap:AddPlayerData` or `livemap:UpdatePlayerData` if the player has been removed using `livemap:RemovePlayer` they will be tracked again.

| Name                     | Parameters               | Description |
| ------------------------ | :----------------------: | ----------- |
| livemap:AddPlayerData    | key (string), data (any) | Adds data to a player that gets sent over Websockets |
| livemap:UpdatePlayerData | key (string), data (any) | Updates the data that is associated with the player. Uses the same "key" as the above event. |
| livemap:RemovePlayerData | key (string)             | Removed data associated with the player. Uses the same "key" as the above events. |
| livemap:RemovePlayer     |                          | Stops sending a player data over Websockets |

Example usage:
```lua
-- Set the player's "Name" to "Havoc"
TriggerServerEvent("livemap:AddPlayerData", "Name", "Havoc")

-- Update the player's name to "John Doe"
TriggerServerEvent("livemap:UpdatePlayerData", "Name", "John Doe")

-- Remove "Name" from the player (stops displaying it in the UI)
TriggerServerEvent("livemap:RemovePlayerData", "Name")

-- Removes a player from the websockets (stops tracking them)
TriggerServerEvent("livemap:RemovePlayer")
```

#### Server Events

Below you can find information on some server-only events. This can only be called on the server.

| Name                              | Parameters                                     | Description |
| --------------------------------- | :--------------------------------------------: | ----------- |
| livemap:internal_AddPlayerData    | identifier (string), key (string), value (any) | Adds data with the key that gets sent over Websockets for the player with the specified identifier |
| livemap:internal_UpdatePlayerData | identifier (string), key (string), value (any) | Updated the data that is associated with the player with the identifier |
| livemap:internal_RemovePlayerData | identifier (string), key (string)              | Removed the data that is associated with the player with the identifier |
| livemap:internal_RemovePlayer     | identifier (string)                            | Removes a player from the websocket data array (stops tracking the player) |

Example usage:
```lua
-- Get the player's identifier
identifier = GetPlayerIdentifier(source, 0)

-- Set the player's "Name" to "Havoc"
TriggerEvent("livemap:internal_AddPlayerData", identifier, "Name", "Havoc")

-- Update the player's "Name"  to "John Doe"
TriggerEvent("livemap:internal_UpdatePlayerData", identifier, "Name", "John Doe")

-- Removes the player's "Name"
TriggerEvent("livemap:internal_RemovePlayerData", identifier, "Name")

-- Removes the player
TriggerEvent("livemap:internal_RemovePlayer", identifier)

```

## Built with
* [Hellslicer/WebSocketServer](https://github.com/Hellslicer/WebSocketServer/blob/master/WebSocketEventListener.cs)
* [deniszykov/WebSocketListener](https://github.com/deniszykov/WebSocketListener)
* [JamesNK/Newtonsoft.Json](https://github.com/JamesNK/Newtonsoft.Json)
