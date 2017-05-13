'use strict';

angular.module('app.affiliateAdmin').service('analyticsService', ['http', function(http){

    this.getDashboardModel = function(statistic){
        var url;
        if(statistic)
            url = "/Analytics" + http.buildQueryString({TotalDebtType: statistic.TotalDebtType, AvailableCashType: statistic.AvailableCashType});
        else
            url = "/Analytics";
        return http.get(url);
    };

    this.getReportsModel = function(){
        var url = '/analytics/getReports';
        return http.get(url);
    };
}]);