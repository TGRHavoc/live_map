using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using vtortola.WebSockets;

/*
            LiveMap - A LiveMap for FiveM servers
              Copyright (C) 2017  Jordan Dalton

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program in the file "LICENSE".  If not, see <http://www.gnu.org/licenses/>.
*/


namespace Havoc.Live_Map
{
    class SocketHandler
    {
        WebSocketServer server;
        JObject playerData;

        ConcurrentDictionary<string, WebSocket> clients = new ConcurrentDictionary<string, WebSocket>();
        static double writeLock = 0;

        public SocketHandler(WebSocketServer server)
        {
            this.server = server;
            playerData = new JObject();

            server.OnConnect += Server_OnConnect;
            server.OnDisconnect += Server_OnDisconnect;
            server.OnError += Server_OnError;
            server.OnMessage += Server_OnMessage;
        }

        private void Server_OnMessage(WebSocket ws, string msg)
        {
            string[] args = msg.Split(' ');
            LiveMap.Log(LiveMap.LogLevel.All, "Recieved message from client {0}:\n\t\"{1}\"", ws.RemoteEndpoint.ToString(), msg);
        }

        private void Server_OnError(WebSocket ws, Exception ex)
        {
            LiveMap.Log(LiveMap.LogLevel.Basic, "Socket error from {0}: {1}", ws == null ? "Unknown" : ws.RemoteEndpoint.ToString(), ex.Message);

            if (ws != null)
            {
                WebSocket destory;
                if (clients.TryRemove(ws.RemoteEndpoint.ToString(), out destory))
                {
                    destory.Dispose();
                    LiveMap.Log(LiveMap.LogLevel.Basic, "Removed {0} socket because of an error: {1}\nInner: {2}", ws.RemoteEndpoint.ToString(), ex.Message, ex.InnerException);
                }
                else
                {
                    LiveMap.Log(LiveMap.LogLevel.Basic, "Couldn't remove {0} from the clients dic.", ws.RemoteEndpoint.ToString());
                }

            }
            
        }

        private void Server_OnDisconnect(WebSocket ws)
        {
            LiveMap.Log(LiveMap.LogLevel.Basic, "Socket connection was closed at {0}", ws.RemoteEndpoint.ToString());

            WebSocket destory;
            if (clients.TryRemove(ws.RemoteEndpoint.ToString(), out destory))
            {
                destory.Dispose();
                LiveMap.Log(LiveMap.LogLevel.All, "Removed {0} socket because it disconnected", ws.RemoteEndpoint.ToString());
            }
            else
            {
                LiveMap.Log(LiveMap.LogLevel.All, "Couldn't remove {0} from the clients dic.", ws.RemoteEndpoint.ToString());
            }
        }

        private void Server_OnConnect(WebSocket ws)
        {
            LiveMap.Log(LiveMap.LogLevel.Basic, "Socket connection opened at {0}", ws.RemoteEndpoint.ToString());

            if (clients.TryAdd(ws.RemoteEndpoint.ToString(), ws))
            {
                LiveMap.Log(LiveMap.LogLevel.All, "Added client {0} to the client dictionary", ws.RemoteEndpoint.ToString());
            }
            else
            {
                LiveMap.Log(LiveMap.LogLevel.Basic, "Couldn't add {0} to the client dic", ws.RemoteEndpoint.ToString());

            }
        }

        private void MakeSurePlayerExists(string identifier)
        {
            lock (playerData)
            {

                if (playerData[identifier] == null)
                {
                    playerData[identifier] = new JObject();
                }
            }
        }

        public async Task SendWebsocketData()
        {
            while (true)
            {
                // Only send the data every .5 seconds
                await Task.Delay(TimeSpan.FromMilliseconds(LiveMap.waitSeconds)).ConfigureAwait(false);

                // Generate the payload
                JObject payload = new JObject();
                JArray playerDataArray = new JArray();
                lock (playerData)
                {
                    foreach (KeyValuePair<string, JToken> data in playerData)
                    {
                        playerDataArray.Add(data.Value);
                    }
                }

                if(playerDataArray.Count == 0)
                {
                    continue;
                }
                payload["type"] = "playerData";
                payload["payload"] = playerDataArray;


                foreach (KeyValuePair<string, WebSocket> keyPair in clients)
                {
                    string endpoint = keyPair.Key;
                    WebSocket socket = keyPair.Value;
                    LiveMap.Log(LiveMap.LogLevel.All, "Sending data to {0}", endpoint);

                    if (!socket.IsConnected)
                    {
                        LiveMap.Log(LiveMap.LogLevel.All, "Disposing of websocket {0} because it's closed..", endpoint);
                        socket.Dispose();
                        return;
                    }

                    if(Interlocked.CompareExchange(ref writeLock, 1, 0) == 1)
                    {
                       while(Interlocked.CompareExchange(ref writeLock, 1, 0) == 1)
                        {
                            LiveMap.Log(LiveMap.LogLevel.All, "Waiting untill writelock is 0");
                            await Task.Delay(100);
                        }
                    }

                    try
                    {
                        await socket.WriteStringAsync(payload.ToString(Newtonsoft.Json.Formatting.None)).ConfigureAwait(true);
                    }
                    catch (Exception)
                    {
                        LiveMap.Log(LiveMap.LogLevel.Basic, "Couldn't send playerData because of read/write stuff");
                    }
                }

                writeLock = 0;

            }
        }

