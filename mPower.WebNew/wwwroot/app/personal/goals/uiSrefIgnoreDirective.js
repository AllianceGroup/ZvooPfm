angular.module('app.personal').directive('uiSrefIgnore', function () {
    return {
        link: function (scope, elem) {
            elem.on('click', function () {
                var uiSref = elem.parents('[ui-sref]').first();
                uiSref.attr({
                    target: 'true'
                });
                setTimeout(function () {
                    uiSref.attr({
                        target: null
                    });
                }, 0);
            });
        }
    };
});