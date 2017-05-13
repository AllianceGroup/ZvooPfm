angular.module('app').filter('ifEmpty', [function () {
    return function (input, defaultValue) {
        if (angular.isUndefined(defaultValue) || defaultValue === null || defaultValue === '') {
            defaultValue = 'n/a';
        }

        if (angular.isUndefined(input) || input === null || input === '') {
            return defaultValue;
        }

        return input;
    };
}]);