'use strict';

angular.module('app.personal').service('realEstateService', ['http', function(http) {
    this.getAll = function() {
        var url = '/realestate';
        return http.get(url);
    };

    this.delete = function(id) {
        var url = '/realestate/' + id;
        return http.delete(url);
    };

    this.search = function(address, zip) {
        var url = '/realestate/search/' + http.buildQueryString({ address : address, zip : zip });
        return http.get(url);
    };

    this.getZillowProperty = function(id) {
        var url = '/realestate/get/zillow/' + id;
        return http.get(url);
    };

    this.saveZillowProperty = function(model) {
        var url = '/realestate/save/zillow';
        return http.post(url, model);
    };

    this.incudeInWorth = function(realEster) {
        var url = '/realestate/incudeInWorthRealEster';
        return http.post(url, realEster);
    }
}]);