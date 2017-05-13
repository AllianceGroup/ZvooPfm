'use strict';

angular.module('app.personal')
.config(function ($stateProvider) {
    $stateProvider
        .state('app.personal.dashboard', {
            url: '/dashboard',
            views: {
                "content@app": {
                    controller: 'DashboardController',
                    controllerAs: 'dashboardCtrl',
                    templateUrl: 'app/personal/dashboard/dashboard.html'
                },
                "leftpanel@app.personal.dashboard": {
                    templateUrl: 'app/personal/layout/partials/accounts.html',
                    controller: 'AccountsController',
                    controllerAs:'accountsCtrl'
                }
            },
            data:{
                title: 'Dashboard'
            },
            resolve: {
                transactionsPage: function(){
                    return "app.personal.transactions";
                },
                budgetsList: function(dashboardService){
                    return dashboardService.getTopTenBudgets();
                },
                alertsList: function(dashboardService){
                    return dashboardService.getAllAlerts();
                }
            }
        })
        .state('app.personal.dashboard.charts',{
            url: '/dashboard/charts',
            views: {
                "content@app": {
                    controller: 'ChartsController',
                    controllerAs: 'chartsCtrl',
                    templateUrl: 'app/personal/dashboard/views/charts.html'
                },
                "leftpanel@app.personal.dashboard.charts": {
                    templateUrl: 'app/personal/layout/partials/accounts.html',
                    controller: 'AccountsController',
                    controllerAs:'accountsCtrl'
                }
            },
            data:{
                title: 'Charts'
            },
            resolve: {
                chartsList: function(dashboardService){
                    return dashboardService.getCharts();
                },
                scripts: function(lazyScript){
                    return lazyScript.register([
                        'morris'
                    ]);
                }
            }
        });
});
