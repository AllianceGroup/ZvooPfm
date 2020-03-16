'use strict';

angular.module('app.personal').service('trendsService', ['http', function (http) {
    this.getTrends = function () {
        var url = '/trends/getTrends';
        return http.get(url);
    };

    this.Refresh = function (model) {
        var url = '/trends/Refresh';
        return http.post(url, model);
    };
}]);