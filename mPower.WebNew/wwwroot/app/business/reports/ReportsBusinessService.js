'use strict';

angular.module('app.business').service('reportsBusinessService', ['http', '$http', 'apiLocator', function(http, $http, apiLocator){
    this.getProfitLoss = function(filter){
        var url = '/reports/getProfitLoss';
        return http.post(url, filter);
    };

    this.getBalanceSheet = function(filter){
        var url = '/reports/getBalanceSheet';
        return http.post(url, filter);
    };

    this.getTransactionDetail = function(filter){
        var url = '/reports/getTransactionDetail';
        return http.post(url, filter);
    };

    this.exportProfitLossToExcel = function(filter){
        var url = apiLocator.assetsBaseUrl() + '/reports/exportProfitLossToExcel' + http.buildQueryString(filter);
        return $http.get(url, {responseType: 'arraybuffer'});
    };

    this.exportBalanceSheetToExcel = function(filter){
        var url = apiLocator.assetsBaseUrl() + '/reports/exportBalanceSheetToExcel' + http.buildQueryString(filter);
        return $http.get(url, {responseType: 'arraybuffer'});
    };

    this.exportTransactionDetailToExcel = function(filter){
        var url = apiLocator.assetsBaseUrl() + '/reports/exportTransactionDetailToExcel' + http.buildQueryString(filter);
        return $http.get(url, {responseType: 'arraybuffer'});
    };

    this.exportProfitLossToPdf = function(filter){
        var url = apiLocator.assetsBaseUrl() + '/reports/generateProfitLossPdf' + http.buildQueryString(filter);
        return $http.get(url, {responseType: 'arraybuffer'});
    };

    this.exportBalanceSheetToPdf = function(filter){
        var url = apiLocator.assetsBaseUrl() + '/reports/generateBalanceSheetPdf' + http.buildQueryString(filter);
        return $http.get(url, {responseType: 'arraybuffer'});
    };

    this.exportTransactionDetailToPdf = function(filter){
        var url = apiLocator.assetsBaseUrl() + '/reports/generateTransactionDetailToPdf' + http.buildQueryString(filter);
        return $http.get(url, {responseType: 'arraybuffer'});
    };
}]);