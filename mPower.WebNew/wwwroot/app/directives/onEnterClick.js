angular.module('app.personal').directive('onEnterClick', function ($parse) {
    return function (scope, element, attrs) {
        element.bind("keydown keypress", function (event) {
            if (event.which === 13) {
                scope.$apply(function () {
                    $parse(attrs.onEnterClick)(scope);
                });

                event.preventDefault();
            }
        });
    };
});
