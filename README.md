socketio4net
============

SocketIO4Net.Client provides a .NET 4.0 &amp; v4.5 (C#) client for Socket.IO.  It provides an overall interface similar to a client JavaScript experience, leveraging the WebSocket4Net project for an underlying websocket implementation.

My goal for this project is a simple & familiar experience for .net clients.  You know you want your .Net app to join in some of the fun, right?  Besides, all the cool kids are using Nodejs and Socket.IO these days anyway, give it a whirl.

## Development Branch

The Development branch contains wip code that incorporates xhr-polling in addition to websocket support.  A new webserver & client dashboard has also been added into the samples in samples/node.sever.

From VS2012/VS2013, compile and start the the Samples/Console_Events project after you have started the samples/node.server.

*In order to run the samples/node.server dashboard - you will need to run `npm install` in that directory initially.*

* `install.cmd` will run npm install for you
* `startServer.cmd` will then run the nodejs server, and launch a chrome browser window to http://localhost:3000
* `debugServer.cmd` will run node-inspector and launch chrome browser and allow you to run a debug session against the server

* At this time, Nuget packages have not yet been updated

This resulting signature is very similar to the socket.io javascript counterpart:

## node.js / JavaScript client
````
socket.on('news', function (data) {
    console.log(data);
});
````
## C# .net client
````
socket.On("news", (data) =>    {
    Console.WriteLine(data);
});
````
The all important - Sample / Demo code snippet 

Client socket;
public void Execute()
{
    Console.WriteLine("Starting TestSocketIOClient Example...");

    socket = new Client("http://127.0.0.1:3000/"); // url to nodejs 
    socket.Opened += SocketOpened;
    socket.Message += SocketMessage;
    socket.SocketConnectionClosed += SocketConnectionClosed;
    socket.Error += SocketError;
            
    // register for 'connect' event with io server
    socket.On("connect", (fn) =>
    {
        Console.WriteLine("\r\nConnected event...\r\n");
        Console.WriteLine("Emit Part object");

        // emit Json Serializable object, anonymous types, or strings
        Part newPart = new Part() 
        { PartNumber = "K4P2G324EC", Code = "DDR2", Level = 1 };
        socket.Emit("partInfo", newPart);
    });

    // register for 'update' events - message is a json 'Part' object
    socket.On("update", (data) =>
    {
        Console.WriteLine("recv [socket].[update] event");
        //Console.WriteLine("  raw message:      {0}", data.RawMessage);
        //Console.WriteLine("  string message:   {0}", data.MessageText);
        //Console.WriteLine("  json data string: {0}", data.Json.ToJsonString());
        //Console.WriteLine("  json raw:         {0}", data.Json.Args[0]);
                
        // cast message as Part - use type cast helper
        Part part = data.Json.GetFirstArgAs<Part>();
        Console.WriteLine(" Part Level:   {0}\r\n", part.Level);
    });

    // make the socket.io connection
    socket.Connect();
}

