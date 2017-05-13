'use strict';

angular.module('app.business')
.config(function ($stateProvider){
    $stateProvider
        .state('app.business.dashboard', {
            url: '/dashboard',
            views: {
                "content@app": {
                    templateUrl: 'app/business/dashboard/dashboard.html',
                    controller: 'DashboardBusinessController',
                    controllerAs: 'dashboardBusinessCtrl'
                },
                "leftpanel@app.business.dashboard": {
                    templateUrl: 'app/personal/layout/partials/accounts.html',
                    controller: 'AccountsController',
                    controllerAs:'accountsCtrl'
                }
            },
            data: {
                title: 'Dashboard'
            },
            resolve: {
                transactionsPage: function(){
                    return "app.business.transactions";
                },
                model: function(businessDashboardService){
                    return businessDashboardService.getDashboardModel();
                },
                scripts: function(lazyScript) {
                    return lazyScript.register([
                        'morris'
                    ]);
                }
            }
        })
});