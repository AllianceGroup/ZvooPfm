"use strict";

angular.module('app.affiliateAdmin')
.config(function($stateProvider){
    $stateProvider
        .state('app.affiliateAdmin.templates', {
            url: '/AffiliateAdmin/Templates',
            views:{
                "content@app":{
                    templateUrl: 'app/affiliateAdmin/messaging/templates/templates.html',
                    controller: 'TemplatesController',
                    controllerAs: 'templatesCtrl'
                }
            },
            data: {
                title: 'Templates'
            },
            resolve: {
                templatesList: function(messagesService){
                    return messagesService.getTemplates();
                }
            }
        })
        .state('app.affiliateAdmin.createTemplate', {
            url: '/AffiliateAdmin/CreateTemplate',
            views:{
                "content@app":{
                    templateUrl: 'app/affiliateAdmin/messaging/templates/views/createTemplate.html',
                    controller: 'CreateTemplateController',
                    controllerAs: 'createTemplateCtrl'
                }
            },
            data:{
                title: 'Create Template'
            },
            resolve: {
                template: function(messagesService){
                    return messagesService.editTemplate();
                },
                srcipts: function(lazyScript) {
                    return lazyScript.register([
                        'summernote'
                    ])
                }
            }
        })
        .state('app.affiliateAdmin.editTemplate', {
            views: {
                "content@app":{
                    templateUrl: 'app/affiliateAdmin/messaging/templates/views/editTemplate.html',
                    controller: 'EditTemplateController',
                    controllerAs: 'editTemplateCtrl'
                }
            },
            data:{
                title: 'Edit Message'
            },
            params:{
                model: null
            },
            resolve: {
                srcipts: function(lazyScript) {
                    return lazyScript.register([
                        'summernote'
                    ])
                }
            }
        })
});