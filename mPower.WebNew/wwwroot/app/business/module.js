"use strict";

angular.module('app.business', ['ui.router'])
.config(function ($stateProvider, $urlRouterProvider){
    $stateProvider
        .state('app.business',{
            url: '/business/:businessId',
            views: {
                "navigation@app":{
                    templateUrl: 'app/business/layout/partials/navigation.html'
                },
                "calendar@app": {
                    templateUrl: 'app/personal/layout/partials/calendar.html',
                    controller: 'CalendarController',
                    controllerAs: 'calendarCtrl'
                }
            },
            resolve : {
                defaultCalendar: function (calendarService) {
                    return calendarService.getDefaultCalendar();
                },
                businessId: function($stateParams, $rootScope) {
                    $rootScope.businessId = $stateParams.businessId;
                }
            }
        });
    $urlRouterProvider.when('/business/:businessId', '/business/:businessId/dashboard')
});