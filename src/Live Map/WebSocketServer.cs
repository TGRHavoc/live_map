using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using vtortola.WebSockets;
using vtortola.WebSockets.Rfc6455;

/*
    Copyright (c) 2017 Kevin Poirot
    Original file can be found at: https://github.com/Hellslicer/WebSocketServer/blob/master/WebSocketEventListener.cs
    
    Modified (slighly) by Jordan Dalton
*/

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
            WebSocketListenerOptions opts = new WebSocketListenerOptions()
            {
                SubProtocols = new string[] { "text" }
            };
            opts.Standards.RegisterRfc6455();

            listener = new WebSocketListener(new System.Net.IPEndPoint(System.Net.IPAddress.Any, port), opts);

            LiveMap.Log(LiveMap.LogLevel.Basic, "Created websocket server");
        }


        public async void Start()
        {
            await listener.StartAsync();
        }

        public async void Stop()
        {
            await listener.StopAsync();
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
                    LiveMap.Log(LiveMap.LogLevel.Basic, "Error in ListenAsync:\n{0}\n---Inner:\n{1} ", ex.Message, ex.InnerException);
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
                LiveMap.Log(LiveMap.LogLevel.Basic, "Error in HandleSocket:\n{0}\n---Inner:\n{1}", e.Message, e.InnerException);
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
