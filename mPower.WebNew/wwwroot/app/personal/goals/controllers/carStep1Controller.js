'use strict';

angular.module('app.personal').controller('CarStep1Controller',
['estimateService', 'model', '$state', function (estimateService, model, $state) {

    var ctrl = this;

    ctrl.model = model;
    ctrl.model.CreditDuration = model.CreditDuration;
    ctrl.errors = [];
    ctrl.model.CreditDuration = ctrl.model.CreditDuration.toString();

    ctrl.estimate = function() {
        ctrl.errors = [];
        //if (ctrl.model.AverageMonthlySpending === null) ctrl.model.AverageMonthlySpending = 0;

        estimateService.estimateCar(ctrl.model).then(function(model) {
            ctrl.model.EstimatedValue = model.EstimatedValue;
            ctrl.model.CostWithTaxs = model.CostWithTaxs;
            ctrl.model.MonthlyPayment = model.MonthlyPayment;
            ctrl.model.Loan = model.Loan;
            ctrl.model.TradingVehicleCost = model.TradingVehicleCost;
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

    ctrl.step2 = function () {
        ctrl.errors = [];
        estimateService.finishCar(ctrl.model).then(function(goal) {
            $state.go('app.personal.goals.items.add.step2', { model: goal, prevStepModel: ctrl.model });
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
}]);