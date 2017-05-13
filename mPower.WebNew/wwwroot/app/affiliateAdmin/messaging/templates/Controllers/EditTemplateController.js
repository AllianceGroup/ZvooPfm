'use strict';

angular.module('app.affiliateAdmin').controller('EditTemplateController', function EditTemplateController($stateParams, $state, messagesService){

    var ctrl = this;
    ctrl.template = $stateParams.model;
    ctrl.errors = [];

    $('.template').summernote({focus: true, height: 450});
    $('.template').summernote("code", ctrl.template.Html);

    ctrl.save = function(){
        ctrl.template.Html = $('.template').summernote('code');

        messagesService.updateTemplate(ctrl.template).then(function(){
            $state.transitionTo('app.affiliateAdmin.templates');
        }, function(errors){
            ctrl.errors = _.reduce(errors, function(result, arr) {
                return result.concat(arr);
            }, []);
        });
    };

    ctrl.previewTemplate = function(){
        ctrl.template.Html = $('.template').summernote('code');
    };
});