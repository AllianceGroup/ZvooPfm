'use strict';

angular.module('app.personal').controller('FinancialController',
['accountsService', 'model', '$uibModal', '_', '$uibModalInstance', function (accountsService, model, $uibModal, _, $uibModalInstance) {

    var ctrl = this;

    ctrl.model = model;

    ctrl.delete = function (id) {
        var modalInstance = $uibModal.open({
            templateUrl: 'app/personal/goals/views/modalTemplate.tpl.html',
            controller: 'ModalController',
            controllerAs: 'ctrl',
            resolve: {
                model: function () {
                    return {
                        Title: 'Account delete confirmation',
                        Text: 'Are you sure?'
                    }
                }
            }
        });
        modalInstance.result.then(function () {
            accountsService.Delete(id).then(function () {
                ctrl.model = _.reject(ctrl.model, function (val) { return val.AccountId === id });
                $.smallBox({
                    title: "Success",
                    content: "Item removed",
                    color: "#739e73",
                    timeout: 4000
                });
            }, function(errors) {
                for (var key in errors) {
                    if (errors.hasOwnProperty(key)) {
                        for (var i = 0; i < errors[key].length; i++) {
                            $.smallBox({
                                title: "Error",
                                content: errors[key][i],
                                color: "#c26565",
                                iconSmall: "fa fa-warning",
                                timeout: 4000
                            });
                        }
                    }
                }
            });
        });
    };

    ctrl.dismiss = function() {
        $uibModalInstance.dismiss();
    };
}]);