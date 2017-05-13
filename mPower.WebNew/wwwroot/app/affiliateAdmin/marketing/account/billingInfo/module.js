"use strict";

angular.module('app.affiliateAdmin')
.config(function($stateProvider){
    $stateProvider
        .state('app.affiliateAdmin.billingInfo', {
            url: '/AffiliateAdmin/Marketing/BillingInfo',
            views: {
                "content@app":{
                    templateUrl: 'app/affiliateAdmin/marketing/account/billingInfo/billingInfo.html'
                }
            },
            data: {
                title: 'Billing Information'
            }
        })
});