using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WebSocket4Net;

namespace SocketIOClient
{
    public class WebSocketProxy : WebSocket, ITransport
    {
        private const int TimeOut = 4000;
        // Expose ITransport events
        public new event EventHandler<EventArgs> Opened;
        public new event EventHandler<TransportReceivedEventArgs> MessageReceived;
        public new event EventHandler<EventArgs> Closed;
        public new event EventHandler<ErrorEventArgs> Error;

        private readonly ManualResetEvent _connectionOpenEvent = new ManualResetEvent(false);

        public new WebSocketState State
        {
            get { return (WebSocketState)(int)base.State; } // expose state as base-lib dependent enum
        }
        public IOHandshake Handshake { get; set; }

        public TransportType TransportType
        {
            get { return TransportType.Websocket; }
        }
        public WebSocketProxy(string uri, string subProtocol, IOHandshake handshake)
            : base(uri, subProtocol, WebSocketVersion.None)
        {
            
            base.EnableAutoSendPing = false;
            base.Opened += WebSocketProxyOpened;
            base.MessageReceived += WebSocketProxyMessageReceived;
            base.Closed += WebSocketProxyClosed;
            base.Error += WebSocketProxyError;
        }

        // Handlers for base/internal events, signal external events as appropriate
        void WebSocketProxyOpened(object sender, EventArgs e)
        {
            _connectionOpenEvent.Set();
            if (this.Opened != null)
            {
                this.Opened(this, EventArgs.Empty);
            }
        }
        void WebSocketProxyMessageReceived(object sender, MessageReceivedEventArgs e)
        {
            if (this.MessageReceived != null)
            {
                this.MessageReceived(this, new TransportReceivedEventArgs(e.Message));
            }
        }
        void WebSocketProxyClosed(object sender, EventArgs e)
        {
            if (this.Closed != null)
            {
                this.Closed(this, EventArgs.Empty);
            }
        }
        void WebSocketProxyError(object sender, SuperSocket.ClientEngine.ErrorEventArgs e)
        {
 	        if (this.Error != null)
                            this.Error(this, new ErrorEventArgs("WebSocket Error", e.Exception));
        }
        
        public Task<bool> OpenAsync()
        {
            var tcs = new TaskCompletionSource<bool>();
            try
            {
                base.Open();
                bool connected = this._connectionOpenEvent.WaitOne(TimeOut); // block while waiting for connection
                tcs.SetResult(connected);
            }
            catch (Exception ex)
            {
                tcs.SetException(ex);
            }
            return tcs.Task;
        }

        public new void Close()
        {
            base.Error -= WebSocketProxyError;
            base.MessageReceived -= WebSocketProxyMessageReceived;
            base.Opened -= WebSocketProxyOpened;
            base.Close();
        }
        public async Task<bool> Reconnect()
        {
            return await OpenAsync();
        }
    }
}
