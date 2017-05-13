'use strict';

angular.module('app.affiliateAdmin').controller('ActivityController', ['model', 'usersService', '$uibModalInstance',
    function(model, usersService, $uibModalInstance){
        var ctrl = this;

        ctrl.user = model.user;

        ctrl.dismiss = function(){
            $uibModalInstance.dismiss('cancel');
        };

        ctrl.refresh = function(){
            usersService.activity({id: ctrl.user.UserId, pageNumber: ctrl.user.Paging.CurrentPage}).then(function(user){
                ctrl.user = user;
            });
        };
    }]);