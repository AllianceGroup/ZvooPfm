; (function (window) {
    var angular = window.angular;

    var templatePath = '/template/overlay-spinner/overlay-spinner.html';
    var template = '';
    template += '<div class="overlay-spinner-content">';
    template += '<div class="overlay-spinner-container">';
    template += '<i class="fa fa-spinner fa-spin fa-3x overlay-spinner"></i>';
    template += '</div>';
    template += '<ng-transclude></ng-transclude>';
    template += '</div>';

    angular.module('app').directive('overlaySpinner', overlaySpinner);
    overlaySpinner.$inject = ['$animate'];
    function overlaySpinner($animate) {
        return {
            templateUrl: templatePath,
            scope: { active: '=' },
            transclude: true,
            restrict: 'E',
            link: link
        };

        function link(scope, iElement) {
            scope.$watch('active', statusWatcher);
            function statusWatcher(active) {
                $animate[active ? 'addClass' : 'removeClass'](iElement, 'overlay-spinner-active');
            }
        }
    }

    angular.module('app').run(overlaySpinnerTemplate);
    overlaySpinnerTemplate.$inject = ['$templateCache'];
    function overlaySpinnerTemplate($templateCache) {
        $templateCache.put(templatePath, template);
    }

}.call(this, window));