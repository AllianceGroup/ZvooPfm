"use strict";

angular.module('app.affiliateAdmin')
.config(function($stateProvider){
    $stateProvider
        .state('app.affiliateAdmin.triggers', {
            url: '/AffiliateAdmin/Triggers',
            views:{
                "content@app":{
                    templateUrl: 'app/affiliateAdmin/messaging/triggers/triggers.html',
                    controller: 'TriggersController',
                    controllerAs: 'triggersCtrl'
                }
            },
            data: {
                title: 'Triggers'
            },
            resolve: {
                triggersList: function(messagesService){
                    return messagesService.getTriggers();
                }
            }
        })
});