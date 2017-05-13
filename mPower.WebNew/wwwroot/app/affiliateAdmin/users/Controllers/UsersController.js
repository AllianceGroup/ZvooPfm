'use strict';

angular.module('app.affiliateAdmin').controller('UsersController', ['$uibModal', 'usersService', 'usersList', '_', 'FileSaver', 'authService',
    function($uibModal, usersService, usersList, _, FileSaver, authService){
        var ctrl = this;

        ctrl.UsersList = usersList;
        ctrl.roles = authService.getRoles();

        ctrl.toggleUserIsActive = function(id){
            usersService.toggleUserIsActive(id);
            var currentUser = _.findWhere(ctrl.UsersList.Users, {Id: id});
            currentUser.IsActive = !currentUser.IsActive;
        };

        ctrl.exportUsers = function(){
            usersService.exportCSV().then(function(data){
                var file = new Blob([data], { type: 'application/pdf' });
                FileSaver.saveAs(file, 'UserReport.csv');
            });
        };

        ctrl.refreshUsers = function(){
            var model = {PageNumber: ctrl.UsersList.Paging.CurrentPage, SearchKey: ctrl.SearchKey};
            usersService.refreshUsers(model).then(function(usersList){
                ctrl.UsersList = usersList;
            });
        };

        ctrl.addUser = function(){
            var modalInstance = $uibModal.open({
                templateUrl: 'app/affiliateAdmin/users/views/addUser.html',
                controller: 'AddUserController',
                controllerAs: 'addUserCtrl',
                resolve: {
                    model: function () {
                        return {
                            actionFunction: usersService.addUser
                        }
                    }
                }
            });
            modalInstance.result.then(function (user) {
                ctrl.UsersList.Users.unshift({Id: user.UserId, CreateDate: user.CreateDate,
                    FullName: user.FirstName + ' ' + user.LastName, UserName: user.UserName, IsActive: user.IsActive});
            });
        };

        ctrl.changeAdviser = function () {
            var modalInstance = $uibModal.open({
                templateUrl: 'app/affiliateAdmin/users/views/changeAdviser.html',
                controller: 'ChangeAdviserController',
                controllerAs: 'changeAdviserCtrl',
                animation: false
            });
        };

        ctrl.editUser = function(id){
            usersService.editUser(id).then(function(user){
                var modalInstance = $uibModal.open({
                    templateUrl: 'app/affiliateAdmin/users/views/editUser.html',
                    controller: 'EditUserController',
                    controllerAs: 'editUserCtrl',
                    resolve: {
                        model: function () {
                            return {
                                user : user,
                                actionFunction: usersService.updateUser
                            }
                        }
                    }
                });
                modalInstance.result.then(function (usersList) {
                    ctrl.UsersList = usersList;
                });
            });
        };

        ctrl.activity = function(id, pageNumber){
            usersService.activity({id: id, pageNumber: pageNumber}).then(function(user){
                $uibModal.open({
                    templateUrl: 'app/affiliateAdmin/users/views/activityUser.html',
                    controller: 'ActivityController',
                    controllerAs: 'activityCtrl',
                    size: 'lg',
                    resolve: {
                        model: function () {
                            return { user : user }
                        }
                    }
                });
            });
        };

        ctrl.userInfo = function(id){
            $uibModal.open({
                templateUrl: 'app/affiliateAdmin/users/views/userInfo.html',
                controller: 'UserInfoController',
                controllerAs: 'userInfoCtrl',
                size: 'lg',
                resolve:{
                    user: function(usersService){
                        return usersService.userInfo(id);
                    }
                }
            });
        };

        ctrl.confirmDelete = function(id){
            var  modalInstance = $uibModal.open({
                templateUrl: 'app/affiliateAdmin/users/views/deleteUser.html',
                controller: 'DeleteUserController',
                controllerAs: 'deleteUserCtrl',
                resolve: {
                    model: function () {
                        return {
                            user : _.findWhere(ctrl.UsersList.Users, {Id: id}),
                            actionFunction: usersService.deleteUser
                        }
                    }
                }
            });

            modalInstance.result.then(function (id) {
                ctrl.UsersList.Users = _.filter(ctrl.UsersList.Users, function(user){
                    return user.Id != id;
                });
            });
        };
}]);