'use strict';

angular.module('app.personal').controller('ChoiceAccountController', ['$uibModal', '$uibModalInstance',
    function ($uibModal, $uibModalInstance) {
        var ctrl = this;

        ctrl.intuitSearch = function(){
            $uibModalInstance.close();
            $uibModal.open({
                templateUrl: 'app/personal/accounts/views/intuit-search.tpl.html',
                controller: 'SearchController',
                controllerAs: 'intuitCtrl'
            });
        };

        ctrl.addManually = function(){
            $uibModalInstance.close();
            $uibModal.open({
                controller: 'EditAccountController',
                controllerAs: 'editAccountCtrl',
                templateUrl: "app/personal/accounts/views/add-manually.tpl.html",
                resolve: {
                    model: function(accountsService) {
                        return accountsService.getEditModel();
                    }
                }
            })
        };

        ctrl.dismiss = function() {
            $uibModalInstance.dismiss('cancel');
        };
    }]);