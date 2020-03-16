'use strict';

angular.module('app.agentUser').controller('UsersAgentController', ['usersList', '$uibModal', '_', 'usersAgentService', '$state', 'FileSaver', 'authService',
    function (usersList, $uibModal, _, usersAgentService, $state, FileSaver, authService) {
        var ctrl = this;
        ctrl.errors = [];
        ctrl.usersList = usersList;
        ctrl.ids = {};
        ctrl.roles = authService.getRoles();

        ctrl.toggleUserIsActive = function(id){
            var currentUser = _.findWhere(ctrl.usersList.Users, {Id: id});

            if(currentUser.IsActive)
                usersAgentService.deactivateUser(id);
            else
                usersAgentService.activateUser(id);

            currentUser.IsActive = !currentUser.IsActive;
        };

        ctrl.toggleIdGuardEnrollment = function(id){
            var currentUser = _.findWhere(ctrl.usersList.Users, {Id: id});

            usersAgentService.toggleIdGuardEnrollment(id).then(function(){
                currentUser.IsEnrolled = !currentUser.IsEnrolled;
            });
        };

        ctrl.loginAsUser = function(id){
            var currentUser = _.findWhere(ctrl.usersList.Users, {Id: id});
            usersAgentService.loginAsUser(id, currentUser.Email).then(function(model){
                if (model.authenticated)
                    $state.go('app.personal.dashboard');
            }, function(errors){
                ctrl.errors = _.reduce(errors, function(result, arr) {
                    return result.concat(arr);
                }, []);
            });
        };

        ctrl.bulk = function(){
            if(ctrl.bulkOption === undefined || ctrl.ids === undefined)
                return;

            var ids = [], k = 0;
            for(var id in ctrl.ids){
                if(ctrl.ids[id] == true)
                    ids[k++] = id;
            }
            if(k === 0)
                return;

            if(ctrl.bulkOption == "Delete")
                usersAgentService.bulkDelete(ids).then(function(){
                    ctrl.usersList.Users = _.filter(ctrl.usersList.Users, function(user){
                            for(var i = 0; i < ids.length; i++){
                                if(user.Id == ids[i])
                                    return false;
                            }
                            return true;
                        });
                });
            if(ctrl.bulkOption == "Deactivate")
                usersAgentService.bulkDeactivate(ids).then(function(){
                    for(var i = 0; i < ids.length; i++){
                        for(var j = 0; j < ctrl.usersList.Users.length; j++){
                            if(ids[i] == ctrl.usersList.Users[j].Id)
                                ctrl.usersList.Users[j].IsActive = false;
                        }
                    }
                });
            if(ctrl.bulkOption == "Activate")
                usersAgentService.bulkActivate(ids).then(function(){
                    for(var i = 0; i < ids.length; i++){
                        for(var j = 0; j < ctrl.usersList.Users.length; j++){
                            if(ids[i] == ctrl.usersList.Users[j].Id)
                                ctrl.usersList.Users[j].IsActive = true;
                        }
                    }
                });

            ctrl.bulkOption = "";
            ctrl.ids = "";
        };

        ctrl.exportUsers = function(){
            usersAgentService.exportCSV().then(function(data){
                var file = new Blob([data], { type: 'application/pdf' });
                FileSaver.saveAs(file, 'UserReport.csv');
            });
        };

        ctrl.addUser = function(){
            usersAgentService.getUsers().then(function (user) {
                var modalInstance = $uibModal.open({
                    templateUrl: 'app/agent/users/views/addUser.html',
                    controller: 'AddUserAgentController',
                    controllerAs: 'addUserAgentCtrl',
                    resolve: {
                        model: function () {
                            return {
                                actionFunction: usersAgentService.addUser,
                                user: user
                            }
                        }
                    }
                });
                modalInstance.result.then(function (user) {
                    ctrl.usersList.Users.unshift(user);
                });
            });
        };

        ctrl.editProfile = function(id){
            usersAgentService.getProfile(id).then(function(user){
                var modalInstance = $uibModal.open({
                    templateUrl: 'app/agent/users/views/editUser.html',
                    controller: 'EditUserAgentController',
                    controllerAs: 'editUserAgentCtrl',
                    resolve: {
                        model: function () {
                            return {
                                actionFunction: usersAgentService.updateProfile,
                                user: user
                            }
                        }
                    }
                });

                modalInstance.result.then(function(user) {
                    for(var i = 0; i < ctrl.usersList.Users.length; i++){
                        if(ctrl.usersList.Users[i].Id == user.Id){
                            ctrl.usersList.Users[i] = user;
                            break;
                        }
                    }
                });
            })
        };

        ctrl.refreshUsers = function(){
            var model = {PageNumber: ctrl.usersList.Paging.CurrentPage, SearchKey: ctrl.SearchKey, Affiliate: ctrl.Affiliate};
            usersAgentService.refreshUsers(model).then(function(usersList){
                ctrl.usersList = usersList;
            });
        };

        ctrl.getUserDetails = function(id){
            $uibModal.open({
                templateUrl: 'app/agent/users/views/userInfo.html',
                controller: 'UserInfoAgentController',
                controllerAs: 'UserInfoAgentCtrl',
                size:'lg',
                resolve: {
                    user: function(usersAgentService){
                        return usersAgentService.getUserDetails(id);
                    }
                }
            });
        };

        ctrl.confirmDelete = function(id){
            var modalInstance = $uibModal.open({
                templateUrl: 'app/agent/users/views/deleteUser.html',
                controller: 'DeleteUserAgentController',
                controllerAs: 'deleteUserAgentCtrl',
                resolve: {
                    model: function(){
                        return {
                            user: _.findWhere(ctrl.usersList.Users, {Id: id}),
                            actionFunction: usersAgentService.deleteUser
                        }
                    }
                }
            });

            modalInstance.result.then(function (id) {
                ctrl.usersList.Users = _.filter(ctrl.usersList.Users, function(user){
                    return user.Id != id;
                });
            });
        };
    }]);