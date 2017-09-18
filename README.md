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
The following convars are available or you to change

| Name          | Type           | Default Value       | Description |
| ------------- | -------------  | ------------------: | ----------- |
| socket_port   | int            | 30120               | Sets the port the socket server should listen on |
| livemap_debug | int            | 0                   | Sets how much information gets printed to the console (0 = none, 1 = basic information, 2 = all) |
| blip_file     | string         | "server/blips.json" | Sets the file that will contain the generated blips that is exposed via HTTP |

## Events

In an effort to make the addon useful to other developers, I've created a few events that can be used to make changes to the map.

Below you can find some info on the events.

```lua

```

## Built with
* [Hellslicer/WebSocketServer](https://github.com/Hellslicer/WebSocketServer/blob/master/WebSocketEventListener.cs)
* [deniszykov/WebSocketListener](https://github.com/deniszykov/WebSocketListener)
* [JamesNK/Newtonsoft.Json](https://github.com/JamesNK/Newtonsoft.Json)
