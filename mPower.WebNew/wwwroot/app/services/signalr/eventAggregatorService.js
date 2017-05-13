angular.module('app.infrastructure', [])
    .factory('eventsAggregatorService', ['$rootScope', '$window', function ($rootScope, $window) {

        return {

            subscriptions: [],

            publish: function (name, data) {
                $rootScope.$emit(name, data);
            },

            subscribe: function (name, callback) {
                var self = this,
                    _callback = function () {
                        var args = arguments;

                        if (typeof ($rootScope.safeApply) === 'function') {
                            $rootScope.safeApply(function () {
                                callback.apply(null, args);
                            });
                        } else {
                            callback.apply(null, args);
                        }

                    };

                if (!$window._.find(self.subscriptions, function (val) { return val.event === name })) {
                    self.subscriptions.push({
                        event: name,
                        callback: $rootScope.$on(name, _callback),
                        originalCallback: callback // to recognize callback while removing
                    });
                }
            },

            unsubscribe: function (name, callback, isAll) {
                var self = this,
                    isFound = false;
                var removeSubscription = function (subscription, index) {
                    subscription.callback();
                    self.subscriptions.splice(index, 1);
                    return true;
                }

                for (var i = 0; i < this.subscriptions.length; i++) {
                    var item = this.subscriptions[i];
                    if (callback) {
                        if (item.originalCallback == callback && item.event == name) {
                            isFound = removeSubscription(item, i);
                            break;
                        }
                    } else if (item.event == name) {
                        isFound = removeSubscription(item, i);
                        break;
                    }
                }

                if (isAll === undefined || isAll) {
                    isFound && this.unsubscribe(name, callback, isAll);
                }
            }
        };
    }]);
