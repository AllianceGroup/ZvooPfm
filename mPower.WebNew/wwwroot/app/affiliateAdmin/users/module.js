"use strict";

angular.module('app.affiliateAdmin')
.config(function($stateProvider) {
    $stateProvider
        .state('app.affiliateAdmin.users', {
            url: '/AffiliateAdmin/Users',
            views: {
                "content@app":{
                    templateUrl: 'app/affiliateAdmin/users/users.html',
                    controller: "UsersController",
                    controllerAs: "usersCtrl"
                }
            },
            data:{
                title:'Users'
            },
            resolve:{
                usersList: function(usersService){
                    return usersService.getUsers();
                },
                scripts: function(lazyScript) {
                    return lazyScript.register([
                        'bootstrap-validator'
                    ])
                }
            }
        });
});