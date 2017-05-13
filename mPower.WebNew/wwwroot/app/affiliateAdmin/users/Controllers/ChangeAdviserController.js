'use strict';

angular.module('app.affiliateAdmin').controller('ChangeAdviserController', function($scope){

        $scope.IsVisible = false;

        $scope.toggleAuto = function () {
            $scope.IsVisible = $scope.ShowList;
        };

    });