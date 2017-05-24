using Newtonsoft.Json.Linq;

using System.Diagnostics;

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
                if (e.Data == "getLocations")
                {
                    JObject obj = new JObject();
                    obj["type"] = "players";
                    lock (LiveMap.playerLocations)
                    {
                        obj["payload"] = LiveMap.playerLocations;
                    }

                    Debug.WriteLine("Sending locations\n{0}", obj.ToString(Newtonsoft.Json.Formatting.None));
                    Send(obj.ToString(Newtonsoft.Json.Formatting.None));
                }
                else if(e.Data == "getBlips")
                {
                    JObject obj = new JObject();
                    obj["type"] = "blips";

                    lock (LiveMap.blipLocations)
                    {
                        obj["payload"] = LiveMap.blipLocations;
                    }

                    Debug.WriteLine("Sending blips\n{0}", obj.ToString(Newtonsoft.Json.Formatting.None));
                    Send(obj.ToString(Newtonsoft.Json.Formatting.None));
                }
                // TODO: Expand for more socket commands
            }
        }
    }
}
