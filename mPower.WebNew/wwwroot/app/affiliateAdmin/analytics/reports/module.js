"use strict";

angular.module('app.affiliateAdmin')
.config(function($stateProvider){
    $stateProvider
        .state('app.affiliateAdmin.reports', {
            url: '/AffiliateAdmin/Analytics/Reports',
            views: {
                "content@app":{
                    templateUrl: 'app/affiliateAdmin/analytics/reports/reports.html',
                    controller: 'ReportsController',
                    controllerAs: 'reportsCtrl'
                }
            },
            data:{
                title: 'Reports'
            },
            resolve: {
                model: function(analyticsService){
                    return analyticsService.getReportsModel();
                }
            }
        })
});