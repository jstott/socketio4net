using System;
using System.Threading.Tasks;
using WebSocket4Net;


namespace SocketIOClient
{
	public interface ITransport
	{
		WebSocketState State { get; }
        IOHandshake Handshake { get; set; }
		bool EnableAutoSendPing { get; set; }
        TransportType TransportType { get; }
		event EventHandler Opened;
		event EventHandler Closed;
		event EventHandler<SocketIOClient.ErrorEventArgs> Error;
        event EventHandler<TransportReceivedEventArgs> MessageReceived;

        Task<bool> OpenAsync();
	    Task<bool> Reconnect();
		void Close();
		void Send(string message);
		
	}
}
