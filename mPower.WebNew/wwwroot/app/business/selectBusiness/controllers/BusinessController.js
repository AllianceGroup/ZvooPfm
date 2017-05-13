'use strict';

angular.module('app.business').controller('BusinessController', ['authService', 'listBusiness', 'businessService', '$uibModal', '$state',
    function(authService, listBusiness, businessService, $uibModal, $state){
        var ctrl = this;

        ctrl.listBusiness = listBusiness;

        ctrl.addBusiness = function(){
            businessService.getBusiness().then(function(business){
                var modalInstance = $uibModal.open({
                    templateUrl: 'app/business/selectBusiness/views/addBusiness.html',
                    controller: 'AddBusinessController',
                    controllerAs: 'addBusinessCtrl',
                    resolve: {
                        model: function(){
                            return{
                                business: business,
                                actionFunction: businessService.addBusiness
                            }
                        }
                    }
                });

                modalInstance.result.then(function(business){
                    ctrl.listBusiness.Companies.unshift(business);
                });
            });
        };

        ctrl.showDashboard = function(state, ledgerId){
            authService.changeLedger(ledgerId).then(function(){
                $state.go(state);
            });
        };
    }]);