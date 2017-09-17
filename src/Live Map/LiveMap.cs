using System;
using System.Collections.Generic;

using vtortola.WebSockets.Async;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using vtortola.WebSockets;

namespace Havoc.Live_Map
{
    public class LiveMap : BaseScript
    {

        WebSocketServer server;
        List<WebSocket> clients;

        public LiveMap()
        {
            int port = API.GetConvarInt("socket_port", 30121);
            Debug.WriteLine("Starting on port {0}", port);

            server = new WebSocketServer(port);

            EventHandlers["onResourceStart"] += new Action<string>(OnStart);
            EventHandlers["onResourceStop"] += new Action<string>(OnStop);


            server.OnConnect += Server_OnConnect;
            server.OnDisconnect += Server_OnDisconnect;
            server.OnError += Server_OnError;
            server.OnMessage += Server_OnMessage;
        }

        private void Server_OnMessage(WebSocket ws, string msg)
        {
            throw new NotImplementedException();
        }

        private void Server_OnError(WebSocket ws, Exception ex)
        {
            throw new NotImplementedException();
        }

        private void Server_OnDisconnect(WebSocket ws)
        {
            throw new NotImplementedException();
        }

        private void Server_OnConnect(vtortola.WebSockets.WebSocket ws)
        {
            throw new NotImplementedException();
        }

        public void OnStart(string name)
        {
            if (name == API.GetCurrentResourceName())
            {
                try
                {
                    server.Start();
                    Tick += server.ListenAsync;

                }catch(Exception e)
                {
                    Debug.WriteLine("Couldn't start {0}: {1}", name, e.Message);
                }
            }
        }

        public void OnStop(string name)
        {
            if (name == API.GetCurrentResourceName())
            {
                server.Stop();
                server.Dispose();
            }
        }

        public void addPlayer(string identifier, string name, float x = 0f, float y = 0f, float z = 0f)
        {
        }
        public void addPlayerString(string id, string key, string data)
        {
            Debug.WriteLine("Adding string data \"{0}\" with value \"{1}\"", key, data);
        }
        public void addPlayerFloat(string id, string key, float data)
        {
            Debug.WriteLine("Adding float data \"{0}\" with value \"{1}\"", key, data);
        }

        public void removePlayer(string identifier)
        {
            Debug.WriteLine("Removing player \"{0}\"", identifier);
        }

        public void addBlip(string name, string desc="", string type = "waypoint", float x = 0f, float y = 0f, float z = 0f)
        {
            Debug.WriteLine("Adding blip with name \"{0}\" of type \"{1}\"", name, type);
        }

        public void initBlips(string blipJson)
        {
        }

    }
}
