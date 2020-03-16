
"use strict";

angular.module('app.personal').config(function ($stateProvider) {
    $stateProvider
        .state('app.personal.basicCalculator', {
            url: '/basicCalculator',
            views: {
                "content@app": {
                    controller: 'calculatorController',
                    controllerAs: 'calculatorCtrl',
                    templateUrl: 'app/personal/calculator/basicCalculator.tpl.html'
                },
            },
            data: {
                title: 'Basic Calculator'
            },
            resolve: {
                transactionsPage: function () {
                    return "app.personal.basicCalculator";
                },
                scripts: function (lazyScript) {
                    return lazyScript.register(['morris']);
                }
            }
        })
    .state('app.personal.marketHistoryCalculator', {
        url: '/marketHistoryCalculator',
        views: {
            "content@app": {
                controller: 'marketHistoryController',
                controllerAs: 'marketHistoryCtrl',
                templateUrl: 'app/personal/calculator/marketHistoryCalculator.tpl.html'
            },
        },
        data: {
            title: 'Market History Calculator'
        },
        resolve: {
            transactionsPage: function () {
                return "app.personal.marketHistoryCalculator";
            },
            scripts: function (lazyScript) {
                return lazyScript.register(['morris']);
            }
        }
    })
});

