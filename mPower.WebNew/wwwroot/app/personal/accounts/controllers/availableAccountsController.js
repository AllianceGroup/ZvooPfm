'use strict';

angular.module('app.personal').controller('AvailableAccountsController', ['intuitService', '_', '$state', 'model', '$uibModalInstance',
    function (intuitService, _, $state, model, $uibModalInstance) {
        var ctrl = this;

        ctrl.model = model;
        ctrl.errors = [];
        ctrl.isAdding = false;

        ctrl.dismiss = function() {
            $uibModalInstance.dismiss('cancel');
        };

        ctrl.aligntoledger = function () {
            ctrl.errors = [];
            ctrl.isAdding = true;
            intuitService.aligntoledger(ctrl.model).then(function(accounts) {
                ctrl.isAdding = false;
                $.smallBox({
                    title: "Success",
                    content: "Accounts added",
                    color: "#739e73",
                    timeout: 4000
                });
                $uibModalInstance.close();
            }, function(errors) {
                ctrl.isAdding = false;
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