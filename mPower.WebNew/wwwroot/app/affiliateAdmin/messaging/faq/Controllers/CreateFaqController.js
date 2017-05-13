'use strict';

angular.module('app.affiliateAdmin').controller('CreateFaqController', function CreateFaqController($state, faq, messagesService) {

    var ctrl = this;
    ctrl.faq = faq;
    ctrl.errors = [];

    $('.faq').summernote({ focus: true, height: 450 });

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