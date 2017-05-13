'use strict';

angular.module('app.business').service('businessService', ['http',
    function(http){
        this.getListBusiness = function(){
            var url = '/start';
            return http.get(url);
        };

        this.getBusiness = function(){
            var url = '/start/addLedger';
            return http.get(url);
        };

        this.addBusiness = function(model){
            var url = '/start/addLedger';
            return http.post(url, model);
        };
    }]);