# Events / API <!-- omit in toc -->

- [Client to server](#client-to-server)
    - [livemap:AddPlayerData(key, data)](#livemapaddplayerdatakey-data)
    - [livemap:UpdatePlayerData(key, data)](#livemapupdateplayerdatakey-data)
    - [livemap:RemovePlayerData(key)](#livemapremoveplayerdatakey)
    - [livemap:RemovePlayer(void)](#livemapremoveplayervoid)
- [Server Events](#server-events)
    - [livemap:internal_AddPlayerData(identifier, key, data)](#livemapinternal_addplayerdataidentifier-key-data)
    - [livemap:internal_UpdatePlayerData(identifier, key, data)](#livemapinternal_updateplayerdataidentifier-key-data)
    - [livemap:internal_RemovePlayerData(identifier, key)](#livemapinternal_removeplayerdataidentifier-key)
    - [livemap:internal_RemovePlayer(identifier)](#livemapinternal_removeplayeridentifier)


## Client to server

Below you can find some info on the server events that __must__ be triggered by the client.

> Note: When using `livemap:AddPlayerData` or `livemap:UpdatePlayerData` if the player has been removed using `livemap:RemovePlayer` they will be tracked again.

### livemap:AddPlayerData(key, data)

Adds data to a player that gets sent over the Websocket.

#### Parameters <!-- omit in toc -->

**key**

- **Type**: `string`
- **Description**: The name of the data to add to the player (e.g. "name").

**data**

- **Type**: `any`
- **Description**: The value of the data being added (e.g. "TGRHavoc")

#### Examples <!-- omit in toc -->

```lua
-- Set the player's "Name" to "Havoc"
TriggerServerEvent("livemap:AddPlayerData", "Name", "Havoc")

-- Pseudo code to add the player's age after they set it 
RegisterEventHandler("playerSetAgeTo", function(newAge)
    TriggerServerEvent("livemap:AddPlayerData", "Age", newAge)
end)
```

### livemap:UpdatePlayerData(key, data)

Updates the data that is associated with the player. Uses the same "key" as the above event.

> Note: If the player doesn't have any data with the given key, it will be added.

#### Parameters <!-- omit in toc -->

**key**

- **Type**: `string`
- **Description**: The name of the data to update on the player (e.g. "name").

**data**

- **Type**: `any`
- **Description**: The value of the data being updated (e.g. "Some Other Name")

#### Examples <!-- omit in toc -->

```lua
-- Update the player's name to "John Doe"
TriggerServerEvent("livemap:UpdatePlayerData", "Name", "John Doe")

-- Pseudo code to update the player's name when they change it
if PlayerChangesName(PlayerId()) then
    TriggerServerEvent("livemap:UpdatePlayerData", "Name", GetPlayerName(PlayerId()))
end
```

### livemap:RemovePlayerData(key)

Removed data associated with the player. Uses the same "key" as the above events.

!!! warning
    If at _any_ point after this, you call the AddPlayerData or UpdatePlayerData events
    the data will be added back to the player.

#### Parameters <!-- omit in toc -->

**key**

- **Type**: `string`
- **Description**: The name of the data to remove from the player (e.g. "name").

#### Examples <!-- omit in toc -->

```lua
-- Remove "Name" from the player (stops displaying it in the UI)
TriggerServerEvent("livemap:RemovePlayerData",  "Name")

-- Pseudo code to remove player's who age is less then 18
if GetPlayerAge(PlayerId()) < 18 then
    TriggerServerEvent("livemap:RemovePlayerData", "Age")
end
```

### livemap:RemovePlayer(void)

Stops sending the player's data over websockets.

!!! warning
    If at _any_ point after this, you call the AddPlayerData or UpdatePlayerData events
    the data added will be sent.

    This event should only be used if you know for 100% sure that no more data should be sent to the interface 
    (e.g. if the player leaves the server). 

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

### livemap:internal_AddPlayerData(identifier, key, data)

Adds data with the key that gets sent over Websockets for the player with the specified identifier.

#### Parameters <!-- omit in toc -->

**identifier**

- **Type**: `string`
- **Description**: The identifier of the player (e.g. "steam:00000000000"). Usually gotten by the [GetPlayerIdentifier](https://docs.fivem.net/natives/?_0x7302DBCF) native.

**key**

- **Type**: `string`
- **Description**: The name of the data to add to the player (e.g. "name").

**data**

- **Type**: `any`
- **Description**: The value of the data being added (e.g. "TGRHavoc")

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

### livemap:internal_UpdatePlayerData(identifier, key, data)

Updated the data that is associated with the player with the identifier.

#### Parameters <!-- omit in toc -->
**identifier**

- **Type**: `string`
- **Description**: The identifier of the player (e.g. "steam:00000000000"). Usually gotten by the [GetPlayerIdentifier](https://docs.fivem.net/natives/?_0x7302DBCF) native.

**key**

- **Type**: `string`
- **Description**: The name of the data to update on the player (e.g. "name").

**data**

- **Type**: `any`
- **Description**: The value that the data should be updated to (e.g. "Havoc's Real Name")

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

### livemap:internal_RemovePlayerData(identifier, key)

Removed the data that is associated with the player with the identifier.

#### Parameters <!-- omit in toc -->
**identifier**

- **Type**: `string`
- **Description**: The identifier of the player (e.g. "steam:00000000000"). Usually gotten by the [GetPlayerIdentifier](https://docs.fivem.net/natives/?_0x7302DBCF) native.

**key**

- **Type**: `string`
- **Description**: The name of the data to update on the player (e.g. "name").

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
### livemap:internal_RemovePlayer(identifier)

Removes a player from the websocket data array (stops tracking the player)

#### Parameters <!-- omit in toc -->

**identifier**

- **Type**: `string`
- **Description**: The identifier of the player (e.g. "steam:00000000000"). Usually gotten by the [GetPlayerIdentifier](https://docs.fivem.net/natives/?_0x7302DBCF) native.

#### Examples <!-- omit in toc -->

```lua
-- Pseudo code to remove a player when they leave the server
AddEventHandler("playerLeft", function()
    -- Get the player's identifier
    identifier = GetPlayerIdentifier(source, 0)
    TriggerEvent("livemap:internal_RemovePlayer", identifier)
end)
```
