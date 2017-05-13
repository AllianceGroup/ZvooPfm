'use strict';

angular.module('app.personal').controller('ModalController',
['model', '$uibModalInstance', function (model, $uibModalInstance) {

    var ctrl = this;
    ctrl.model = model;

    ctrl.confirm = function() {
        $uibModalInstance.close();
    };

    ctrl.dismiss = function() {
        $uibModalInstance.dismiss('cancel');
    };
}]);