using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocketIOClient
{
    public interface IClientTransport : IDisposable
    {
        string Name { get; }
        bool SupportsKeepAlive { get; }

        //Task<NegotiationResponse> Negotiate(IClient connection);
        //Task Start(IClient connection, string data, CancellationToken disconnectToken);
        //Task Send(IClient connection, string data);
        //void Abort(IClient connection, TimeSpan timeout);

        //void LostConnection(IConnection connection);
    }
}
