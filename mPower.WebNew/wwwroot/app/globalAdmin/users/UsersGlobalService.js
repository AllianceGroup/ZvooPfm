'use strict';

angular.module('app.globalAdmin').service('usersGlobalService', ['http', 'localStorageService', 'authService', function(http, localStorageService, authService){

    this.getUsers = function(){
        var url = '/Global/GetUsers';
        return http.get(url);
    };

    this.getUser = function(){
        var url = '/Global/addUser';
        return http.get(url);
    };

    this.addUser = function(user){
        var url = '/Global/addUser';
        return http.post(url, user);
    };

    this.getProfile = function(id){
        var url = '/Global/getProfile/' + id;
        return http.get(url);
    };

    this.updateProfile = function(user){
        var url = '/Global/updateProfile';
        return http.post(url, user);
    };

    this.refreshUsers = function(model){
        var url = '/Global/refreshUsers' +
            http.buildQueryString({PageNumber: model.PageNumber, SearchKey: model.SearchKey, Affiliate: model.Affiliate});
        return http.get(url);
    };

    this.getUserDetails = function(id){
        var url = '/Global/getUserDetails/' + id;
        return http.get(url);
    };

    this.deleteUser = function(id){
        var url = '/Global/deleteUser/' + id;
        return http.delete(url);
    };

    this.activateUser = function(id){
        var url = '/Global/activate/' + id;
        return http.get(url);
    };

    this.deactivateUser = function(id){
        var url = '/Global/deactivate/' + id;
        return http.get(url);
    };

    this.toggleIdGuardEnrollment = function(id){
        var url = '/Global/toggleIdGuardEnrollment/' + id;
        return http.get(url);
    };

    this.bulkDelete = function(ids){
        var url = '/Global/bulkDeleteUser' + http.buildQueryString({ids: ids});
        return http.get(url);
    };

    this.bulkDeactivate = function(ids){
        var url = '/Global/bulkDeactivateUser' + http.buildQueryString({ids: ids});
        return http.get(url);
    };

    this.bulkActivate = function(ids){
        var url = '/Global/bulkActivateUser' + http.buildQueryString({ids: ids});
        return http.get(url);
    };

    this.exportCSV = function(){
        var url = "/Global/exportUsersToCsv";
        return http.get(url);
    };

    this.loginAsUser = function(id, Email){
        var url = '/authentication/loginAsUser/' + id;
        return http.get(url).then(function(data){
            if(data.authenticated){
                authService.logOut();
                var claims = authService.getClaims(data.token);
                authService.authentication.isAuth = true;
                authService.authentication.claims = claims;
                localStorageService.set('authorizationData', { token: data.token, claims: claims, userName: Email });
                localStorageService.set('settings', { hasAccounts: data.hasAccounts, personalLedgerId: data.ledgerId });
            }
            return data;
        });
    };
}]);