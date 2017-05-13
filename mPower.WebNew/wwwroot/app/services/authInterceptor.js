"use strict";

angular.module('app').factory('AuthInterceptor', [
    '$q', '$location', 'localStorageService', '$rootScope', function ($q, $location, localStorageService, $rootScope) {
        return{
            request: function(config) {

                config.headers = config.headers || {};

                var authData = localStorageService.get('authorizationData');
                if (authData) {
                    config.headers.Authorization = 'Bearer ' + authData.token;
                }
                if($rootScope.businessId){
                    config.headers.LedgerId = $rootScope.businessId;
                }
                else {
                    var settings = localStorageService.get('settings');
                    config.headers.LedgerId = settings ? settings.personalLedgerId : null;
                }

                return config;
            }
        }
    }
]);