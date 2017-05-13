"use strict";

angular.module('app.affiliateAdmin').controller('CreateSegmentController', ['segment', 'messagesService', '$state',
    function (segment, messagesService, $state){

        var ctrl = this;
        ctrl.model = segment;
        ctrl.errors = [];
        ctrl.Limit = 5;
        ctrl.customDateRangeStart = segment.Segment.CustomDateRangeStart;
        ctrl.customDateRangeEnd = segment.Segment.CustomDateRangeEnd;
        convertSpendingCategories();

        ctrl.TypeEnum = {
            Default: 0,
            Flag: 1,
            Trend: 2,
            Custom: 3,
            Frequency: 4,
            Full: 5
        };

        ctrl.save = function(){
            if(ctrl.model.Segment.DateRange === '3'){
                ctrl.model.Segment.CustomDateRangeStart = ctrl.customDateRangeStart;
                ctrl.model.Segment.CustomDateRangeEnd = ctrl.customDateRangeEnd;
            }
            else {
                ctrl.model.Segment.CustomDateRangeStart = null;
                ctrl.model.Segment.CustomDateRangeEnd = null;
            }
            messagesService.createSegment(ctrl.model.Segment).then(function(){
                $state.transitionTo('app.affiliateAdmin.userGroups');
            },function(errors){
                ctrl.errors = _.reduce(errors, function(result, arr) {
                    return result.concat(arr);
                }, []);
            });
        };

        ctrl.estimateSegment = function(){
            if(ctrl.model.Segment.DateRange === '3'){
                ctrl.model.Segment.CustomDateRangeStart = ctrl.customDateRangeStart;
                ctrl.model.Segment.CustomDateRangeEnd = ctrl.customDateRangeEnd;
            }
            else {
                ctrl.model.Segment.CustomDateRangeStart = null;
                ctrl.model.Segment.CustomDateRangeEnd = null;
            }
            messagesService.estimateSegment(ctrl.model.Segment).then(function(segment){
                ctrl.model.Segment.Reach = segment.Reach;
                ctrl.model.Segment.ReachFormatted = segment.ReachFormatted;
            });
        };

        ctrl.update = function(){
            messagesService.updateSegment(ctrl.model.Segment).then(function(){
                $state.transitionTo('app.affiliateAdmin.userGroups');
            }, function(errors){
                ctrl.errors = _.reduce(errors, function(result, arr) {
                    return result.concat(arr);
                }, []);
            })
        };

        ctrl.addItem = function(item){
            if(item[item.length - 1] != null && item[item.length - 1] != ""){

                if(item.length >= ctrl.Limit)
                    return;
                item.length++;
            }
        };
        ctrl.removeItem = function(item){
            if(item.length == 1)
                return;

            item.length--;
        };

        ctrl.checkDateRange = function(){

        };

        function convertSpendingCategories() {
            var newCategories = [], k = 0;

            for (var i = 0; i < ctrl.model.SpendingCategories.length; i++) {
                newCategories[k++] = ctrl.model.SpendingCategories[i].Name;
                for (var j = 0; j < ctrl.model.SpendingCategories[i].SubAccounts.length; j++)
                    newCategories[k++] = ctrl.model.SpendingCategories[i].SubAccounts[j].Name;
            }

            ctrl.model.NewSpendingCategories = newCategories;
        }
    }]);