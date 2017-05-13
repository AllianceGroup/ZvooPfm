'use strict';

angular.module('app.personal').controller('AuthenticateController', ['intuitService', '_', '$state', 'model', '$uibModalInstance', '$uibModal',
    function (intuitService, _, $state, model, $uibModalInstance, $uibModal) {
        var ctrl = this;

        ctrl.model = model;
        ctrl.errors = [];
        ctrl.mfaSession = null;
        ctrl.disabledSubmit = false;

        ctrl.dismiss = function() {
            $uibModalInstance.dismiss('cancel');
        };

        ctrl.submit = function () {
            ctrl.errors = [];
            ctrl.disabledSubmit = true;
            
              intuitService.authenticateToBank({
                Dto: {
                    InstitutionId: ctrl.model.InstitutionId,
                    Keys: ctrl.model.Keys
                },
                MfaSession: ctrl.mfaSession
            }).then(function (model) {
                ctrl.disabledSubmit = false;
                if (model.MfaSession) {
                    ctrl.mfaSession = model.MfaSession;
                }
                else{
                    $uibModalInstance.close();
                    $uibModal.open({
                        templateUrl: 'app/personal/accounts/views/intuit-availableAccounts.tpl.html',
                        controller: 'AvailableAccountsController',
                        controllerAs: 'intuitCtrl',
                        resolve:{
                            model: model
                        }
                    });
                }
            }, function (errorModel) {
                ctrl.mfaSession = null;

                for (var key in errorModel.Errors) {
                    if (errorModel.Errors.hasOwnProperty(key)) {
                        ctrl.errors.push(errorModel.Errors[key].ErrorMessage);
                    }
                }
                ctrl.disabledSubmit = false;
            });  
        };

        ctrl.back = function () {
            ctrl.dismiss();
            $uibModal.open({
                templateUrl: 'app/personal/accounts/views/intuit-search.tpl.html',
                controller: 'SearchController',
                controllerAs: 'intuitCtrl'
            });
        };

        ctrl.submitReauthenticate = function() {
            ctrl.errors = [];
            ctrl.disabledSubmit = true;

            intuitService.reauthentication({
                Dto: {
                    InstitutionId: ctrl.model.InstitutionId,
                    Keys: ctrl.model.Keys
                },
                IntuitAccountId: ctrl.model.IntuitAccountId
            }).then(function (ok) {
                ctrl.disabledSubmit = false;
                $uibModalInstance.close();                              
            }, function (errorModel) {
                ctrl.mfaSession = null;

                for (var key in errorModel.Errors) {
                    if (errorModel.Errors.hasOwnProperty(key)) {
                        ctrl.errors.push(errorModel.Errors[key].ErrorMessage);
                    }
                }
                ctrl.disabledSubmit = false;
            });
        }
    }]);