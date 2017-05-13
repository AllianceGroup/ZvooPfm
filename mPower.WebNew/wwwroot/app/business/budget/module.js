"use strict";

angular.module('app.business')
    .config(function ($stateProvider){
        $stateProvider
            .state('app.business.budget', {
                url: '/budget',
                views: {
                    "content@app": {
                        controller: 'BudgetController',
                        controllerAs: 'BudgetCtrl',
                        templateUrl: 'app/personal/budget/budget.tpl.html'
                    }
                },
                data: {
                    title: 'Budget'
                },
                resolve: {
                    budgetsList: function (budgetService) {
                        return budgetService.getAllBudgets();
                    },
                    transactionsPage: function(){
                        return "app.business.transactions";
                    }
                }
            });
    });