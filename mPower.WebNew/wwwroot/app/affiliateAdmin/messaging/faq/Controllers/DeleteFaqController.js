'use strict';

angular.module('app.affiliateAdmin').controller('DeleteFaqController', ['$uibModalInstance', 'model',
    function ($uibModalInstance, model) {

        var ctrl = this;
        ctrl.faq = model.faq;
        ctrl.actionFunction = model.actionFunction;
        ctrl.errors = [];

        ctrl.dismiss = function () {
            $uibModalInstance.dismiss('cancel');
        };

        ctrl.deleteFaq = function (id) {
            ctrl.actionFunction(id).then(function () {
                $uibModalInstance.close(id);
            }, function (errors) {
                ctrl.errors = _.reduce(errors, function (result, arr) {
                    return result.concat(arr);
                }, []);
            });
        };
    }]);