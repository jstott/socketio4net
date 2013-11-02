define(['durandal/system'], function (system) {
  var remoteSettings = {
    baseRemoteApiUrl: "http://localhost:8000/api/"
  },
      analytics = {
        app: {
          visits: 'app.vists',
          apiCount: 'app.apiCount',
          errors: 'app.errors'
        },
        io: {
          connections: {
            websockets: 'io.connect.websockets',
            others: 'io.connect.others'
          },
          errors: 'io.errors'
        }
      },
      watch = {
        connection: 'connection',
        disconnect: 'disconnect'
      },
      errorEvents = {
        ajax: 'error:ajax'
      };

  return {
    remoteSettings: remoteSettings,
    analytics: analytics,
    watch: watch,
    errorEvents: errorEvents
  };
});