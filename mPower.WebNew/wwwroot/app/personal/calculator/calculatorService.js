
'use strict';

angular.module('app.personal').service('calculatorService', ['http', function (http) {

    this.getDefaultModel = function () {
        var url = '/calculator/marketHistoryCalculator';
        return http.get(url);
    };
}]);