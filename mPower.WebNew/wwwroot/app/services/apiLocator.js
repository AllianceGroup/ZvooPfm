"use strict";

angular.module('app').service('apiLocator', ['webapiUrl',
    function(webapiUrl) {
        this.assetsBaseUrl = function() {
            return webapiUrl + 'api';
        }
    }
]);
