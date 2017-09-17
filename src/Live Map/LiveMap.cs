using System;
using System.Collections.Generic;

using CitizenFX.Core;
using CitizenFX.Core.Native;

namespace Havoc.Live_Map
{
    public class LiveMap : BaseScript
    {

        public static bool doDebug = false;

        WebSocketServer server;
        SocketHandler handler;

        public static void Log(string format, params object[] vars)
        {
            if (doDebug)
            {
                Debug.WriteLine("Havoc's LiveMap:\n\t" + format + "\n", vars);
            }
        }

        public LiveMap()
        {
            int port = API.GetConvarInt("socket_port", 30121);

            string debugEnabled = API.GetConvar("livemap_debug", bool.FalseString);
            bool suc = bool.TryParse(debugEnabled, out doDebug);

            if (!suc)
            {
                Debug.WriteLine("Couldn't parse \"{0}\". Apparently it isn't a boolean (\"{1}\" or \"{2}\")\n\tDefaulting to {2}", debugEnabled, bool.TrueString, bool.FalseString);
            }

            Log("Starting on port {0}", port);

            server = new WebSocketServer(port);
            handler = new SocketHandler(server);

            EventHandlers["onResourceStart"] += new Action<string>(OnStart);
            EventHandlers["onResourceStop"] += new Action<string>(OnStop);

            EventHandlers["livemap:internal_AddPlayerData"] += new Action<string, string, dynamic>(InternalAddPlayerData);
            EventHandlers["livemap:internal_UpdatePlayerData"] += new Action<string, string, dynamic>(InternalUpdatePlayerData);

            EventHandlers["livemap:internal_RemovePlayerData"] += new Action<string, string>(InternalRemovePlayerData);
            EventHandlers["livemap:internal_RemovePlayer"] += new Action<string>(InternalRemovePlayer);
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
                    Debug.WriteLine("Couldn't start {0}: {1}", name, e.StackTrace);
                }
            }
        }

        public void OnStop(string name)
        {
            if (name == API.GetCurrentResourceName())
            {

                if (server != null) // Apparently this is a thing that can happen :/
                {
                    server.Stop();
                    server.Dispose();
                }
                
            }
        }

        private void InternalAddPlayerData(string identifier, string key, dynamic data)
        {
            handler.AddPlayerData(identifier, key, data);
        }

        private void InternalUpdatePlayerData(string identifier, string key, dynamic data)
        {
            handler.UpdatePlayerData(identifier, key, data);
        }
        private void InternalRemovePlayerData(string id, string key)
        {
            handler.RemovePlayerData(id, key);
        }
        private void InternalRemovePlayer(string id)
        {
            handler.RemovePlayer(id);
        }


    }
}
