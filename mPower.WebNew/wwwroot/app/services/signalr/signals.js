angular.module("app.infrastructure")
    .service('signalsService', ['$q', 'eventsAggregatorService', 'webapiUrl', 'http', function ($q, eventAggregatorService, webapiUrl, http) {

        var hubs = {
                ledgerHub: null
            },
            isEstablished = false;
        var connectionId;

        var connect = function (id) {
            connectionId = id;
            return http.post('/hubs/connect/' + connectionId);
        };

        var disconnect = function () {
            return http.post('/hubs/disconnect/' + connectionId).then(function () {
                connectionId = null;
                return $q.when($.connection.hub.stop());
            });
        };

        var initialize = function () {
            $.connection.hub.url = webapiUrl + 'signalr/hubs';
            $.connection.logging = true;
           
            hubs.ledgerHub = $.connection.ledgerHub;

            hubs.ledgerHub.client.accountUpdated = function (obj) {
                eventAggregatorService.publish('accountUpdated', obj);
            };
            hubs.ledgerHub.client.accountAdded = function(obj){
                eventAggregatorService.publish('accountAdded', obj);
            };

            return $q.when($.connection.hub.start()).then(function(data) {
                isEstablished = true;
                return connect(data.id);
            }, function(data) {
                console.log("Error while connecting SignalR " + data.message);
            }).then(function(data) {
                console.log("SignalR is connected");
            });
        };

        var invoke = function (hubName, method, data) {
            if (isEstablished) {

                if (data && Array.isArray(data)) {
                    return hubs[hubName].server[method].apply(this, data);
                } else if (data) {
                    return hubs[hubName].server[method](data);
                } else {
                    return hubs[hubName].server[method]();
                }

            } else {
                return $q.reject({ message: 'Connection is not established yet.' });
            }
        };

        return {
            initialize: initialize,
            invoke: invoke,
            connect: connect,
            disconnect: disconnect
        };
    }]);