'use strict';

angular.module('app.personal').controller('Step2Controller',
['estimateService', '$stateParams', '_', 'goalsService', '$state', 'prevState', function (estimateService, $stateParams, _, goalsService, $state, prevState) {

    var ctrl = this;

    ctrl.model = $stateParams.model;
    ctrl.model.PlannedMonth = ctrl.model.PlannedMonth.toString();
    ctrl.model.PlannedYear = ctrl.model.PlannedYear.toString();
    ctrl.errors = [];
    ctrl.oldStartingBalance = ctrl.model.StartingBalance;
    ctrl.hasPrevStepModel = $stateParams.prevStepModel === null ? false : true;


    ctrl.estimateByAmmountAndDate = function () {
        ctrl.errors = [];        
        if (!(ctrl.model.StartingBalance > 0 && ctrl.model.StartingBalance < ctrl.model.Amount)) {
            ctrl.model.StartingBalance = ctrl.oldStartingBalance;
            return ctrl.errors.push("The amount that you already have should be more than 0 и less than Goal Amount");
        }
        ctrl.oldStartingBalance = ctrl.model.StartingBalance;
        estimateService.estimateByAmmountAndDate(ctrl.model).then(function(model) {
            if (!_.isUndefined(model.Summary)) ctrl.model.Summary = model.Summary;
            if (!_.isUndefined(model.DateAway)) ctrl.model.DateAway = model.DateAway;
            if (!_.isUndefined(model.MonthlyContribution)) ctrl.model.MonthlyContribution = model.MonthlyContribution;

        },function(errors) {
            for (var key in errors) {
                if (errors.hasOwnProperty(key)) {
                    for (var i = 0; i < errors[key].length; i++) {
                        ctrl.errors.push(errors[key][i]);
                    }
                }
            }
        });
    };

    ctrl.estimateByMonthlyPayment = function () {
        ctrl.errors = [];

        estimateService.estimateByMonthlyPayment(ctrl.model).then(function(model) {
            if (!_.isUndefined(model.Summary)) ctrl.model.Summary = model.Summary;
        }, function(errors) {
            for (var key in errors) {
                if (errors.hasOwnProperty(key)) {
                    for (var i = 0; i < errors[key].length; i++) {
                        ctrl.errors.push(errors[key][i]);
                    }
                }
            }
        });
    };

    ctrl.createGoal = function () {
        ctrl.errors = [];
        goalsService.createGoal(ctrl.model).then(function() {
            $state.go('app.personal.goals.items', '', { reload: true });
        }, function(errors) {
            for (var key in errors) {
                if (errors.hasOwnProperty(key)) {
                    for (var i = 0; i < errors[key].length; i++) {
                        ctrl.errors.push(errors[key][i]);
                    }
                }
            }
        });
    };

    ctrl.back = function() {
        $state.go(prevState.name, { model: $stateParams.prevStepModel });
    };
}]);