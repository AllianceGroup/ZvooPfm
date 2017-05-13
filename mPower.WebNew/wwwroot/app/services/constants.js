'use strict';

angular.module('app').constant('USER_ROLES', {
    pfm: {
        value: "1",
        text: 'Personal Finances'
    },
    affiliate: {
        value: "10",
        text: 'Affiliate Admin'
    },
    global: {
        value: "13",
        text: 'Global Admin'
    },
    bfm: {
        value: "100",
        text: 'Business Finances'
    }
});