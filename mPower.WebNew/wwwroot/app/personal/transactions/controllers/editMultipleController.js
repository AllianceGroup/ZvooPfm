'use strict';

angular.module('app.personal').controller('EditMultipleController',
['transactionsService', '$uibModalInstance', 'categories', 'transactionIds', function (transactionsService, $uibModalInstance, categories, transactionIds) {

    var ctrl = this;

    ctrl.categories = categories;
    ctrl.transactionIds = transactionIds;
    ctrl.selectedCategory = '';
    ctrl.memo = '';

    ctrl.editMultiple = function () {
        var model = {
            Transactions: ctrl.transactionIds,
            AccountId: ctrl.selectedCategory,
            Memo: ctrl.memo
        }
        transactionsService.editMultiple(model).then(function() {
            $uibModalInstance.close(model);
        }, function(errors) {

        });
    }

    ctrl.dismiss = function () {
        $uibModalInstance.dismiss('cancel');
    }
}]);