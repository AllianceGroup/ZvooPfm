'use strict';

angular.module('app.affiliateAdmin').controller('ReportsController', function ReportsController(model, analyticsService, _, messagesService, $uibModal, authService){
    var ctrl = this;
    ctrl.model = model;
    ctrl.roles = authService.getRoles();

    for(var i = 0; i < ctrl.model.Segments.length; i++)
        ctrl.model.Segments.show = false;

    ctrl.showDetails = function(id){
        _.findWhere(ctrl.model.Segments, {Id: id}).show = true;
    };

    ctrl.hideDetails = function(id){
        _.findWhere(ctrl.model.Segments, {Id: id}).show = false;
    };

    ctrl.delete = function(id){
        var modalInstance = $uibModal.open({
            templateUrl: 'app/affiliateAdmin/messaging/userGroups/views/deleteSegment.html',
            controller: 'DeleteSegmentController',
            controllerAs: 'deleteSegmentCtrl',
            resolve: {
                model: function () {
                    return {
                        segment : _.findWhere(ctrl.model.Segments, {Id: id}),
                        actionFunction: messagesService.deleteSegment
                    }
                }
            }
        });

        modalInstance.result.then(function(id){
            ctrl.model.Segments = _.filter(ctrl.model.Segments, function(segment){return segment.Id != id});
        });
    };
});