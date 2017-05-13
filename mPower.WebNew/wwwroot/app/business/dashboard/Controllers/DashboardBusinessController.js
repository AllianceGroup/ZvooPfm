'use strict';

angular.module('app.business').controller('DashboardBusinessController', ['model', 'businessDashboardService', '_',
    function(model, businessDashboardService, _){
        var ctrl = this;

        ctrl.model = model;

        ctrl.deleteAlert = function(id){
            businessDashboardService.deleteAlert(id).then(function(){
                ctrl.model.Alerts = _.filter(ctrl.model.Alerts, function(alert){
                    return alert.Id != id;
                });
            });
        };
    }]);