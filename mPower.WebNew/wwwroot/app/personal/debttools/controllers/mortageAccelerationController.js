'use strict';

angular.module('app.personal').controller('MortgageAccelerationController',
['debttoolsService', 'mortgageModel', '_', function (debttoolsService, mortgageModel, _) {
    var ctrl = this;

    ctrl.model = mortgageModel;
    ctrl.disableSaveOrUpdate = false;

    ctrl.saveOrUpdate = function () {
        ctrl.errors = [];
        ctrl.disableSaveOrUpdate = true;
        debttoolsService.saveOrUpdateMortageAccelerationModel({
            Id: ctrl.model.SelectedProgram.Id,
            Title: ctrl.model.SelectedProgram.Title,
            LoanAmountInDollars: ctrl.model.SelectedProgram.LoanAmountInDollars,
            InterestRatePerYear: ctrl.model.SelectedProgram.InterestRatePerYear,
            LoanTermInYears: ctrl.model.SelectedProgram.LoanTermInYears,
            PaymentPeriod: ctrl.model.SelectedProgram.PaymentPeriod,
            ExtraPaymentInDollarsPerPeriod: ctrl.model.SelectedProgram.ExtraPaymentInDollarsPerPeriod,
            DisplayResolution: ctrl.model.SelectedProgram.DisplayResolution
        }).then(function(model) {
            ctrl.model.SelectedProgram = model;
            ctrl.disableSaveOrUpdate = false;
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

    ctrl.changeModel = function(programId) {
        ctrl.errors = [];
        if (programId !== ctrl.model.SelectedProgram.Id) {
            debttoolsService.getMortageAccelerationModel(programId).then(function(model) {
                ctrl.model = model;
            });
        }
    };

    ctrl.deleteProgram = function () {
        ctrl.errors = [];
        debttoolsService.deleteMortageAccelerationProgram(ctrl.model.SelectedProgram.Id).then(function () {
            debttoolsService.getMortageAccelerationModel().then(function (model) {
                ctrl.model = model;
            });
        });
    };

    ctrl.addToCalendar = function () {
        debttoolsService.addToCalendarMortageAccelerationProgram(ctrl.model.SelectedProgram.Id).then(function () {
            ctrl.model.SelectedProgram.AddedToCalendar = true;
        });
    };
}]);