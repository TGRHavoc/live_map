# Configuring the resource
## Convars
The following convars are available for you to change.

You should already have them in your server.cfg file and looking like the config below.

```config
set socket_port 30121
set livemap_debug "warn" # "[all]" 'trace', 'debug', 'info', 'warn', 'error', 'fatal', 'off'
set blip_file "server/blips.json"
set livemap_access_control "*"
set livemap_use_nucleus true # Allow livemap to set up a secure reverseProxy using the Nucleus project
```

### socket_port

- **Type**: `number`
- **Default Value**: `0`

Sets the port the socket server should listen on

### livemap_debug

- **Type**: `string`
- **Default Value**: `warn`

Sets how much information gets printed to the console.
- `[all]` - Print _all_ messages.
- `trace` - Pretty much the same as "all".
- `debug` - Show debug and higher messages.
- `info` - Show informational messages and higher (e.g. when blips are saved to file);
- `warn` - Show warning messages and higher. Warnings shouldn't interfere with the resource working but, should be looked at. 
- `error` - Not used.
- `fatal` - Not used.
- `off` - Turns off _all_ messages.

### blip_file

- **Type**: `string`
- **Default Value**: `server/blips.json`

Sets the file that will contain the generated blips that is exposed via HTTP.

This directory should be **writeable** by the FXServer process (on Linux, the user who is running the FX process).

### livemap_access_control 

- **Type**: `string`
- **Default Value**: `*`

Sets the domain that is allowed to access the blips.json file.

E.g. "https://example.com" will only allow the UI on http://example.com to get the blips), "*" will allow everyone no matter the domain.

!!! info "Note"

    Only use "*" if you don't mind _anyone_ being able to access your
    player's data and your blip data.
    It is recommended that you set this to your own website.

### livemap_use_nucleus

- **Type**: `boolean`
- **Default Value**: `true`
  
Sets wether LiveMap should use the nucleus project to set up a secure proxy to the websocket.
It will print out the reverseProxy values you should use in the interface's config file.

??? example "Example output"
    ```
      -------------------------------------------------------------------
      -------------------------------------------------------------------
      -------------------------------------------------------------------
      Hey! LiveMap was able to use the Nucleus project to automatically set up a secure proxy to the resource.
      If you want to use this in your config use the revserProxy settings printed below (https://docs.tgrhavoc.co.uk/livemap-interface/reverse_proxy/)
      If you don't want to use Nucleus then put "set livemap_use_nucleus false" in your server.cfg file.
      "reverseProxy": {"blips": "https://monitor-somthing.users.cfx.re/blips", "socket": "wss://monitor-somthing.users.cfx.re/"}
      -------------------------------------------------------------------
      -------------------------------------------------------------------
      -------------------------------------------------------------------
    ```

