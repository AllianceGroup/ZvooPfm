"use strict";

angular.module('app.affiliateAdmin')
.config(function($stateProvider){
    $stateProvider
        .state('app.affiliateAdmin.dashboard',{
            url: '/AffiliateAdmin/Analytics/Dashboard',
            views: {
                "content@app":{
                    templateUrl: 'app/affiliateAdmin/analytics/dashboard/dashboard.html',
                    controller: 'AnalyticsDashboardController',
                    controllerAs: 'analyticsDashboardCtrl'
                }
            },
            data: {
                title: 'Dashboard'
            },
            resolve: {
                model: function(analyticsService){
                    return analyticsService.getDashboardModel();
                },
                scripts: function(lazyScript){
                    return lazyScript.register([
                        'morris'
                    ]);
                }
            }
        })
});