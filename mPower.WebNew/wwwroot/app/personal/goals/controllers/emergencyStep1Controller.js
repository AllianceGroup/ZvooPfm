'use strict';

angular.module('app.personal').controller('EmergencyStep1Controller',
['estimateService', 'model', '_', '$state', function (estimateService, model, _, $state) {

    var ctrl = this;

    ctrl.model = model;
    ctrl.errors = [];

    ctrl.estimate = function() {
        ctrl.errors = [];

        estimateService.estimateEmergency(ctrl.model).then(function(model) {
            ctrl.model.EstimatedValue = model.EstimatedValue;
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
        estimateService.finishEmergency(ctrl.model).then(function (goal) {
            $state.go('app.personal.goals.items.add.step2', { model: goal, prevStepModel: ctrl.model });
        }, function (errors) {
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