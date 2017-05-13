"use strict";

angular.module('app').factory('http', ['$http', '$q', 'apiLocator', '_', '$state', '$location', function($http, $q, apiLocator, _, $state, $location) {

        function buildUrl(url) {
            return apiLocator.assetsBaseUrl() + url;
        }

        var errorCounter = 0;
        function notifyError(data, statusText) {
            console.log(data);
            $.bigBox({
                title: "Error",
                content: statusText,
                color: "#C46A69",
                icon: "fa fa-warning shake animated",
                number: ++errorCounter,
                timeout: 6000
            });
        }

        function handleErrors(response) {
            console.log("Server error: ", response.data);
            switch (response.status) {
            case 400:
                break;
            case 401:
                $state.go('login');
                break;
            case 403:
                $state.go("not-found");
                break;
            case 404:
                $state.go("not-found");
                break;
            case 503:
                // maintenance mode
                // $window.location.href = "/";
                break;
            default:
                notifyError(response.data, response.statusText);
                break;
            }
        }

        function getDefaultHeaders() {
            return { 'Content-Type': 'application/json' };
        }

        function verb(method, url, model, options) {
            var httpPromise;

            if (!options) options = {};

            var config = {
                headers: _.defaults(options.headers ? options.headers : {}, getDefaultHeaders())
            };

            switch (method) {
                case 'GET':
                    config.params = options.params ? options.params : {};
                    httpPromise = $http({ method: method, url: buildUrl(url), config: config});
                    break;
                case 'POST':
                    httpPromise = $http({ method: method, url: buildUrl(url), data: model, config: config});
                    break;
                case 'PUT':
                    httpPromise = $http({ method: method, url: buildUrl(url), data: model, config: config});
                    break;
                case 'DELETE':
                    httpPromise = $http({ method: method, url: buildUrl(url), data: model, config: config });
                    break;
            default:
                httpPromise = $q.when({});
                break;
            }

            return httpPromise.then(function(response) {
                return response.data;
            }, function(response) {
                var isProcessed = false;
                if (options && typeof (options.handleErrors) === 'function' && response.status !== 503) {
                    isProcessed = options.handleErrors(response);
                }
                if (!isProcessed) {
                    handleErrors(response);
                }
                return $q.reject(response.data);
            });
        }

        var http = {
            get: function(url, options) {
                return verb('GET', url, null, options);
            },
            post: function (url, model, options) {
                return verb('POST', url, model, options);
            },
            put: function (url, model, options) {
                return verb('PUT', url, model, options);
            },
            delete: function(url, options){
                return verb('DELETE', url, null, options);
            },

            buildQueryString: function (obj, prefix) {
                var str = [];
                for (var p in obj) {
                    if (obj.hasOwnProperty(p)) {
                        var k = prefix ? prefix + "[" + p + "]" : p,
                            v = obj[p];

                        if (angular.isObject(v)) {
                            str.push(http.buildQueryString(v, k).substr(1));
                        } else {
                            str.push(v || v === 0 ? (k) + "=" + encodeURIComponent(v) : (k) + "=");
                        }
                    }
                }
                return "?" + str.join("&");
            }
        }
        return http;
    }
]);