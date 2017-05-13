angular.module('app').filter('centsCurrency', ["$filter", function ($filter) {
    return function (amount, currencySymbol) {
        var currency = $filter('currency');
        amount = amount / 100;

        return currency(amount, currencySymbol);
    };
}]);