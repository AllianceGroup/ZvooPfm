'use strict';

angular.module('app.affiliateAdmin').controller('EditMessageController', function EditMessageController($stateParams, $state, messagesService){
    var ctrl = this;
    ctrl.message = $stateParams.model;
    ctrl.errors = [];

    $('.message').summernote({focus: true, height: 450});
    $('.message').summernote("code", ctrl.message.Html);

    ctrl.save = function(status){
        if(status == "Active")
            ctrl.message.Status = "Active";
        else
            ctrl.message.Status = "Draft";

        delete ctrl.message.Templates;
        ctrl.message.Html = $('.message').summernote('code');

        messagesService.updateMessage(ctrl.message).then(function(){
            $state.transitionTo('app.affiliateAdmin.messages');
        }, function(errors) {
            ctrl.errors = _.reduce(errors, function(result, arr) {
                return result.concat(arr);
            }, []);
        });
    };

    ctrl.previewMessage = function(){
        ctrl.isPreview = true;
        ctrl.message.Html = $('.message').summernote('code');
        messagesService.previewMessage(ctrl.message).then(function(message){
            ctrl.message.Html = message;
            ctrl.message.Templates = templates;
        });
    };
});