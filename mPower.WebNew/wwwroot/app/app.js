'use strict';

/**
 * @ngdoc overview
 * @name app [smartadminApp]
 * @description
 * # app [smartadminApp]
 *
 * Main module of the application.
 */

angular.module('app', [
        'ui.router',
        'ngAnimate',
        'ngSanitize',
        'restangular',
        'ui.bootstrap',
        'angular.morris-chart',
        'nya.bootstrap.select',
        'ui.bootstrap-slider',
        'checklist-model',
        'LocalStorageModule',
        'ngProgress',
        'ngFileSaver',

        // Smartadmin Angular Common Module
        'SmartAdmin',
        //'app.calendar',
        'app.config',
        'app.infrastructure',
        'app.auth',
        'app.personal',
        'app.business',
        'app.layout',
        'app.affiliateAdmin',
        'app.globalAdmin',
        'app.business.select'
    ])
    .config(function ($provide, $httpProvider, $locationProvider) {
        $httpProvider.interceptors.push('AuthInterceptor');

        $locationProvider.html5Mode(true);
    })
    .run(function ($rootScope, $state, $stateParams, ngProgressFactory, authService) {
        $rootScope.$state = $state;
        $rootScope.$stateParams = $stateParams;
        $rootScope.porgressbar = ngProgressFactory.createInstance();
        authService.fillAuthData();

        $rootScope.$on("$routeChangeStart", function () {
            $rootScope.progressbar.start();
        });

        $rootScope.$on("$routeChangeSuccess", function () {
            $rootScope.progressbar.complete();
        });

        $rootScope.mobileMenu = false;
        $rootScope.toggleMobileMenu = function () {
            $rootScope.mobileMenu = !$rootScope.mobileMenu;
        };

        $rootScope.utils = {
            getObjectKeys: Object.keys
        }

    });


