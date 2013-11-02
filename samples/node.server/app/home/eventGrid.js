define(['durandal/system', 'global/datacontext', 'durandal/events'], function (system, datacontext, Events) {
  var ctor = function () {
    var self = this;
    this.events = [];
    this.datacontext = datacontext;
    this.logData = [{ dateTime: new Date(Date.now()), zone: 'init', eventCode: 'client', message: '' }];
    // Date.now(); // returns an integer, e.g., 1355549799408
    // new Date(Date.now()) the 'now' DateTime you expected (don't forget new)
    this.grid;
    this.updateGridData = function (event) {
        event.dateTime = new Date(event.dateTime);
        if (event.eventCode) {
            if (typeof event.eventCode === 'object') {
                event.eventCode = JSON.stringify(event.eventCode);
            }
        }
      if (event.message) {
          if (typeof event.message === 'object') { //event.data = event.data || '';
              event.message = JSON.stringify(event.message);
        } 
      }
      self.grid.dataSource.add(event);
    };
    this.clearGrid = function() {
      self.grid.dataSource.data([]);
    };
  };
  
  ctor.prototype.activate = function () {
    var self = this;
    var evt = this.datacontext.ioClient.on('log')
      .then(self.updateGridData);
    this.events.push(evt);
    system.log('=> home.eventGrid: activate');
  };


  ctor.prototype.compositionComplete = function () {
    var self = this;
    system.log('=> home.eventGrid: compositionComplete');
    // setup kendo grid
    $('#gridLogs').kendoGrid({
      dataSource: {
        data: [{ dateTime: new Date(Date.now()), zone: 'init', eventCode: 'client', message: '' }],
            schema: {
              model: { //data type of the field {Number|String|Boolean|Date} default is String
                fields: {
                  dateTime: { type: "date" },
                  zone: { type: "string" },
                  eventCode: { type: "string" },
                  message: { type: "string" }
                }
              }
            },
        pageSize: 20,
        sort: { field: 'dateTime', dir:'desc'}
      },
      pageable: {
        buttonCount: 5
      },
      sortable: {
        mode: "single",
        allowUnsort: false
      },
      filterable: true,
      columns: [
        { field: "dateTime", title: "Time", format: "{0: HH:mm:ss fff}", width: 130, filterable: false },
        {
          field: "zone", title: "Zone", width: 110,
          filterable: {
            extra: false,
            operators: {
              string: {
                startswith: "Starts with",
                eq: "Is equal to",
                neq: "Is not equal to"
              }
            }
          }
        },
        {
          field: "eventCode", title: "Event", width: 130,
          filterable: {
            extra: false,
            operators: {
              string: {
                startswith: "Starts with",
                eq: "Is equal to",
                neq: "Is not equal to"
              }
            }
          }
        },
        { field: "message", title: "Message" }
      ]
    });
    self.grid = $("#gridLogs").data("kendoGrid");  // keep ref to widget;
  };
  ctor.prototype.detached = function () {
    // unsubscribe from any events we've listened for...
    this.events.forEach(function(evt) {
      evt.off();
    });
    system.log('=> home.index: detached ');
  };

  return ctor;

});