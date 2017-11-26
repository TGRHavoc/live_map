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

        ConcurrentQueue<JObject> sendQueue = new ConcurrentQueue<JObject>();

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
            LiveMap.Log(LiveMap.LogLevel.Basic, "Recieved message from client {0}:\n\t\"{1}\"", ws.RemoteEndpoint.ToString(), msg);
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
            LiveMap.Log(LiveMap.LogLevel.All, "Socket connection was closed at {0}", ws.RemoteEndpoint.ToString());

            WebSocket destory;
            if (clients.TryRemove(ws.RemoteEndpoint.ToString(), out destory))
            {
                destory.CloseAsync().GetAwaiter().GetResult();
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
            LiveMap.Log(LiveMap.LogLevel.All, "Socket connection opened at {0}", ws.RemoteEndpoint.ToString());

            if (clients.TryAdd(ws.RemoteEndpoint.ToString(), ws))
            {
                LiveMap.Log(LiveMap.LogLevel.All, "Added client {0} to the client dictionary", ws.RemoteEndpoint.ToString());
            }
            else
            {
                LiveMap.Log(LiveMap.LogLevel.All, "Couldn't add {0} to the client dic", ws.RemoteEndpoint.ToString());

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

                //LiveMap.Log(LiveMap.LogLevel.All, "Checking send queue");
                List<string> disconnectedClients = new List<string>();
                if (sendQueue.Count != 0)
                {
                    JObject payload;
                    if (sendQueue.TryDequeue(out payload))
                    {

                        foreach (KeyValuePair<string, WebSocket> pair in clients)
                        {
                            string endpoint = pair.Key;
                            WebSocket ws = pair.Value;

                            if (!ws.IsConnected)
                            {
                                disconnectedClients.Add(endpoint);
                                continue;
                            }

                            LiveMap.Log(LiveMap.LogLevel.All, "Sending payload of \"{0}\" to {1}", payload["type"], endpoint);

                            await ws.WriteStringAsync(payload.ToString(Newtonsoft.Json.Formatting.None), CancellationToken.None).ConfigureAwait(false);
                        }
                    }
                    else
                    {
                        LiveMap.Log(LiveMap.LogLevel.Basic, "Couldn't get the latest payload to send.");
                    }

                }
                else
                {
                    // No payload in the queue, may as well send player data

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

                    if (playerDataArray.Count == 0)
                    {
                        //LiveMap.Log(LiveMap.LogLevel.All, "playerDataArray.Count is 0");
                        continue;
                    }
                    payload["type"] = "playerData";
                    payload["payload"] = playerDataArray;

                    foreach (KeyValuePair<string, WebSocket> pair in clients)
                    {
                        string endpoint = pair.Key;
                        WebSocket ws = pair.Value;

                        if (!ws.IsConnected)
                        {
                            disconnectedClients.Add(endpoint);
                            continue;
                        }

                        //LiveMap.Log(LiveMap.LogLevel.All, "Sending payload of \"{0}\" to {1}", payload["type"], endpoint);
                        await ws.WriteStringAsync(payload.ToString(Newtonsoft.Json.Formatting.None), CancellationToken.None).ConfigureAwait(false);
                    }
                } // end of payload sending.. Time to disconnect clients that are no longer connected

                //LiveMap.Log(LiveMap.LogLevel.All, "Client size: {0}", clients.Count);
                foreach(string endpoint in disconnectedClients)
                {
                    WebSocket des;
                    if(clients.TryRemove(endpoint, out des))
                    {
                        LiveMap.Log(LiveMap.LogLevel.All, "Garbage cleanup.. Removed disconnected client {0}", endpoint);
                        des.Dispose();
                    }
                }

            }
        }

        public void AddPlayerData(string identifier, string key, object data)
        {
            if (string.IsNullOrEmpty(identifier))
            {
                LiveMap.Log(LiveMap.LogLevel.Basic, "Identifier is null or empty");
                return;
            }
            if (string.IsNullOrEmpty(key))
            {
                LiveMap.Log(LiveMap.LogLevel.Basic, "Cannot add key to player ({0}) because it's null or empty", identifier);
                return;
            }

            if (data == null)
            {
                LiveMap.Log(LiveMap.LogLevel.Basic, "Cannot add \"{1}\" to player ({0}) because it's null.", identifier, key);
                return;
            }

            LiveMap.Log(LiveMap.LogLevel.All, "Adding player {0}'s \"{1}\"", identifier, key);
            MakeSurePlayerExists(identifier);

            lock (playerData)
            {
                JObject playerObj = (JObject)playerData[identifier];

                if(playerObj[key] == null)
                    playerObj.Add(key, JToken.FromObject(data));

                //PlayerHadBeenUpdated(identifier, playerObj);
            }

            LiveMap.Log(LiveMap.LogLevel.Basic, "Added \"{1}\" to player {0} with value of \"{2}\"", identifier, key, data);
        }

        public void UpdatePlayerData(string identifier, string key, object newData)
        {
            if (string.IsNullOrEmpty(identifier))
            {
                LiveMap.Log(LiveMap.LogLevel.Basic, "Identifier is null or empty. Cannot update player data");
                return;
            }
            if (string.IsNullOrEmpty(key))
            {
                LiveMap.Log(LiveMap.LogLevel.Basic, "Cannot update player ({0}) because key is null or empty", identifier);
                return;
            }

            // Check if `data` is null
            if (newData == null)
            {
                LiveMap.Log(LiveMap.LogLevel.Basic, "Cannot update \"{0}\" for {1} because it's null", key, identifier);
                return;
            }
            LiveMap.Log(LiveMap.LogLevel.All, "Updating player {0}'s \"{1}\"", identifier, key);
            MakeSurePlayerExists(identifier);

            lock (playerData)
            {
                JObject playerObj = (JObject)playerData[identifier];
                playerObj[key] = JToken.FromObject(newData);
                playerData[identifier] = playerObj;

                //PlayerHadBeenUpdated(identifier, playerObj);
            }

            LiveMap.Log(LiveMap.LogLevel.All, "Updated player {0}'s \"{1}\" to \"{2}\"", identifier, key, newData);
        }

        public void RemovePlayerData(string identifier, string key)
        {

            if (string.IsNullOrEmpty(identifier))
            {
                LiveMap.Log(LiveMap.LogLevel.Basic, "Identifier is null or empty.. Cannot remove player data");
                return;
            }
            if (string.IsNullOrEmpty(key))
            {
                LiveMap.Log(LiveMap.LogLevel.Basic, "Cannot remove data from player ({0}) because key is null or empty", identifier);
                return;
            }

            MakeSurePlayerExists(identifier);
            lock (playerData)
            {
                JObject playerObj = (JObject)playerData[identifier];
                if (playerObj[key] != null)
                {
                    if (playerObj.Remove(key))
                    {
                        LiveMap.Log(LiveMap.LogLevel.Basic, "Removed \"{0}\" from player {1}", key, identifier);
                        //PlayerHadBeenUpdated(identifier, playerObj);
                    }
                    else
                    {
                        LiveMap.Log(LiveMap.LogLevel.Basic, "Couldn't remove \"{0}\" from player {1}", key, identifier);
                    }
                }// else = already removed
            }
        }

        public void RemovePlayer(string identifier)
        {
            if (string.IsNullOrEmpty(identifier))
            {
                LiveMap.Log(LiveMap.LogLevel.Basic, "Identifier is null or empty. Cannot remove player");
                return;
            }

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
                JObject payload = new JObject();
                payload["type"] = "playerLeft";
                payload["payload"] = identifier;

                sendQueue.Enqueue(payload);
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

        public void AddBlip(dynamic blip)
        {
            if(blip == null)
            {
                LiveMap.Log(LiveMap.LogLevel.Basic, "Cannot add blip as it's null");
                return;
            }

            JObject payload = new JObject();
            payload["type"] = "addBlip";
            payload["payload"] = ConvertBlip(blip);

            sendQueue.Enqueue(payload);
        }

        public void RemoveBlip(dynamic blip)
        {
            if (blip == null)
            {
                LiveMap.Log(LiveMap.LogLevel.Basic, "Cannot remove blip as it's null");
                return;
            }

            JObject payload = new JObject();
            payload["type"] = "removeBlip";
            payload["payload"] = ConvertBlip(blip);

            sendQueue.Enqueue(payload);
        }

        public void UpdateBlip(dynamic blip)
        {
            if (blip == null)
            {
                LiveMap.Log(LiveMap.LogLevel.Basic, "Cannot update blip as it's null");
                return;
            }

            JObject payload = new JObject();
            payload["type"] = "updateBlip";
            payload["payload"] = ConvertBlip(blip);

            sendQueue.Enqueue(payload);

        }

    }
}
