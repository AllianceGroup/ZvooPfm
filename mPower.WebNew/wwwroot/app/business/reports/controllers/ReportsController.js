'use strict';

angular.module('app.business').controller('ReportsBusinessController', ['modelProfitLoss', 'modelBalanceSheet', 'reportsBusinessService', '_', 'FileSaver',
    function(modelProfitLoss, modelBalanceSheet, reportsBusinessService, _, FileSaver){
        var ctrl = this;
        ctrl.modelProfitLoss = modelProfitLoss;
        ctrl.modelBalanceSheet = modelBalanceSheet;
        ctrl.refreshProfitLoss = reportsBusinessService.getProfitLoss;
        ctrl.refreshBalanceSheet = reportsBusinessService.getBalanceSheet;
        ctrl.getProfitLossToExcel = reportsBusinessService.exportProfitLossToExcel;
        ctrl.getBalanceSheetToExcel = reportsBusinessService.exportBalanceSheetToExcel;
        ctrl.getProfitLossToPdf = reportsBusinessService.exportProfitLossToPdf;
        ctrl.getBalanceSheetToPdf = reportsBusinessService.exportBalanceSheetToPdf;

        ctrl.changeDate = function(report, method, identifier){
            var currentDate =  _.findWhere(report.ReportDateFormats, {Id: parseInt(report.CurrentReportDateFormat, 10)});

            method({From: currentDate.From, To: currentDate.To, Format: report.Format, Dates: report.CurrentReportDateFormat})
                .then(function(model){
                    if(identifier == "ProfitLoss"){
                        ctrl.modelProfitLoss = model;
                        ctrl.modelProfitLoss.Format = report.Format.toString();
                        ctrl.modelProfitLoss.CurrentReportDateFormat = report.CurrentReportDateFormat.toString();
                    }
                    if(identifier == "BalanceSheet"){
                        ctrl.modelBalanceSheet = model;
                        ctrl.modelBalanceSheet.Format = report.Format.toString();
                        ctrl.modelBalanceSheet.CurrentReportDateFormat = report.CurrentReportDateFormat.toString();
                    }
                });
        };

        ctrl.refresh = function(report , method, identifier){
            method({From: report.From, To: report.To, Format: report.Format, Dates: report.CurrentReportDateFormat})
                .then(function(model){
                    if(identifier == "ProfitLoss"){
                        ctrl.modelProfitLoss = model;
                        ctrl.modelProfitLoss.Format = report.Format.toString();
                        ctrl.modelProfitLoss.CurrentReportDateFormat = report.CurrentReportDateFormat.toString();
                    }
                    if(identifier == "BalanceSheet"){
                        ctrl.modelBalanceSheet = model;
                        ctrl.modelBalanceSheet.Format = report.Format.toString();
                        ctrl.modelBalanceSheet.CurrentReportDateFormat = report.CurrentReportDateFormat.toString();
                    }
                });
        };

        ctrl.exportReport = function(method, report, type, name){
            method({From: report.From, To: report.To, Format: report.Format, Dates: report.CurrentReportDateFormat})
                .then(function(data){
                    var date = new Date();
                    var file = new Blob([data.data], { type: type });
                    FileSaver.saveAs(file, name + (date.getMonth()+1) + '-' + date.getDate() + '-' + date.getFullYear());
                })
        };

        ctrl.printReport = function(method, report){
            method({From: report.From, To: report.To, Format: report.Format, Dates: report.CurrentReportDateFormat})
                .then(function(data){
                    var file = new Blob([data.data], { type: 'application/pdf' });
                    var fileUrl = URL.createObjectURL(file);
                    window.open(fileUrl).print();
                })
        };
    }]);