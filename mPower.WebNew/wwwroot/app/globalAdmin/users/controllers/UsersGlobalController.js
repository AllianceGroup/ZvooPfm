'use strict';

angular.module('app.globalAdmin').controller('UsersGlobalController', ['usersList', '$uibModal', '_', 'usersGlobalService', '$state', 'FileSaver', 'authService',
    function(usersList, $uibModal, _, usersGlobalService, $state, FileSaver, authService){
        var ctrl = this;
        ctrl.errors = [];
        ctrl.usersList = usersList;
        ctrl.ids = {};
        ctrl.roles = authService.getRoles();

        ctrl.toggleUserIsActive = function(id){
            var currentUser = _.findWhere(ctrl.usersList.Users, {Id: id});

            if(currentUser.IsActive)
                usersGlobalService.deactivateUser(id);
            else
                usersGlobalService.activateUser(id);

            currentUser.IsActive = !currentUser.IsActive;
        };

        ctrl.toggleIdGuardEnrollment = function(id){
            var currentUser = _.findWhere(ctrl.usersList.Users, {Id: id});

            usersGlobalService.toggleIdGuardEnrollment(id).then(function(){
                currentUser.IsEnrolled = !currentUser.IsEnrolled;
            });
        };

        ctrl.loginAsUser = function(id){
            var currentUser = _.findWhere(ctrl.usersList.Users, {Id: id});
            usersGlobalService.loginAsUser(id, currentUser.Email).then(function(model){
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
                usersGlobalService.bulkDelete(ids).then(function(){
                    ctrl.usersList.Users = _.filter(ctrl.usersList.Users, function(user){
                            for(var i = 0; i < ids.length; i++){
                                if(user.Id == ids[i])
                                    return false;
                            }
                            return true;
                        });
                });
            if(ctrl.bulkOption == "Deactivate")
                usersGlobalService.bulkDeactivate(ids).then(function(){
                    for(var i = 0; i < ids.length; i++){
                        for(var j = 0; j < ctrl.usersList.Users.length; j++){
                            if(ids[i] == ctrl.usersList.Users[j].Id)
                                ctrl.usersList.Users[j].IsActive = false;
                        }
                    }
                });
            if(ctrl.bulkOption == "Activate")
                usersGlobalService.bulkActivate(ids).then(function(){
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
            usersGlobalService.exportCSV().then(function(data){
                var file = new Blob([data], { type: 'application/pdf' });
                FileSaver.saveAs(file, 'UserReport.csv');
            });
        };

        ctrl.addUser = function(){
            usersGlobalService.getUser().then(function(user){
                var modalInstance = $uibModal.open({
                    templateUrl: 'app/globalAdmin/users/views/addUser.html',
                    controller: 'AddUserGlobalController',
                    controllerAs: 'addUserGlobalCtrl',
                    resolve: {
                        model: function () {
                            return {
                                actionFunction: usersGlobalService.addUser,
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
            usersGlobalService.getProfile(id).then(function(user){
                var modalInstance = $uibModal.open({
                    templateUrl: 'app/globalAdmin/users/views/editUser.html',
                    controller: 'EditUserGlobalController',
                    controllerAs: 'editUserGlobalCtrl',
                    resolve: {
                        model: function () {
                            return {
                                actionFunction: usersGlobalService.updateProfile,
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
            usersGlobalService.refreshUsers(model).then(function(usersList){
                ctrl.usersList = usersList;
            });
        };

        ctrl.getUserDetails = function(id){
            $uibModal.open({
                templateUrl: 'app/globalAdmin/users/views/userInfo.html',
                controller: 'UserInfoGlobalController',
                controllerAs: 'UserInfoGlobalCtrl',
                size:'lg',
                resolve: {
                    user: function(usersGlobalService){
                        return usersGlobalService.getUserDetails(id);
                    }
                }
            });
        };

        ctrl.confirmDelete = function(id){
            var modalInstance = $uibModal.open({
                templateUrl: 'app/globalAdmin/users/views/deleteUser.html',
                controller: 'DeleteUserGlobalController',
                controllerAs: 'deleteUserGlobalCtrl',
                resolve: {
                    model: function(){
                        return {
                            user: _.findWhere(ctrl.usersList.Users, {Id: id}),
                            actionFunction: usersGlobalService.deleteUser
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