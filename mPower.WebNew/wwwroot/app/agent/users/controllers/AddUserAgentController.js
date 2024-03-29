'use strict';

angular.module('app.agentUser').controller('AddUserAgentController', ['model', '$uibModalInstance',
    function(model, $uibModalInstance){

        var ctrl = this;
        ctrl.errors = [];
        ctrl.actionFunction = model.actionFunction;
        ctrl.user = model.user;

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
            if(ctrl.user.GlobalAdminEdit)
                ctrl.user.GlobalAdminView = true;
            if(ctrl.user.GlobalAdminDelete){
                ctrl.user.GlobalAdminEdit = true;
                ctrl.user.GlobalAdminView = true;
            }
            if (ctrl.user.Agent)
                ctrl.user.Agent = true;
            //if (ctrl.user.AgentEdit)
            //    ctrl.user.AgentView = true;
            //if (ctrl.user.AgentDelete) {
            //    ctrl.user.AgentView = true;
            //    ctrl.user.AgentEdit = true
            //}
        };

        ctrl.dismiss = function(){
            $uibModalInstance.dismiss('cancel');
        };
    }]);