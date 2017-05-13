"use strict";

angular.module('app.personal').directive('fullCalendar', function ($log, $timeout) {
    return {
        restrict: 'E',
        replace: true,
        templateUrl: 'app/directives/calendar/full-calendar.tpl.html',
        scope: {
            events: "=events",
            date: "=date",
            dateChanged: "=dateChanged",
            monthChanged: "=monthChanged"
        },
        link: function (scope, element) {


            var $calendar = $("#calendar");

            var calendar = null;


            function initCalendar() {

                // $log.log(events);


                calendar = $calendar.fullCalendar({
                    lang: 'en',
                    editable: false,
                    draggable: false,
                    selectable: true,
                    selectHelper: true,
                    unselectAuto: false,
                    disableResizing: false,
                    disableDragging: true,
                    droppable: true,

                    header: {
                        left: 'title', //,today
                        center: 'prev, next, today',
                        right: 'month, agendaWeek, agendaDay' //month, agendaDay,
                    },

                    drop: function (date, allDay) { // this function is called when something is dropped

                        // retrieve the dropped element's stored Event Object
                        var originalEventObject = $(this).data('eventObject');
            
                        // we need to copy it, so that multiple events don't have a reference to the same object
                        var copiedEventObject = $.extend({}, originalEventObject);
            
                        // assign it the date that was reported
                        copiedEventObject.start = date;
                        copiedEventObject.allDay = allDay;

                        // $log.log(scope);
            
                        // render the event on the calendar
                        // the last `true` argument determines if the event "sticks" (http://arshaw.com/fullcalendar/docs/event_rendering/renderEvent/)
                        $('#calendar').fullCalendar('renderEvent', copiedEventObject, true);
            
                        // is the "remove after drop" checkbox checked?
                        if ($('#drop-remove').is(':checked')) {

                            // if so, remove the element from the "Draggable Events" list
                            // $(this).remove();
                            // $log.log($(this).scope());
                            var index = $(this).scope().$index;

                            $("#external-events").scope().eventsExternal.splice(index, 1);
                            $(this).remove();

                        }
            
                    },

                    select: function (start, end, allDay) {
                        debugger;
                        scope.date = new Date(start);
                        if (scope.dateChanged) scope.dateChanged(scope.date);
                    },

                    // events: scope.events,

                    events: function(start, end, timezone, callback) {

                        callback(scope.events);

                    },

                    eventRender: function (event, element, icon) {
                        if (!event.description == "") {
                            element.find('.fc-event-title').append("<br/><span class='ultra-light'>" + event.description + "</span>");
                        }
                        if (!event.icon == "") {
                            element.find('.fc-event-title').append("<i class='air air-top-right fa " + event.icon + " '></i>");
                        }
                    }
                });

                $('.fc-header-right, .fc-header-center', $calendar).hide();
            }


            initCalendar();


            // Now events will be refetched every time events scope is updated in controllers!!!
            scope.$watch("events", function(newValue, oldValue) {

                $calendar.fullCalendar( 'refetchEvents' );

            }, true);


            scope.next = function () {
                $('.fc-button-next', $calendar).click();
                var currentMonth = new Date(scope.date);
                scope.date = new Date(moment(currentMonth).add(1, 'M'));
                if (scope.monthChanged) scope.monthChanged(scope.date);
            };
            scope.prev = function () {
                $('.fc-button-prev', $calendar).click();
                var currentMonth = new Date(scope.date);
                scope.date = new Date(moment(currentMonth).add(-1, 'M'));
                if (scope.monthChanged) scope.monthChanged(scope.date);
            };
            scope.today = function () {
                $('.fc-button-today', $calendar).click();
            };
            scope.changeView = function (period) {
                $calendar.fullCalendar('changeView', period);
            };
        }
    }
});