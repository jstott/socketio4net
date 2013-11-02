define(['durandal/system', 'global/datacontext', 'durandal/events'], function (system, datacontext, Events) {
    var ctor = function () {
        var self = this;
        this.datacontext = datacontext;

        this.message = {
            room: ko.observable(),
            event: ko.observable(),
            data: ko.observable(),
            send: function (event) {
                if (self.message.event()) { // only send when we have event text value
                    self.datacontext.ioClient.socket.emit('clientBroadcast', {
                        room: self.message.room(),
                        event: self.message.event(),
                        data: self.message.data()
                    });
                }
            },
            sendPing: function (event) {
                    self.datacontext.ioClient.socket.emit('clientBroadcast', {
                        room: '',
                        event: 'ping',
                        data: Date.now()
                    });
            }
        };

    };

    ctor.prototype.activate = function () {
        system.log('=> home.index: activate');
    };
    ctor.prototype.binding = function () {
        system.log('Lifecycle : binding : home');
        return { cacheViews: false }; //cancels view caching for this module, allowing the triggering of the detached callback
    };
    ctor.prototype.attached = function () {
        system.log('=> home.index: attached ');
    };
    ctor.prototype.compositionComplete = function () {
        system.log('Lifecycle : compositionComplete : home');
        $('#collapseEdit').on('click', function (e) {
            $('#collapseEdit').toggleClass('icon-double-angle-up, icon-double-angle-down', 200);
        });

    };
    ctor.prototype.detached = function () {
        system.log('=> home.index: detached ');
    };

    return ctor;

});