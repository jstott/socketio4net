exports.config = function(weyland) {
    weyland.build('main')
        .task.jshint({
            include:'app/**/*.js'
        })
        .task.uglifyjs({
            include:['app/**/*.js', 'scripts/durandal/**/*.js']
        })
        .task.rjs({
            include:['app/**/*.{js,html}', 'scripts/durandal/**/*.js'],
            loaderPluginExtensionMaps:{
                '.html':'text'
            },
            rjs:{
                name:'../scripts/almond-custom', //to deploy with require.js, use the build's name here instead
                insertRequire:['main'], //not needed for require
                baseUrl : 'app',
                wrap:true, //not needed for require
                paths : {
                    'text': '../scripts/text',
                    'durandal': '../scripts/durandal',
                    'plugins': '../scripts/durandal/plugins',
                    'transitions': '../scripts/durandal/transitions',
                    'knockout': 'empty:',
                    'bootstrap': 'empty:',
                    'jquery': 'empty:'
                },
                inlineText: true,
                optimize : 'none',
                pragmas: {
                    build: true
                },
                stubModules : ['text'],
                keepBuildDir: true,
                out:'app/main-built.js'
            }
        });
}