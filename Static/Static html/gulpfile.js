//load dependecies
var gulp             = require('gulp'),
	autoprefixer     = require('autoprefixer'),
	iconfont         = require('gulp-iconfont'),
	iconfontCss      = require('gulp-iconfont-css'),
	postcss          = require('gulp-postcss'),
	sass             = require('gulp-sass'),
	sassLint         = require('gulp-sass-lint'),
	size             = require('gulp-size'),
	sourcemaps       = require('gulp-sourcemaps'),
	svgmin           = require('gulp-svgmin'),
	gutil            = require('gulp-util'),
	path             = require('path'),
	flexBugsFix      = require('postcss-flexbugs-fixes');

//icon fonts
gulp.task('iconfont', function(){
  return gulp.src(['assets/svg/*.svg'])
	.pipe(iconfontCss({
		fontName: 'svgicons',
		cssClass: 'font',
		path: 'conf/icon-font.scss',
		targetPath: '../../scss/layout/_icon-font.scss',
		fontPath: '../fonts/'
	}))
	.pipe(iconfont({
		fontName: 'svgicons', // required
		prependUnicode: false, // recommended option
		formats: ['ttf', 'woff'], // default, 'woff2' and 'svg' are available
		normalize: true,
		centerHorizontally: true
	}))
	.on('glyphs', function(glyphs, options) {
		// CSS templating, e.g.
		console.log(glyphs, options);
	})
	.pipe(gulp.dest('assets/fonts/'));
});


// SVG optimization
gulp.task('svgomg', function () {
	return gulp.src('assets/svg/*.svg')
		.pipe(svgmin({
			plugins: [
				{ removeTitle: true },
				{ removeRasterimg: true },
				{ sortAttrs: true }
			]
		}))
		.pipe(gulp.dest('assets/svg'));
});

//styles
gulp.task('styles', function() {
	var processors = [
		autoprefixer({ browsers: ['last 2 versions', 'ios >= 8'] }),
		flexBugsFix
	];
	return gulp.src(['scss/**/*.scss'])
		.pipe(sourcemaps.init())
		.pipe(sass({outputStyle: 'compressed'}))
		.pipe(postcss(processors))
		.pipe(sourcemaps.write('.'))
		.pipe(gulp.dest('assets/css'));
});

gulp.task('sasslint', function () {
  return gulp.src('scss/**/*.scss')
    .pipe(sassLint({
      config: '.sass-lint.yml'
    }))
    .pipe(sassLint.format())
    .pipe(sassLint.failOnError())
});

//watch
gulp.task('default', ['styles', 'sasslint'], function () {
	//watch .scss files
	gulp.watch('scss/**/*.scss', ['styles', 'sasslint']);
	//watch added or changed svg files to optimize them
	gulp.watch('assets/svg/*.svg', ['svgomg']);
});
