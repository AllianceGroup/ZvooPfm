'use strict';

angular.module('app.auth').controller('LoginController',
['authService', '$state', 'chargifyModel', function (authService, $state, chargifyModel) {

    var ctrl = this;

    ctrl.model = {};
    ctrl.errors = [];

    ctrl.login = function () {
        ctrl.errors = [];
        authService.login(ctrl.model).then(function (model) {
            if (model.authenticated) {
                $state.go('app.personal.dashboard');
            } else {
                ctrl.model.SecurityQuestion = model.SecurityQuestion;
            }
        }, function(errors) {
            for (var key in errors) {
                if (errors.hasOwnProperty(key)) {
                    for (var i = 0; i < errors[key].length; i++) {
                        ctrl.errors.push(errors[key][i]);
                    }
                }
            }
        });
    }

    if (chargifyModel && chargifyModel.errors) {
        $.smallBox({
            title: "Error",
            content: chargifyModel.errors,
            color: "rgb(196, 106, 105)",
            iconSmall: "fa fa-warning",
            timeout: 4000
        });
    }
    if (chargifyModel && chargifyModel.message) {
        $.smallBox({
            title: "Success",
            content: chargifyModel.message,
            color: "#739e73",
            timeout: 4000
        });
    }

}]);