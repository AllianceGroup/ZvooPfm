'use strict';

angular.module('app.personal').service('chargifyService', ['http', function(http) {
    this.subscribeUser = function(id) {
        var url = '/chargify/subscribeuser/' + http.buildQueryString({ id });
        return http.post(url);
    };
}]);