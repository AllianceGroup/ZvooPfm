'use strict';

angular.module('app.personal').controller('InterectiveRefreshController', ['intuitService', '_', '$state', 'model', '$uibModalInstance',
    function (intuitService, _, $state, model, $uibModalInstance) {
        var ctrl = this;

        ctrl.model = model;
        ctrl.errors = [];
        ctrl.mfaSession = null;
        ctrl.disabledSubmit = false;

        ctrl.dismiss = function () {
            $uibModalInstance.dismiss('cancel');
        };

        ctrl.submit = function () {
            ctrl.errors = [];
            ctrl.disabledSubmit = true;

            intuitService.interactiveRefreshMfa({
                FinicityAccountId: ctrl.model.FinicityAccountId,
                MfaSession: ctrl.model.MfaSession            
            }).then(function (model) {
                ctrl.disabledSubmit = false;
                if (model.MfaSession) {
                    ctrl.mfaSession = model.MfaSession;
                }
                else {
                    $uibModalInstance.close();                   
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
    }]);