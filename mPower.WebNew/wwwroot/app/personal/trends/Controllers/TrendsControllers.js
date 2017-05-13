'use strict';

angular.module('app.personal').controller('TrendsController',
    function TrendsControllers($rootScope, trendsService, trendsList, filter, _, $state, $filter, transactionsPage, accountsPage){
        var ctrl = this;

        ctrl.Trends = trendsList;
        ctrl.filter = filter;
        ConvertData();
        FindNegativeValues(ctrl.Chart);


        ctrl.Refresh = function(filter){
            trendsService.Refresh(filter).then(function(trends){
                ctrl.Trends = trends;
                ConvertData();
                FindNegativeValues(ctrl.Chart);
            });
        };

        ctrl.RefreshByQuantity = function(all){
            ctrl.filter.All = all;
            ctrl.Refresh(ctrl.filter);
        };

        ctrl.RefreshByTime = function(filter){
            ctrl.filter.Filter = filter;
            ctrl.Refresh(ctrl.filter);
        };

        ctrl.RefreshByFormat = function(showFormat){
            ctrl.filter.ShowFormat = showFormat;
            ctrl.Refresh(ctrl.filter);
        };

        ctrl.RefreshByMonth = function(month, year){
            ctrl.filter.Month = month;
            ctrl.filter.Year = year;
            ctrl.filter.Filter = 'SelectedMonth';
            ctrl.Refresh(ctrl.filter);
        };

        ctrl.ViewTransactions = function (accountId, name){
            var filter = {
                accountId: accountId,
                to: encodeURIComponent(ctrl.Trends.To),
                from: encodeURIComponent(ctrl.Trends.From),
                accountName: name
            };
            if($rootScope.businessId)
                filter.businessId = $rootScope.businessId;
            $state.transitionTo(transactionsPage, filter);
        };

        ctrl.showAllAccounts = function(){
            $state.go(accountsPage);
        };

        ctrl.Formatter = function (value, label){
            var percentage = '(' + (value / ctrl.totalAmount * 100).toFixed(2) + '%)';
            if(label.negative === true)
                return '(' + $filter('currency')(value / 100) + percentage + ')';
            return $filter('currency')(value / 100) + percentage;
        };

        function ConvertData(){
            ctrl.Chart = [];
            var sum = 0;
            for(var i = 0; i < ctrl.Trends.Categories.length; i++){
                sum += ctrl.Trends.Categories[i].Amount;
                ctrl.Chart[i] = {value: ctrl.Trends.Categories[i].Amount, label: ctrl.Trends.Categories[i].Name};
            }
            _.findWhere(ctrl.Trends.Monthes, {Month: new Date(ctrl.Trends.Date).getMonth() + 1}).Active = "active";

            if(sum == 0)
                ctrl.Data = false;
            else
                ctrl.Data = true;

            ctrl.totalAmount = sum;
        }

        function FindNegativeValues(dataChart){
            for(var i = 0; i < dataChart.length; i++){
                if(dataChart[i].value < 0){
                    dataChart[i].negative = true;
                    dataChart[i].value = Math.abs(dataChart[i].value);
                }
                else
                    dataChart[i].negative = false;
            }
        }
    });