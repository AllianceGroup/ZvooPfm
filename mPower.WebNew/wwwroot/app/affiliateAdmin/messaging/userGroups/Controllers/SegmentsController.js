"use strict";

angular.module('app.affiliateAdmin').controller('SegmentsController', function SegmentsController(segmentsList, messagesService, _, $uibModal, $state, authService){
    var ctrl = this;
    ctrl.segments = segmentsList.Segments;
    ctrl.roles = authService.getRoles();

    ctrl.sendMail = function(id){
        $state.go('app.affiliateAdmin.sendmail', {Id: id});
    };

    ctrl.delete = function(id){
        var modalInstance = $uibModal.open({
            templateUrl: 'app/affiliateAdmin/messaging/userGroups/views/deleteSegment.html',
            controller: 'DeleteSegmentController',
            controllerAs: 'deleteSegmentCtrl',
            resolve: {
                model: function () {
                    return {
                        segment : _.findWhere(ctrl.segments, {Id: id}),
                        actionFunction: messagesService.deleteSegment
                    }
                }
            }
        });

        modalInstance.result.then(function(id){
           ctrl.segments = _.filter(ctrl.segments, function(segment){return segment.Id != id});
        });
    };
});