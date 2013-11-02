define(['durandal/system', 'global/config', 'service/webApi', 'service/ioClient'], function (system, config, webApi, IOClient) {
  var name = ko.observable();
  var context = {
    init: function () {
      return webApi.loadConfiguration()
        .then(function (data) {
          extend(config.remoteSettings, data);
        })
       .then(webApi.getStatus)
       .then(function (data) {
         context.webStatus.update(data.webStatus);
         context.ioStatus.update(data.ioStatus);
       })
       .then(function() {
         context.ioClient = new IOClient();
         return context.ioClient.connect()
           .then(function (socket) {
             registerAnalyticEvents(socket);
             return true;
           }).fail(function(err) {
             system.log('error connecting with socket.io' + err);
           });
       });
    },
    ioClient: {},
    webStatus: {
      visits: ko.observable(0),
      webApi: ko.observable(0),
      errors: ko.observable(0),
      update: function (obj) {
        ko.mapping.fromJS(obj, { ignore: ['update', 'count'] }, context.webStatus);
      }
    },
    ioStatus: {
      connections: {
        websocket: ko.observable(0),
        other: ko.observable(0)
      },
      count: ko.computed({
        read: function() {
          return 0 + context.ioStatus.connections.websocket() + context.ioStatus.connections.other();
        },
        write: function(value) {
          
        },
        deferEvaluation: true
      }),
      errors: ko.observable(0),
      update: function (obj) {
        ko.mapping.fromJS(obj, { exclue: ['update', 'count'] }, context.ioStatus);
      }
    }
  };

  return context;
  function registerAnalyticEvents(socket) {
    socket.on('analytics', function (data) {
      if (data.ioStatus) {
        context.ioStatus.update(data.ioStatus);
      }
      if (data.webStatus) {
        context.webStatus.update(data.webStatus);
      }
    });

  }
  /**
  * Extends the first object with the properties of the following objects, handles ko.observable 'functions'.
  * @method extend
  * @param {object} obj The target object to extend.
  * @param {object} extension* Uses to extend the target object.
*/
  function extend(obj) {
    var slice = Array.prototype.slice,
        rest = slice.call(arguments, 1);

    for (var i = 0; i < rest.length; i++) {
      var source = rest[i];

      if (source) {
        for (var prop in source) {
          if (typeof obj[prop] === 'function') {
            obj[prop](source[prop]);
          } else {
            obj[prop] = source[prop];
          }
        }
      }
    }

    return obj;
  }
  function map(data, vm) {
    ko.mapping.fromJS(data, vm);
  }
});
