'use strict';

angular.module('app.personal').controller('StartController', ['intuitService', '_', '$uibModal', '$uibModalInstance', '$state', '$rootScope', 'localStorageService',
    function(intuitService, _, $uibModal, $uibModalInstance, $state, $rootScope, localStorageService){
        var ctrl = this;

        var personalLedgerId = localStorageService.get('settings').personalLedgerId;
        ctrl.isAuthenticated = false;
        ctrl.isSearching = false;
        ctrl.isSelected = false;
        ctrl.isFound = true;

        ctrl.errors = [];
        ctrl.institutions = [];
        ctrl.institutionsFull = [];
        ctrl.searchText = "";
        ctrl.currentPage = 1;
        ctrl.intemsPerPage = 10;

        ctrl.answers = null;
        ctrl.questions = null;
        ctrl.AggregationData = null;
        ctrl.disabledSubmit = false;

        ctrl.model = null;
        ctrl.isAdding = false;

        ctrl.dismiss = function(state) {
            if(state)
                $state.go(state);
            localStorageService.set('settings', {hasAccounts: true, personalLedgerId: personalLedgerId});
            $uibModalInstance.dismiss();
        };

        ctrl.back = function(){
            if(ctrl.isAuthenticated){
                ctrl.isAuthenticated = false;
                ctrl.model = null;
            }
            if(ctrl.isSelected){
                ctrl.isSelected = false;
                ctrl.model = null;
            }
            ctrl.isFound = true;
        };

        ctrl.showDashboard = function(){
            $uibModalInstance.dismiss();
            localStorageService.set('settings', {hasAccounts: true, personalLedgerId: personalLedgerId});
            $state.go('app.personal.dashboard', '', { reload: true });
        };

        ctrl.getFinancialInstitutions = function () {
            ctrl.isSearching = true;
            intuitService.getFinancialInstitutions(ctrl.searchText).then(function (institutions) {
                ctrl.currentPage = 1;
                ctrl.institutionsFull = institutions.ContentServices;
                ctrl.updatePaging();
                ctrl.isSearching = false;
            });
        };

        ctrl.updatePaging = function() {
            var firstIndex = (ctrl.currentPage - 1) * ctrl.intemsPerPage;
            ctrl.institutions = ctrl.institutionsFull.slice(firstIndex, firstIndex + ctrl.intemsPerPage);
        };

        ctrl.authenticate = function (id) {
            ctrl.errors = [];
            intuitService.authenticate(id).then(function (model) {
                ctrl.model = model;
                ctrl.isFound = false;
                ctrl.isAuthenticated = true;
            },function(errors) {
                for (var key in errors) {
                    if (errors.hasOwnProperty(key)) {
                        for (var i = 0; i < errors[key].length; i++) {
                            ctrl.errors.push(errors[key][i]);
                        }
                    }
                }
            });
        };

        ctrl.submit = function () {
            ctrl.errors = [];
            ctrl.disabledSubmit = true;

            intuitService.authenticateToBank({
                Dto: {
                    InstitutionId: ctrl.model.InstitutionId,
                    Keys: ctrl.model.Keys
                },
                MfaSession: ctrl.mfaSession
            }).then(function (model) {
                ctrl.disabledSubmit = false;
                if (model.MfaSession) {
                    ctrl.mfaSession = model.MfaSession;
                }
                else {
                    $uibModalInstance.close();
                    $uibModal.open({
                        templateUrl: 'app/personal/accounts/views/intuit-availableAccounts.tpl.html',
                        controller: 'AvailableAccountsController',
                        controllerAs: 'intuitCtrl',
                        resolve: {
                            model: model
                        }
                    });
                }
            }, function (errorModel) {
                ctrl.mfaSession = null;

                for (var key in errorModel.Errors) {
                    if (errorModel.Errors.hasOwnProperty(key)) {
                        ctrl.errors.push(errorModel.Errors[key].ErrorMessage);
                    }
                }
                ctrl.disabledSubmit = false;
            });
        };

        ctrl.aligntoledger = function () {
            ctrl.errors = [];
            ctrl.isAdding = true;
            intuitService.aligntoledger(ctrl.model).then(function() {
                ctrl.isAdding = false;

                localStorageService.set('settings', {hasAccounts: true, personalLedgerId: personalLedgerId});
                $rootScope.hasDashboardHints = true;
                $rootScope.hasTransactionsHints = true;

                ctrl.isSelected = false;
                ctrl.isFound = true;
                $('[data-smart-wizard-next]').click();
                $.smallBox({
                    title: "Success",
                    content: "Accounts added",
                    color: "#739e73",
                    timeout: 4000
                });
            }, function(errors) {
                ctrl.isAdding = false;
                for (var key in errors) {
                    if (errors.hasOwnProperty(key)) {
                        for (var i = 0; i < errors[key].length; i++) {
                            ctrl.errors.push(errors[key][i]);
                        }
                    }
                }
            });
        };
    }]);