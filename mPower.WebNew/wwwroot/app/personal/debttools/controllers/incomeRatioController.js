'use strict';

angular.module('app.personal').controller('IncomeRatioController',
['debttoolsService', 'incomeRatioModel', '_', function (debttoolsService, incomeRatioModel, _) {
    var ctrl = this;

    ctrl.model = incomeRatioModel;
    ctrl.disableSaveOrUpdate = false;
    FindNegativeValues(ctrl.model.LeftChart);
    FindNegativeValues(ctrl.model.RightChart);

    ctrl.saveOrUpdate = function() {
        ctrl.disableSaveOrUpdate = true;
        ctrl.errors = [];

        debttoolsService.saveOrUpdateincomeRatioModel({
                MonthlyGrossIncome: ctrl.model.MonthlyGrossIncome,
                TotalMonthlyRent: ctrl.model.TotalMonthlyRent,
                TotalMonthlyPitia: ctrl.model.TotalMonthlyPitia,
                TotalMonthlyDebt: ctrl.model.TotalMonthlyDebt
            })
            .then(function(model) {
                ctrl.disableSaveOrUpdate = false;
                ctrl.model = model;
            }, function(errors) {
                ctrl.disableSaveOrUpdate = false;
                for (var key in errors) {
                    if (errors.hasOwnProperty(key)) {
                        for (var i = 0; i < errors[key].length; i++) {
                            ctrl.errors.push(key + ':' + errors[key][i]);
                        }
                    }
                }
            });
    };

    ctrl.Formatter = function(value, label){
        if(label.negative === true){
            return '($' + value + ')';
        }
        return '$' + value;
    };

    ctrl.deleteProgram = function () {
        ctrl.errors = [];
        debttoolsService.deleteIncomeRatioModel().then(function () {
            debttoolsService.getIncomeRatioModel().then(function(model) {
                ctrl.model = model;
            });
        });
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