        public void AddPlayerData(string identifier, string key, object data)
        {
            LiveMap.Log(LiveMap.LogLevel.All, "Adding player {0}'s \"{1}\"", identifier, key);
            MakeSurePlayerExists(identifier);

            if (data == null)
            {
                LiveMap.Log(LiveMap.LogLevel.Basic, "Cannot add \"{1}\" to player ({0}) because it's null.", identifier, key);
                return;
            }

            lock (playerData)
            {
                JObject playerObj = (JObject)playerData[identifier];

                if(playerObj[key] == null)
                    playerObj.Add(key, JToken.FromObject(data));
            }

            LiveMap.Log(LiveMap.LogLevel.Basic, "Added \"{1}\" to player {0} with value of \"{2}\"", identifier, key, data);
        }

        public void UpdatePlayerData(string identifier, string key, object newData)
        {
            LiveMap.Log(LiveMap.LogLevel.All, "Updating player {0}'s \"{1}\"", identifier, key);
            MakeSurePlayerExists(identifier);

            // Check if `data` is null
            if (newData == null)
            {
                LiveMap.Log(LiveMap.LogLevel.Basic, "Cannot update \"{0}\" for {1} because it's null", key, identifier);
                return;
            }

            lock (playerData)
            {
                JObject playerObj = (JObject)playerData[identifier];
                playerObj[key] = JToken.FromObject(newData);
                playerData[identifier] = playerObj;
            }

            LiveMap.Log(LiveMap.LogLevel.All, "Updated player {0}'s \"{1}\" to \"{2}\"", identifier, key, newData);
        }

        public void RemovePlayerData(string identifier, string key)
        {
            MakeSurePlayerExists(identifier);
            lock (playerData)
            {
                JObject playerObj = (JObject)playerData[identifier];
                if (playerObj[key] != null)
                {
                    if (playerObj.Remove(key))
                    {
                        LiveMap.Log(LiveMap.LogLevel.Basic, "Removed \"{0}\" from player {1}", key, identifier);
                    }else
                    {
                        LiveMap.Log(LiveMap.LogLevel.Basic, "Couldn't remove \"{0}\" from player {1}", key, identifier);
                    }
                }// else = already removed
            }
        }

        public async void RemovePlayer(string identifier)
        {
            bool playerLeftBool = false;
            lock (playerData)
            {
                if (playerData[identifier] != null)
                {
                    if (playerData.Remove(identifier))
                    {
                        playerLeftBool = true;
                        LiveMap.Log(LiveMap.LogLevel.Basic, "Removed player {0}", identifier);
                    }
                    else
                    {
                        LiveMap.Log(LiveMap.LogLevel.Basic, "Couldn't remove player {0}... Seriously, there's something fucking wrong here...", identifier);
                    }
                }
            }

            LiveMap.Log(LiveMap.LogLevel.All, "Notifying ws clients that a player left? {0}", playerLeftBool);

            if (playerLeftBool)
            {
                // Tell the client's that someone has left

                JObject payload = new JObject();
                payload["type"] = "playerLeft";
                payload["payload"] = identifier;

                if (Interlocked.CompareExchange(ref writeLock, 1, 0) == 1)
                {
                    while (Interlocked.CompareExchange(ref writeLock, 1, 0) == 1)
                    {
                        LiveMap.Log(LiveMap.LogLevel.All, "RemovePlayer waiting..");
                        await Task.Delay(100);
                    }
                }

                foreach (KeyValuePair<string, WebSocket> pair in clients)
                {
                    string endpoint = pair.Key;
                    WebSocket ws = pair.Value;

                    if (!ws.IsConnected)
                    {
                        LiveMap.Log(LiveMap.LogLevel.All, "Disposing of websocket {0} because it's closed..", endpoint);
                        ws.Dispose();
                        return;
                    }

                    await ws.WriteStringAsync(payload.ToString(Newtonsoft.Json.Formatting.None)).ConfigureAwait(false);

                }

                writeLock = 0;
            }
        }

