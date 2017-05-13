'use strict';

angular.module('app.personal').service('transactionsService', ['http', function(http) {
    this.getByFilter = function(filter) {
        var url = '/transactions/filter/' + http.buildQueryString(filter);
        return http.get(url);
    };

    this.assignToAccount = function(transactionId, accountId, newAccountId) {
        var url = '/transactions/assign';
        return http.post(url, {
            'TransactionId': transactionId,
            'PreviousAccountId': accountId,
            'NewAccountId': newAccountId
        });
    };

    this.deleteMultiple = function(transactionIds) {
        var url = '/transactions/deletemultiple';
        return http.post(url, {
            TransactionIds: transactionIds
        });
    };

    this.editMultiple = function(model) {
        var url = '/transactions/editmultiple';
        return http.post(url, model);
    };

    this.getEditModel = function (transactionId) {
        var url = '/transactions/edit/' + transactionId;
        return http.get(url);
    };

    this.edit = function(model) {
        var url = '/transactions/edit';
        return http.post(url, model);
    };

    this.delete = function(transactionId) {
        var url = '/transactions/delete/' + transactionId;
        return http.delete(url);
    };

    this.getAddModel = function(type, accountId) {
        var url = '/transactions/add' + http.buildQueryString({ type: type, accountId: accountId });
        return http.get(url);
    }

    this.add = function(model) {
        var url = '/transactions/add';
        return http.post(url, model);
    };


}]);