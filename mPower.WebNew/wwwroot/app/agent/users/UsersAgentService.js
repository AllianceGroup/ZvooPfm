'use strict';

angular.module('app.agentUser').service('usersAgentService', ['http', 'localStorageService', 'authService', function (http, localStorageService, authService) {

    this.getUsers = function(){
        var url = '/Agent';
        return http.get(url);
    };

    this.getUser = function(){
        var url = '/Agent/addUser';
        return http.get(url);
    };

    this.addUser = function(user){
        var url = '/Agent/addUser';
        return http.post(url, user);
    };

    this.getProfile = function(id){
        var url = '/Agent/getProfile/' + id;
        return http.get(url);
    };

    this.updateProfile = function(user){
        var url = '/Agent/updateProfile';
        return http.post(url, user);
    };

    this.refreshUsers = function(model){
        var url = '/Agent/refreshUsers' +
            http.buildQueryString({PageNumber: model.PageNumber, SearchKey: model.SearchKey, Affiliate: model.Affiliate});
        return http.get(url);
    };

    this.getUserDetails = function(id){
        var url = '/Agent/getUserDetails/' + id;
        return http.get(url);
    };

    this.deleteUser = function(id){
        var url = '/Agent/deleteUser/' + id;
        return http.delete(url);
    };

    this.activateUser = function(id){
        var url = '/Agent/activate/' + id;
        return http.get(url);
    };

    this.deactivateUser = function(id){
        var url = '/Agent/deactivate/' + id;
        return http.get(url);
    };

    this.toggleIdGuardEnrollment = function(id){
        var url = '/Agent/toggleIdGuardEnrollment/' + id;
        return http.get(url);
    };

    this.bulkDelete = function(ids){
        var url = '/Agent/bulkDeleteUser' + http.buildQueryString({ids: ids});
        return http.get(url);
    };

    this.bulkDeactivate = function(ids){
        var url = '/Agent/bulkDeactivateUser' + http.buildQueryString({ids: ids});
        return http.get(url);
    };

    this.bulkActivate = function(ids){
        var url = '/Agent/bulkActivateUser' + http.buildQueryString({ids: ids});
        return http.get(url);
    };

    this.exportCSV = function(){
        var url = "/Agent/exportUsersToCsv";
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