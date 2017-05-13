angular.module('app.personal').service('goalsService', ['http', function (http) {
    this.getAll = function (filter) {
        var url = '/goals/all/' + http.buildQueryString(filter);
        return http.get(url);
    };

    this.getById = function(id) {
        var url = '/goals/get/' + id;
        return http.get(url);
    };

    this.setupLinkedAccount = function(id) {
        var url = '/goals/setup/linkedaccount/' + http.buildQueryString({ linkedAccountId: id });
        return http.post(url);
    };

    this.adjustAmount = function(goalId, adjustment) {
        var url = '/goals/adjustamount/' + http.buildQueryString({ goalId: goalId, adjustment: adjustment });
        return http.post(url);
    };

    this.createGoal = function(model) {
        var url = '/goals/create';
        return http.post(url, model);
    };

    this.edit = function(goalId) {
        var url = '/goals/editmodel/' + goalId;
        return http.get(url);
    };

    this.delete = function(goalId) {
        var url = '/goals/delete/' + goalId;
        return http.delete(url);
    };

    this.complete = function(goalId) {
        var url = '/goals/complete/' + goalId;
        return http.post(url);
    }

    this.archive = function(goalId) {
        var url = '/goals/archive/' + goalId;
        return http.post(url);
    }
}]);