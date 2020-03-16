'use strict';

angular.module('app.personal').service('debttoolsService', ['http', function (http) {

    this.getDefaultModel = function () {
        var url = '/debttools';
        return http.get(url);
    };

    this.getMortageAccelerationModel = function(programId) {
        var url = '/mortgageacceleration' + http.buildQueryString({ programId: programId });
        return http.get(url);
    }

    this.saveOrUpdateMortageAccelerationModel = function(model) {
        var url = '/mortgageacceleration/saveorupdate';
        return http.post(url, model);
    };

    this.deleteMortageAccelerationProgram = function(programId) {
        var url = '/mortgageacceleration/' + programId;
        return http.delete(url);
    };

    this.addToCalendarMortageAccelerationProgram = function(id) {
        var url = '/mortgageacceleration/addtocalendar' + http.buildQueryString({ programId: id });
        return http.post(url);
    }

    this.getIncomeRatioModel = function() {
        var url = '/debttoincome';
        return http.get(url);
    };

    this.saveOrUpdateincomeRatioModel = function(model) {
        var url = '/debttoincome/saveorupdate';
        return http.post(url, model);
    };

    this.deleteIncomeRatioModel = function () {
        var url = '/debttoincome';
        return http.delete(url);
    }

    this.addToCalendarEliminationProgram = function () {
        var url = '/debtelimination/addtocalendar';
        return http.post(url);
    }

    this.getStep1Model = function() {
        var url = '/debtelimination/step1';
        return http.get(url);
    }

    this.proceedToStep2 = function (model) {
        var url = '/debtelimination/proceedstep2';
        console.log(model);
        return http.post(url, model);
    }

    this.getStep2Model = function () {
        var url = '/debtelimination/step2';
        return http.get(url);
    }

    this.proceedToStep3 = function (model) {
        var url = '/debtelimination/proceedstep3';
        return http.post(url, model);
    }

    this.getStep3Model = function() {
        var url = '/debtelimination/step3';
        return http.get(url);
    };

    this.updateCharts = function (model) {
        var url = '/debtelimination/updatecharts';
        return http.post(url, model);
    }
}]);