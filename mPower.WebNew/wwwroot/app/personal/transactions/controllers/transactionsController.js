'use strict';

angular.module('app.personal').controller('TransactionsController',
    ['$rootScope', 'transactionsService', 'transactionsList', 'accountName', 'filter', '_', '$state', '$uibModal', '$filter', '$animate',
        function ($rootScope, transactionsService, transactionsList, accountName, filter, _, $state, $uibModal, $filter) {

            var ctrl = this;

            ctrl.filter = getFilter(filter);
            ctrl.filter.itemsPerPage = ctrl.filter.itemsPerPage.toString();
            ctrl.transactionIds = [];
            ctrl.categories = transactionsList.CategorySelectList;
            ctrl.transactionsList = transactionsList;
            var lastNumber = ctrl.transactionsList.Paging.IndexOfFirstItem + parseInt(ctrl.filter.itemsPerPage);
            ctrl.lastTransactionNumber = lastNumber < ctrl.transactionsList.Paging.TotalCount
                ? lastNumber
                : ctrl.transactionsList.Paging.TotalCount;
            ctrl.accountName = accountName ? accountName : 'All accounts';

            if($rootScope.hasTransactionsHints){
                var enjoyhint_instance = new EnjoyHint({});
                var enjoyhint_script_steps = [
                    {
                        'next #transactions': '<span style="font-size: 24px; font-weight: bold; matgin-bottom: 10px; display: block;">Transactions</span>' + '<p style="font-size: 16px;">Your transactions are automatically downloaded from your financial institution and shown here.</p>'
                    },
                    {
                        'next #addTransaction': '<span style="font-size: 24px; font-weight: bold; matgin-bottom: 10px; display: block;">Manual Transactions</span>' + '<p style="font-size: 16px;">You can add manual transactions by clicking this button</p>'
                    },
                    {
                        'next #search': '<span style="font-size: 24px; font-weight: bold; matgin-bottom: 10px; display: block;">Search</span>' +
                        '<p style="font-size: 16px;">Search for specific transactions here.</p>'
                    },
                    {
                        'skip #pages' : '<span style="font-size: 24px; font-weight: bold; matgin-bottom: 10px; display: block;">View More Per Page.</span>' +
                        '<p style="font-size: 16px;">You can change the number of transactions listed on each page here. This can make it easier to categorize large numbers of transactions.</p>',
                        skipButton: {className: 'hint-finish', text: 'Finish'},
                        shape: 'circle',
                        raduis: 80
                    }
                ];

                enjoyhint_instance.set(enjoyhint_script_steps);
                enjoyhint_instance.run();

                $rootScope.hasTransactionsHints = false;
            }

            function getFilter(filterParameter) {
                if (filterParameter.accountId !== undefined) {
                    filterParameter.mode = "custom";
                }

                return filterParameter;
            }

            ctrl.toggleId = function(id) {
                if (ctrl.transactionIds.indexOf(id) > -1) {
                    ctrl.transactionIds = _.reject(ctrl.transactionIds, function(transactionId) { return transactionId === id });
                } else {
                    ctrl.transactionIds.push(id);
                }
            };

            ctrl.toggleAllIds = function() {
                if (ctrl.transactionIds.length === ctrl.transactionsList.Entries.length) {
                    ctrl.transactionIds = [];
                } else {
                    ctrl.transactionIds = _.map(ctrl.transactionsList.Entries, function (val) { return val.TransactionId });
                }
            }

            ctrl.updateFilter = function () {
                transactionsService.getByFilter(ctrl.filter).then(function(data) {
                    ctrl.transactionIds = [];
                    ctrl.categories = data.CategorySelectList;
                    ctrl.transactionsList = data;

                    var lastNumber = ctrl.transactionsList.Paging.IndexOfFirstItem + parseInt(ctrl.filter.itemsPerPage);
                    ctrl.lastTransactionNumber = lastNumber < ctrl.transactionsList.Paging.TotalCount
                        ? lastNumber
                        : ctrl.transactionsList.Paging.TotalCount;
                });
            };

            ctrl.updateByMode = function (mode) {
                ctrl.filter.mode = ctrl.filter.mode === mode ? '' : mode;

                if (ctrl.filter.accountId !== undefined) {
                    ctrl.filter.accountId = undefined;
                    $state.transitionTo("app.personal.transactions", ctrl.filter, { reload: false });
                }

                ctrl.updateFilter();
            }

            ctrl.showEtitMultiple = function() {
                var modalInstance = $uibModal.open({
                    templateUrl: 'app/personal/transactions/views/editMultiple.tpl.html',
                    controller: 'EditMultipleController',
                    controllerAs: 'editMultipleCtrl',
                    resolve: {
                        categories: function() {
                            return ctrl.categories;
                        },
                        transactionIds: function() { return ctrl.transactionIds; }
                    }
                });
                modalInstance.result.then(function (model) {
                    ctrl.transactionIds = [];
                    var selectedTransactions = _.filter(ctrl.transactionsList.Entries, function (val) {
                        return _.contains(model.Transactions, val.TransactionId);
                    });
                    for (var i = 0; selectedTransactions.length; i++) {
                        selectedTransactions[i].OffsetAccountId = model.AccountId;
                        selectedTransactions[i].Memo = model.Memo;
                    }
                }, function() {
                });
            };

            ctrl.showEditStandart = function (transactionId) {
                transactionsService.getEditModel(transactionId).then(function(model) {
                    var modalInstance = $uibModal.open({
                        templateUrl: 'app/personal/transactions/views/edit' + model.EditType + '.tpl.html',
                        controller: 'EditController',
                        controllerAs: 'editCtrl',
                        resolve: {
                            model: function () {
                                return {
                                    model: model,
                                    actionType: 'Save',
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

                        var transaction = _.find(ctrl.transactionsList.Entries, function (val) { return val.TransactionId === model.TransactionId });
                        transaction.BookedDate = model.BookedDate;
                        transaction.FormattedAmountInDollars = $filter('currency')(model.AmountInDollars.replace(",", ""));
                        transaction.OffsetAccountId = model.OffSetAccountId;
                        transaction.Memo = model.Memo;
                        transaction.Payee = model.Payee;
                        transaction.AccountId = model.AccountId;
                        transaction.AccountName = _.find(model.FilteredAccounts, function (acc) { return acc.Value === model.AccountId; }).Text;
                    });
                });
            };

            ctrl.showAdd = function(type) {
                transactionsService.getAddModel(type, filter.accountId).then(function(model) {
                    var modalInstance = $uibModal.open({
                        templateUrl: 'app/personal/transactions/views/edit' + type + '.tpl.html',
                        controller: 'EditController',
                        controllerAs: 'editCtrl',
                        resolve: {
                            model: function () {
                                return {
                                    model: model,
                                    actionType: 'Add',
                                    actionFunction: transactionsService.add
                                };
                            }
                        }
                    });
                    modalInstance.result.then(function () {
                        ctrl.updateFilter();
                    });
                });
            };

            ctrl.deleteMultiple = function() {
                var modal = $uibModal.open({
                    templateUrl: 'app/personal/transactions/views/deleteMultiple.html',
                    controller: 'DeleteTransactionsController',
                    controllerAs: 'deleteTransactionsCtrl',
                    resolve: {
                        model: function () {
                            return {
                                transactionIds: ctrl.transactionIds,
                                actionFunction: transactionsService.deleteMultiple
                            }
                        }
                    }
                });

                modal.result.then(function(){
                    ctrl.transactionsList.Entries = _.filter(ctrl.transactionsList.Entries, function(val) {
                        return !(_.contains(ctrl.transactionIds, val.TransactionId));
                    });
                    ctrl.transactionIds = [];
                });
            };

            ctrl.assignToAccount = function(transactionId, newAccountId) {
                var transaction = _.find(ctrl.transactionsList.Entries, function (val) { return val.TransactionId === transactionId });
                var oldAccountId = transaction.OffsetAccountId;
                transactionsService.assignToAccount(transactionId, transaction.OffsetAccountId, newAccountId)
                    .then(function () {
                        $.smallBox({
                            title: "Success",
                            content: "Category successfully changed",
                            color: "#739e73",
                            timeout: 2000
                        });
                    },
                    function (rejection) {
                        transaction.OffsetAccountId = oldAccountId;
                        for (var key in rejection) {
                            if (rejection.hasOwnProperty(key)) {
                                $.bigBox({
                                    title: key,
                                    content: rejection[key],
                                    color: "#C46A69",
                                    icon: "fa fa-warning shake animated",
                                    timeout: 6000
                                });
                            }
                        }
                    });
            };

            ctrl.show = function(transaction){
                transaction.show = true;
            };

            ctrl.hide = function(transaction){
                transaction.show = false;
            };

            this.toggle = false;
            this.transactionsToggle = function () {
                this.toggle = !this.toggle;
            }
        }]);