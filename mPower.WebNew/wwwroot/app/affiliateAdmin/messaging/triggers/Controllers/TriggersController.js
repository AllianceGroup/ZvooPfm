'use strict';

angular.module('app.affiliateAdmin').controller('TriggersController', ['$uibModal', 'messagesService', 'triggersList','authService',
    function($uibModal, messagesService, triggersList, authService){

        var ctrl = this;
        ctrl.triggers = triggersList.Triggers;
        ctrl.roles = authService.getRoles();

        ctrl.editTrigger = function(id){
            $uibModal.open({
                templateUrl: 'app/affiliateAdmin/messaging/triggers/views/editTrigger.html',
                controller: 'editTriggerController',
                controllerAs: 'editTriggerCtrl',
                resolve:{
                    trigger: function(messagesService){
                        return messagesService.editTrigger(id);
                    },
                    actionFunction: function(messagesService){
                        return messagesService.updateTrigger;
                    }
                }
            });
        };
    }]);