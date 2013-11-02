using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WebSocket4Net;

namespace SocketIOClient.Transports
{
	public class TransportFactory
	{
		public static ITransport Transport(TransportType type, IOHandshake handshake )
		{
			ITransport transport = null;
			switch (type)
			{
				case TransportType.Websocket:
                    string wsScheme = (handshake.Uri.Scheme == Uri.UriSchemeHttps ? "wss" : "ws");
                    var cs = string.Format("{0}://{1}:{2}/socket.io/1/websocket/{3}{4}", wsScheme, handshake.Uri.Host, handshake.Uri.Port,
												   handshake.SID, handshake.Uri.Query);
                    transport = new WebSocketProxy(cs, string.Empty, handshake);
					break;
				case TransportType.XhrPolling:
					transport = new XhrPolling(handshake);
					break;
				default:
					throw new ArgumentException(string.Format("An transport of type {0} cannot be found", Enum.GetName(typeof(TransportType), type)));
			}
			return transport;
		}
	}
}
