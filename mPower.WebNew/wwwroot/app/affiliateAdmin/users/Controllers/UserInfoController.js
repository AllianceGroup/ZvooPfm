'use strict';

angular.module('app.affiliateAdmin').controller('UserInfoController', ['$uibModalInstance', 'user',
function($uibModalInstance, user){
    var ctrl = this;

    ctrl.user = user;

    ctrl.dismiss = function() {
        $uibModalInstance.dismiss('cancel');
    };
}]);