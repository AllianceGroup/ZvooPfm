'use strict';

angular.module('app.affiliateAdmin').service('usersService',['http', function(http){
    this.getUsers = function(){
        var url = "/Affiliate/GetUsers";
        return http.get(url);
    };

    this.toggleUserIsActive = function(id){
        var url = "/Affiliate/toggleUserIsActive/" + id;
        return http.get(url);
    };

    this.editUser = function(id){
        var url = "/Affiliate/profile/" + id;
        return http.get(url);
    };

    this.activity = function(model){
        var url = "/Affiliate/activity" + http.buildQueryString({ PageNumber: model.pageNumber, Id: model.id});
        return http.get(url);
    };

    this.refreshUsers = function(model){
        var url = "/Affiliate/refreshUsers" + http.buildQueryString({ PageNumber: model.PageNumber, SearchKey: model.SearchKey});
        return http.get(url)
    };

    this.addUser = function(user){
        var url = "/Affiliate/addUser";
        return http.post(url, user);
    };

    this.updateUser = function(user){
        var url = "/Affiliate/profile";
        return http.post(url, user);
    };

    this.userInfo = function(id){
        var url = "/Affiliate/userInfo/" + id;
        return http.get(url);
    };

    this.deleteUser = function(id){
        var url = "/Affiliate/deleteUser/" + id;
        return http.delete(url);
    };

    this.exportCSV = function(){
        var url = "/Affiliate/exportUsersToCsv";
        return http.get(url);
    };
}]);