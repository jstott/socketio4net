using System;
using System.Diagnostics;
using System.Threading;
using SocketIOClient;
using Newtonsoft.Json.Linq;
using System.Linq;
using System.Text.RegularExpressions;
using SocketIOClient.Messages;
using System.Net;

namespace ConsoleEvents
{
	/// <summary>
	/// Example usage class for SocketIO4Net
	/// </summary>
	public class EventClient
	{
		Client socket;
		public void Execute()
		{
			Console.WriteLine("Starting SocketIO4Net Client Events Example...");

            socket = new Client("http://localhost:3000/")
            {
            }; // url to the nodejs / socket.io instance
            //socket.TransportPeferenceTypes.Add(TransportType.XhrPolling);
			socket.Opened += SocketOpened;
			socket.Message += SocketMessage;
			socket.SocketConnectionClosed += SocketConnectionClosed;
			socket.Error += SocketError;

			// Optional to add HandShake headers - comment out if you do not have use
			socket.HandShake.Headers.Add("OrganizationId", "1034");
			socket.HandShake.Headers.Add("UserId", "TestSample");
            socket.HandShake.Headers.Add("Cookie", "somekookie=magicValue");
			// Register for all On Events published from the server - prior to connecting

			// register for 'connect' event with io server
			socket.On("connect", (fn) =>
			{
                Console.WriteLine("\r\nConnected event...{0}\r\n", socket.ioTransport.TransportType);
                socket.Emit("subscribe", new { room = "eventRoom" }); // client joins 'eventRoom' on server
			});


			// register for 'update' events - message is a json 'Part' object
			socket.On("update", (data) =>
			{
				Console.WriteLine("recv [socket].[update] event");
				Console.WriteLine("  raw message:      {0}", data.RawMessage);
				Console.WriteLine("  string message:   {0}", data.MessageText);
				Console.WriteLine("  json data string: {0}", data.Json.ToJsonString());
				// cast message as Part - use type cast helper
				Part part = data.Json.GetArgAs<Part>();
				Console.WriteLine(" PartNumber:   {0}\r\n", part.PartNumber);
			});

			// register for 'alerts' events - broadcast only to clients joined to 'Room1'
			socket.On("log", (data) =>
			{
				Console.WriteLine(" log: {0}", data.Json.ToJsonString());
			});
			socket.On("empty", (data) =>
			{
				Console.WriteLine(" message 'empty'");
			});
            //socket.Connect(SocketIOClient.TransportType.XhrPolling);
            socket.Connect();
		}

		public void SendMessageSamples()
		{

			// random examples of different styles of sending / recv payloads - will add to...
			socket.Send(new TextMessage("Hello from C# !")); // send plain string message
            socket.Emit("hello", new { msg = "My name is SocketIO4Net.Client!" }); // event w/string payload
			//socket.Emit("heartbeat"); // event w/o data payload (nothing to do with socket.io heartbeat)
			
			//socket.Emit("hello", "simple string msg");
			//socket.Emit("partInfo", new { PartNumber = "AT80601000741AA", Code = "SLBEJ", Level = 1 }); // event w/json payload
			
			//Part newPart = new Part() { PartNumber = "K4P2G324EC", Code = "DDR2", Level = 1 };
			//socket.Emit("partInfo", newPart); // event w/json payload


			// callback using namespace example 
			//Console.WriteLine("Emit [socket.logger].[messageAck] - should recv callback [socket::logger].[messageAck]");
			//socket.Emit("messageAck", new { hello = "papa" }, "",
			//	(callback) =>
			//	{
			//		var jsonMsg = callback as JsonEncodedEventMessage; // callback will be of type JsonEncodedEventMessage, cast for intellisense
			//		Console.WriteLine(string.Format("callback [socket::logger].[messageAck]: {0} \r\n", jsonMsg.ToJsonString()));
			//	});
		}

		void SocketError(object sender, ErrorEventArgs e)
		{
			Console.WriteLine("socket client error:");
			Console.WriteLine(e.Message);
		}

		void SocketConnectionClosed(object sender, EventArgs e)
		{
			Console.WriteLine("WebSocketConnection was terminated!");
		}

		void SocketMessage(object sender, MessageEventArgs e)
		{
			// uncomment to show any non-registered messages
			if (string.IsNullOrEmpty(e.Message.Event))
				Console.WriteLine("Generic SocketMessage: {0}", e.Message.MessageText);
			else
				Console.WriteLine("Generic SocketMessage: {0} : {1}", e.Message.Event, e.Message.JsonEncodedMessage.ToJsonString());
		}

		void SocketOpened(object sender, EventArgs e)
		{

		}

		public void Close()
		{
			if (this.socket != null)
			{
				socket.Opened -= SocketOpened;
				socket.Message -= SocketMessage;
				socket.SocketConnectionClosed -= SocketConnectionClosed;
				socket.Error -= SocketError;
				this.socket.Dispose(); // close & dispose of socket client
			}
		}
	}

}
