'use strict';

angular.module('app.personal').service('profileService', ['http', function(http) {
    this.getMembership = function() {
        var url = '/profile/membership';
        return http.get(url);
    };

    this.cancelSubscription = function(id) {
        var url = '/profile/membership/cancel';
        return http.post(url, id);
    };

    this.getProfileModel = function() {
        var url = '/profile/model';
        return http.get(url);
    };

    this.saveUserDetails = function(model) {
        var url = '/profile/save/userdetails';
        return http.post(url, model);
    };

    this.changePassword = function(model) {
        var url = '/profile/change/password';
        return http.post(url, model);
    };

    this.saveSecurity = function(model) {
        var url = '/profile/save/security';
        return http.post(url, model);
    };

    this.saveSequrityQuestion = function(model) {
        var url = '/profile/save/security/questions';
        return http.post(url, model);
    };

    this.saveSequritySettings = function(model) {
        var url = '/profile/save/security/settings';
        return http.post(url, model);
    };

    this.getAllAlerts = function() {
        var url = '/profile/alerts';
        return http.get(url);
    };

    this.changeAlerts = function(model) {
        var url = '/profile/alerts/settings/' + http.buildQueryString(model);
        return http.post(url);
    };

    this.deletePhone = function(phone) {
        var url = '/profile/phones/' + phone;
        return http.delete(url);
    };

    this.deleteEmail = function (email) {
        var url = '/profile/emails/' + email;
        return http.delete(url);
    };

    this.savePhone = function(phone) {
        var url = '/profile/phones/add/' + http.buildQueryString({ phone : phone });
        return http.post(url);
    };

    this.saveEmail = function (email) {
        var url = '/profile/emails/add/' + http.buildQueryString({ email : email });
        return http.post(url);
    };
}]);