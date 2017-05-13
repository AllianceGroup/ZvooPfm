'use strict';

angular.module('app.personal').controller('AuthenticateBusinessController', ['intuitService', '_', '$state', '$stateParams', function (intuitService, _, $state, $stateParams) {
    var ctrl = this;
    var initialize = function (model) {
        for (var i = 0; i < model.Keys.length; i++) {
            model.Keys[i].Type = model.Keys[i].Name.toLowerCase().indexOf('pass') > -1 || model.Keys[i].Description.toLowerCase().indexOf('pass') > -1 ? 'password' : 'text';
        }
        return model;
    }

    ctrl.model = initialize($stateParams.model);
    ctrl.errors = [];
    ctrl.answers = null;
    ctrl.questions = null;
    ctrl.AggregationData = null;
    ctrl.disabledSubmit = false;

    ctrl.submit = function () {
        ctrl.errors = [];
        ctrl.disabledSubmit = true;
        var answers = ctrl.answers ? Object.keys(ctrl.answers).map(function(key) { return ctrl.answers[key] }) : null;
        intuitService.authenticateToBank({
            Dto: {
                contentServiceId: ctrl.model.ContentServiceId,
                accountId: ctrl.model.AccountId,
                keys: ctrl.model.Keys
            },
            Answers: answers,
            AggregationData: ctrl.AggregationData
        }).then(function (model) {
            ctrl.disabledSubmit = false;
            if (model.MfaQuestions) {
                ctrl.questions = model.MfaQuestions.Questions;
                ctrl.AggregationData = model.AggregationData;
            } else {
                $state.go('app.business.accounts-add.intuit-availableccounts', { model: model });
            }
        }, function(errorModel) {
            for (var key in errorModel.Errors) {
                if (errorModel.Errors.hasOwnProperty(key)) {
                    ctrl.errors.push(errorModel.Errors[key].ErrorMessage);
                }
            }
            ctrl.disabledSubmit = false;
        });
    }
}]);
