"use strict";

angular.module('app.personal')

    .config(function ($stateProvider) {
        $stateProvider
            .state('app.personal.budget', {
                url: '/budget',
                views: {
                    "content@app": {
                        controller: 'BudgetController',
                        controllerAs: 'BudgetCtrl',
                        templateUrl: 'app/personal/budget/budget.tpl.html'
                    }
                },
                data:{
                    title: 'Budget'
                },
                resolve: {
                    budgetsList: function(budgetService){
                        return budgetService.getAllBudgets();
                    },
                    transactionsPage: function(){
                        return "app.personal.transactions";
                    }
                }
            });
    });