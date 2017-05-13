var es = require('event-stream');
var gulp = require('gulp');
var concat = require('gulp-concat');
var connect = require('gulp-connect');
var templateCache = require('gulp-angular-templatecache');
var ngAnnotate = require('gulp-ng-annotate');
var uglify = require('gulp-uglify');
var stylus = require('gulp-stylus');
var minify = require('gulp-minify-css');
var rename = require('gulp-rename');
var autoprefixer = require('gulp-autoprefixer');
var watch = require('gulp-watch');
var gulpNgConfig = require('gulp-ng-config');


var scripts = require('./wwwroot/app.scripts.json');

var source = {
    js: {
        main: 'wwwroot/app/main.js',
        src: [
            // application config
            'wwwroot/app.config.js',

            // application bootstrap file
            'wwwroot/app/main.js',

            // main module
            'wwwroot/app/app.js',

            // module files

            'wwwroot/app/**/module.js',

            // other js files [controllers, services, etc.]
            'wwwroot/app/**/!(module)*.js'
        ],
        tpl: 'wwwroot/app/**/*.tpl.html'
    }
};

var destinations = {
    js: 'wwwroot/build',
    css: 'wwwroot/styles/css'
};

gulp.task('connect', function () {
    connect.server({
        root: 'wwwroot'
    });
});

gulp.task('stylus', function () {
    gulp.src('wwwroot/styles/stylus/custom.styl')
       .pipe(stylus())
       .pipe(autoprefixer())
       .pipe(minify())
       .pipe(rename('custom.min.css'))
       .pipe(gulp.dest(destinations.css));
});

gulp.task('build', function () {
    return es.merge(gulp.src(source.js.src), getTemplateStream(), getConfig())
        // .pipe(ngAnnotate())
        // .pipe(uglify())
        .pipe(concat('app.js'))
        .pipe(gulp.dest(destinations.js));
});

gulp.task('vendor', function () {
    var paths = [];
    scripts.prebuild.forEach(function (script) {
        paths.push('wwwroot/' + scripts.paths[script]);
    });
    gulp.src(paths)
        .pipe(concat('vendor.js'))
        //.on('error', swallowError)
        .pipe(gulp.dest(destinations.js));
});

gulp.task('watch', function () {
    gulp.watch(source.js.src, ['build']);
    gulp.watch(source.js.tpl, ['build']);
});

gulp.task('watcher', function () {
    watch(['wwwroot/styles/stylus/**/*.styl'], function (event, cb) {
        gulp.start('stylus');
    });
    watch(source.js.src, function (event, cb) {
        gulp.start('build');
    });
    watch(source.js.tpl, function (event, cb) {
        gulp.start('build');
    });
});

gulp.task('default', ['vendor', 'build']);

gulp.task('fe', ['vendor', 'build','connect', 'stylus', 'watcher']);

var swallowError = function (error) {
    console.log(error.toString());
    this.emit('end');
};

var getTemplateStream = function () {
    return gulp.src(source.js.tpl)
        .pipe(templateCache({
            root: 'wwwroot/app/',
            module: 'app'
        }));
};

var getConfig = function() {
    return gulp.src('./wwwroot/app.config.json')
        .pipe(gulpNgConfig('app.config', {
            environment: process.env.ASPNETCORE_ENVIRONMENT
        }));
};