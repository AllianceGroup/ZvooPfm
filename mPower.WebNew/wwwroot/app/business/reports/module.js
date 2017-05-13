"use strict";

angular.module('app.business')
    .config(function($stateProvider){
       $stateProvider
           .state('app.business.reports', {
               url: '/reports',
               views: {
                   "content@app": {
                       controller: 'ReportsBusinessController',
                       controllerAs: 'reportsBusinessCtrl',
                       templateUrl: 'app/business/reports/reports.html'
                   }
               },
               data: {
                   title: 'reports'
               },
               resolve: {
                   modelProfitLoss: function(reportsBusinessService){
                       return reportsBusinessService.getProfitLoss({From: null, To: null});
                   },
                   modelBalanceSheet: function(reportsBusinessService){
                       return reportsBusinessService.getBalanceSheet({From: null, To: null});
                   }
               }
           })
           .state('app.business.transactionDetail', {
               url: '/transactionDetail?From&To&Id',
               views: {
                   "content@app": {
                       templateUrl: 'app/business/reports/views/transactionDetail.html',
                       controller: 'TransactionDetailController',
                       controllerAs: 'transactionDetailCtrl'
                   }
               },
               data:{
                   title: 'Transaction Detail'
               },
               resolve:{
                   transactionDetailModel: function(reportsBusinessService, $stateParams){
                       return reportsBusinessService.getTransactionDetail({
                           From: decodeURIComponent($stateParams.From),
                           To: decodeURIComponent($stateParams.To),
                           Id: decodeURIComponent($stateParams.Id),
                           P: 0
                       });
                   }
               }
           })
    });