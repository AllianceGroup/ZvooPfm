'use strict';

angular.module('app.affiliateAdmin').controller('editTriggerController', ['$uibModalInstance', 'trigger', 'actionFunction',
    function($uibModalInstance, trigger, actionFunction){
        var ctrl = this;

        ctrl.trigger = trigger;
        ctrl.errors = [];

        ctrl.save = function(){
            actionFunction(ctrl.trigger).then(function(){
                $uibModalInstance.dismiss();
            },function(errors) {
                ctrl.errors = _.reduce(errors, function(result, arr) {
                    return result.concat(arr);
                }, []);
            })
        };

        ctrl.dismiss = function () {
            $uibModalInstance.dismiss('cancel');
        }
    }]);