"use strict";

angular.module('app.affiliateAdmin')
.config(function($stateProvider){
    $stateProvider
        .state('app.affiliateAdmin.account', {
            url: '/AffiliateAdmin/Marketing/Profile',
            views: {
                "content@app":{
                    templateUrl: 'app/affiliateAdmin/marketing/account/profile/profile.html'
                }
            },
            data: {
                title: 'Profile'
            }
        })
});