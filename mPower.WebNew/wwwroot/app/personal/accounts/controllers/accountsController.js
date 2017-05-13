'use strict';

angular.module('app.personal').controller('AccountsController', ['accountsService', 'transactionsPage', 'intuitService', '$state',
    '$uibModal', '_', 'eventsAggregatorService', '$scope', '$rootScope',
    function (accountsService, transactionsPage, intuitService, $state, $uibModal, _, eventsAggregatorService, $scope, $rootScope) {
        var ctrl = this;
        ctrl.Accounts = [];
        ctrl.expandAllShow = true;
        ctrl.errors = [];
        $('.panel-collapse:not(".in")').collapse('show');

       

        accountsService.getShortAccounts().then(function (accounts) {
            ctrl.Accounts = accounts;
            ctrl.accIsUpdating = ctrl.Accounts.IsUpdating;
        });

        $rootScope.$on('updateAccounts', function (event, data) {
            accountsService.getShortAccounts().then(function (accounts) {
                ctrl.Accounts = accounts;
                ctrl.accIsUpdating = ctrl.Accounts.IsUpdating;
            });
        });
     
        ctrl.updateAccounts = function() {
            ctrl.expandAllShow = true;
            $('.panel-collapse:not(".in")').collapse('show');
            intuitService.aggregateUser().then(function() {
                accountsService.getShortAccounts().then(function(accounts) {
                    ctrl.Accounts = accounts;
                    ctrl.accIsUpdating = true;
                });
            });
        };

        var accountUpdateddSubc = eventsAggregatorService.subscribe('accountUpdated', function (event, data) {
            ctrl.Accounts = data;
            ctrl.accIsUpdating = ctrl.Accounts.IsUpdating;
        });

        var updateAccountsSubc = eventsAggregatorService.subscribe('updateAccounts', function (event, data) {
            alert(data);
        });

        var accountAddedSubc = eventsAggregatorService.subscribe('accountAdded', function (event, data) {
            accountsService.getShortAccounts().then(function(accounts) {
                ctrl.Accounts = accounts;
                ctrl.accIsUpdating = ctrl.Accounts.IsUpdating;
            });
        });

        $scope.$on('$destroy', function() {
            eventsAggregatorService.unsubscribe('updateAccounts', updateAccountsSubc);
            eventsAggregatorService.unsubscribe('accountUpdated', accountUpdateddSubc);
            eventsAggregatorService.unsubscribe('accountAdded', accountAddedSubc);
        });

        ctrl.expandAll = function () {
            ctrl.expandAllShow = true;
            $('.panel-collapse:not(".in")').collapse('show');
        };

        ctrl.hideAll = function () {
            ctrl.expandAllShow = false;
            $('.panel-collapse.in').collapse('hide');
        };

        ctrl.showTransaction = function (id, name) {
            $state.go(transactionsPage, { accountId: id, accountName: name });
        };

        ctrl.handlingErrorStatus = function (intuitInstitutionId, intuitAccountId, textError) {
            if (textError === "Need Reauthentication") {
                intuitService.reathenticateGetLogonForm(intuitInstitutionId, intuitAccountId).then(function (model) {
                    $uibModal.open({
                        templateUrl: 'app/personal/accounts/views/intuit-reauthenticate.html',
                        controller: 'AuthenticateController',
                        controllerAs: 'intuitCtrl',
                        resolve: {
                            model: model                            
                        }
                    });
                }, function (errors) {
                    for (var key in errors) {
                        if (errors.hasOwnProperty(key)) {
                            for (var i = 0; i < errors[key].length; i++) {
                                ctrl.errors.push(errors[key][i]);
                            }
                        }
                    }
                });
            }
            else if (textError === "Need Interactive Refresh") {
                intuitService.interactiveRefresh(intuitAccountId).then(function (model) {
                    $uibModal.open({
                        templateUrl: 'app/personal/accounts/views/intuit-interactiveRefresh.html',
                        controller: 'InterectiveRefreshController',
                        controllerAs: 'interRefreshCtrl',
                        resolve: {
                            model: model                            
                        }
                    });
                }, function (errors) {
                    for (var key in errors) {
                        if (errors.hasOwnProperty(key)) {
                            for (var i = 0; i < errors[key].length; i++) {
                                ctrl.errors.push(errors[key][i]);
                            }
                        }
                    }
                });
            }
        }

        ctrl.dismissError = function(errorMessage) {
            for (var i = 0; i < ctrl.errors.length; i++) {
                if (errorMessage === ctrl.errors[i].ErrorMessage) {
                    ctrl.errors.splice(i, 1);
                    break;
                }             
            }
        }

        ctrl.showAllAccounts = function(){
            $state.go("^.chartofaccounts");
        };

        ctrl.addAccounts = function() {
            $uibModal.open({
                templateUrl: "app/personal/accounts/views/add-template.tpl.html",
                controller: 'ChoiceAccountController',
                controllerAs: 'choiceAccountCtrl'
            });
        };
}]);