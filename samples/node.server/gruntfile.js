module.exports = function(grunt) {

  // Project configuration.
  grunt.initConfig({
    pkg: grunt.file.readJSON('package.json'),
    jshint: {
		options: {
			jshintrc: '.jshintrc'
		},
		all: [
		'store.js', 'router.js', 'routes/**/.js',
			'app/**/*.js'
			]
	}
  });

  // Load the plugin that provides the "uglify" task.
  grunt.loadNpmTasks('grunt-contrib-jshint');

  // Default task(s).
  grunt.registerTask('default', ['jshint']);

};