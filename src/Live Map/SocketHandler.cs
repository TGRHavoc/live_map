using Newtonsoft.Json.Linq;
using System;
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

        List<WebSocket> clients;

        public SocketHandler(WebSocketServer server)
        {
            this.server = server;
            playerData = new JObject();
            clients = new List<WebSocket>();

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
            lock (clients)
            {
                clients.Remove(ws);
            }
        }

        private void Server_OnDisconnect(WebSocket ws)
        {
            LiveMap.Log(LiveMap.LogLevel.Basic, "Socket connection was closed at {0}", ws.RemoteEndpoint.ToString());
            lock (clients)
            {
                clients.Remove(ws);
            }
        }

        private void Server_OnConnect(WebSocket ws)
        {
            LiveMap.Log(LiveMap.LogLevel.Basic, "Socket connection opened at {0}", ws.RemoteEndpoint.ToString());
            lock (clients)
            {
                clients.Add(ws);
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
                await Task.Delay(500).ConfigureAwait(false);

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

                lock (clients)
                {
                    foreach(WebSocket ws in clients)
                    {
                        if (ws.IsConnected)
                        { // Some error occures when someone disconnects from the socket and this is called...
                            //LiveMap.Log(LiveMap.LogLevel.All, "Sent data payload to {0}", ws.RemoteEndpoint);
                            ws.WriteStringAsync(payload.ToString(Newtonsoft.Json.Formatting.None), CancellationToken.None);
                        }
                    }
                }

            }
        }

        public void AddPlayerData(string identifier, string key, object data)
        {
            LiveMap.Log(LiveMap.LogLevel.All, "Adding player {0}'s \"{1}\"", identifier, key);
            MakeSurePlayerExists(identifier);
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

        public void RemovePlayer(string identifier)
        {
            lock (playerData)
            {
                if (playerData[identifier] != null)
                {
                    if (playerData.Remove(identifier))
                    {
                        LiveMap.Log(LiveMap.LogLevel.Basic, "Removed player {0}", identifier);
                    }else
                    {
                        LiveMap.Log(LiveMap.LogLevel.Basic, "Couldn't remove player {0}... Seriously, there's something fucking wrong here...", identifier);
                    }
                }
            }

            //LiveMap.Log(LiveMap.LogLevel.All, "Notifying ws clients that a player left.");
            // Tell the client's that someone has left
            lock (clients)
            {
                foreach (WebSocket s in clients)
                {
                    JObject payload = new JObject();
                    payload["type"] = "playerLeft";
                    payload["payload"] = identifier;
                    //LiveMap.Log(LiveMap.LogLevel.All, "Sending playerLeft payload for {0}", identifier);
                    if (s.IsConnected)
                    {
                        //LiveMap.Log(LiveMap.LogLevel.All, "Sent PlayerLeft payload to {0}", s.RemoteEndpoint);
                        s.WriteStringAsync(payload.ToString(Newtonsoft.Json.Formatting.None), CancellationToken.None);
                    }
                }
            }

        }

    }
}
