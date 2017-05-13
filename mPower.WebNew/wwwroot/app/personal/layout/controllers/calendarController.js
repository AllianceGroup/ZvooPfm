'use strict';

angular.module('app.personal').controller('CalendarController',
['calendarService', 'defaultCalendar', '_', '$uibModal', function (calendarService, defaultCalendar, _, $uibModal) {

    var ctrl = this;
    var trancateString = function(title) {
        return jQuery.trim(title).substring(0, 10)
            .split(" ").slice(0, -1).join(" ") + "...";
    };

    ctrl.IsCalendarView = false;
    ctrl.collapseCalendar = false;
    ctrl.fullViewCalendar = false;
    ctrl.newCalendar = {};
    ctrl.newEvent = {};
    ctrl.IsEventView = false; 
    ctrl.events = [];
    ctrl.formatedEvents = [];
    ctrl.errors = [];
    ctrl.calendar = defaultCalendar;
    ctrl.filter = {
        CalendarId: ctrl.calendar.SelectedCalendarId,
        Date: new Date().toISOString()
    };

    ctrl.currentCalendarDate = ctrl.filter.Date;

    ctrl.toggleCalendar = function () {
        ctrl.collapseCalendar = !ctrl.collapseCalendar;
    };
    ctrl.toggleFullCalendar = function () {
        ctrl.fullViewCalendar = !ctrl.fullViewCalendar;
    };

    ctrl.updateEvents = function() {
        calendarService.getEventsByFilter(ctrl.filter).then(function (events) {
            ctrl.events = events;
        });
    };

    ctrl.updateCalendarEvents = function (date) {
        ctrl.currentCalendarDate = date;

        var filter = {
            Date: new Date(date).toISOString(),
            TimeSpanFilter: 'ByQuarter',
            CalendarId: ctrl.filter.CalendarId
        };

        calendarService.getEventsByFilter(filter).then(function (events) {
            var grouppedEvents = _.groupBy(events,
                function (obj) { return new Date(obj.Date).setHours(0, 0, 0, 0); });

            ctrl.formatedEvents = _.map(Object.keys(grouppedEvents), function (obj) {
                return {
                    start: moment(new Date(parseInt(obj))).format('YYYY-MM-DD'),
                    title: ""
                }
            });
        });
    };

    ctrl.updateByDate = function(date) {
        ctrl.filter.Date = date.toISOString();
        ctrl.updateEvents();
    };

    ctrl.showAddCalendarView = function() {
        ctrl.IsCalendarView = true;
        ctrl.newCalendar = {};
    };

    ctrl.showAddEventView = function () {
        ctrl.IsEventView = true;
        ctrl.newEvent = {};
        calendarService.getAddEventModel().then(function(model) {
            ctrl.newEvent = model;
            ctrl.newEvent.Type = 'Onetime';
            ctrl.newEvent.CalendarId = ctrl.newEvent.Calendars[0].Value;
            ctrl.newEvent.SendAlertOptions.Mode = ctrl.newEvent.SendAlertOptions.Modes[0].Value;
            ctrl.newEvent.Frequency = ctrl.newEvent.Frequencies[0].Text;
            ctrl.newEvent.Repeat = ctrl.newEvent.RepeatList[0].Text;
            ctrl.newEvent.EndEventRepeating = 'Never';
        });
    };

    ctrl.backToDefault = function() {
        ctrl.IsCalendarView = false;
        ctrl.IsEventView = false;
        ctrl.errors = [];
    }

    ctrl.saveCalendar = function () {
        ctrl.errors = [];
        calendarService.addCalendar(ctrl.newCalendar).then(function (calendars) {
            ctrl.calendar.Calendars = calendars;
            ctrl.filter.CalendarId = _.find(calendars, function(val) { return val.Selected === true; }).Value;
            ctrl.backToDefault();
            ctrl.updateEvents();
            if (moment(ctrl.filter.Date).diff(moment(ctrl.currentCalendarDate), 'months', true) < 2) {
                ctrl.updateCalendarEvents(ctrl.filter.Date);
            }
        }, function(errors) {
            for (var key in errors) {
                if (errors.hasOwnProperty(key)) {
                    for (var i = 0; i < errors[key].length; i++) {
                        ctrl.errors.push(errors[key][i]);
                    }
                }
            }
        });
    };

    ctrl.saveEvent = function () {
        ctrl.errors = [];
        calendarService.addEvent({
            CalendarId: ctrl.newEvent.CalendarId,
            Description: ctrl.newEvent.Description,
            Type: ctrl.newEvent.Type,
            Date: ctrl.newEvent.Date,
            Frequency: ctrl.newEvent.Frequency,
            Repeat: ctrl.newEvent.Repeat,
            StartDate: ctrl.newEvent.StartDate,
            Days: ctrl.newEvent.Days,
            DayAsPartOf: ctrl.newEvent.DayAsPartOf,
            EndAfter: ctrl.newEvent.EndAfter,
            EndDate: ctrl.newEvent.EndDate,
            SendAlertOptions: {
                Count: ctrl.newEvent.SendAlertOptions.Count,
                Mode: ctrl.newEvent.SendAlertOptions.Mode
            },
            UserId: ctrl.newEvent.UserId
        }).then(function () {
            ctrl.backToDefault();
            ctrl.updateEvents();
            if (moment(ctrl.filter.Date).diff(moment(ctrl.currentCalendarDate), 'months', true) < 2) {
                ctrl.updateCalendarEvents(ctrl.filter.Date);
            }
        }, function (errors) {
            for (var key in errors) {
                if (errors.hasOwnProperty(key)) {
                    for (var i = 0; i < errors[key].length; i++) {
                        ctrl.errors.push(errors[key][i]);
                    }
                }
            }
        });
    };

    ctrl.deleteEvent = function (event) {
        var modalInstance = $uibModal.open({
            templateUrl: 'app/personal/layout/partials/deleteEvent.tpl.html',
            controller: 'DeleteEventController',
            controllerAs: 'ctrl'
        });
        modalInstance.result.then(function (model) {
            calendarService.deleteEvent({
                Calendarid: event.CalendarId,
                EventId: event.Id,
                ParentId: event.ParentId,
                Type: model.type
            }).then(function () {
                ctrl.updateEvents();
                if (moment(ctrl.filter.Date).diff(moment(ctrl.currentCalendarDate), 'months', true) < 2) {
                    ctrl.updateCalendarEvents(ctrl.filter.Date);
                }
            });
        });

    };

    ctrl.changeEventStatus = function (event) {
        var sendFunction = event.DeleteAction === 'DeleteRepeatingEvent' ? calendarService.completeRepeatingEvent : calendarService.changeEventStatus;
        sendFunction({
            calendarid: event.CalendarId,
            eventId: event.Id,
            isDone: event.IsDone
        }).then(function() {});
    };

    //initialize
    ctrl.updateEvents();
    ctrl.updateCalendarEvents(ctrl.filter.Date);
}]);