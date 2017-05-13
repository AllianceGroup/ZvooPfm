'use strict';

angular.module('app.affiliateAdmin').directive('smartJquiAccordion', function () {
    return {
        restrict: 'A',
        link: function (scope, element, attributes) {

            element.accordion({
                autoHeight : false,
                heightStyle : "content",
                collapsible : true,
                animate : 300,
                icons: {
                    header: "fa fa-plus",    // custom icon class
                    activeHeader: "fa fa-minus" // custom icon class
                },
                header : "h4"
            })
        }
    }
});