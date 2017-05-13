'use strict';

angular.module('app.auth').controller('resetPasswordController', ['authService', '$stateParams', '$state', function(authService, $stateParams, $state){
    var ctrl = this;
    ctrl.errors = [];
    ctrl.model = {
        Token: $stateParams.token
    };

    ctrl.submit = function(){
        ctrl.errors = [];
        authService.resetPassword(ctrl.model).then(function(){
            $.smallBox({
                title: "Success",
                content: "You have reseted your password.",
                color: "#739e73",
                timeout: 2000
            });
            $state.go('login');
        }, function(errors){
            ctrl.errors = _.reduce(errors, function(result, arr) {
                return result.concat(arr);
            }, []);
        });
    };
}]);