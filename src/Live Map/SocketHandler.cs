using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using vtortola.WebSockets;

namespace Havoc.Live_Map
{
    class SocketHandler
    {
        WebSocketServer server;
        JObject playerData;

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
            LiveMap.Log("Recieved message from client {0}:\n\t\"{1}\"", ws.RemoteEndpoint.ToString(), msg);

            if(args[0] == "getLocations")
            {
                // TODO: Send them the "playerData"
                lock (playerData)
                {
                    LiveMap.Log("Debug\n\n{0}\n\n", playerData.ToString(Newtonsoft.Json.Formatting.Indented));
                }
            }

        }

        private void Server_OnError(WebSocket ws, Exception ex)
        {
            LiveMap.Log("Socket error from {0}: {1}", ws == null ? "Unknown" : ws.RemoteEndpoint.ToString(), ex.Message);
        }

        private void Server_OnDisconnect(WebSocket ws)
        {
            LiveMap.Log("Socket connection was closed at {0}", ws.RemoteEndpoint.ToString());
        }

        private void Server_OnConnect(WebSocket ws)
        {
            LiveMap.Log("Socket connection opened at {0}", ws.RemoteEndpoint.ToString());
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

        public void AddPlayerData(string identifier, string key, object data)
        {
            LiveMap.Log("Adding player {0}'s \"{1}\"", identifier, key);
            MakeSurePlayerExists(identifier);
            lock (playerData)
            {
                JObject playerObj = (JObject)playerData[identifier];
                
                if(playerObj[key] == null)
                    playerObj.Add(key, JToken.FromObject(data));
            }

            LiveMap.Log("Added \"{1}\" to player {0} with value of \"{2}\"", identifier, key, data);
        }

        public void UpdatePlayerData(string identifier, string key, object newData)
        {
            LiveMap.Log("Updating player {0}'s \"{1}\"", identifier, key);
            MakeSurePlayerExists(identifier);
            lock (playerData)
            {
                JObject playerObj = (JObject)playerData[identifier];
                playerObj[key] = JToken.FromObject(newData);
                playerData[identifier] = playerObj;
            }

            LiveMap.Log("Updated player {0}'s \"{1}\" to \"{2}\"", identifier, key, newData);
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
                        LiveMap.Log("Removed \"{0}\" from player {1}", key, identifier);
                    }else
                    {
                        LiveMap.Log("Couldn't remove \"{0}\" from player {1}", key, identifier);
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
                        LiveMap.Log("Removed player {0}", identifier);
                    }else
                    {
                        LiveMap.Log("Couldn't remove player {0}... Seriously, there's something fucking wrong here...", identifier);
                    }
                }
            }
        }

    }
}
