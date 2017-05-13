'use strict';

angular.module('app.business.select', ['ui.router'])
    .config(function ($stateProvider){
        $stateProvider
            .state('business',{
                url: '/Business',
                views: {
                    root:{
                        templateUrl: 'app/business/selectBusiness/business.html',
                        controller: 'BusinessController',
                        controllerAs: 'businessCtrl'
                    }
                },
                resolve:{
                    listBusiness: function(businessService){
                        return businessService.getListBusiness();
                    }
                }
            })
    });