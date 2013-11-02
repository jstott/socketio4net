using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SocketIOClient
{
    public class TransportReceivedEventArgs : EventArgs
    {
        public TransportReceivedEventArgs(string message)
        {
            this.Message = message;
        }

        public string Message { get; set; }
    }
}
