'use strict';

angular.module('app.personal').controller('SearchController', ['intuitService', '_', '$uibModal', '$uibModalInstance',
    function (intuitService, _, $uibModal, $uibModalInstance) {
        var ctrl = this;

        ctrl.isSearching = false;
        ctrl.errors = [];
        ctrl.institutions = [];
        ctrl.institutionsFull = [];
        ctrl.searchText = "";
        ctrl.currentPage = 1;
        ctrl.intemsPerPage = 10;

        ctrl.dismiss = function() {
            $uibModalInstance.dismiss();
        };
        ctrl.close = function(){
            $uibModalInstance.close();
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
            intuitService.authenticate(id).then(function(model) {
                $uibModalInstance.close();
                $uibModal.open({
                    templateUrl: 'app/personal/accounts/views/intuit-authenticate.tpl.html',
                    controller: 'AuthenticateController',
                    controllerAs: 'intuitCtrl',
                    resolve:{
                        model: model
                    }
                });
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
    }]);