'use strict';

angular.module('app.personal').controller('CollegeStep1Controller',
['estimateService', 'model', '$state', function (estimateService, model, $state) {

    var ctrl = this;

    ctrl.model = model;
    ctrl.errors = [];

    ctrl.estimateCollege = function () {
        var selectedCollege = _.find(ctrl.model.InitialModels, function(val) { return val.CollegeType === parseInt(ctrl.model.CollegeType) });
        ctrl.model.CostPerYear = selectedCollege.CostPerYear;
        ctrl.model.CollegeYears = selectedCollege.CollegeYears;
        ctrl.estimate();
    };

    ctrl.estimate = function () {
        ctrl.errors = [];

        estimateService.estimateCollege(ctrl.model).then(function (model) {
            ctrl.model.EstimatedValue = model.EstimatedValue;
        }, function (errors) {
            for (var key in errors) {
                if (errors.hasOwnProperty(key)) {
                    for (var i = 0; i < errors[key].length; i++) {
                        ctrl.errors.push(errors[key][i]);
                    }
                }
            }
        });
    }

    ctrl.step2 = function () {
        ctrl.errors = [];
        estimateService.finishCollege(ctrl.model).then(function (goal) {
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