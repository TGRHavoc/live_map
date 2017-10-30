using System;
using System.Collections.Generic;

using CitizenFX.Core;
using CitizenFX.Core.Native;
using System.Linq;

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
    public class LiveMap : BaseScript
    {
        // The current debug level (from the "livemap_debug" convar)
        public static int debugLevel = 0;

        public static int waitSeconds = 500;

        public enum LogLevel
        {
            None = 0, // Don't display any log messages
            Basic = 1,
            All = 2
        }

        WebSocketServer server;
        SocketHandler handler;

        public static void Log(LogLevel level, string format, params object[] vars)
        {
            if (debugLevel >= (int)level && debugLevel != 0)
            {
                Debug.WriteLine("Havoc's LiveMap(" + level + "):\n\t" + format + "\n", vars);
            }
        }

        public LiveMap()
        {
            int port = API.GetConvarInt("socket_port", 30121);

            string debugEnabled = API.GetConvar("livemap_debug", "0");
            bool suc = int.TryParse(debugEnabled, out debugLevel);

            if (!suc)
            {
                Debug.WriteLine("Couldn't parse \"{0}\". Apparently it isn't a int \n\tDefaulting to {2}", debugEnabled, 0);
            }

            waitSeconds = API.GetConvarInt("livemap_milliseconds", 500);

            Log(LogLevel.Basic, "Starting on port {0}", port);

            server = new WebSocketServer(port);
            handler = new SocketHandler(server);

            EventHandlers["onResourceStart"] += new Action<string>(OnStart);
            EventHandlers["onResourceStop"] += new Action<string>(OnStop);

            EventHandlers["livemap:internal_AddPlayerData"] += new Action<string, string, dynamic>(InternalAddPlayerData);
            EventHandlers["livemap:internal_UpdatePlayerData"] += new Action<string, string, dynamic>(InternalUpdatePlayerData);

            EventHandlers["livemap:internal_RemovePlayerData"] += new Action<string, string>(InternalRemovePlayerData);
            EventHandlers["livemap:internal_RemovePlayer"] += new Action<string>(InternalRemovePlayer);

            EventHandlers["livemap:internal_AddBlip"] += new Action<dynamic>(InternalAddBlip);
            EventHandlers["livemap:internal_UpdateBlip"] += new Action<dynamic>(InternalUpdateBlip);
            EventHandlers["livemap:internal_RemoveBlip"] += new Action<dynamic>(InternalRemoveBlip);
        }

        public void OnStart(string name)
        {
            if (name == API.GetCurrentResourceName())
            {
                try
                {
                    server.Start();
                    Tick += server.ListenAsync;
                    Tick += handler.SendWebsocketData;

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

        private void InternalAddBlip(dynamic blip)
        {
            handler.AddBlip(blip);
        }
        private void InternalUpdateBlip(dynamic blip)
        {
            handler.UpdateBlip(blip);
        }
        private void InternalRemoveBlip(dynamic blip)
        {
            handler.RemoveBlip(blip);
        }

    }
}
