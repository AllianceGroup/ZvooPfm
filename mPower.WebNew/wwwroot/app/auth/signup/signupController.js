'use strict';

angular.module('app.auth').controller('SignUpController',
['authService', '$state', 'productHandle', "$window", function (authService, $state, productHandle, $window) {

    var ctrl = this;
    ctrl.errors = [];

    ctrl.model = {
        ProductHandle: productHandle
    };

    ctrl.register = function () {
        ctrl.errors = [];
        authService.saveRegistration(ctrl.model).then(function(url) {
            $window.location.href = url;
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