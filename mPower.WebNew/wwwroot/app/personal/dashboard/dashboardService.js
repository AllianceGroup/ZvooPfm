'use strict';

angular.module('app.personal').service('dashboardService', ['http', function(http){
    this.getTopTenBudgets = function(){
        var url = '/budget/GetTopTenBudgets';
        return http.get(url);
    };

    this.getAllAlerts = function(){
        var url = '/alerts/latest';
        return http.get(url);
    };

    this.getCharts = function(){
        var url = '/dashboard/getCharts';
        return http.get(url);
    };

    this.deleteAlert = function(alertId){
        var url = '/alerts/delete/';
        return http.delete(url + alertId);
    };
}]);