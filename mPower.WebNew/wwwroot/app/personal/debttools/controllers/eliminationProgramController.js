'use strict';

angular.module('app.personal').controller('EliminationProgramController',
    ['debttoolsService', 'stepModel', '_', '$state', '$uibModal', '$stateParams', 'accountsService', 'eventsAggregatorService', '$scope',
        function (debttoolsService, stepModel, _, $state, $uibModal, $stateParams, accountsService, eventsAggregatorService, $scope) {

            var ctrl = this;
            ctrl.errors = [];
            ctrl.popoverTemplate = "myPopoverTemplate.html";
            ctrl.stepModel = stepModel;
            selectedDebts();

            $('#Wizard').wizard('selectedItem', {
                step: $stateParams.currentStep
            });

            debttoolsService.getDefaultModel().then(function(debttoolsModel){
                ctrl.hasDebtElimination = debttoolsModel.DebtElimination;
            });

            ctrl.toggleAllIds = function() {
                if (ctrl.selectedDebts.length === ctrl.stepModel.Debts.length) {
                    ctrl.selectedDebts = [];
                } else {
                    ctrl.selectedDebts = _.map(ctrl.stepModel.Debts, function (val) { return val.Id });
                }
            };

            ctrl.toggleId = function(id) {
                if (ctrl.selectedDebts.indexOf(id) > -1) {
                    ctrl.selectedDebts = _.reject(ctrl.selectedDebts, function (debtId) { return debtId === id });
                } else {
                    ctrl.selectedDebts.push(id);
                }
            };

            ctrl.edit = function(id){
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

                    modalInstance.result.then(function () {
                        debttoolsService.getStep1Model().then(function(stepModel){
                            ctrl.stepModel = stepModel;
                            selectedDebts();
                        });
                    });
                });
            };

            ctrl.proceedToStep1 = function(){
                ctrl.errors = [];
                debttoolsService.getStep1Model().then(function(stepModel){
                    ctrl.stepModel = stepModel;
                    selectedDebts();
                    $('#Wizard').wizard('selectedItem', {
                        step: 1
                    });
                }, function(errors) {
                    for (var key in errors) {
                        if(errors.hasOwnProperty(key)) {
                            for (var i = 0; i < errors[key].length; i++) {
                                ctrl.errors.push(errors[key][i]);
                            }
                        }
                    }
                });
            };

            ctrl.addAccounts = function(){
                $uibModal.open({
                    templateUrl: "app/personal/accounts/views/add-template.tpl.html",
                    controller: 'ChoiceAccountController',
                    controllerAs: 'choiceAccountCtrl'
                })
            };

            ctrl.proceedToStep2 = function (isCheck){debugger
                ctrl.errors = [];
                if(ctrl.hasDebtElimination !== null && !isCheck){
                    debttoolsService.getStep2Model().then(function(stepModel){debugger
                        $('#Wizard').wizard('selectedItem', {
                            step: 2
                        });
                        ctrl.stepModel = stepModel;
                    });
                }
                else{
                    debttoolsService.proceedToStep2({ DebtIds: ctrl.selectedDebts }).then(function() {debugger
                        debttoolsService.getStep2Model().then(function(stepModel){debugger
                            ctrl.stepModel = stepModel;
                            $('#Wizard').wizard('selectedItem', {
                                step: 2
                            });
                        });
                    }, function(errors) {
                        for (var key in errors) {
                            if (errors.hasOwnProperty(key)) {
                                for (var i = 0; i < errors[key].length; i++) {
                                    ctrl.errors.push(errors[key][i]);
                                }
                            }
                        }
                    });
                }
            };

            ctrl.recommend = function () {
                ctrl.stepModel.MonthlyBudget = ctrl.stepModel.RecommendedBudgetInCents/100;
            };

            ctrl.proceedToStep3 = function (isCheck) {
                ctrl.errors = [];
                if(ctrl.hasDebtElimination !== null && !isCheck) {
                    debttoolsService.getStep3Model().then(function (stepModel) {
                        $('#Wizard').wizard('selectedItem', {
                            step: 3
                        });
                        ctrl.stepModel = stepModel;
                        ctrl.stepModel.DisplayMode = ctrl.stepModel.DisplayMode.toString();
                    });
                }
                else {
                    debttoolsService.proceedToStep3({ Plan: ctrl.stepModel.Plan, MonthlyBudget: ctrl.stepModel.MonthlyBudget, YearsUntilRetirement: ctrl.stepModel.YearsUntilRetirement, EstimatedInvestmentEarningsRate: ctrl.stepModel.EstimatedInvestmentEarningsRate }).then(function () {                        
                        debttoolsService.getStep3Model().then(function(stepModel){
                            ctrl.stepModel = stepModel;
                            ctrl.stepModel.DisplayMode = ctrl.stepModel.DisplayMode.toString();
                            $('#Wizard').wizard('selectedItem', {
                                step: 3
                            });
                        });
                    }, function(errors) {
                        for (var key in errors) {
                            if (errors.hasOwnProperty(key)) {
                                for (var i = 0; i < errors[key].length; i++) {
                                    ctrl.errors.push(errors[key][i]);
                                }
                            }
                        }
                    });
                }
            };

            ctrl.changeDisplayMode = function() {
                debttoolsService.updateCharts(ctrl.stepModel.DisplayMode).then(function (model) {
                    ctrl.stepModel = model;
                    ctrl.stepModel.DisplayMode = ctrl.stepModel.DisplayMode.toString();
                });
            };

            ctrl.addToCalendar = function() {
                debttoolsService.addToCalendarEliminationProgram().then(function() {
                    ctrl.stepModel.AddedToCalendar = true;
                });
            };

            var accountAddedSubc = eventsAggregatorService.subscribe('accountAdded', function (event, data) {
                debttoolsService.getStep1Model().then(function(stepModel){
                    ctrl.stepModel = stepModel;
                    selectedDebts();
                })
            });

            $scope.$on("$destroy", function () {
                eventsAggregatorService.unsubscribe('accountAdded', accountAddedSubc);
            });

            function selectedDebts(){
                ctrl.selectedDebts = _.chain(ctrl.stepModel.Debts)
                    .filter(function(elem) { return elem.UseInProgram; })
                    .map(function (val) { return val.Id })
                    .value();
            }
        }]);