'use strict';


angular.module('app.affiliateAdmin').controller('DeleteUserController', ['model', 'usersService', '$uibModalInstance', '_',
    function(model, usersService, $uibModalInstance){

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