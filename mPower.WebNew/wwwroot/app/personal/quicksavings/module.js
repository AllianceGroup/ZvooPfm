
"use strict";

angular.module('app.personal').config(function ($stateProvider) {
    $stateProvider
        .state('app.personal.quicksavings', {
            url: '/quicksavings',
            views: {
                "content@app": {
                    controller: 'quickSavingsController',
                    controllerAs: 'savingsCtrl',
                    templateUrl: 'app/personal/quicksavings/quickSavings.tpl.html'
                },
            },
            data: {
                title: 'QuickSavings'
            },
            resolve: {
                transactionsPage: function () {
                    return "app.personal.quicksavings";
                },
                scripts: function (lazyScript) {
                    return lazyScript.register(['morris']);
                }
            }
        })

 .state('app.personal.quickSavingsResult', {
     url: '/quickSavingsResult',
     views: {
         "calendar@app": {
             templateUrl: 'app/personal/quicksavings/views/quickSavingsResult.tpl.html',
             controller: 'QuickSavingsResultController',
             controllerAs: 'quickSavingsResultCtrl'
         }
     },
     data: {
         title: 'Quick Saving Result'
     },
     resolve: {
         stepModel: function (quickSavingsService) {
             return quickSavingsService.quickSavingResults();
         },
     }
 });
});

