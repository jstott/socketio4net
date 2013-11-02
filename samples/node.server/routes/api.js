var config = require('../config.json'),
    store = require('../store'),
    routerMap = {
      // always use lower case!
      'status': statusCounts // http://127.0.0.1:8000/api/status
    };

exports.get = function (req, res, next) {
  var target = req.params.target,
      route = routerMap[target];
  
  if (route) {
    res.send(route(req, res));
  } else {
    res.send(404);
  }
};

exports.statusCounts = statusCounts;
function statusCounts(req, res, next) {
  var status = {
    webStatus: {
      visits: get('app.visits', 0),
      errors: get('app.errors', 0)
    },
    ioStatus: {
      connections: {
        websocket: get('io.connection.websocket', 0),
        other: get('io.connection.other', 0),
      }, 
      errors: get('io.errors', 0)
    }
  };

  return status;
}
function get(key, def) {
  return store.get(key) || def;
}




