'use strict';

angular.module('app.affiliateAdmin').controller('EditFaqController', function EditFaqController($stateParams, $state, messagesService) {

    var ctrl = this;
    ctrl.faq = $stateParams.model;
    ctrl.errors = [];
    ctrl.Name = ctrl.faq.Name;

    $('.faq').summernote({ focus: true, height: 450 });
    $('.faq').summernote("code", ctrl.faq.Html);

    ctrl.save = function () {
        ctrl.faq.Html = $('.faq').summernote('code');

        messagesService.updateFaq(ctrl.faq).then(function () {
            $state.transitionTo('app.affiliateAdmin.faq');
        }, function (errors) {
            ctrl.errors = _.reduce(errors, function (result, arr) {
                return result.concat(arr);
            }, []);
        });
    };

    ctrl.previewFaq = function () {
        ctrl.faq.Html = $('.faq').summernote('code');
        ctrl.Name = ctrl.faq.Name;
    };

    ctrl.dismiss = function () {
        $state.transitionTo('app.affiliateAdmin.faq');
    };
});