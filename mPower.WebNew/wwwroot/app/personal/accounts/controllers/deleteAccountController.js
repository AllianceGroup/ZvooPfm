'use strict';

angular.module('app.personal').controller('deleteAccountController', ['accountsService', 'model', '$uibModalInstance',
    function (accountsService, model, $uibModalInstance) {
        var ctrl = this;

        ctrl.account = model.account;
        ctrl.actionFunction = model.actionFunction;

        ctrl.Delete = function(){
            ctrl.actionFunction(ctrl.account.AccountId).then(function(){
                $uibModalInstance.close();
            });
        };

        ctrl.dismiss = function() {
            $uibModalInstance.dismiss('cancel');
        };
    }]);