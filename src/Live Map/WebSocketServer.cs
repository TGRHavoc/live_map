using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using vtortola.WebSockets;

namespace Havoc.Live_Map
{

    public delegate void Connection(WebSocket ws);
    public delegate void Disconnection(WebSocket ws);
    public delegate void Message(WebSocket ws, string msg);
    public delegate void Error(WebSocket ws, Exception ex);

    public class WebSocketServer
    {
        public event Message OnMessage;
        public event Connection OnConnect;
        public event Disconnection OnDisconnect;
        public event Error OnError;

        WebSocketListener listener;

        public WebSocketServer(int port)
        {
            listener = new WebSocketListener(new System.Net.IPEndPoint(System.Net.IPAddress.Any, port));
        }


        public void Start()
        {
            listener.StartAsync().Wait();
        }

        public void Stop()
        {
            listener.StopAsync().Wait();
        }

        public async Task ListenAsync()
        {
            while (listener.IsStarted)
            {
                try
                {
                    WebSocket socket = await listener.AcceptWebSocketAsync(CancellationToken.None).ConfigureAwait(false);

                    if(socket != null)
                    {
                        Task.Run(() => HandleSocket(socket));
                    }

                }catch(Exception ex)
                {
                    if (OnError != null)
                        OnError.Invoke(null, ex);
                }
            }
        }

        private async Task HandleSocket(WebSocket ws)
        {
            try
            {
                if (OnConnect != null)
                    OnConnect.Invoke(ws);

                while (ws.IsConnected)
                {
                    string message = await ws.ReadStringAsync(CancellationToken.None).ConfigureAwait(false);
                    if (message != null && OnMessage != null)
                        OnMessage.Invoke(ws, message);

                }

                if (OnDisconnect != null)
                    OnDisconnect.Invoke(ws);

            }catch(Exception e)
            {
                if (OnError != null)
                    OnError.Invoke(ws, e);
            }
            finally
            {
                ws.Dispose();
            }
        }

        public void Dispose()
        {
            listener.Dispose();
        }


    }
}
