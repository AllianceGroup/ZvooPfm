'use strict';

angular.module('app.personal').service('intuitService', ['http', function (http) {

    this.getFinancialInstitutions = function (searchName) {
        var url = '/aggregation/search/' + searchName;
        return http.get(url);
    };

    this.authenticate = function(id) {
        var url = '/aggregation/authenticate/' + id;
        return http.post(url);
    };

    this.authenticateToBank = function(model) {
        var url = '/aggregation/authenticateToBank';
        return http.post(url, model);
    };

    this.aligntoledger = function(model) {
        var url = '/aggregation/aligntoledger';
        return http.post(url, model);
    }

    this.aggregateUser = function() {
        var url = '/aggregation/aggregateuser';
        return http.post(url);
    }

    this.reathenticateGetLogonForm = function (intuitInstitutionId, intuitAccountId) {
        var url = '/aggregation/reathenticateGetLogonForm/' + http.buildQueryString({ intuitInstitutionId: intuitInstitutionId, intuitAccountId: intuitAccountId });
        return http.post(url);
    };

    this.reauthentication = function (model) {
        var url = '/aggregation/reauthentication';
        return http.post(url, model);
    }

    this.interactiveRefresh = function (intuitAccountId) {
        var url = '/aggregation/interactiveRefresh/' + intuitAccountId;
        return http.post(url);
    }

    this.interactiveRefreshMfa = function(model) {
        var url = '/aggregation/interactiveRefreshMfa';
        return http.post(url, model);
    }
}]);