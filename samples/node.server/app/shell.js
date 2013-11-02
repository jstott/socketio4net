define(['plugins/router', 'global/datacontext'], function (router, datacontext) {
  return {
    router: router,
    activate: function () {
      return datacontext.init()
        .then(function () {
          router.map([
              { route: ['', 'home'], moduleId: 'home/index', title: 'Home', nav: true }

          ]).buildNavigationModel()
            .mapUnknownRoutes('home/index', 'not-found')
            .activate();
        });
    }
  };
});