"use strict";

angular.module('app.affiliateAdmin')
.config(function($stateProvider){
    $stateProvider
        .state('app.affiliateAdmin.messages', {
            url: '/AffiliateAdmin/Messages',
            views:{
                "content@app":{
                    templateUrl: 'app/affiliateAdmin/messaging/messages/messages.html',
                    controller: "MessagesController",
                    controllerAs: "messagesCtrl"
                }
            },
            data: {
                title: 'Messages'
            },
            resolve: {
                messagesList: function(messagesService){
                    return messagesService.getMessages();
                }
            }
        })
        .state('app.affiliateAdmin.createMessages', {
            url: '/AffiliateAdmin/CreateMessages',
            views:{
                "content@app":{
                    templateUrl: 'app/affiliateAdmin/messaging/messages/views/createMessage.html',
                    controller: 'CreateMessageController',
                    controllerAs: 'createMessageCtrl'
                }
            },
            data: {
                title: 'Create Message'
            },
            resolve: {
                message: function(messagesService){
                    return messagesService.editMessage();
                },
                srcipts: function(lazyScript) {
                    return lazyScript.register([
                        'summernote'
                    ])
                }
            }
        })
        .state('app.affiliateAdmin.editMessage',{
            views: {
                "content@app":{
                    templateUrl: 'app/affiliateAdmin/messaging/messages/views/editMessage.html',
                    controller: 'EditMessageController',
                    controllerAs: 'editMessageCtrl'
                }
            },
            data:{
                title: 'Edit Message'
            },
            resolve: {
                srcipts: function(lazyScript) {
                    return lazyScript.register([
                        'summernote'
                    ])
                }
            },
            params:{
                model: null
            }
        })
});