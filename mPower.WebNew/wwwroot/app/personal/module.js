"use strict";

angular.module('app.personal', [ 'ui.router','ng-currency'])
.config(function ($stateProvider) {
        $stateProvider
            .state('app.personal', {
                views: {
                    "navigation@app": {
                        templateUrl: 'app/personal/layout/partials/navigation.html'
                    },
                    "calendar@app": {
                        templateUrl: 'app/personal/layout/partials/calendar.html',
                        controller: 'CalendarController',
                        controllerAs: 'calendarCtrl'
                    }
                },
                resolve : {
                    businessId: function($rootScope){
                        if($rootScope.businessId)
                            $rootScope.businessId = null;
                    },
                    defaultCalendar : function(calendarService) {
                        return calendarService.getDefaultCalendar();
                    }
                }
            });
    });