'use strict';

angular.module('app.personal').controller('GoalsController',
['goalsService', 'model', 'availableAccounts', '$stateParams', function (goalsService, model, availableAccounts, $stateParams) {

    var ctrl = this;
    ctrl.showStartWizard = true;

    var initializeModel = function (model) {
        if (!model.ActiveItems) return model;
        for (var i = 0; i < model.ActiveItems.length; i++) {
            model.ActiveItems[i].OldCurrentAmountInDollars = model.ActiveItems[i].CurrentAmountInDollars + model.ActiveItems[i].StartingBalanceInDollars;
            model.ActiveItems[i].CurrentAmountInDollars += model.ActiveItems[i].StartingBalanceInDollars;
            model.ActiveItems[i].Disabled = false;
        }
        return model;
    };

    ctrl.model = initializeModel(model);
    ctrl.activeGoalsExist = ctrl.model.ActiveItems !== null && ctrl.model.ActiveItems.length > 0;
    ctrl.comleteGoalsExist = ctrl.model.CompletedItems !== null && ctrl.model.CompletedItems.length > 0;
    ctrl.archivedGoalsExist = ctrl.model.ArchivedItems !== null && ctrl.model.ArchivedItems.length > 0;

    ctrl.type = $stateParams.type ? $stateParams.type : 'Projected';
    ctrl.availableAccounts = availableAccounts;
    ctrl.isShowIcon = false;
    if(ctrl.availableAccounts !== undefined){
          changeLinkedAccount(ctrl.availableAccounts.LinkedAccountId, ctrl.availableAccounts.LinkedAccountId);  
    }

    ctrl.changeMonthlyAmount = function (goal) {
        var difference = goal.CurrentAmountInDollars - goal.OldCurrentAmountInDollars;
        if (difference > ctrl.model.AvailableAmountInDollars && goal.CurrentAmountInDollars >= goal.StartingBalanceInDollars) {
            difference = ctrl.model.AvailableAmountInDollars;
            goal.CurrentAmountInDollars = goal.OldCurrentAmountInDollars + difference;            
        }
        if (goal.StartingBalanceInDollars > goal.CurrentAmountInDollars) {
            goal.CurrentAmountInDollars = goal.StartingBalanceInDollars;
            difference = goal.StartingBalanceInDollars - goal.OldCurrentAmountInDollars;           
        }
        goal.MonthlyActualAmountInDollars = goal.MonthlyActualAmountInDollars + difference;
        ctrl.model.AvailableAmountInDollars -= difference;
        if (difference !== 0) {
            goalsService.adjustAmount(goal.Id, difference).then(function () {
                goal.OldCurrentAmountInDollars = goal.CurrentAmountInDollars;
                goal.Disabled = false;
            }, function (errors) {
                goal.CurrentAmountInDollars = goal.OldCurrentAmountInDollars;
                goal.Disabled = false;
            });
        }
    };

    function changeLinkedAccount(newId) {
        goalsService.setupLinkedAccount(newId).then(function() {
            ctrl.model.AvailableAmountInDollars = _.find(ctrl.availableAccounts.Accounts, function (val) { return val.Id === newId; }).BalanceInCents / 100;
            for (var i = 0; i < ctrl.model.ActiveItems.length; i++) {
                ctrl.model.AvailableAmountInDollars -= (ctrl.model.ActiveItems[i].CurrentAmountInDollars - ctrl.model.ActiveItems[i].StartingBalanceInDollars);
            }
        }
    )};

    ctrl.changeLinkedAccount = function (newId, oldValue) {
        changeLinkedAccount(newId), 
        function(errors) {
            ctrl.availableAccounts.LinkedAccountId = oldValue;
            for (var key in errors) {
                if (errors.hasOwnProperty(key)) {
                    for (var i = 0; i < errors[key].length; i++) {
                        $.bigBox({
                            title: 'Error',
                            content: errors[key][i],
                            color: "#C46A69",
                            icon: "fa fa-warning shake animated",
                            timeout: 6000
                        });
                    }
                }
            }
        };
    };

    ctrl.switch = function(isShowIcon){
        ctrl.isShowIcon = isShowIcon;
    };

    ctrl.switchWizard = function(){
        ctrl.showStartWizard = !ctrl.showStartWizard;
    };
}]);