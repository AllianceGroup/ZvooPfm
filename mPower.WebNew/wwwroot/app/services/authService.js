"use strict";

angular.module('app').factory('authService', ['localStorageService','http', '$q', function (localStorageService, http, $q) {

    var _authentication = {
        isAuth: false,
        userName: "",
        claims: {}
    };

    var _fillAuthData = function () {
        var authData = localStorageService.get('authorizationData');
        if (authData) {
            _authentication.isAuth = true;
            _authentication.userName = authData.userName;
            _authentication.claims = authData.claims;
        }
    };

    var _getRoles = function(){
        if(_authentication.claims.role){
            var roles = {};
            for(var i = 0; i < _authentication.claims.role.length; i++){
                if(_authentication.claims.role[i] == '10'){
                    roles.AffiliateAdminView = true;
                    continue;
                }
                if(_authentication.claims.role[i] == '11'){
                    roles.AffiliateAdminEdit = true;
                    continue;
                }
                if(_authentication.claims.role[i] == '12'){
                    roles.AffiliateAdminDelete = true;
                    continue;
                }
                if(_authentication.claims.role[i] == '13'){
                    roles.GlobalAdminView = true;
                    continue;
                }
                if(_authentication.claims.role[i] == '14'){
                    roles.GlobalAdminEdit = true;
                    continue;
                }
                if(_authentication.claims.role[i] == '15')
                    roles.GlobalAdminDelete = true;
            }
            return roles;
        }
    };

    var _getClaims = function (token) {
        var parts = token.split('.');
        if (parts[1]) {
            return JSON.parse(atob(parts[1]));
        }
        return {};
    };

    var _logOut = function () {
        localStorageService.remove('authorizationData');
        localStorageService.remove('settings');
        _authentication.isAuth = false;
        _authentication.userName = "";
        _authentication.claims = {};
    };

    var _saveRegistration = function (model) {
        _logOut();
        var url = '/authentication/register';
        return http.post(url, model);
    };

    var _login = function (loginData) {
        var url = '/authentication/login';
        return http.post(url, loginData).then(function(data) {
            if (data.authenticated) {
                var claims = _getClaims(data.token);
                localStorageService.set('authorizationData', { token: data.token, claims: claims, userName: loginData.Email });
                localStorageService.set('settings', { hasAccounts: data.hasAccounts, personalLedgerId: data.ledgerId });
                _authentication.isAuth = true;
                _authentication.userName = loginData.userName;
                _authentication.claims = claims;
                _getRoles();
            }
            return data;
        }, function(errors) {
            _logOut();
            return $q.reject(errors);
        });
    };

    var _forgotPassword = function (email) {
        _logOut();
        var url = '/authentication/forgotPassword' + http.buildQueryString({ email: email });
        return http.post(url);
    };

    var _resetPassword = function(model){
        _logOut();
        var url = '/authentication/resetPassword';
        return http.post(url, model);
    };

    var _getProductHandle = function () {
        var url = '/authentication/producthandle';
        return http.get(url);
    };

    return{
        saveRegistration: _saveRegistration,
        login: _login,
        logOut: _logOut,
        fillAuthData: _fillAuthData,
        authentication: _authentication,
        getProductHandle: _getProductHandle,
        getRoles: _getRoles,
        getClaims: _getClaims,
        forgotPassword: _forgotPassword,
        resetPassword: _resetPassword
    };
}]);