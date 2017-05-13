"use strict";

angular.module('app.globalAdmin')
    .config(function($stateProvider) {
        $stateProvider
            .state('app.globalAdmin.users', {
                url: '/GlobalAdmin',
                views: {
                    "content@app":{
                        templateUrl: 'app/globalAdmin/users/users.html',
                        controller: 'UsersGlobalController',
                        controllerAs: 'usersGlobalCtrl'
                    }
                },
                data:{
                    title:'Users'
                },
                resolve:{
                    usersList: function(usersGlobalService){
                        return usersGlobalService.getUsers();
                    },
                    scripts: function(lazyScript) {
                        return lazyScript.register([
                            'bootstrap-validator'
                        ])
                    }
                }
            });
    });
