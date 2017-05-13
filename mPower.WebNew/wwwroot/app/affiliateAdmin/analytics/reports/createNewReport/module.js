"use strict";

angular.module('app.affiliateAdmin')
    .config(function($stateProvider){
        $stateProvider
            .state('app.affiliateAdmin.createReport', {
                url: '/AffiliateAdmin/Analytics/CreateNewReport',
                views: {
                    "content@app":{
                        templateUrl: 'app/affiliateAdmin/analytics/reports/createNewReport/createNewReport.html'
                    }
                },
                data:{
                    title: 'Create report'
                }
            })
    });