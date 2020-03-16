'use strict';

angular.module('app.personal').controller('DebtStartController',
['debttoolsService', 'debttoolsModel', '$uibModal', function (debttoolsService, debttoolsModel, $uibModal) {
    var ctrl = this;

    ctrl.debttoolsModel = debttoolsModel;
    ctrl.addAccounts = function () {
        $uibModal.open({
            templateUrl: "app/personal/accounts/views/add-template.tpl.html",
            controller: 'ChoiceAccountController',
            controllerAs: 'choiceAccountCtrl'
        })
    };
}]);