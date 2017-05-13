'use strict';

angular.module('app.affiliateAdmin').controller('CreateMessageController', ['$state', 'message', 'messagesService',
    function ($state, message, messagesService){
        var ctrl = this;
        ctrl.message = message;
        ctrl.errors = [];

        $('.message').summernote({focus: true, height: 450});

        ctrl.save = function(status){
            if(status == "Active")
                ctrl.message.Status = "Active";
            else
                ctrl.message.Status = "Draft";

            ctrl.message.Html = $('.message').summernote('code');

            messagesService.updateMessage(ctrl.message).then(function(){
                $state.transitionTo("app.affiliateAdmin.messages");
            }, function(errors) {
                ctrl.errors = _.reduce(errors, function(result, arr) {
                    return result.concat(arr);
                }, []);
            });
        };

        ctrl.previewMessage = function(){
            ctrl.message.Html = $('.message').summernote('code');
            messagesService.previewMessage(ctrl.message).then(function(message){
                ctrl.message.Html = message;
                ctrl.message.Templates = templates;
            });
        };
    }]);