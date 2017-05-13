"use strict";

angular.module('app.affiliateAdmin')
.config(function($stateProvider){
    $stateProvider
        .state('app.affiliateAdmin.sendmail', {
            url: '/AffiliateAdmin/SendMail',
            views:{
                "content@app":{
                    templateUrl: 'app/affiliateAdmin/messaging/sendmail/sendmail.html',
                    controller: 'MailController',
                    controllerAs: 'mailCtrl'
                }
            },
            data: {
                title: 'Send Mail'
            },
            resolve:{
                model: function(messagesService){
                    return messagesService.getMail();
                },
                srcipts: function(lazyScript){
                    return lazyScript.register([
                        'bootstrap-duallistbox'
                    ])

                }
            },
            params:{
                Id: null
            }
        })
});