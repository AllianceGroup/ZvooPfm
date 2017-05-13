'use strict';

angular.module('app.personal').controller('DeleteTransactionsController', ['model', '$uibModalInstance',
    function(model, $uibModalInstance) {
        var ctrl = this;

        ctrl.transactionIds = model.transactionIds;
        ctrl.actionFunction = model.actionFunction;

        ctrl.delete = function(){
            ctrl.actionFunction(ctrl.transactionIds).then(function(){
                $uibModalInstance.close();
            });
        };

        ctrl.dismiss = function() {
            $uibModalInstance.dismiss('cancel');
        };
    }]);