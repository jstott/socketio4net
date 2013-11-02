define(['durandal/system', 'durandal/app', 'durandal/events', 'global/config'], function (system, app, Events, config) {
  var maxReconnect = 6,
      defOptions = {
        'reconnect': true,
        'connect timeout': 8000,
        'reconnection delay': 250,
        'reconnection limit': 4000,
        'max reconnection attempts': maxReconnect,
        // to debug specific transports, comment out 'try multiple transports', and uncomment 'transports'
        'try multiple transports': true,
        //'transports': ['xhr-polling'],
        'force new connection': true
      },
      Client = function (options) {
        var self = this;
        this.socket = undefined;
        this.options = system.extend(defOptions, options);
        Events.includeIn(this);
      };

  Client.prototype.connect = function (url) {
    var self = this;
    return system.defer(function (dfd) {
      self.socket = io.connect(url, self.options);

      // ******************************************
      // Socket.io client events (built-in)
      // ******************************************
      if (self.socket) {
        self.socket.on('connecting', function(transport_type) { // connection attempted by type
          var msg = { status: 'connecting via ', transport: transport_type };
          dfd.notify(msg);
          system.log("socket.io.client:: connecting ", transport_type);
          self.trigger('connecting', msg);
        });
        self.socket.on('connect', function () { //connection established and handshake successful
          system.log("socket.io.client:: connected!");
          self.trigger('connect');
          self.socket.emit('subscribe', { room: 'watch' }); // join admin/watch 'room'
          dfd.resolve(self.socket);
        });
        self.socket.on('connect_failed', function() {
          self.trigger('connect_failed'); //this.events.fire('ConnectFailed');
          system.log("socket.io.client:: connect_failed");
          dfd.reject('connect_failed');
        });
        self.socket.on('reconnect', function(transportType, reconnectionAttempts) {
          self.trigger('reconnect', { transport: transportType, attempts: reconnectionAttempts }); //this.events.fire('Reconnect', transport_type, reconnectionAttempts);
          system.log("socket.io.client:: reconnect");
        });
        self.socket.on('reconnecting', function(reconnectionDelay, reconnectionAttempts) {
          if (reconnectionAttempts === maxReconnect) {
            self.trigger('reconnect_failed');
          } else {
            self.trigger('reconnecting', { delay: reconnectionDelay, attempts: reconnectionAttempts }); //this.events.fire('onReconnecting', reconnectionDelay, reconnectionAttempts);
          }
        });
        self.socket.on('close', function () {
          self.trigger('close');
        });
        self.socket.on('error', function (e) {
          self.trigger('error', e ? e : 'This device was not able to connect with the server.  The server may be offline, or this device may not be fully HTML 5 compatible.  Please try again, if the problem persists, please contact your support technician.'); // this.events.fire('Error', e ? e : 'This device was not able to connect with the server.  The server may be offline, or this device may not be fully HTML 5 compatible.  Please try again, if the problem persists, please contact your support technician.');
          system.log("socket.io.client:: error", e);
        });
        self.socket.on('log', function (data) {
          system.log('=> ioClient.log : ' + JSON.stringify(data));
          self.trigger('log', data);
        });
      } else {
        dfd.reject();
      }
    });
  };
  return Client;
});
