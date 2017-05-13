"use strict";

angular.module('app.personal')

.config(function ($stateProvider) {
    $stateProvider
        .state('app.personal.goals', {
            url: '/goals',
            views: {
                "content@app": {
                    templateUrl: 'app/personal/goals/goals.tpl.html'
                }
            }
        })
        .state('app.personal.goals.items', {
            url: '/items?type',
            views: {
                "goals@app.personal.goals": {
                    templateUrl: 'app/personal/goals/views/goals-items.tpl.html',
                    controller: 'GoalsController',
                    controllerAs: 'goalsCtrl'
                }
            },
            data: {
                title: 'Goals'
            },
            resolve: {
                model: function (goalsService, $stateParams) {
                    return goalsService.getAll({ type: $stateParams.type });
                },
                availableAccounts: function (accountsService) {
                    return accountsService.getAvailableForGoals();
                }
            }
        })
        .state('app.personal.goals.details', {
            url: '/details',
            views: {
                "goals@app.personal.goals": {
                    templateUrl: 'app/personal/goals/views/details-template.tpl.html'
                },
                "sidebar@app.personal.goals.details": {
                    templateUrl: 'app/personal/goals/views/goals-sidebar.tpl.html',
                    controller: 'GoalsController',
                    controllerAs: 'goalsCtrl'
                }
            },
            data: {
                title: 'Goals Details'
            },
            resolve: {
                model: function (goalsService) {
                    return goalsService.getAll();
                },
                availableAccounts: function () {}
            }
        })
        .state('app.personal.goals.details.item', {
            url: '/item?goalId',
            views: {
                "details@app.personal.goals.details": {
                    templateUrl: 'app/personal/goals/views/goal-details.tpl.html',
                    controller: 'GoalDetailsController',
                    controllerAs: 'goalDetailsCtrl'
                }
            },
            data: {
                title: 'Goals Details'
            },
            resolve: {
                goal: function (goalsService, $stateParams) {
                    return goalsService.getById($stateParams.goalId);
                }
            }
        })



        //Add/edit states
        .state('app.personal.goals.items.add', {
            url: '/add',
            views: {
                "edit@app.personal.goals": {
                    template: '<div ui-view="content"></div>'
                }
            }
        })
        .state('app.personal.goals.items.add.choose', {
            url: '/choose',
            views: {
                "content@app.personal.goals.items.add": {
                    templateUrl: 'app/personal/goals/views/edit/add-choose.tpl.html'
                }
            }
        })
        .state('app.personal.goals.items.add.emergency-step1', {
            url: '/emergency/step1',
            views: {
                "content@app.personal.goals.items.add": {
                    templateUrl: 'app/personal/goals/views/edit/emergency-step1.tpl.html',
                    controller: 'EmergencyStep1Controller',
                    controllerAs: 'emergencyStep1Ctrl'
                }
            },
            resolve: {
                model: function (estimateService, $stateParams) {
                    return $stateParams.model || estimateService.getEmergencyModel();
                }
            },
            params: {
                model: null
            }
        })
        .state('app.personal.goals.items.add.retirement-step1', {
            url: '/retirement/step1',
            views: {
                "content@app.personal.goals.items.add": {
                    templateUrl: 'app/personal/goals/views/edit/retirement-step1.tpl.html',
                    controller: 'RetirementStep1Controller',
                    controllerAs: 'retirementStep1Ctrl'
                }
            },
            resolve: {
                model: function (estimateService, $stateParams) {
                    return $stateParams.model || estimateService.getRetirementModel();
                }
            },
            params: {
                model: null
            }
        })
        .state('app.personal.goals.items.add.buyhome-step1', {
            url: '/home/step1',
            views: {
                "content@app.personal.goals.items.add": {
                    templateUrl: 'app/personal/goals/views/edit/home-step1.tpl.html',
                    controller: 'HomeStep1Controller',
                    controllerAs: 'homeStep1Ctrl'
                }
            },
            resolve: {
                model: function (estimateService, $stateParams) {
                    return $stateParams.model || estimateService.getHomeModel();
                }
            },
            params: {
                model: null
            }
        })
        .state('app.personal.goals.items.add.car-step1', {
            url: '/car/step1',
            views: {
                "content@app.personal.goals.items.add": {
                    templateUrl: 'app/personal/goals/views/edit/car-step1.tpl.html',
                    controller: 'CarStep1Controller',
                    controllerAs: 'carStep1Ctrl'
                }
            },
            resolve: {
                model: function (estimateService, $stateParams) {
                    return $stateParams.model || estimateService.getCarModel();
                }
            },
            params: {
                model: null
            }
        })
        .state('app.personal.goals.items.add.college-step1', {
            url: '/college/step1',
            views: {
                "content@app.personal.goals.items.add": {
                    templateUrl: 'app/personal/goals/views/edit/college-step1.tpl.html',
                    controller: 'CollegeStep1Controller',
                    controllerAs: 'collegeStep1Ctrl'
                }
            },
            resolve: {
                model: function (estimateService, $stateParams) {
                    return $stateParams.model || estimateService.getCollegeModel();
                }
            },
            params: {
                model: null
            }
        })
        .state('app.personal.goals.items.add.trip-step1', {
            url: '/trip/step1',
            views: {
                "content@app.personal.goals.items.add": {
                    templateUrl: 'app/personal/goals/views/edit/trip-step1.tpl.html',
                    controller: 'TripStep1Controller',
                    controllerAs: 'tripStep1Ctrl'
                }
            },
            resolve: {
                model: function (estimateService, $stateParams) {
                    return $stateParams.model || estimateService.getTripModel();
                }
            },
            params: {
                model: null
            }
        })
        .state('app.personal.goals.items.add.improvehome-step1', {
            url: '/improvehome/step1',
            views: {
                "content@app.personal.goals.items.add": {
                    templateUrl: 'app/personal/goals/views/edit/improvehome-step1.tpl.html',
                    controller: 'ImprovehomeStep1Controller',
                    controllerAs: 'improvehomeStep1Ctrl'
                }
            },
            resolve: {
                model: function (estimateService, $stateParams) {
                    return $stateParams.model || estimateService.getImprovehomeModel();
                }
            },
            params: {
                model: null
            }
        })
        .state('app.personal.goals.items.add.custom-step1', {
            url: '/custom/step1',
            views: {
                "content@app.personal.goals.items.add": {
                    templateUrl: 'app/personal/goals/views/edit/custom-step1.tpl.html',
                    controller: 'CustomStep1Controller',
                    controllerAs: 'customStep1Ctrl'
                }
            },
            resolve: {
                model: function (estimateService, $stateParams) {
                    return $stateParams.model || estimateService.getCustomModel();
                }
            },
            params: {
                model: null
            }
        })
        .state('app.personal.goals.items.add.step2', {
            url: '/step2',
            views: {
                "content@app.personal.goals.items.add": {
                    templateUrl: 'app/personal/goals/views/edit/step2.tpl.html',
                    controller: 'Step2Controller',
                    controllerAs: 'step2Ctrl'
                }
            },
            resolve: {
                prevState: function ($state) {
                    return {
                        name: $state.current.name
                    }                                   
                }
            },                              
            params: {
                model: null,
                prevStepModel: null
            }
        });
});