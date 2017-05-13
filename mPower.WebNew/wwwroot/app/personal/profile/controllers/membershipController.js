'use strict';

angular.module('app.personal').controller('MembershipController',
['realEstateService', '$uibModal', '_', '$state', 'model', '$uibModalInstance', function (realEstateService, $uibModal, _, $state, model, $uibModalInstance) {

    var ctrl = this;

    ctrl.model = model;

    ctrl.canelSubscription = function(id) {
        var modalInstance = $uibModal.open({
            templateUrl: 'app/personal/goals/views/modalTemplate.tpl.html',
            controller: 'ModalController',
            controllerAs: 'ctrl',
            resolve: {
                model: function() {
                    return {
                        Title: 'Cancel confirmation',
                        Text: 'Are you sure?'
                    }
                }
            }
        });
        modalInstance.result.then(function() {
            realEstateService.cancelSubscription(id).then(function () {
                ctrl.model.Subscriptions = _.reject(ctrl.model.Subscriptions, function (val) { return val.Id === id });
            }, function(errors) {
                for (var key in errors) {
                    if (errors.hasOwnProperty(key)) {
                        for (var i = 0; i < errors[key].length; i++) {
                            $.smallBox({
                                title: "Error",
                                content: errors[key][i],
                                color: "#296191",
                                iconSmall: "fa fa-warning",
                                timeout: 4000
                            });
                        }
                    }
                }
            });
        });
    };

    ctrl.dismiss = function() {
        $uibModalInstance.dismiss();
    };
}]);