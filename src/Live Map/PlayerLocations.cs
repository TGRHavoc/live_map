using Newtonsoft.Json.Linq;

using System.Diagnostics;

namespace Havoc.Live_Map
{
    /*
    class PlayerLocations : WebSocketBehavior
    {
        protected override void OnOpen()
        {
        }

        protected override void OnMessage(MessageEventArgs e)
        {
            if (e.IsText)
            {
                string[] args = e.Data.Split(' ');
                if (args[0] == "getLocations")
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
            }
        }
    }
    */
}
