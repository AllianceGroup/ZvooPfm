"use strict";

angular.module('app.personal')
    .config(function($stateProvider) {
        $stateProvider
            .state('app.personal.debttools', {
                url: '/debttools',
                views: {
                    "content@app": {
                        controller: 'DebttoolsController',
                        controllerAs: 'debttoolsCtrl',
                        templateUrl: 'app/personal/debttools/debttools.tpl.html'
                    }
                },
                data: {
                    title: 'Debt Tools'
                },
                resolve: {
                    debttoolsModel: function(debttoolsService) {
                        return debttoolsService.getDefaultModel();
                    },
                    scripts: function(lazyScript) {
                        return lazyScript.register(['morris']);
                    }
                }
            })
            .state('app.personal.mortageAcceleration', {
                url: '/mortageAcceleration',
                views: {
                    "content@app": {
                        controller: 'MortgageAccelerationController',
                        controllerAs: 'mortgageAccelerationCtrl',
                        templateUrl: 'app/personal/debttools/views/mortageAcceleration.tpl.html'
                    }
                },
                data: {
                    title: 'Mortage acceleration'
                },
                resolve: {
                    mortgageModel: function(debttoolsService) {
                        return debttoolsService.getMortageAccelerationModel();
                    },
                    scripts: function(lazyScript) {
                        return lazyScript.register(['morris']);
                    }
                }
            })
            .state('app.personal.incomeRation', {
                url: '/incomeRation',
                views: {
                    "content@app": {
                        controller: 'IncomeRatioController',
                        controllerAs: 'incomeRatioCtrl',
                        templateUrl: 'app/personal/debttools/views/incomeRatio.tpl.html'
                    }
                },
                data: {
                    title: 'Debt to income ration'
                },
                resolve: {
                    incomeRatioModel: function(debttoolsService) {
                        return debttoolsService.getIncomeRatioModel();
                    },
                    scripts: function(lazyScript) {
                        return lazyScript.register(['morris']);
                    }
                }
            })
            .state('app.personal.eliminationProgram', {
                url: '/eliminationProgram',
                views: {
                    "content@app": {
                        controller: 'EliminationProgramController',
                        controllerAs: 'eliminationProgramCtrl',
                        templateUrl: 'app/personal/debttools/views/eliminationProgram.tpl.html'
                    }
                },
                data: {
                    title: 'Debt elimination program'
                },
                resolve: {
                    stepModel: function (debttoolsService, $stateParams) {
                        if($stateParams.currentStep == 1 || !$stateParams.currentStep)
                            return debttoolsService.getStep1Model();
                        if($stateParams.currentStep == 2)
                            return debttoolsService.getStep2Model();
                        if($stateParams.currentStep == 3)
                            return debttoolsService.getStep3Model();
                    },
                    scripts: function (lazyScript) {
                        return lazyScript.register(['morris', 'jquery-maskedinput', 'fuelux-wizard', 'jquery-validation' ]);
                    }
                },
                params:{
                    currentStep: null,
                    hasDebtElimination: false
                }
            })
    });