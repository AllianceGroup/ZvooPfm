'use strict';

angular.module('app.business').controller('TransactionDetailController', ['transactionDetailModel', '_', 'transactionsService', '$uibModal', 'reportsBusinessService', 'FileSaver',
    function(transactionDetailModel, _, transactionsService, $uibModal, reportsBusinessService, FileSaver){
        var ctrl = this;

        ctrl.model = transactionDetailModel;
        ctrl.getTransactionDetailToPdf = reportsBusinessService.exportTransactionDetailToPdf;
        ctrl.getTransactionDetailToExcel = reportsBusinessService.exportTransactionDetailToExcel;
        ctrl.entries = [];
        changeFormat();

        ctrl.changeDate = function(){
            var currentDate =  _.findWhere(ctrl.model.ReportDateFormats, {Id: parseInt(ctrl.model.CurrentReportDateFormat, 10)});

            reportsBusinessService.getTransactionDetail(
                {
                    From: currentDate.From,
                    To: currentDate.To,
                    Id: ctrl.model.ParentAccountId,
                    P: 0,
                    Dates: ctrl.model.CurrentReportDateFormat}
            ).then(function(transactionDetailModel){
                    ctrl.model = transactionDetailModel;
                    ctrl.entries = [];
                    ctrl.model.CurrentReportDateFormat = ctrl.model.CurrentReportDateFormat.toString();
                    changeFormat();
                });
        };

        ctrl.refresh = function(){
            reportsBusinessService.getTransactionDetail(
                {
                    From: ctrl.model.From,
                    To: ctrl.model.To,
                    Id: ctrl.model.ParentAccountId,
                    P: 0
                }
            ).then(function(transactionDetailModel){
                    ctrl.model = transactionDetailModel;
                    ctrl.entries = [];
                    ctrl.model.CurrentReportDateFormat = ctrl.model.CurrentReportDateFormat.toString();
                    changeFormat();
                });
        };

        ctrl.exportReport = function(method, type){
            method({
                From: ctrl.model.From,
                To: ctrl.model.To,
                Id: ctrl.model.ParentAccountId,
                P: 0,
                Dates: ctrl.model.CurrentReportDateFormat
            }).then(function(data){
                var date = new Date();
                var file = new Blob([data.data], { type: type });
                FileSaver.saveAs(file, 'TransactionDetail_' + (date.getMonth()+1) + '-' + date.getDate() + '-' + date.getFullYear());
            });
        };

        ctrl.printReport = function(){
            reportsBusinessService.exportTransactionDetailToPdf({
                From: ctrl.model.From,
                To: ctrl.model.To,
                Id: ctrl.model.ParentAccountId,
                P: 0,
                Dates: ctrl.model.CurrentReportDateFormat
            }).then(function(data){
                var file = new Blob([data.data], { type: 'application/pdf' });
                var fileUrl = URL.createObjectURL(file);
                window.open(fileUrl).print();
            });
        };

        ctrl.showEditStandart = function (transactionId) {
            transactionsService.getEditModel(transactionId).then(function (model) {
                var modalInstance = $uibModal.open({
                    templateUrl: 'app/personal/transactions/views/edit' + model.EditType + '.tpl.html',
                    controller: 'EditController',
                    controllerAs: 'editCtrl',
                    resolve: {
                        model: function () {
                            return {
                                model: model,
                                actionType: 'Edit',
                                actionFunction: transactionsService.edit
                            };
                        }
                    }
                });
                modalInstance.result.then(function (model) {
                    if (_.isUndefined(model)) {
                        ctrl.updateFilter();
                        return;
                    }
                    for(var i = 0; i < ctrl.entries.length; i++){
                        if(ctrl.entries[i].Id === model.TransactionId){
                            ctrl.entries[i].BookedDate = model.BookedDate;
                            ctrl.entries[i].balance = model.AmountInDollars;
                            ctrl.entries[i].DebitAmountInCents = model.AmountInDollars;
                            ctrl.entries[i].Memo = model.Memo;
                            break;
                        }
                    }
                });
            });
        }

        function changeFormat(){
            var transactions = ctrl.model.Transactions;

            for(var j = 0; j < transactions.length; j++){
                for(var item in ctrl.model.AccountIds){
                    for(var i = 0; i < transactions[j].Entries.length; i++){
                        if(ctrl.model.AccountIds[item] == transactions[j].Entries[i].AccountId){
                            ctrl.entries[j] = transactions[j].Entries[i];
                            ctrl.model.InitialBalance += (ctrl.entries[j].DebitAmountInCents - ctrl.entries[j].CreditAmountInCents);
                            ctrl.entries[j].balance = ctrl.model.InitialBalance;
                            ctrl.entries[j].Id = transactions[j].Id;
                            break;
                        }
                    }
                }
            }
        }
    }]);