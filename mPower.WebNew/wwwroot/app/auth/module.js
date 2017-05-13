"use strict";

angular.module('app.auth', ['ui.router'])
    .config(function($stateProvider) {
        $stateProvider
            .state('login', {
                url: "/login?subscriptionId",
                views: {
                    root: {
                        templateUrl: 'app/auth/login/login.html',
                        controller: 'LoginController',
                        controllerAs: 'loginCtrl'
                    }
                },
                resolve: {
                    chargifyModel: function ($stateParams, chargifyService, $q) {
                        if ($stateParams.subscriptionId) {
                            var deferred = $q.defer();
                            chargifyService.subscribeUser($stateParams.subscriptionId).then(function(message) {
                                deferred.resolve({ message:message });
                            }, function(errors) {
                                deferred.resolve({ errors:errors });
                            });
                            return deferred.promise;
                        } else {
                            return {};
                        }
                    }
                }
            })
            .state('signup', {
                url: "/signup",
                views: {
                    root: {
                        templateUrl: 'app/auth/signup/signup.html',
                        controller: 'SignUpController',
                        controllerAs: 'signUpCtrl'
                    }
                },
                resolve: {
                    productHandle: function (authService) {
                        return authService.getProductHandle();
                    }
                }
            })
            .state('forgotPassword', {
                url: "/forgotPassword",
                views: {
                    root: {
                        templateUrl: 'app/auth/forgotPassword/forgot-password.html',
                        controller: 'forgotPasswordController',
                        controllerAs: 'forgotCtrl'
                    }
                }
            })
            .state('resetPassword', {
                url: "/resetPassword?token",
                views:{
                    root: {
                        templateUrl: 'app/auth/forgotPassword/reset-password.html',
                        controller: 'resetPasswordController',
                        controllerAs: 'resetCtrl'
                    }
                }
            })
    });
