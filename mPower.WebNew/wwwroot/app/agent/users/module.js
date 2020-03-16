"use strict";

angular.module('app.agentUser')
    .config(function($stateProvider) {
        $stateProvider
            .state('app.agentUser.users', {
                url: '/Agent',
                views: {
                    "content@app":{
                        templateUrl: 'app/agent/users/users.html',
                        controller: 'UsersAgentController',
                        controllerAs: 'usersAgentCtrl'
                    }
                },
                data:{
                    title:'Users'
                },
                resolve:{
                    usersList: function (usersAgentService) {
                        return usersAgentService.getUsers();
                    },
                    scripts: function(lazyScript) {
                        return lazyScript.register([
                            'bootstrap-validator'
                        ])
                    }
                }
            });
    });
