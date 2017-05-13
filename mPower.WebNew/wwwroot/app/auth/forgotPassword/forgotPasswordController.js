"use strict";

angular.module("app.auth").controller('forgotPasswordController', ['authService', function(authService){
    var ctrl = this;

    ctrl.errors = [];
    ctrl.email = "";

    ctrl.submit = function() {
        ctrl.errors = [];
        authService.forgotPassword(ctrl.email).then(function () {
            $.smallBox({
                title: "Success",
                content: "Instructions for resetting your password have sent to email.",
                color: "#739e73",
                timeout: 10000
            });
            $state.go('login');
        }, function(errors) {
            ctrl.errors = _.reduce(errors, function(result, arr) {
                return result.concat(arr);
            }, []);
        });
    };
}]);