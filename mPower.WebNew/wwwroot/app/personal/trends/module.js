"use strict";

angular.module('app.personal')

    .config(function ($stateProvider) {
        $stateProvider
            .state('app.personal.trends', {
                url: '/trends',
                views: {
                    "content@app": {
                        controller: 'TrendsController',
                        controllerAs: 'trendsCtrl',
                        templateUrl: 'app/personal/trends/trends.tpl.html'
                    }
                },
                data:{
                    title: 'Trends'
                },
                resolve: {
                    transactionsPage: function(){
                        return "app.personal.transactions";
                    },
                    accountsPage: function(){
                        return "app.personal.chartofaccounts"
                    },
                    trendsList: function(trendsService){
                        return trendsService.getTrends();
                    },
                    filter: function(){
                        return {
                            Filter: 'ThisMonth',
                            ShowFormat: 'Spending',
                            Month: 0,
                            Year: 0,
                            TakeCategories: 10,
                            AccountId: null,
                            LedgerId: null,
                            All: false,
                            From: null,
                            To: null,
                            FilterByAccountUrl: null
                        };
                    },
                    scripts: function(lazyScript){
                        return lazyScript.register([
                            'morris'
                        ]);
                    }
                }

            });
    });