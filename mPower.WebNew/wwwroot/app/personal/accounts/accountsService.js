'use strict';

angular.module('app.personal').service('accountsService', ['http', function (http) {

    this.getAccounts = function(){
        var url = '/accounts/getAccounts';
        return http.get(url);
    };

    this.getShortAccounts = function () {
        var url = '/accounts';
        return http.get(url);
    };

    this.getEditModel = function() {
        var url = '/accounts/add';
        return http.get(url);
    };

    this.addManually = function(model) {
        var url = '/accounts/add';
        return http.post(url, model);
    };

    this.getAvailableForGoals = function() {
        var url = '/accounts/get/goals';
        return http.get(url);
    };

    this.getFinancialAccounts = function() {
        var url = '/accounts/get/financial';
        return http.get(url);
    };

    this.getEditAccount = function(id){
        var url = '/accounts/edit/' + id;
        return http.get(url);
    };

    this.Save = function(model){
        var url = '/accounts/edit';
        return http.post(url, model);
    };

    this.ConfirmDelete = function(id){
        var url = '/accounts/confirmDelete/' + id;
        return http.get(url);
    };

    this.Delete = function(id){
        var url = '/accounts/delete/' + id;
        return http.get(url);
    };

    this.getParents = function(accountLabel, id) {
        var url = '/accounts/parents/' + http.buildQueryString({ accountLabel: accountLabel, id: id });
        return http.get(url);
    }
}]);