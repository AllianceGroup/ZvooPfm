"use strict";

angular.module('app.personal')

.config(function ($stateProvider) {
    $stateProvider
        .state('app.personal.transactions', {
            url: '/transactions?accountName&mode&itemsPerPage&page&request&accountId&from&to&s&type&_',
            views: {
                "content@app": {
                    controller: 'TransactionsController',
                    controllerAs: 'transactionsCtrl',
                    templateUrl: 'app/personal/transactions/transactions.tpl.html'
                },
                "leftpanel@app.personal.transactions": {
                    templateUrl: 'app/personal/layout/partials/accounts.html',
                    controller: 'AccountsController',
                    controllerAs: 'accountsCtrl'
                }
            },
            data:{
                title: 'Transactions'
            },
            resolve: {
                accountName: function($stateParams){
                    return $stateParams.accountName ? $stateParams.accountName : 'All accounts';
                },
                transactionsPage: function(){
                    return "app.personal.transactions";
                },
                transactionsList: function (transactionsService, $stateParams, _) {
                    var filter = {
                        mode: _.isUndefined($stateParams.mode) ? '' : $stateParams.mode,
                        itemsPerPage: _.isUndefined($stateParams.itemsPerPage) ? 10 : $stateParams.itemsPerPage,
                        page: $stateParams.page,
                        request: $stateParams.request,
                        accountId: $stateParams.accountId,
                        from: _.isUndefined($stateParams.from) ? '' : decodeURIComponent($stateParams.from),
                        to: _.isUndefined($stateParams.to) ? '' : decodeURIComponent($stateParams.to),
                        s: $stateParams.s,
                        type: $stateParams.type,
                        _: $stateParams._
                    };
                    return transactionsService.getByFilter(filter);
                },
                filter : function($stateParams, _) {
                    return {
                        mode: _.isUndefined($stateParams.mode) ? '' : $stateParams.mode,
                        itemsPerPage: _.isUndefined($stateParams.itemsPerPage) ? 10 : $stateParams.itemsPerPage,
                        page: $stateParams.page,
                        request: $stateParams.request,
                        accountId: $stateParams.accountId,
                        from: _.isUndefined($stateParams.from) ? '' : decodeURIComponent($stateParams.from),
                        to: _.isUndefined($stateParams.to) ? '' : decodeURIComponent($stateParams.to),
                        s: $stateParams.s,
                        type: $stateParams.type,
                        _: $stateParams._
                    };
                }
            }
        })
        .state('app.personal.transactions.add', {
            url: '/add',
            template: '<div ui-view></div>'
        })
        .state('app.personal.transactions.add.choose', {
            url: '/choose',
            templateUrl: 'app/personal/transactions/views/add-choose.tpl.html'
        });
});