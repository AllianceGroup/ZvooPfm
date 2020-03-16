'use strict';

angular.module('app.agentUser').controller('UserInfoAgentController', ['$uibModalInstance', 'user',
    function($uibModalInstance, user){
        var ctrl = this;

        ctrl.user = user;

        ctrl.dismiss = function() {
            $uibModalInstance.dismiss('cancel');
        };
    }]);