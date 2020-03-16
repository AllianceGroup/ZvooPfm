"use strict";

angular.module('app.agentUser', ['ui.router'])
    .config(function ($stateProvider){
        $stateProvider
            .state('app.agent', {
                views:{
                    "navigation@app": {
                        templateUrl: 'app/agent/layout/navigation.html'
                    }
                }
            })
    });