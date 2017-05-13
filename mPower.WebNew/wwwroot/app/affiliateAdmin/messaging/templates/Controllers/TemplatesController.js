'use strict';

angular.module('app.affiliateAdmin').controller('TemplatesController', ['templatesList', 'messagesService', '$uibModal', '_', '$state', 'authService',
    function(templatesList, messagesService, $uibModal, _, $state, authService){

        var ctrl = this;
        ctrl.templates = templatesList.Templates;
        ctrl.roles = authService.getRoles();

        ctrl.deleteTemplate = function(id){
            var modalInstance = $uibModal.open({
                templateUrl: 'app/affiliateAdmin/messaging/templates/views/deleteTemplate.html',
                controller: 'DeleteTemplateController',
                controllerAs: 'deleteTemplateCtrl',
                resolve: {
                    model: function () {
                        return {
                            template : _.findWhere(ctrl.templates, {Id: id}),
                            actionFunction: messagesService.deleteTemplate
                        }
                    }
                }
            });

            modalInstance.result.then(function(id){
                ctrl.templates = _.filter(ctrl.templates, function(template){return template.Id != id});
            })
        };

        ctrl.editTemplate = function(id){
            messagesService.editTemplate(id).then(function(model){
                $state.go("app.affiliateAdmin.editTemplate", {model : model});
            });
        };
    }]);