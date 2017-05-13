"use strict";

angular.module('app.globalAdmin', ['ui.router'])
    .config(function ($stateProvider){
        $stateProvider
            .state('app.globalAdmin', {
                views:{
                    "navigation@app": {
                        templateUrl: 'app/globalAdmin/layout/navigation.html'
                    }
                }
            })
    });