        private JObject ConvertBlip(dynamic blip)
        {
            JObject obj = new JObject();
            JObject pos = new JObject();

            pos["x"] = blip.pos.x;
            pos["y"] = blip.pos.y;
            pos["z"] = blip.pos.z;

            if(blip.type != null)
            {
                obj["type"] = blip.type;
            }

            if (blip.name != null && !(string.IsNullOrEmpty(blip.name)))
            {
                obj["name"] = blip.name;
            }

            if (blip.description != null && !(string.IsNullOrEmpty(blip.description)))
            {
                obj["description"] = blip.description;
            }

            obj["pos"] = pos;

            return obj;
        }

        public async void AddBlip(dynamic blip)
        {
            JObject payload = new JObject();
            payload["type"] = "addBlip";
            payload["payload"] = ConvertBlip(blip);

            if (Interlocked.CompareExchange(ref writeLock, 1, 0) == 1)
            {
                while (Interlocked.CompareExchange(ref writeLock, 1, 0) == 1)
                {
                    LiveMap.Log(LiveMap.LogLevel.All, "AddBlip waiting..");
                    await Task.Delay(100);
                }
            }

            foreach (KeyValuePair<string, WebSocket> pair in clients)
            {
                string endpoint = pair.Key;
                WebSocket ws = pair.Value;

                LiveMap.Log(LiveMap.LogLevel.Basic, "addBlip for blip with id {0} to {1}", blip.type, endpoint);

                if (!ws.IsConnected)
                {
                    LiveMap.Log(LiveMap.LogLevel.All, "Disposing of websocket {0} because it's closed..", endpoint);
                    ws.Dispose();
                    return;
                }

                await ws.WriteStringAsync(payload.ToString(Newtonsoft.Json.Formatting.None)).ConfigureAwait(false);
                //ws.WriteStringAsync(payload.ToString(Newtonsoft.Json.Formatting.None)).Wait(500); 
            }

            writeLock = 0;

        }

        public async void RemoveBlip(dynamic blip)
        {
            JObject payload = new JObject();
            payload["type"] = "removeBlip";
            payload["payload"] = ConvertBlip(blip);

            if(Interlocked.CompareExchange(ref writeLock, 1, 0) == 1)
            {
                while(Interlocked.CompareExchange(ref writeLock, 1, 0) == 1)
                {
                    LiveMap.Log(LiveMap.LogLevel.All, "RemoveBlip waiting..");
                    await Task.Delay(100);
                }
            }

            foreach (KeyValuePair<string, WebSocket> pair in clients)
            {
                string endpoint = pair.Key;
                WebSocket ws = pair.Value;

                LiveMap.Log(LiveMap.LogLevel.Basic, "removeBlip for blip with id {0} to {1}", blip.type, endpoint);

                if (!ws.IsConnected)
                {
                    LiveMap.Log(LiveMap.LogLevel.All, "Disposing of websocket {0} because it's closed..", endpoint);
                    ws.Dispose();
                    return;
                }

                await ws.WriteStringAsync(payload.ToString(Newtonsoft.Json.Formatting.None)).ConfigureAwait(false);

                //ws.WriteStringAsync(payload.ToString(Newtonsoft.Json.Formatting.None)).Wait(500);
            }

            writeLock = 0;

        }

        public async void UpdateBlip(dynamic blip)
        {
            JObject payload = new JObject();
            payload["type"] = "updateBlip";
            payload["payload"] = ConvertBlip(blip);

            if (Interlocked.CompareExchange(ref writeLock, 1, 0) == 1)
            {
                while (Interlocked.CompareExchange(ref writeLock, 1, 0) == 1)
                {
                    LiveMap.Log(LiveMap.LogLevel.All, "UpdateBlip waiting..");
                    await Task.Delay(100);
                }
            }

            foreach (KeyValuePair<string, WebSocket> pair in clients)
            {
                string endpoint = pair.Key;
                WebSocket ws = pair.Value;

                LiveMap.Log(LiveMap.LogLevel.Basic, "updateBlip for blip with id {0} to {1}", blip.type, endpoint);

                if (!ws.IsConnected)
                {
                    LiveMap.Log(LiveMap.LogLevel.All, "Disposing of websocket {0} because it's closed..", endpoint);
                    ws.Dispose();
                    return;
                }

                ws.WriteStringAsync(payload.ToString(Newtonsoft.Json.Formatting.None)).Wait(2000);

                //ws.WriteStringAsync(payload.ToString(Newtonsoft.Json.Formatting.None)).Wait();
            }
            writeLock = 0;
        }

    }
}
