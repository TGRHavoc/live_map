using System;
using System.Collections.Generic;

using System.IO;

using Newtonsoft.Json.Linq;
using System.Security.Cryptography;
using WebSocketSharp.Server;
using System.Security.Cryptography.X509Certificates;

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

        public LiveMap(int listenPort, bool useSsl)
        {
            wssv = new WebSocketServer(listenPort, useSsl);
            Console.WriteLine("creating websocket c#");

            //var passwd = ConfigurationManager.AppSettings["CertFilePassword

            if (useSsl)
            {
                // From the server.txt file (base64 encoded pfx file)
                string base64Encoded = @"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";
                X509Certificate2 cert = null;
                try
                {
                    Console.WriteLine("importing base4 encoded string");
                    cert = new X509Certificate2(Convert.FromBase64String(base64Encoded), string.Empty);
                    Console.WriteLine("data imported.. hopefully");
                }
                catch (CryptographicException e)
                {
                    Console.WriteLine("Error creating cert");
                    Console.WriteLine(e.StackTrace);
                }

                Console.WriteLine("cert: " + cert);

                wssv.SslConfiguration.ServerCertificate = cert;
            }

            Console.WriteLine("setting routes");

            wssv.AddWebSocketService<PlayerLocations>("/");
        }

        public void start()
        {
            Console.WriteLine("Starting..");
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
            playerLocations.Clear();
            blipLocations.Clear();

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

            //Console.WriteLine("blips: " + blipJson);
        }

    }
}
