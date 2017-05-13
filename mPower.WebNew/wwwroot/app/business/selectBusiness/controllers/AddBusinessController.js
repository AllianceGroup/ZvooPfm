'use strict';

angular.module('app.business').controller('AddBusinessController', ['model', '$uibModalInstance',
    function(model, $uibModalInstance){
        var ctrl = this;
        ctrl.errors = [];

        ctrl.business = model.business;
        ctrl.actionFunction = model.actionFunction;

        ctrl.dismiss = function(){
            $uibModalInstance.dismiss('cancel');
        };

        ctrl.addBusiness = function(){
            ctrl.actionFunction(ctrl.business).then(function(addedBusiness){
                $uibModalInstance.close(addedBusiness);
            }, function(errors){
                ctrl.errors = _.reduce(errors, function(result, arr) {
                    return result.concat(arr);
                }, []);
            });
        };
    }]);