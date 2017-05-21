using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using Newtonsoft.Json.Linq;
using System.Security.Cryptography;
using System.Threading;
using WebSocketSharp.Server;

namespace Havoc.Live_Map
{
    public class LiveMap
    {
        WebSocketServer wssv;

        public static StreamWriter file = new StreamWriter("live_map.log");

        public static JArray playerLocations = new JArray();
        public static JArray blipLocations = new JArray();

        public bool isOpen()
        {
            return wssv.IsListening;
        }

        public LiveMap(int listenPort)
        {
            wssv = new WebSocketServer(listenPort);

            wssv.AddWebSocketService<PlayerLocations>("/");

            wssv.Start();
            if (wssv.IsListening)
            {
                file.WriteLine("Listening on port {0}", wssv.Port);
                Console.WriteLine("Listening on port {0}, and providing WebSocket services:", wssv.Port);
                foreach (var path in wssv.WebSocketServices.Paths)
                {
                    file.WriteLine("- {0}", path);
                }
            }
        }

        public void stop()
        {
            file.Flush();
            wssv.Stop();
        }

        public void addPlayer(string name, float x = 0f, float y = 0f, float z = 0f)
        {
            lock (playerLocations)
            {
                bool updatedPlayer = false;
                foreach (var item in playerLocations)
                {
                    if (item["name"].ToString() == name)
                    {
                        // Update it
                        item["x"] = x;
                        item["y"] = y;
                        item["z"] = z;

                        updatedPlayer = true;
                    }
                    else
                    {
                        continue;
                    }
                }

                if (!updatedPlayer)
                {
                    // Add them
                    JObject playerObj = new JObject();
                    playerObj.Add("name", name);
                    playerObj.Add("x", x);
                    playerObj.Add("y", y);
                    playerObj.Add("z", z);

                    playerLocations.Add(playerObj);
                }
            }
        }

        public void removePlayer(string name)
        {
            lock (playerLocations)
            {
                JToken token = null;
                foreach (var item in playerLocations)
                {
                    if (item["name"].ToString() == name)
                    {
                        token = item;
                    }
                }
                if(token != null)
                {
                    playerLocations.Remove(token);
                }
            }

            JObject obj = new JObject();
            obj["type"] = "playerLeft";
            obj["payload"] = name;

            wssv.WebSocketServices["/"].Sessions.Broadcast(obj.ToString(Newtonsoft.Json.Formatting.None));
        }

        public void addBlip(string name, string type = "waypoint", float x = 0f, float y = 0f, float z = 0f)
        {
            JObject blip = new JObject();

            blip["name"] = name;
            blip["type"] = type;
            blip["x"] = x;
            blip["y"] = y;
            blip["z"] = z;

            lock (blipLocations)
            {
                blipLocations.Add(blip);
            }
        }

        public void addBlips(string blipJson)
        {
            lock (blipLocations)
            {
                blipLocations = JArray.Parse(blipJson);
            }

            Console.WriteLine("blips: " + blipJson);
        }

    }
}
