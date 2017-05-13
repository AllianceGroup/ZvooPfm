'use strict';

angular.module('app.affiliateAdmin').controller('DeleteSegmentController', ['$uibModalInstance', 'model',
    function($uibModalInstance, model){

        var ctrl = this;
        ctrl.segment = model.segment;
        ctrl.actionFunction = model.actionFunction;

        ctrl.dismiss = function() {
            $uibModalInstance.dismiss('cancel');
        };

        ctrl.deleteSegment = function(id){
            ctrl.actionFunction(id).then(function(){
                $uibModalInstance.close(id);
            });
        };
    }]);