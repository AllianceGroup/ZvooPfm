'use strict';

angular.module('app.personal').controller('EditController',
['transactionsService', '$uibModalInstance', 'model', function (transactionsService, $uibModalInstance, model) {

    var ctrl = this;

    ctrl.model = model.model;
    ctrl.model.BookedDate = new Date(ctrl.model.BookedDate);
    ctrl.actionType = model.actionType;
    ctrl.actionFunction = model.actionFunction;
    ctrl.errors = [];

    ctrl.dismiss = function() {
        $uibModalInstance.dismiss('cancel');
    };

    ctrl.edit = function() {
        ctrl.actionFunction(ctrl.model).then(function () {
            $uibModalInstance.close(ctrl.model);
        }, function(errors) {
            ctrl.errors = _.reduce(errors, function(result, arr) {
                return result.concat(arr);
            }, []);
        });
    };

    ctrl.delete = function() {
        transactionsService.delete(ctrl.model.TransactionId).then(function() {
            $uibModalInstance.close();
        });
    };
}]);