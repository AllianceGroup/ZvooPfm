'use strict';

angular.module('app.personal').controller('ChartOfAccountsCtrl', function ($window, $uibModal, accountsList, _, accountsService, $state, eventsAggregatorService, $scope) {

    var ctrl = this;
    ctrl.Income = [];
    ctrl.Expense = [];
    ctrl.Other = [];
    ctrl.Institution = [];
    SortByAccounts(accountsList);
    $('#collapseThree').collapse('show');

    ctrl.Edit = function(id){
        accountsService.getEditAccount(id).then(function(account){
            var  modalInstance = $uibModal.open({
                templateUrl: 'app/personal/accounts/views/editAccount.html',
                controller: 'accountEditController',
                controllerAs: 'accountEditCtrl',
                resolve: {
                    model: function () {
                        return {
                            account : account,
                            actionFunction: accountsService.Save
                        }
                    }
                }
            });

            modalInstance.result.then(function (model) {
                if (_.isUndefined(model)) {
                    return;
                }
                accountsService.getAccounts().then(function(accounts){
                    SortByAccounts(accounts);
                })
            });
        });
    };

    ctrl.Delete = function(id){
        accountsService.ConfirmDelete(id).then(function(account){
                var  modalInstance = $uibModal.open({
                    templateUrl: 'app/personal/accounts/views/deleteAccount.html',
                    controller: 'deleteAccountController',
                    controllerAs: 'deleteAccountCtrl',
                    resolve: {
                        model: function () {
                            return {
                                account: account,
                                actionFunction: accountsService.Delete
                            }
                        }
                    }
                });

                modalInstance.result.then(function () {
                    accountsService.getAccounts().then(function(accounts){
                        SortByAccounts(accounts);
                    })
                });
            }
        );
    };

    ctrl.ViewTransactions = function (accountId, name){
        var filter = {
            accountId: accountId,
            accountName: name
        };
        $state.transitionTo("app.personal.transactions", filter);
    };

    var accountAddedSubc = eventsAggregatorService.subscribe('accountAdded', function (event, data) {
        accountsService.getAccounts().then(function(accountsList){
            SortByAccounts(accountsList);
        });
    });

    $scope.$on('$destroy', function() {
        eventsAggregatorService.unsubscribe('accountAdded', accountAddedSubc);
    });

    function SortByAccounts(accountsList) {
        ctrl.Income = _.where(accountsList.Accounts, {Type: "Income"});
        ctrl.Income = AddSubAccounts(ctrl.Income);

        ctrl.Expense = _.where(accountsList.Accounts, {Type: "Expense"});
        ctrl.Expense = AddSubAccounts(ctrl.Expense);

        ctrl.Institution = _.filter(accountsList.Accounts, function(account){
            if(account.Type == "Asset" || account.Type == "Liability")
                return true;
            return false;
        });
        ctrl.Institution = AddSubAccounts(ctrl.Institution);

        ctrl.Other = _.where(accountsList.Accounts, {Type: "Equity"});
        ctrl.Other = AddSubAccounts(ctrl.Other);
    }

    function AddSubAccounts(accounts){
        var newAccounts = [], k = 0;

        for(var i = 0; i < accounts.length; i++) {
            newAccounts[k++] = accounts[i];

            for (var j = 0; j < accounts[i].Children.length; j++)
                newAccounts[k++] = accounts[i].Children[j];

            accounts[i].Children = [];
        }

        return newAccounts;
    }

    ctrl.addAccounts = function() {
        $uibModal.open({
            templateUrl: "app/personal/accounts/views/add-template.tpl.html",
            controller: 'ChoiceAccountController',
            controllerAs: 'choiceAccountCtrl'
        });
    };

    ctrl.back = function() {
        $window.history.back();
    };
});