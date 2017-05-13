'use strict';

angular.module('app.globalAdmin').controller('UserInfoGlobalController', ['$uibModalInstance', 'user',
    function($uibModalInstance, user){
        var ctrl = this;

        ctrl.user = user;

        ctrl.dismiss = function() {
            $uibModalInstance.dismiss('cancel');
        };
    }]);