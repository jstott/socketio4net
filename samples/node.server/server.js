process.env['DEBUG'] = 'server:*';
var config = require('./config.json'),
    debug = {
        io: require('debug')('server:io'),
        app: require('debug')('server:app'),
        server: require('debug')('server:server'),
        config: require('debug')('server:config'),
        error: require('debug')('server:error')
    },
    
    http = require('http'),
    express = require('express'),
    app = express(),
    util = require('util'),
    socketio = require('socket.io'),
    socketioWildcard = require('socket.io-wildcard'),
    server, io,
    
    //server = http.createServer(app),
    //io = socketio.listen(server),
    webApi = require('./routes/api'),
    store = require('./store'); // cheap key-value stand-in for redis

// Splash Info
debug.config('');
debug.config('SocketIO4Net Sample Server\r\n');
debug.config('\tNodejs: %s', process.version);
debug.config('\tsocket.io: v%s', socketio.version);
debug.config('\tListening on port %d', config.web.port);
debug.config('');

// *******************************
// Configure Express
// *******************************
app.configure('development', function () { // only in development
  app.use(logErrors);
});

app.configure(function () {
  app.use(express.favicon());
  app.use(express.bodyParser());
  app.use(express.compress());
  app.use(express.methodOverride());
  app.use(errorHandler);
  
  // serve static assets from these folder
  app.use('/scripts', express.static('scripts'));
  app.use('/content', express.static('content'));
  app.use('/app', express.static('app'));
  

  // basic usage logging
  app.use(function (req, res, next) {
    // console.log('%s %s', req.method, req.url);
    if (req.url.indexOf('/api') === 0) {
      store.incr('app.apiCount');
    }
    //watchBcast('log', { level: 5, zone: 'app', eventCode: 'request', message: 'url', data: { 'method': req.method, 'url': req.url, 'count': cnt } });
    next();
  });
  
});

server = http.createServer(app).listen(config.web.port);
//io = socketio.listen(server);
io = socketioWildcard(socketio).listen(server);

// General Handlers
app.get('/api/:target', webApi.get);
app.get('/config', function(req, res) {
  res.send(config.clientConfiguration);
});
app.get('/', function (req, res) {
  store.incr('app.visits');
  res.sendfile(__dirname + '/index.html');
  watchBcastAnalytics();
});

// *******************************
// Configure socket.io
// *******************************
io.enable('browser client minification');  // send minified client
io.enable('browser client etag');          // apply etag caching logic based on version number
io.enable('browser client gzip');          // gzip the file
io.set('log level', 1);                    // reduce logging: 0-error, 1-warn, 2-info, 3-debug
io.set('transports', ['websocket', 'xhr-polling', 'jsonp-polling', 'htmlfile']);

// *******************************
// socket.io handlers
// *******************************
io.sockets.on('connection', function (socket) { // initial connection from a client
  var transport = io.transports[socket.id].name,
      key = transport === 'websocket' ? 'websocket' : 'other';
  
  store.incr('io.connection.' + key);
  debug.io('client connection: %s', transport);
  watchBcastAnalytics();
  watchBcast('log', { zone: 'io', eventCode: 'connection', message: 'connection' }); // bcast connection count to 'watch' room
  watchBcast('log', { zone: 'server', eventCode: 'api', message: webApi.statusCounts() });
  socket.on('*', function onWildcard(event) {
      watchBcast('log', { zone: 'io', eventCode: event.name || '?', message: util.inspect(event) });
  });
  socket.on('message', function (data) {
      watchBcast('log', { zone: 'client', eventCode: 'message', message: data });
  });
  
  socket.on('echo', function (message) {
    socket.emit('echo', message);
  });
  socket.emit('news', { hello: 'world' });

  socket.on('clientBroadcast', function (message) {
    var combinedMsg;
    try {
      if (message.room) {
        combinedMsg = [message.room, message.event];
        socket.broadcast.to(message.room).emit(message.event, message.data); //emit to 'room' except this socket
      } else {
        combinedMsg = [message.event];
        socket.broadcast.emit(message.event, message.data); //emit to all sockets except this one
      }
      //watchBcast('log', { zone: 'clientBroadcast', eventCode: combinedMsg, message: message.data });
    } catch (err) {
        debug.io('clientBroadcast error', message, err);
      store.incr('io.errors');
      watchBcastAnalytics();
    }
  });
  // client request to join 'room' data.room by name
  socket.on('subscribe', function(event) {
    socket.join(event.room);
    if (event.room === 'watch') {
      socket.emit('analytics', webApi.statusCounts());
    }
  });
  // client request to leave 'room' data.room by name
  socket.on('unsubscribe', function (event) { socket.leave(event.room); });

  socket.on('disconnect', function () { // client-server connection is closed
    store.decr('io.connection.' + key);
    watchBcastAnalytics();
    watchBcast('log', { zone: 'io', eventCode: 'disconnect', message: 'client' });
  });
});

// *******************************
// Error Logging / broadcast helpers
// *******************************
function watchBcast(event, data) {
  try {
    data.dateTime = new Date(Date.now());
    io.sockets.in('watch').emit(event, data);
    debug.io(util.inspect(data));
  } catch(err) {
      debug.error('watchBcast error: %s', err);
    store.incr('io.errors');
  }
}
function watchBcastAnalytics() {
  watchBcast('analytics', webApi.statusCounts());
}

function logErrors(err, req, res, next) {
  store.incr('app:errors');
  var status = err.statusCode || 500;
  debug.io(status + ' ' + (err.message ? err.message : err));
  if (err.stack) {
      debug.error(err.stack);
  }
  watchBcastAnalytics();
  next(err);
}

function errorHandler(err, req, res, next) {
  var status = err.statusCode || 500;
  if (err.message) {
    res.send(status, err.message);
  } else {
    res.send(status, err);
  }
  watchBcastAnalytics();
}

process.on('uncaughtException', function (err) {
  // handle the error safely
    debug.error(err);
  store.incr('app:errors');
  watchBcastAnalytics();
});