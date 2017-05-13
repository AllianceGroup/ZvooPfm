'use strict';

angular.module('app.personal').service('calendarService', ['http', function (http) {
    this.getEventsByFilter = function (filter) {
        var url = '/calendar/events/filter/' + http.buildQueryString(filter);
        return http.get(url);
    };

    this.getDefaultCalendar = function() {
        var url = '/calendar/';
        return http.get(url);
    }

    this.addCalendar = function(model) {
        var url = '/calendar/add';
        return http.post(url, model);
    };

    this.getAddEventModel = function() {
        var url = '/calendar/events/addmodel';
        return http.get(url);
    };

    this.addEvent = function (model) {
        var url = '/calendar/events/add';
        return http.post(url, model);
    };

    this.deleteEvent = function(filter) {
        var url = '/calendar/events/delete/' + http.buildQueryString(filter);
        return http.delete(url);
    };

    this.changeEventStatus = function(model) {
        var url = '/calendar/events/mark/' + http.buildQueryString(model);
        return http.post(url);
    };

    this.completeRepeatingEvent = function(model) {
        var url = '/calendar/events/complete/repeating/' + http.buildQueryString(model);
        return http.post(url, model);
    };
}]);