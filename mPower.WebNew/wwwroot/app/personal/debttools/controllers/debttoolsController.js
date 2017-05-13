'use strict';

angular.module('app.personal').controller('DebttoolsController',
['debttoolsService', 'debttoolsModel', '$uibModal', function (debttoolsService, debttoolsModel, $uibModal) {
    var ctrl = this;

    ctrl.debttoolsModel = debttoolsModel;
    FindNegativeValues(ctrl.debttoolsModel.DebtToIncomeRatio);

    ctrl.Formatter = function(value, label){
        if(label.negative === true)
            return '($' + value + ')';
        return '$' + value;
    };

    ctrl.addAccounts = function(){
        $uibModal.open({
            templateUrl: "app/personal/accounts/views/add-template.tpl.html",
            controller: 'ChoiceAccountController',
            controllerAs: 'choiceAccountCtrl'
        })
    };

    function FindNegativeValues(dataChart){
        if(dataChart){
            for(var j = 0; j < dataChart.length; j++){
                var currentChart = dataChart[j];
                for(var i = 0; i < currentChart.length; i++){
                    if(currentChart[i].value < 0){
                        currentChart[i].negative = true;
                        currentChart[i].value = Math.abs(currentChart[i].value);
                    }
                    else
                        currentChart[i].negative = false;
                }
            }
        }
    }
}]);