"use strict";

angular.module('app.affiliateAdmin')
.config(function($stateProvider) {
    $stateProvider
        .state('app.affiliateAdmin.userGroups', {
            url: '/AffiliateAdmin/UserGroups',
            views: {
                "content@app":{
                    templateUrl: 'app/affiliateAdmin/messaging/userGroups/userGroups.html',
                    controller: 'SegmentsController',
                    controllerAs: 'segmentsCtrl'
                }
            },
            data:{
                title:'User Groups'
            },
            resolve: {
                segmentsList: function(messagesService){
                    return messagesService.getSegments();
                }
            }
        })
        .state('app.affiliateAdmin.createUserGroup', {
            url: '/AffiliateAdmin/CreateUserGroup',
            views: {
                "content@app":{
                    templateUrl: 'app/affiliateAdmin/messaging/userGroups/views/createUserGroup.html',
                    controller: 'CreateSegmentController',
                    controllerAs: 'createSegmentCtrl'
                }
            },
            data:{
                title: 'Create user group'
            },
            resolve: {
                segment: function(messagesService){
                    return messagesService.getSegment();
                }
            }
        })
        .state('app.affiliateAdmin.editUserGroup', {
            url: '/AffiliateAdmin/EditUserGroup?id',
            views: {
                "content@app":{
                    templateUrl: 'app/affiliateAdmin/messaging/userGroups/views/createUserGroup.html',
                    controller: 'CreateSegmentController',
                    controllerAs: 'createSegmentCtrl'
                }
            },
            data:{
                title: 'Edit user group'
            },
            resolve: {
                segment: function(messagesService, $stateParams){
                    return messagesService.editSegment($stateParams.id);
                }
            }
        });
    });