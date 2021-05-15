---
layout: default
title: Events
nav_order: 2
parent: LiveMap Resource
---

# Events / API <!-- omit in toc -->

- [Client to server](#client-to-server)
  - [livemap:AddPlayerData](#livemapaddplayerdata)
  - [livemap:UpdatePlayerData](#livemapupdateplayerdata)
  - [livemap:RemovePlayerData](#livemapremoveplayerdata)
  - [livemap:RemovePlayer](#livemapremoveplayer)
- [Server Events](#server-events)
  - [livemap:internal_AddPlayerData](#livemapinternal_addplayerdata)
  - [livemap:internal_UpdatePlayerData](#livemapinternal_updateplayerdata)
  - [livemap:internal_RemovePlayerData](#livemapinternal_removeplayerdata)
  - [livemap:internal_RemovePlayer](#livemapinternal_removeplayer)

## Client to server

Below you can find some info on the server events that __must__ be triggered by the client.

> Note: When using `livemap:AddPlayerData` or `livemap:UpdatePlayerData` if the player has been removed using `livemap:RemovePlayer` they will be tracked again.

### livemap:AddPlayerData

Adds data to a player that gets sent over the Websocket.

#### Parameters <!-- omit in toc -->

<dl>
    <dt>Name</dt>
    <dd>key</dd>
    <dt>Type</dt>
    <dd>string</dd>
    <dt>Description</dt>
    <dd>The name of the data to add to the player (e.g. "name").</dd>
</dl>

<dl>
    <dt>Name</dt>
    <dd>data</dd>
    <dt>Type</dt>
    <dd>any</dd>
    <dt>Description</dt>
    <dd>The value of the data being added (e.g. "TGRHavoc")</dd>
</dl>

#### Examples <!-- omit in toc -->

```lua
-- Set the player's "Name" to "Havoc"
TriggerServerEvent("livemap:AddPlayerData", "Name", "Havoc")

-- Pseudo code to add the player's age after they set it 
RegisterEventHandler("playerSetAgeTo", function(newAge)
    TriggerServerEvent("livemap:AddPlayerData", "Age", newAge)
end)
```

### livemap:UpdatePlayerData

Updates the data that is associated with the player. Uses the same "key" as the above event.

> Note: If the player doesn't have any data with the given key, it will be added.

#### Parameters <!-- omit in toc -->

<dl>
    <dt>Name</dt>
    <dd>key</dd>
    <dt>Type</dt>
    <dd>string</dd>
    <dt>Description</dt>
    <dd>The name of the data to update on the player (e.g. "name").</dd>
</dl>

<dl>
    <dt>Name</dt>
    <dd>data</dd>
    <dt>Type</dt>
    <dd>any</dd>
    <dt>Description</dt>
    <dd>The value of the data being updated (e.g. "Some Other Name")</dd>
</dl>

#### Examples <!-- omit in toc -->

```lua
-- Update the player's name to "John Doe"
TriggerServerEvent("livemap:UpdatePlayerData", "Name", "John Doe")

-- Pseudo code to update the player's name when they change it
if PlayerChangesName(PlayerId()) then
    TriggerServerEvent("livemap:UpdatePlayerData", "Name", GetPlayerName(PlayerId()))
end
```

### livemap:RemovePlayerData

Removed data associated with the player. Uses the same "key" as the above events.

> Note: If at _any_ point after this, you call the AddPlayerData or UpdatePlayerData events
> the data will be added back to the player.

#### Parameters <!-- omit in toc -->

<dl>
    <dt>Name</dt>
    <dd>key</dd>
    <dt>Type</dt>
    <dd>string</dd>
    <dt>Description</dt>
    <dd>The name of the data to remove from the player (e.g. "name").</dd>
</dl>

#### Examples <!-- omit in toc -->

```lua
-- Remove "Name" from the player (stops displaying it in the UI)
TriggerServerEvent("livemap:RemovePlayerData",  "Name")

-- Pseudo code to remove player's who age is less then 18
if GetPlayerAge(PlayerId()) < 18 then
    TriggerServerEvent("livemap:RemovePlayerData", "Age")
end
```

### livemap:RemovePlayer

Stops sending the player's data over websockets.

> Note: If at _any_ point after this, you call the AddPlayerData or UpdatePlayerData events
> the data added will be sent.
> This event should only be used if you know for 100% sure that no more data should be sent to the interface (e.g. if the player leaves the server). 

#### Parameters <!-- omit in toc -->

This events requires no parameters to be set.

#### Examples <!-- omit in toc -->

```lua
-- Pseudo code to remove a player if they leave
if NetworkIsPlayerActive(PlayerId()) then
    -- DO stuff to update the player data
else
    TriggerServerEvent("livemap:RemovePlayer")
end
```

## Server Events

Below you can find information on some server-only events. 
These can only be called on the server.

### livemap:internal_AddPlayerData

Adds data with the key that gets sent over Websockets for the player with the specified identifier.

#### Parameters <!-- omit in toc -->
<dl>
    <dt>Name</dt>
    <dd>identifier</dd>
    <dt>Type</dt>
    <dd>string</dd>
    <dt>Description</dt>
    <dd>The identifier of the player (e.g. "steam:00000000000"). Usually gotten by the <a href="https://docs.fivem.net/natives/?_0x7302DBCF">GetPlayerIdentifier</a> native.</dd>
</dl>
<dl>
    <dt>Name</dt>
    <dd>key</dd>
    <dt>Type</dt>
    <dd>string</dd>
    <dt>Description</dt>
    <dd>The name of the data to add to the player (e.g. "name").</dd>
</dl>
<dl>
    <dt>Name</dt>
    <dd>data</dd>
    <dt>Type</dt>
    <dd>any</dd>
    <dt>Description</dt>
    <dd>The value of the data being added (e.g. "TGRHavoc")</dd>
</dl>

#### Examples <!-- omit in toc -->

```lua
-- Pseudo code to add player data when player spawns
AddEventHandler("playerSpawned", function()
    -- Get the player's identifier
    identifier = GetPlayerIdentifier(source, 0)
    -- Set the player's "Name" to their name
    TriggerEvent("livemap:internal_AddPlayerData", identifier, "Name", GetPlayerName(source))
end)
```

### livemap:internal_UpdatePlayerData

Updated the data that is associated with the player with the identifier.

#### Parameters <!-- omit in toc -->
<dl>
    <dt>Name</dt>
    <dd>identifier</dd>
    <dt>Type</dt>
    <dd>string</dd>
    <dt>Description</dt>
    <dd>The identifier of the player (e.g. "steam:00000000000"). Usually gotten by the <a href="https://docs.fivem.net/natives/?_0x7302DBCF">GetPlayerIdentifier</a> native.</dd>
</dl>
<dl>
    <dt>Name</dt>
    <dd>key</dd>
    <dt>Type</dt>
    <dd>string</dd>
    <dt>Description</dt>
    <dd>The name of the data to update on the player (e.g. "name").</dd>
</dl>
<dl>
    <dt>Name</dt>
    <dd>data</dd>
    <dt>Type</dt>
    <dd>any</dd>
    <dt>Description</dt>
    <dd>The value of the data being updated (e.g. "Some other name")</dd>
</dl>

#### Examples <!-- omit in toc -->

```lua
-- Pseudo code to change players name when they change it
AddEventHandler("playerHasChangedNameByDead", function()
    -- Get the player's identifier
    identifier = GetPlayerIdentifier(source, 0)
    -- Set the player's "Name" to their name
    TriggerEvent("livemap:internal_UpdatePlayerData", identifier, "Name", GetPlayerName(source))
end)
```

### livemap:internal_RemovePlayerData

Removed the data that is associated with the player with the identifier.

#### Parameters <!-- omit in toc -->
<dl>
    <dt>Name</dt>
    <dd>identifier</dd>
    <dt>Type</dt>
    <dd>string</dd>
    <dt>Description</dt>
    <dd>The identifier of the player (e.g. "steam:00000000000"). Usually gotten by the <a href="https://docs.fivem.net/natives/?_0x7302DBCF">GetPlayerIdentifier</a> native.</dd>
</dl>
<dl>
    <dt>Name</dt>
    <dd>key</dd>
    <dt>Type</dt>
    <dd>string</dd>
    <dt>Description</dt>
    <dd>The name of the data to remove from the player (e.g. "name").</dd>
</dl>

#### Examples <!-- omit in toc -->

```lua
-- Pseudo code to remove player's age if they're under 18
AddEventHandler("playerHasAged", function(newAge)
    -- Get the player's identifier
    identifier = GetPlayerIdentifier(source, 0)

    if newAge < 18 then 
        TriggerEvent("livemap:internal_RemovePlayerData", identifier, "Age")
    end
end)
```
### livemap:internal_RemovePlayer

Removes a player from the websocket data array (stops tracking the player)

#### Parameters <!-- omit in toc -->
<dl>
    <dt>Name</dt>
    <dd>identifier</dd>
    <dt>Type</dt>
    <dd>string</dd>
    <dt>Description</dt>
    <dd>The identifier of the player (e.g. "steam:00000000000"). Usually gotten by the <a href="https://docs.fivem.net/natives/?_0x7302DBCF">GetPlayerIdentifier</a> native.</dd>
</dl>

#### Examples <!-- omit in toc -->

```lua
-- Pseudo code to remove a player when they leave the server
AddEventHandler("playerLeft", function()
    -- Get the player's identifier
    identifier = GetPlayerIdentifier(source, 0)
    TriggerEvent("livemap:internal_RemovePlayer", identifier)
end)
```
