'use strict';

angular.module('app.personal').service('budgetService', ['http', function(http) {
    this.getAllBudgets = function(){
        var url = '/budget/GetAllTenBudgets';
        return http.get(url);
    };

    this.Show = function(month, year){
        var url = "/budget/Show/";
        return http.get(url + month + '/' + year);
    };

    this.Update = function(model) {
        var url = '/budget/UpdateBudget';
        return http.post(url, model);
    };

    this.Create = function(model){
        var url = "/budget/CreateBudget";
        return http.post(url, model);
        console.log(model);
    };

    this.Add = function(model){
        var url = "/budget/AddBudget";
        return http.post(url, model);
    };

    this.Delete = function(id){
        var url = '/accounts/delete/' + id;
        return http.get(url);
    }
}]);