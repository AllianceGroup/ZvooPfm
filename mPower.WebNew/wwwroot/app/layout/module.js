"use strict";


angular.module('app.layout', ['ui.router', 'ngAnimate'])

.config(function ($stateProvider, $urlRouterProvider) {
        $stateProvider
            .state('app', {
                views: {
                    root: {
                        templateUrl: 'app/layout/layout.tpl.html',
                        controller: 'AuthController',
                        controllerAs: 'authCtrl'
                    }
                },
                resolve: {
                    scripts: function(lazyScript, webapiUrl) {
                        return lazyScript.register([
                            'sparkline',
                            'easy-pie',
                            webapiUrl + 'signalr/hubs'
                        ]);
                    }
                }
            })
            //TODO replacewith page
            .state('access-denied', {
                url: '/access-denied',
                views: {
                    root: {
                        template: "<h1>Access denied<h1>"
                    }
                }
            })
            .state('not-found', {
                url: '/page-not-found',
                views: {
                    root: {
                        templateUrl: 'app/layout/partials/404.html'
                    }
                }
            });
    $urlRouterProvider.otherwise('/dashboard');

}).run(['$rootScope', '$location',function ($rootScope, $location) {
    var path = function () { return $location.path(); };
    $rootScope.$watch(path, function () {
        $rootScope.animRoute = 'anim-route';
    });
}]);

