'use strict';

angular.module('app.personal').controller('EditAccountController', ['accountsService', '$state', 'model', '$uibModalInstance',
    function (accountsService, $state, model, $uibModalInstance) {
        var ctrl = this;

        ctrl.errors = [];
        ctrl.model = {
            AccountLabels: model,
            Accounts: {},
            Label: null
        };

        ctrl.add = function() {
            ctrl.errors = [];
            accountsService.addManually(ctrl.model).then(function() {
                ctrl.model.AccountLabels.Selected = true;
                $.smallBox({
                    title: "Success",
                    content: "Account added",
                    color: "#739e73",
                    timeout: 4000
                });
                $uibModalInstance.close();
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

        ctrl.loadParentAccounts = function(label) {
            accountsService.getParents(label).then(function (data) {
                angular.copy(data, ctrl.model.Accounts);
            });
        };

        ctrl.dismiss = function() {
            $uibModalInstance.dismiss('cancel');
        };
    }]);