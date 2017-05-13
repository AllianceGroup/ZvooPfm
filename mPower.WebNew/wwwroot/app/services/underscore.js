"use strict";

angular.module('app').service('_', ['$window', function ($window) {
    return $window._;
}]);