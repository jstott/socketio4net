define(['durandal/system', 'durandal/app', 'global/config'], function (system, app, config) {
  var
    loadConfiguration = function () {
      return $.ajax({
        url: "/config"
      })
        .fail(function (err) {
          app.trigger(config.errorEvent.ajax, 'Error loading client configurations');
        })
        .success(function (data, status, xhr) {
          return data;
        });
    },
    getStatus = function () {
      return $.ajax({
        url: config.remoteSettings.baseRemoteApiUrl + 'status'
      })
          .fail(function (err) {
            app.trigger(config.errorEvent.ajax, 'Error loading client configurations');
          })
          .success(function (data, status, xhr) {
            return data;
          });
    };
  return {
    loadConfiguration: loadConfiguration,
    getStatus: getStatus
  };
});