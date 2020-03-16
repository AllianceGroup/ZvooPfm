'use strict';

angular.module('app.auth').controller('AuthController', ['authService', '$location', '$state', '$uibModal', 'signalsService', 'USER_ROLES', '_',
    function (authService, $location, $state, $uibModal, signalsService, USER_ROLES, _) {

        var ctrl = this;
        signalsService.initialize();
        ctrl.availableRoles = USER_ROLES;
        ctrl.roles = authService.authentication.claims.role;

        ctrl.getSelectedRole = function(){
            var level;
            var path = $location.path();
            if (path.indexOf("/business") === 0) {
                level = ctrl.availableRoles.bfm;
            } else if (path.indexOf("/AffiliateAdmin") === 0) {
                level = ctrl.availableRoles.affiliate;
            } else if (path.indexOf("/GlobalAdmin") === 0) {
                level = ctrl.availableRoles.global;
            } else if (path.indexOf("/agent") === 0) {
                level = ctrl.availableRoles.agent;
            }
            else
                level = ctrl.availableRoles.pfm;

            return level;
        };

        ctrl.isRoleAvailable = function (role) {
            return _.contains(ctrl.roles, role.value);
        };

        ctrl.logOut = function () {
            authService.logOut();
            signalsService.disconnect();
            $location.path('/login');
        };

        ctrl.authentication =  function() {
            return authService.authentication;
        };

        ctrl.getRealEstate = function() {
            $uibModal.open({
                templateUrl: 'app/personal/profile/views/realestate.tpl.html',
                controller: 'RealEstateController',
                controllerAs: 'realEstateCtrl',
                size: 'lg',
                resolve: {
                    model: function (realEstateService) {
                        return realEstateService.getAll();
                    }
                }
            });
        };

        ctrl.getFinancialInfo = function() {
            $uibModal.open({
                templateUrl: 'app/personal/profile/views/financial.tpl.html',
                controller: 'FinancialController',
                controllerAs: 'financialCtrl',
                size: 'lg',
                resolve: {
                    model: function (accountsService) {
                        return accountsService.getFinancialAccounts();
                    }
                }
            });
        };

        ctrl.getMembership = function(){
            $uibModal.open({
                templateUrl: 'app/personal/profile/views/membership.tpl.html',
                controller: 'MembershipController',
                controllerAs: 'membershipCtrl',
                size: 'lg',
                resolve: {
                    model: function (profileService) {
                        return profileService.getMembership();
                    }
                }
            });
        };

        ctrl.getAlerts = function(){
            $uibModal.open({
                templateUrl: 'app/personal/profile/views/alerts.tpl.html',
                controller: 'AlertsController',
                controllerAs: 'alertsCtrl',
                size: 'lg',
                resolve: {
                    model: function (profileService) {
                        return profileService.getAllAlerts();
                    }
                }
            });
        };

        ctrl.getProfile = function(){
            $uibModal.open({
                templateUrl: 'app/personal/profile/views/profiletab.tpl.html',
                controller: 'ProfileController',
                controllerAs: 'profileCtrl',
                size: 'lg',
                resolve: {
                    model:function(profileService) {
                        return profileService.getProfileModel();
                    }
                }
            });
        };
    }]);