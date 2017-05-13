'use strict';

angular.module('app.affiliateAdmin').controller('AnalyticsDashboardController', function DashboardController(analyticsService, model){
    var ctrl = this;
    ctrl.model = model;

    ctrl.changeType = function(type, value){
        if(type == 'totalDebt'){
            if(ctrl.model.Statistic.TotalDebtType == value)
                return;
            else{
                ctrl.model.Statistic.TotalDebtType = value;
                analyticsService.getDashboardModel(ctrl.model.Statistic).then(function(model){
                    ctrl.model = model;
                });
            }
        }
        if(type == 'cashType'){
            if(ctrl.model.Statistic.AvailableCashType == value)
                return;
            else{
                ctrl.model.Statistic.AvailableCashType = value;
                analyticsService.getDashboardModel(ctrl.model.Statistic).then(function(model){
                    ctrl.model = model;
                });
            }
        }

    };
});