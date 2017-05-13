"use strict";

angular.module('app.affiliateAdmin')
.config(function ($stateProvider) {
    $stateProvider
        .state('app.affiliateAdmin.faq', {
            url: '/AffiliateAdmin/Faq',
            views: {
                "content@app": {
                    templateUrl: 'app/affiliateAdmin/messaging/faq/faq.html',
                    controller: 'FaqController',
                    controllerAs: 'faqCtrl'
                }
            },
            data: {
                title: 'FAQ'
            },
            resolve: {
                faqList: function (messagesService) {
                    return messagesService.getFaq();
                }
            }
        })
        .state('app.affiliateAdmin.createFaq', {
            url: '/AffiliateAdmin/CreateFaq',
            views: {
                "content@app": {
                    templateUrl: '/app/affiliateAdmin/messaging/faq/views/createFaq.html',
                    controller: 'CreateFaqController',
                    controllerAs: 'createFaqCtrl'
                }
            },
            data: {
                title: 'Create FAQ'
            },
            resolve: {
                faq: function (messagesService) {
                    return messagesService.editFaq();
                },
                srcipts: function (lazyScript) {
                    return lazyScript.register([
                        'summernote'
                    ]);
                }
            }
        })
        .state('app.affiliateAdmin.editFaq', {
            views: {
                "content@app": {
                    templateUrl: 'app/affiliateAdmin/messaging/faq/views/editFaq.html',
                    controller: 'EditFaqController',
                    controllerAs: 'editFaqCtrl'
                }
            },
            data: {
                title: 'Edit FAQ'
            },
            params: {
                model: null
            },
            resolve: {
                srcipts: function (lazyScript) {
                    return lazyScript.register([
                        'summernote'
                    ]);
                }
            }
        })
    .state('app.personal.dashboard.helpFaq', {
        views: {
            "content@app": {
                templateUrl: '/app/personal/faq/helpFaq.html',
                controller: 'FaqController',
                controllerAs: 'faqCtrl'
            }
        },
        data: {
            title: 'FAQ'
        },
        resolve: {
            faqList: function (messagesService) {
                return messagesService.getFaq();
            }
        }
    });
});