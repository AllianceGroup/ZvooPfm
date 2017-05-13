'use strict';

angular.module('app.affiliateAdmin').controller('AddUserController', ['model', '$uibModalInstance',
    function(model, $uibModalInstance){

        var ctrl = this;
        ctrl.actionFunction = model.actionFunction;
        ctrl.errors = [];

        ctrl.addUser = function(){
            ctrl.actionFunction(ctrl.user).then(function(user){
                $uibModalInstance.close(user);
            }, function(errors){
                ctrl.errors = _.reduce(errors, function(result, arr) {
                    return result.concat(arr);
                }, []);
            });
        };

        ctrl.checkPermissions = function(){
            if(ctrl.user.AffiliateAdminEdit)
                ctrl.user.AffiliateAdminView = true;
            if(ctrl.user.AffiliateAdminDelete){
                ctrl.user.AffiliateAdminView = true;
                ctrl.user.AffiliateAdminEdit = true
            }
        };

        ctrl.dismiss = function(){
            $uibModalInstance.dismiss('cancel');
        };
    }]);