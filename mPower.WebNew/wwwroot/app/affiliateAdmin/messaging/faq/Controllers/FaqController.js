'use strict';

angular.module('app.affiliateAdmin').controller('FaqController', ['faqList', 'messagesService', '$uibModal', '_', '$state', 'authService',
    function (faqList, messagesService, $uibModal, _, $state, authService) {

        var ctrl = this;
        ctrl.faqs = faqList.FaqList;
        ctrl.roles = authService.getRoles();

        ctrl.deleteFaq = function (id) {
            var modalInstance = $uibModal.open({
                templateUrl: 'app/affiliateAdmin/messaging/faq/views/deleteFaq.html',
                controller: 'DeleteFaqController',
                controllerAs: 'deleteFaqCtrl',
                resolve: {
                    model: function () {
                        return {
                            faq: _.findWhere(ctrl.faqs, { Id: id }),
                            actionFunction: messagesService.deleteFaq
                        }
                    }
                }
            });

            modalInstance.result.then(function(id) {
                ctrl.faqs = _.filter(ctrl.faqs, function(faq) { return faq.Id != id });
            });
        };

        ctrl.editFaq = function (id) {
            messagesService.editFaq(id).then(function (model) {
                $state.go("app.affiliateAdmin.editFaq", { model: model });
            });
        };

        ctrl.dismiss = function () {
            $state.transitionTo('app.personal.dashboard');
        };
    }]);