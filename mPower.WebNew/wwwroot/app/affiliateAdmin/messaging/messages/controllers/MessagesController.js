'use strict';

angular.module('app.affiliateAdmin').controller('MessagesController', ['$uibModal', 'messagesService', 'messagesList', '_', '$state', 'authService',
    function($uibModal, messagesService, messagesList, _, $state, authService){

        var ctrl = this;
        ctrl.messages = messagesList.Messages;
        ctrl.roles = authService.getRoles();

        ctrl.delete = function(id){
            var modalInstance = $uibModal.open({
                templateUrl: 'app/affiliateAdmin/messaging/messages/views/deleteMessage.html',
                controller: 'DeleteMessageController',
                controllerAs: 'deleteMessageCtrl',
                resolve: {
                    model: function () {
                        return {
                            message : _.findWhere(ctrl.messages, {Id: id}),
                            actionFunction: messagesService.deleteMessage
                        }
                    }
                }
            });

            modalInstance.result.then(function(id){
                ctrl.messages = _.filter(ctrl.messages, function(message){return message.Id != id});
            })
        };

        ctrl.editMessage = function(id){
            messagesService.editMessage(id).then(function(model){
                $state.go("app.affiliateAdmin.editMessage", {model : model});
            });
        };
    }]);