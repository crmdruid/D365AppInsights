/// <binding BeforeBuild='build:all' />
/*
This file is the main entry point for defining Gulp tasks and using Gulp plugins.
Click here to learn more. https://go.microsoft.com/fwlink/?LinkId=518007
*/

var gulp = require("gulp"),
    concat = require("gulp-concat"),
    sourcemaps = require("gulp-sourcemaps"),
    uglify = require("gulp-uglify"),
    rename = require("gulp-rename"),
    ts = require("gulp-typescript");

var paths = {
    root: ""
};

paths.jsDest = paths.root + "./content/D365AppInsightsJs";
paths.concatJsDest = paths.jsDest + "/lat_AiLogger.js";
paths.concatJsMinDest = paths.jsDest + "/lat_AiLogger.min.js";

gulp.task("build:ts", function () {
    var tsProject = ts.createProject("../AppInsightsJsLogger/tsconfig.json");

    var tsResult = tsProject.src().pipe(tsProject());

    return tsResult.js.pipe(gulp.dest("../AppInsightsJsLogger/js"));
});

gulp.task("move:js", ["build:ts"], function () {
    gulp.src([paths.root + "../AppInsightsJsLogger/scripts/AiLogger.js", paths.root + "../AppInsightsJsLogger/js/AiFormLogger.js"])
        .pipe(concat(paths.concatJsDest))
        .pipe(gulp.dest(""));
});

gulp.task("min:js", ["move:js"], function () {
    gulp.src([paths.root + "../AppInsightsJsLogger/scripts/AiLogger.js", paths.root + "../AppInsightsJsLogger/js/AiFormLogger.js"])
        .pipe(sourcemaps.init())
        .pipe(concat(paths.concatJsMinDest))
        .pipe(gulp.dest(""))
        .pipe(uglify())
        .pipe(sourcemaps.write(""))
        .pipe(gulp.dest(""));
});

gulp.task("build:all", ["build:ts", "move:js", "min:js"]);