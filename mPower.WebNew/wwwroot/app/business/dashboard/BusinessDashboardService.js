'use strict';

angular.module('app.business').service('businessDashboardService', ['http', function(http){
    this.getDashboardModel = function(){
        var url = '/BusinessDashboard';
        return http.get(url);
    };

    this.deleteAlert = function(id){
        var url = '/BusinessDashboard/deleteAlert/' + id;
        return http.get(url);
    };
}]);