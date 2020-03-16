
'use strict';

angular.module('app.personal').service('quickSavingsService', ['http', function (http) {

    this.postapiurl = function (model) {
        var url = '/debtelimination/QuickSavingsAnalysis';
        return http.post(url, model);
    };

    this.quickSavingResults = function (model) {
        var url = 'quickSavingsResult';
        return http.post(url, model);
    };
}]);