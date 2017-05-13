'use strict';

angular.module('app.personal').controller('RealEstateController',
['realEstateService', 'model', '$uibModal', '_', '$uibModalInstance', '$scope', '$rootScope', function (realEstateService, model, $uibModal, _, $uibModalInstance, $scope, $rootScope) {

    var ctrl = this;

    ctrl.model = model.Items;
    ctrl.ShowAddingField = false;
    ctrl.showSaveField = false;

    ctrl.searchModel = {};
    ctrl.saveModel = {};
    ctrl.errors = [];

    ctrl.delete = function (id) {
        var modalInstance = $uibModal.open({
            templateUrl: 'app/personal/goals/views/modalTemplate.tpl.html',
            controller: 'ModalController',
            controllerAs: 'ctrl',
            resolve: {
                model: function () {
                    return {
                        Title: 'Delete confirmation',
                        Text: 'Are you sure?'
                    }
                }
            }
        });
        modalInstance.result.then(function () {
            realEstateService.delete(id).then(function () {
                ctrl.model = _.reject(ctrl.model, function (val) { return val.Id === id });
                $.smallBox({
                    title: "Success",
                    content: "Item removed",
                    color: "#739e73",
                    timeout: 4000
                });
                $rootScope.$emit('updateAccounts', {});
            }, function(errors) {
                for (var key in errors) {
                    if (errors.hasOwnProperty(key)) {
                        for (var i = 0; i < errors[key].length; i++) {
                            $.smallBox({
                                title: "Error",
                                content: errors[key][i],
                                color: "#296191",
                                iconSmall: "fa fa-warning",
                                timeout: 4000
                            });
                        }
                    }
                }
            });
        });
    };

    ctrl.search = function() {
        ctrl.errors = [];
        realEstateService.search(ctrl.searchModel.address, ctrl.searchModel.zip).then(function(items) {
            ctrl.items = items;
        }, function(errors) {
            for (var key in errors) {
                if (errors.hasOwnProperty(key)) {
                    for (var i = 0; i < errors[key].length; i++) {
                        ctrl.errors.push(errors[key][i]);
                    }
                }
            }
        });
    };

    ctrl.addRealEstate = function(){
        ctrl.searchModel = {};
        ctrl.items = {};
        ctrl.ShowAddingField = true;
        ctrl.showSaveField = false
    };

    ctrl.getModalRealEstate = function(model){
        if(model){
            realEstateService.getZillowProperty(model).then(function(saveModel){
                ctrl.saveModel = saveModel;
            });
        }
        else
            ctrl.saveModel = {};

        ctrl.showSaveField = true;
        ctrl.ShowAddingField= false;
    };

    ctrl.save = function () {
        ctrl.errors = [];
        realEstateService.saveZillowProperty(ctrl.saveModel).then(function (model) {
            ctrl.model.push(model);
            ctrl.showSaveField = false;
            ctrl.ShowAddingField = false;         
        }, function(errors) {
            for (var key in errors) {
                if (errors.hasOwnProperty(key)) {
                    for (var i = 0; i < errors[key].length; i++) {
                        ctrl.errors.push(errors[key][i]);
                    }
                }
            }
        });
    };

    ctrl.dismiss = function() {
        $uibModalInstance.dismiss();
    };

    ctrl.incudeInWorth = function(realEster) {
        realEstateService.incudeInWorth(realEster).then(function () {
            $rootScope.$emit('updateAccounts', {});
        });
    };
}]);