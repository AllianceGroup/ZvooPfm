'use strict';

angular.module('app.personal').controller('CategoriesController', ['model', 'budgetService', '$uibModalInstance', '_',
    function (model, budgetService, $uibModalInstance, _) {

        var ctrl = this;

        ctrl.Budget = model.budgets;
        ctrl.actionFunction = model.actionFunction;

        ctrl.errors = {};

        ctrl.dismiss = function() {
            $uibModalInstance.dismiss('cancel');
        };

        ctrl.Finish = function(){
            var model = {};
            model.Income = [];
            model.Expense = [];

            for(var i = 0; i < ctrl.Budget.IncomeItems.length; i++)
                model.Income[i] = {Id: ctrl.Budget.IncomeItems[i].AccountId,
                    Budget: ctrl.Budget.IncomeItems[i].BudgetAmountInDollars,
                    IncludeBudget: ctrl.Budget.IncomeItems[i].IsIncludedInBudget};

            for(i = 0; i < ctrl.Budget.ExpenseItems.length; i++)
                model.Expense[i] = {Id: ctrl.Budget.ExpenseItems[i].AccountId,
                    Budget: ctrl.Budget.ExpenseItems[i].BudgetAmountInDollars,
                    IncludeBudget: ctrl.Budget.ExpenseItems[i].IsIncludedInBudget};

            model.Month = ctrl.Budget.Month;
            model.Year = ctrl.Budget.Year;

            ctrl.actionFunction(model).then(function(){
                $uibModalInstance.close();
            });
        };

        ctrl.AddBudget = function(){
            if(ctrl.Added){
                ctrl.Added = false;
                return;
            }
            ctrl.NewBudget = {};
            ctrl.Added = true;
        };

        ctrl.ChangeBudget = function(type){
            if(type == 'income'){
                ctrl.Budget.TotalIncome = 0;
                for(var i = 0; i < ctrl.Budget.IncomeItems.length; i++){
                    if(ctrl.Budget.IncomeItems[i].IsIncludedInBudget)
                        ctrl.Budget.TotalIncome += ctrl.Budget.IncomeItems[i].BudgetAmountInDollars;
                }
            }
            if(type == 'expense'){
                ctrl.Budget.TotalExpense = 0;
                for(i = 0; i < ctrl.Budget.ExpenseItems.length; i++){
                    if(ctrl.Budget.ExpenseItems[i].IsIncludedInBudget){
                        ctrl.Budget.TotalExpense += ctrl.Budget.ExpenseItems[i].BudgetAmountInDollars;
                    }
                }
            }
        };

        ctrl.SaveBudget = function(category, type){
            ctrl.NewBudget.Type = type;

            if(type === "Income")
                ctrl.Budget.TotalIncome += ctrl.NewBudget.BudgetAmountInDollars;
            if(type === "Expense")
                ctrl.Budget.TotalExpense += ctrl.NewBudget.BudgetAmountInDollars;

            budgetService.Add(ctrl.NewBudget).then(function(model){
                category[category.length] = model;
            }, function(errors) {
                ctrl.errors = _.reduce(errors, function(result, arr) {
                    return result.concat(arr);
                }, []);
            });

            ctrl.Added = false;
        };

        ctrl.HideBudget = function(budget, type, hide){
            if(type == 'income'){
                if(!hide)
                    ctrl.Budget.TotalIncome -= budget.BudgetAmountInDollars;
                if(hide)
                    ctrl.Budget.TotalIncome += budget.BudgetAmountInDollars
            }
            if(type == 'expense'){
                if(!hide)
                    ctrl.Budget.TotalExpense -= budget.BudgetAmountInDollars;
                if(hide)
                    ctrl.Budget.TotalExpense += budget.BudgetAmountInDollars
            }

            budget.IsIncludedInBudget = hide;
        };

        ctrl.Delete = function(id){
            budgetService.Delete(id).then(function(){
                var incomeAccount = _.findWhere(ctrl.Budget.IncomeItems, {AccountId: id});
                var expenseAccount = _.findWhere(ctrl.Budget.ExpenseItems, {AccountId: id});

                if(incomeAccount !== undefined){
                    if(incomeAccount.IsIncludedInBudget == true)
                        ctrl.Budget.TotalIncome -= incomeAccount.BudgetAmountInDollars;

                    ctrl.Budget.IncomeItems = _.filter(ctrl.Budget.IncomeItems,
                        function(item){return (item.AccountId != id);});
                    return;
                }

                if(expenseAccount !== undefined) {
                    if(expenseAccount.IsIncludedInBudget == true)
                        ctrl.Budget.TotalExpense -= expenseAccount.BudgetAmountInDollars;

                    ctrl.Budget.ExpenseItems = _.filter(ctrl.Budget.ExpenseItems,
                        function (item){return (item.AccountId != id);});
                }
            }, function(errors) {
                ctrl.errors = _.reduce(errors, function(result, arr) {
                    return result.concat(arr);
                }, []);
            });
        };
}]);