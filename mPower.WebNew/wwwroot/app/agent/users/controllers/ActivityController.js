'use strict';

angular.module('app.agentUser').controller('ActivityAgentController', ['model', 'usersAgentService', '$uibModalInstance',
    function (model, usersAgentService, $uibModalInstance) {
        var ctrl = this;

        ctrl.user = model.user;

        ctrl.dismiss = function(){
            $uibModalInstance.dismiss('cancel');
        };

        ctrl.refresh = function(){
            usersAgentService.activity({ id: ctrl.user.UserId, pageNumber: ctrl.user.Paging.CurrentPage }).then(function (user) {
                ctrl.user = user;
            });
        };
    }]);