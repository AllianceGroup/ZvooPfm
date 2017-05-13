"use strict";

angular.module('app.business')
    .config(function ($stateProvider){
        $stateProvider
            .state('app.business.chartofaccounts', {
                url: '/accounts',
                data: {
                    title: 'Chart Of Accounts'
                },
                views: {
                    "content@app": {
                        controller: 'ChartOfAccountsCtrl',
                        controllerAs:'accountsCtrl',
                        templateUrl: "app/business/chartOfAccounts/views/ChartOfAccounts.html"
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
            .state('app.business.accounts-add', {
                url: '/accounts/add',
                data: {
                    title: 'Add accounts'
                },
                views: {
                    "content@app": {
                        templateUrl: "app/business/chartOfAccounts/views/add-template.tpl.html"
                    }
                },
                resolve: {
                }
            })
            .state('app.business.accounts-add.manually', {
                url: '/manually',
                controller: 'EditAccountController',
                controllerAs: 'editAccountCtrl',
                templateUrl: "app/business/chartOfAccounts/views/add-manually.tpl.html",
                resolve: {
                    model:function(accountsService) {
                        return accountsService.getEditModel();
                    }
                }
            })
            .state('app.business.accounts-add.intuit-search', {
                url: '/intuit/search',
                templateUrl: "app/business/chartOfAccounts/views/intuit-search.tpl.html",
                controller: 'SearchBusinessController',
                controllerAs: 'intuitCtrl'
            })
            .state('app.business.accounts-add.intuit-authenticate', {
                url: '/intuit/authenticate',
                templateUrl: "app/business/chartOfAccounts/views/intuit-authenticate.tpl.html",
                controller: 'AuthenticateBusinessController',
                controllerAs: 'intuitCtrl',
                params: {
                    model: null
                }
            })
            .state('app.business.accounts-add.intuit-availableccounts', {
                url: '/intuit/available',
                templateUrl: "app/business/chartOfAccounts/views/intuit-availableAccounts.tpl.html",
                controller: 'AvailableAccountsController',
                controllerAs: 'intuitCtrl',
                params: {
                    model: null
                }
            })
            .state('app.business.accounts-add.choice', {
                url: '/choice',
                templateUrl: "app/business/chartOfAccounts/views/choice.tpl.html",
                resolve: {
                }
            });
    });