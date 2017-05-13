'use strict';

angular.module('app.personal').controller('DashboardController', ['$rootScope', 'dashboardService', 'budgetsList', 'alertsList', '$uibModal', 'localStorageService',
    function ($rootScope, dashboardService, budgetsList, alertsList, $uibModal , localStorageService) {
        var ctrl = this;

        ctrl.Date = new Date();

        ctrl.Alerts = alertsList;
        ctrl.Budgets = budgetsList;

        ctrl.delete = function (alertId){
            dashboardService.deleteAlert(alertId);
        };

        if(!localStorageService.get('settings').hasAccounts){
            $uibModal.open({
                templateUrl: 'app/personal/dashboard/views/startPage.html',
                controller: 'StartController',
                controllerAs: 'intuitCtrl'
            });
        }

        if($rootScope.hasDashboardHints){
            var enjoyhint_instance = new EnjoyHint({});
            var enjoyhint_script_steps = [
                {
                    'next .hint-1': '<span style="font-size: 24px; font-weight: bold; matgin-bottom: 10px; display: block;">Accounts</span>' + '<p style="font-size: 16px;">This is where you will find updated account balances for each of the accounts you\'ve added.</p>',
                    onBeforeStart: function () {
                        ctrl.disableBtn = true;
                    }
                },
                {
                    'next .hint-2': '<span style="font-size: 24px; font-weight: bold; matgin-bottom: 10px; display: block;">Financial Snapshot</span>' +
                    '<p style="font-size: 16px;">Your financial snapshot will give you a brief overview of your finances for the month, <br /> including how much money has come in, and how much has gone out.</p> <br />',
                    onBeforeStart: function () {
                        ctrl.disableBtn = true;
                    }
                },
                {
                    'next .hint-3': '<span style="font-size: 24px; font-weight: bold; matgin-bottom: 10px; display: block;">Budget Overview</span>' +
                    '<p style="font-size: 16px;">This is a snapshot of your budget, it shows you how you\'re doing on your ten most active budget categories. <br /> You can see all of your budget categories by clicking "see all accounts" at the bottom of this section.</p>',
                    onBeforeStart: function () {
                        ctrl.disableBtn = true;
                    }
                },
                {
                    'skip .hint-4': '<span style="font-size: 24px; font-weight: bold; matgin-bottom: 10px; display: block;">Alerts</span>' +
                    '<p style="font-size: 16px;">Here you\'ll find the alerts that you\'ve configured in your profile section.</p>',
                    'shape': 'circle',
                    'radius': 120,
                    'skipButton': {className: 'hint-finish', text: 'Finish'},
                    onBeforeStart: function () {
                        ctrl.disableBtn = false;
                    }
                }
            ];

            enjoyhint_instance.set(enjoyhint_script_steps);
            enjoyhint_instance.run();
            $rootScope.hasDashboardHints = false;
        }
}]);
