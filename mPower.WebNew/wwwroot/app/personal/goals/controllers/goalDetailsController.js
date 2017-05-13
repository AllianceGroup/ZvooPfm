'use strict';

angular.module('app.personal').controller('GoalDetailsController',
['goalsService', 'goal', '$uibModal', '$state', '$scope', function (goalsService, goal, $uibModal, $state, $scope) {

    var ctrl = this;

    ctrl.model = goal;

    ctrl.delete = function() {
        var modalInstance = $uibModal.open({
            templateUrl: 'app/personal/goals/views/modalTemplate.tpl.html',
            controller: 'ModalController',
            controllerAs: 'ctrl',
            resolve: {
                model: function () {
                    return {
                        Title: 'Please confirm goal deletion',
                        Text: 'Are you sure?'
                    }
                }
            }
        });
        modalInstance.result.then(function () {
            goalsService.delete(ctrl.model.Id).then(function() {
                $state.go('app.personal.goals.items');
            });
        });
    };

    ctrl.markAsComplete = function() {
        var modalInstance = $uibModal.open({
            templateUrl: 'app/personal/goals/views/modalTemplate.tpl.html',
            controller: 'ModalController',
            controllerAs: 'ctrl',
            resolve: {
                model: function () {
                    return {
                        Title: 'Please confirm goal completion',
                        Text: 'Congratulations! One important note: marking this goal as "complete" will unlink any linked accounts, freeing them up for future goals.'
                    }
                }
            }
        });
        modalInstance.result.then(function () {
            goalsService.complete(ctrl.model.Id).then(function () {
                $state.go($state.current, {}, {reload: true});
            });
        });
    };

    ctrl.edit = function() {
        goalsService.edit(ctrl.model.Id).then(function(model) {
            $state.go('app.personal.goals.items.add.step2', { model: model });
        });
    };

    ctrl.archive = function() {
        var modalInstance = $uibModal.open({
            templateUrl: 'app/personal/goals/views/modalTemplate.tpl.html',
            controller: 'ModalController',
            controllerAs: 'ctrl',
            resolve: {
                model: function () {
                    return {
                        Title: 'Confirm',
                        Text: 'Are you sure you want to archive this goal? All archived goals can be viewed by clicking the "view archived goals" button.'
                    }
                }
            }
        });
        modalInstance.result.then(function () {
            goalsService.archive(ctrl.model.Id).then(function () {
                $state.go('app.personal.goals.items', {}, {reload: true});
            });
        });

    };
}]);