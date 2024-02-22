# Configuring the resource

The following are convars are available for you to change.

You should place these inside your `server.cfg` file (or similar).

!!! info "Note"

    Default values will be used if you omit the convars from your server config.


```lua
set livemap_debug "trace" -- Log level for LiveMap. Possible values: trace/[all], debug, info, warn, error, critical, off/anything else
set livemap_blipFile "blips.json" -- File to load blips from
set livemap_accessControl "*" -- Access control for LiveMap.
```

Convars are used so you can update them without access to the `fxmanifest.lua` file.
For example, if you have rcon access you can use the commands `livemap_debug "off"` and `restart live_map` to disable logging without needing physical access to the server.

### livemap_debug

- **Type**: `string`
- **Default Value**: `off`

Sets how much information gets printed to the console.
- `trace` or `[all]` - Print _all_ messages.
- `debug` - Show debug and higher messages.
- `info` - Show informational messages and higher (e.g. when blips are saved to file);
- `warn` - Show warning messages and higher. Warnings shouldn't interfere with the resource working but, should be looked at. 
- `error` - Logs when an error occurs
- `critical` - Logs when something critical has happened that means LiveMap cannot continue to work as intended
- `off` - Turns off _all_ messages.

### livemap_blipFile

- **Type**: `string`
- **Default Value**: `blips.json`

Sets the file that will contain the generated blips that is exposed via HTTP.

This directory should be **writeable** by the FXServer process (on Linux, the user who is running the FX process).

By default this will place a "blip.json" file 

### livemap_accessControl 

- **Type**: `string`
- **Default Value**: `*`

Sets the domain that is allowed to access the blips.json file.

E.g. "https://example.com" will only allow the UI on http://example.com to get the blips, "*" will allow everyone no matter the domain.

You can also specify multiple domains by using a comma to separate the values, for example `https://example.com, https://map.example.com` will allow both domains access.

!!! info "Note"

    Only use "*" if you don't mind _anyone_ being able to access your
    player's data and your blip data.
    It is recommended that you set this to your own website.
