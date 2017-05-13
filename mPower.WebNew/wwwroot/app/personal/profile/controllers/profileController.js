'use strict';

angular.module('app.personal').controller('ProfileController',
['profileService', '$uibModal', '_', 'model', '$uibModalInstance', function (profileService, $uibModal, _, model, $uibModalInstance) {

    var ctrl = this;
    var handleErrors = function(errors, errorsContainer) {
        for (var key in errors) {
            if (errors.hasOwnProperty(key)) {
                for (var i = 0; i < errors[key].length; i++) {
                    errorsContainer.push(errors[key][i]);
                }
            }
        }
    };

    ctrl.model = model;
    ctrl.model.ChangePasswordModel = {};
    ctrl.model.SecurityLevel.SelectedLevel = ctrl.model.SecurityLevel.SelectedLevel !== null || !_.isUndefined(model.SecurityLevel.SelectedLevel)
        ? ctrl.model.SecurityLevel.SelectedLevel.toString()
        : ctrl.model.SecurityLevel.SelectedLevel;
    ctrl.model.UserDetails.Gender = ctrl.model.UserDetails.Gender !== null && !_.isUndefined(ctrl.model.UserDetails.Gender)
        ? ctrl.model.UserDetails.Gender.toString()
        : ctrl.model.UserDetails.Gender;


    ctrl.saveUserDetails = function () {
        ctrl.model.UserDetails.errors = [];
        ctrl.model.UserDetails.disabled = true;
        profileService.saveUserDetails(ctrl.model.UserDetails).then(function(message) {
            $.smallBox({
                title: "Success",
                content: message,
                color: "#739e73",
                timeout: 4000
            });
            ctrl.model.UserDetails.disabled = false;
        }, function(errors) {
            handleErrors(errors, ctrl.model.UserDetails.errors);
            ctrl.model.UserDetails.disabled = false;
        });
    };

    ctrl.changePassword = function() {
        ctrl.model.ChangePasswordModel.errors = [];
        ctrl.model.ChangePasswordModel.disabled = true;
        profileService.changePassword(ctrl.model.ChangePasswordModel).then(function (message) {
            $.smallBox({
                title: "Success",
                content: message,
                color: "#739e73",
                timeout: 4000
            });
            ctrl.model.ChangePasswordModel.disabled = false;
        }, function (errors) {
            handleErrors(errors, ctrl.model.ChangePasswordModel.errors);
            ctrl.model.ChangePasswordModel.disabled = false;
        });
    };

    ctrl.saveSecurity = function() {
        ctrl.model.SecurityLevel.errors = [];
        ctrl.model.SecurityLevel.disabled = true;
        profileService.saveSecurity({
            SelectedLevel: ctrl.model.SecurityLevel.SelectedLevel
        }).then(function (message) {
            $.smallBox({
                title: "Success",
                content: message,
                color: "#739e73",
                timeout: 4000
            });
            ctrl.model.SecurityLevel.disabled = false;
        }, function (errors) {
            handleErrors(errors, ctrl.model.SecurityLevel.errors);
            ctrl.model.SecurityLevel.disabled = false;
        });
    };

    ctrl.saveSequrityQuestion = function() {
        ctrl.model.SecurityQuestion.errors = [];
        ctrl.model.SecurityQuestion.disabled = true;
        profileService.saveSequrityQuestion({
            SecurityQuestion: ctrl.model.SecurityQuestion.SecurityQuestion,
            Answer: ctrl.model.SecurityQuestion.Answer
        }).then(function (message) {
            $.smallBox({
                title: "Success",
                content: message,
                color: "#739e73",
                timeout: 4000
            });
            ctrl.model.SecurityQuestion.disabled = false;
        }, function (errors) {
            handleErrors(errors, ctrl.model.SecurityQuestion.errors);
            ctrl.model.SecurityQuestion.disabled = false;
        });
    };

    ctrl.saveSequritySettings = function() {
        ctrl.model.SecuritySettings.errors = [];
        ctrl.model.SecuritySettings.disabled = true;
        profileService.saveSequritySettings(ctrl.model.SecuritySettings).then(function (message) {
            $.smallBox({
                title: "Success",
                content: message,
                color: "#739e73",
                timeout: 4000
            });
            ctrl.model.SecuritySettings.disabled = false;
        }, function (errors) {
            handleErrors(errors, ctrl.model.SecuritySettings.errors);
            ctrl.model.SecuritySettings.disabled = false;
        });
    };

    ctrl.dismiss = function() {
        $uibModalInstance.dismiss();
    };
}]);