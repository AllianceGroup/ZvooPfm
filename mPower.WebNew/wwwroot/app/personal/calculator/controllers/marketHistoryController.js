'use strict';
angular.module('app.personal').controller('marketHistoryController',
['calculatorService', 'MarketHistoryCalculatorData', '$uibModal', function (calculatorService, MarketHistoryCalculatorData, $uibModal) {
    var ctrl = this;

    ctrl.MarketHistoryCalculatorData = MarketHistoryCalculatorData;
    calculatorService.getDefaultModel().then(function (model) {
        ctrl.MarketHistoryCalculatorData.ListofData = model.ListofData;
    });
}]);
