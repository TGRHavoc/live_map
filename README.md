# fivem-live_map

This is the "backend" code for the live_map addon for FiveM that is ran on the
game server.
It creates a websocket server so that it can communicate to the
web interface.

## How to install

Download the ZIP file. And extract the contents into `resources/live_map/`.

<!-- TODO: How to start in FX server -->

## Configuration

### Convars
The following convars are available for you to change

| Name                    | Type           | Default Value       | Description |
| ----------------------- | -------------  | ------------------: | ----------- |
| socket_port             | int            | 30120               | Sets the port the socket server should listen on |
| livemap_debug           | int            | 0                   | Sets how much information gets printed to the console (0 = none, 1 = basic information, 2 = all) |
| blip_file               | string         | "server/blips.json" | Sets the file that will contain the generated blips that is exposed via HTTP |
| livemap_access_control  | string         | "*"                 | Sets the domain that is allowed to access the blips.json file (E.g. "https://example.com"), "*" will allow everyone |

## Events
In an effort to make the addon useful to other developers, I've created a few events that can be used to make changes to the data being sent to the UI.

#### Client to server

Below you can find some info on the server events that can be triggered by the client.

| Name                     | Parameters               | Description |
| ------------------------ | :----------------------: | ----------- |
| livemap:AddPlayerData    | key (string), data (any) | Adds data to a player that get's sent over Websockets |
| livemap:UpdatePlayerData | key (string), data (any) | Updates the data that is associated with the player. Uses the same "key" as the above event. |
| livemap:RemovePlayerData | key (string)             | Removed data associated with the player. Uses the same "key" as the above events. |
| livemap:RemovePlayer     |                          | Stops sending a player data over Websockets |

Example usage:
```lua
```

#### Server Events


## Built with
* [Hellslicer/WebSocketServer](https://github.com/Hellslicer/WebSocketServer/blob/master/WebSocketEventListener.cs)
* [deniszykov/WebSocketListener](https://github.com/deniszykov/WebSocketListener)
* [JamesNK/Newtonsoft.Json](https://github.com/JamesNK/Newtonsoft.Json)
