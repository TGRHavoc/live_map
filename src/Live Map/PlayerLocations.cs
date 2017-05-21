using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace Havoc.Live_Map
{
    class PlayerLocations : WebSocketBehavior
    {
        protected override void OnOpen()
        {
        }

        protected override void OnMessage(MessageEventArgs e)
        {
            if (e.IsText)
            {
                LiveMap.file.WriteLine(e.Data);
                if (e.Data == "getLocations")
                {
                    // TODO: Send locations
                    JObject obj = new JObject();
                    obj["type"] = "players";
                    lock (LiveMap.playerLocations)
                    {
                        obj["payload"] = LiveMap.playerLocations;
                    }
                    LiveMap.file.WriteLine("Sending\n" + obj.ToString(Newtonsoft.Json.Formatting.None));
                    Send(obj.ToString(Newtonsoft.Json.Formatting.None));
                }
                else if(e.Data == "getBlips")
                {
                    //TODO: Send blips
                    JObject obj = new JObject();
                    obj["type"] = "blips";

                    lock (LiveMap.blipLocations)
                    {
                        obj["payload"] = LiveMap.blipLocations;
                    }
                    LiveMap.file.WriteLine("Sending\n" + obj.ToString(Newtonsoft.Json.Formatting.None));
                    Send(obj.ToString(Newtonsoft.Json.Formatting.None));
                }
            }
        }
    }
}
