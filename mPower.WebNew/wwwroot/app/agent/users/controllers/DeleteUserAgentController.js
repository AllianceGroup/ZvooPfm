'use strict';


angular.module('app.agentUser').controller('DeleteUserAgentController', ['model', '$uibModalInstance',
    function(model, $uibModalInstance){

        var ctrl = this;
        ctrl.user = model.user;
        ctrl.actionFunction = model.actionFunction;

        ctrl.dismiss = function() {
            $uibModalInstance.dismiss('cancel');
        };

        ctrl.deleteUser = function(id){
            ctrl.actionFunction(id).then(function(){
                $uibModalInstance.close(id);
            });
        }
    }]);