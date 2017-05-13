'use strict';

angular.module('app.personal').controller('ChartsController', ['dashboardService', 'chartsList', '$filter',
    function (dashboardService, chartsList, $filter) {
        var ctrl = this;


        ctrl.AccountsChart = chartsList.AccountsChart;
        ctrl.LeftChart = chartsList.LeftChart;
        ctrl.RightChart = chartsList.RightChart;
        ctrl.DebtToIncomeRatio = chartsList.DebtToIncomeRatio;
        ctrl.MortgageAccelerator = chartsList.MortageAccelerator;
        ctrl.TotalSavingsInCents = chartsList.TotalSavingsInCents;

        FindNegativeValues(ctrl.AccountsChart);
        FindNegativeValues(ctrl.LeftChart);
        FindNegativeValues(ctrl.RightChart);

        ctrl.BudgetsChart = [];
        for(var i = 0; i < chartsList.BudgetsChart.Spent.length; i++){
            ctrl.BudgetsChart[i] = {period: chartsList.BudgetsChart.Spent[i].label,
                budget: chartsList.BudgetsChart.Budget[i].value, expense: chartsList.BudgetsChart.Spent[i].value};
        }

        ctrl.Formatter = function(value, label){
            if(label.negative === true){
                return '(' + $filter('currency')(value) + ')';
            }
            return $filter('currency')(value);
        };

        function FindNegativeValues(dataChart){
            if(dataChart){
                for(var i = 0; i < dataChart.length; i++){
                    if(dataChart[i].value < 0){
                        dataChart[i].negative = true;
                        dataChart[i].value = Math.abs(dataChart[i].value);
                    }
                    else
                        dataChart[i].negative = false;
                }
            }
        }
    }]);
