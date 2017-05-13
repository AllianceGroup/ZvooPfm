'use strict';

angular.module('app.personal').controller('AlertsController',
['profileService', '$uibModal', '_', 'model', '$uibModalInstance', function (profileService, $uibModal, _, model, $uibModalInstance) {

    var ctrl = this;
    var initialize = function(model) {
        for (var i = 0; i < model.Alerts.length; i++) {
            model.Alerts[i].BorderValue = model.Alerts[i].BorderValue.toString();
        }
        return model;
    };
    var handleErrors = function (errors, errorsContainer) {
        for (var key in errors) {
            if (errors.hasOwnProperty(key)) {
                for (var i = 0; i < errors[key].length; i++) {
                    errorsContainer.push(errors[key][i]);
                }
            }
        }
    };

    ctrl.model = initialize(model);
    ctrl.newEmail = null;
    ctrl.newPhone = null;

    ctrl.addPhone = function() {
        ctrl.newPhone = "";
    };

    ctrl.deletePhone = function(item) {
        profileService.deletePhone(item).then(function() {
            ctrl.model.Phones = _.reject(ctrl.model.Phones, function (val) { return val === item });
        });
    };

    ctrl.savePhone = function () {
        ctrl.phoneErrors = [];
        profileService.savePhone(ctrl.newPhone).then(function () {
            ctrl.model.Phones.push(ctrl.newPhone);
            ctrl.newPhone = null;
        },function(errors) {
            handleErrors(errors, ctrl.phoneErrors);
        });
    };

    ctrl.addEmail = function () {
        ctrl.newEmail = "";
    };

    ctrl.deleteEmail = function (item) {
        profileService.deleteEmail(item.Value).then(function() {
            ctrl.model.Emails = _.reject(ctrl.model.Emails, function(val) { return val.Value === item.Value });
        });
    };

    ctrl.saveEmail = function () {
        ctrl.emailErrors = [];
        profileService.saveEmail(ctrl.newEmail).then(function () {
            ctrl.model.Emails.push({ Value: newEmail, IsMain: false });
            ctrl.newEmail = null;
        }, function (errors) {
            handleErrors(errors, ctrl.emailErrors);
        });
    };

    ctrl.changeAlerts = function (item) {
        ctrl.alertErrors = [];
        profileService.changeAlerts({
            type: item.Type,
            sendEmail: item.SendEmail,
            sendText: item.SendText,
            borderValue: item.BorderValue
        }).then(function() {

        }, function(errors) {
            handleErrors(errors, ctrl.alertErrors);
        });
    };

    ctrl.dismiss = function() {
        $uibModalInstance.dismiss();
    };
}]);