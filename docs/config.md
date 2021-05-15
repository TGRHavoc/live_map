---
layout: default
title: Configuration
nav_order: 1
parent: LiveMap Resource
---

## Convars
The following convars are available for you to change.

You should already have them in your server.cfg file and looking like the config below.

```config
set socket_port 30121
set livemap_debug "warn" # "[all]" 'trace', 'debug', 'info', 'warn', 'error', 'fatal', 'off'
set blip_file "server/blips.json"
set livemap_access_control "*"
```

### socket_port

<dl>
  <dt>Type</dt>
  <dd>Number</dd>

  <dt>Default Value</dt>
  <dd>0</dd>
</dl>

Sets the port the socket server should listen on

### livemap_debug

<dl>
  <dt>Type</dt>
  <dd>String</dd>
  
  <dt>Default Value</dt>
  <dd>warn</dd>
</dl>

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

<dl>
  <dt>Type</dt>
  <dd>String</dd>
  
  <dt>Default Value</dt>
  <dd>server/blips.json</dd>
</dl>

Sets the file that will contain the generated blips that is exposed via HTTP.

This directory should be **writeable** by the FXServer process (on Linux, the user who is running the FX process).

### livemap_access_control 

<dl>
  <dt>Type</dt>
  <dd>String</dd>
  
  <dt>Default Value</dt>
  <dd>*</dd>
</dl>

Sets the domain that is allowed to access the blips.json file.

E.g. "https://example.com" will only allow the UI on http://example.com to get the blips), "*" will allow everyone no matter the domain.

> **Note**: Only use "*" if you don't mind _anyone_ being able to access your
> player's data and your blip data.
> It is recommended that you set this to your own website.
{: .fs-3}