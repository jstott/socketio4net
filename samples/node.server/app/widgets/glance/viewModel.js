define(['durandal/composition'], function (composition) {
  var ctor = function() {
  };

  ctor.prototype.activate = function (settings) {
    this.settings = settings;
  };

  

  ctor.prototype.afterRenderItem = function (elements, item) {
  };

  return ctor;
});