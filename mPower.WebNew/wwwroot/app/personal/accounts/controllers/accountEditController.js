'use strict';

angular.module('app.personal').controller('accountEditController', ['accountsService', 'model', '$uibModalInstance',
    function (accountsService, model, $uibModalInstance) {
        var ctrl = this;

        ctrl.account = model.account;
        ctrl.actionFunction = model.actionFunction;
        ctrl.errors = [];

        ctrl.Save = function(){
            ctrl.actionFunction(ctrl.account).then(function(){
                    $uibModalInstance.close(ctrl.account);
                },
                function(errors) {
                    ctrl.errors = _.reduce(errors, function(result, arr) {
                        return result.concat(arr);
                    }, []);
                });
        };

        ctrl.dismiss = function() {
            $uibModalInstance.dismiss('cancel');
        };
}]);
