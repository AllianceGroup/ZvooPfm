"use strict";

angular.module('app.affiliateAdmin', ['ui.router'])
.config(function ($stateProvider){
    $stateProvider
        .state('app.affiliateAdmin', {
            views:{
                "navigation@app": {
                    templateUrl: 'app/affiliateAdmin/layout/partials/navigation.html'
                }
            }
        })
});