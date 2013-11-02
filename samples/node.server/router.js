var config = require('./config.json'),
	redis = require('redis'),
	api = require('./routes/api'),
	jobs = require('./routes/jobs'),
	web = require('./routes/web'),
	client = redis.createClient(config.redis.port, config.redis.ip);

client.on("error", function(err) {
	console.log("Redis: Error " + err);
});

if (config.redis.auth) {
	client.auth(config.redis.auth);
}

client.on('ready', function(e) {
	console.log("Redis: Ready!");
});


module.exports = function(app) {

  return {
    web: web(app, client),
    api: api(app, client)
    //,jobs: jobs(app, client);
  };
};