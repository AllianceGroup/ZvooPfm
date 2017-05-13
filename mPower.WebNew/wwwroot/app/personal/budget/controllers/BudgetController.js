'use strict';

angular.module('app.personal').controller('BudgetController', function BudgetController($uibModal ,budgetService, budgetsList, _, $state, transactionsPage, $rootScope) {

    var ctrl = this;
    ctrl.Budget = budgetsList;
    ctrl.isShowIcon = false;
    ctrl.Months = ["Jan", "Feb", "Mar", "Apr", "May", "Jun",
        "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"];

    _.findWhere(ctrl.Budget.GraphModel.Monthes, {Month: new Date(ctrl.Budget.GraphModel.Date).getMonth() + 1}).Active = "active";

    ctrl.Show = function (month, year){
        budgetService.Show(month, year).then(function(budget) {
            ctrl.Budget = budget;

            _.findWhere(ctrl.Budget.GraphModel.Monthes, {Month: new Date(ctrl.Budget.GraphModel.Date).getMonth() + 1}).Active = "active";
        });
    };

    ctrl.Update = function (currentBudget){
        var date = new Date(ctrl.Budget.GraphModel.Date);

        var model = {
            budgetId: currentBudget.Id,
            amount: currentBudget.newAmountBudgeted,
            month: date.getMonth() + 1,
            year: date.getFullYear()
        };

        budgetService.Update(model).then(function () {
            var difference = currentBudget.newAmountBudgeted - currentBudget.AmountBudgeted;
            currentBudget.AmountBudgeted = currentBudget.newAmountBudgeted;
            currentBudget.Edit = false;
            currentBudget.Persent = parseInt(currentBudget.AmountSpent / currentBudget.AmountBudgeted * 100);

            if(currentBudget.Persent > 100)
                currentBudget.Persent = 100;

            if (_.findWhere(ctrl.Budget.GraphModel.IncomeItems, { Id: currentBudget.Id }) != undefined) {
                ctrl.Budget.GraphModel.IncomeBudgetedTotal += difference;
                currentBudget.Color = 'red';
                if(currentBudget.Persent > 33 && currentBudget.Persent <= 66)
                    currentBudget.Color = 'yellow';
                if(currentBudget.Persent > 66)
                    currentBudget.Color = 'green';
            }
            if (_.findWhere(ctrl.Budget.GraphModel.ExpenseItems, { Id: currentBudget.Id }) != undefined) {
                ctrl.Budget.GraphModel.ExpenseBudgetedTotal += difference;
                currentBudget.Color = 'green';
                if(currentBudget.Persent > 50 && currentBudget.Persent < 100)
                    currentBudget.Color = 'yellow';
                if(currentBudget.Persent >= 100)
                    currentBudget.Color = 'red';
            }
        });
    };

    ctrl.EditBudget = function (budget) {
        budget.Edit = budget.Edit ? false: true;
    };

    ctrl.ViewTransactions = function (accountId, name){
        var date = new Date(ctrl.Budget.GraphModel.Date);

        var firstDay = new Date(date.getFullYear(), date.getMonth(), 1);
        var lastDay = new Date(date.getFullYear(), date.getMonth() + 1, 0);

        var filter = {
            accountId: accountId,
            to: encodeURIComponent(date.getMonth()+ 1 + '/' + lastDay.getDate() + '/' + date.getFullYear()),
            from: encodeURIComponent(date.getMonth() + 1 + '/' + firstDay.getDate() + '/' + date.getFullYear()),
            accountName: name
        };

        if($rootScope.businessId)
            $state.go(transactionsPage, filter, {businessId: $rootScope.businessId});
        else
            $state.go(transactionsPage, filter)
    };

    ctrl.EditCategories = function(){
        var  modalInstance = $uibModal.open({
            templateUrl: 'app/personal/budget/views/AddBudget.html',
            controller: 'CategoriesController',
            controllerAs: 'categoriesCtrl',
            resolve: {
                model: function () {
                    return {
                        budgets: ctrl.Budget.WizardModel,
                        actionFunction: budgetService.Create
                    }
                }
            }
        });

        modalInstance.result.then(function () {
            budgetService.getAllBudgets().then(function(budgets){
                ctrl.Budget = budgets;
            })
        });
    };

    ctrl.switch = function(isShow){
        ctrl.isShowIcon = isShow;
    };

    ctrl.show = function(budget){
        budget.show = true;
    };

    ctrl.hide = function(budget){
        budget.show = false;
        budget.Edit = false;
    };

    ctrl.ColorBar = function (color){
        if(color == "green")
            return "progress-bar-success";
        if(color == "red")
            return "progress-bar-danger";
        if(color == "yellow")
            return "progress-bar-warning";
    };
});
