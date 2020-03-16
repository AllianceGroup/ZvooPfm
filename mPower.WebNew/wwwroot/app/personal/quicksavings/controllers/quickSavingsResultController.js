'use strict';

angular.module('app.personal').controller('QuickSavingsResultController',
    ['quickSavingsService', 'stepModel', '$scope',
        function (quickSavingsService, stepModel, $scope) {            
            var ctrl = this;
            ctrl.stepModel = stepModel;
                ctrl.stepModel.DisplayMode = ctrl.stepModel.DisplayMode.toString();
        }]);




'use strict';
angular.module('app.personal').config(function ($stateProvider) {
    $stateProvider

 .state('app.personal.quickSavingsResult', {
     url: '/quickSavingsResult',
     views: {
         "calendar@app": {
             templateUrl: 'app/personal/quicksavings/views/quickSavingsResult.tpl.html',
             controller: 'QuickSavingsResultController',
             controllerAs: 'quickSavingsResultCtrl'
         }
     },
     data: {
         title: 'Quick Saving Result'
     },
     resolve: {
         stepModel: function (quickSavingsService) {             
             return quickSavingsService.quickSavingResults();
         },
     }
 });
});
