'use strict';

angular.module('app.personal').controller('DeleteEventController',
['$uibModalInstance', function ($uibModalInstance) {

    var ctrl = this;

    ctrl.confirm = function (type) {
        $uibModalInstance.close({ type: type });
    };

    ctrl.dismiss = function () {
        $uibModalInstance.dismiss('cancel');
    };
}]);