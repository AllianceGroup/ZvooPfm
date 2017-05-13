"use strict";

angular.module('app.personal')
    .config(function ($stateProvider) {
        $stateProvider
            .state('app.personal.chartofaccounts', {
                url: '/accounts',
                data: {
                    title: 'Chart Of Accounts'
                },
                views: {
                    "content@app": {
                        controller: 'ChartOfAccountsCtrl',
                        controllerAs:'accountsCtrl',
                        templateUrl: "app/personal/accounts/views/ChartOfAccounts.html"
                    }
                },
                resolve: {
                    accountsList: function(accountsService){
                        return accountsService.getAccounts();
                    },
                    srcipts: function (lazyScript) {
                        return lazyScript.register([
                            'jqgrid',
                            'jqgrid-locale-en'
                        ]);

                    }
                }
            })
    });